SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF COL_LENGTH('dbo.tbAgenteTransacciones', 'Moneda') IS NULL
BEGIN
    ALTER TABLE dbo.tbAgenteTransacciones
    ADD Moneda CHAR(3) NOT NULL
        CONSTRAINT DF_tbAgenteTransacciones_Moneda DEFAULT ('NIO');
END;
GO

IF COL_LENGTH('dbo.tbAgenteTransacciones', 'TasaCambio') IS NULL
BEGIN
    ALTER TABLE dbo.tbAgenteTransacciones
    ADD TasaCambio DECIMAL(10,4) NOT NULL
        CONSTRAINT DF_tbAgenteTransacciones_TasaCambio DEFAULT (1);
END;
GO

IF COL_LENGTH('dbo.tbAgenteTransacciones', 'MontoCordoba') IS NULL
BEGIN
    ALTER TABLE dbo.tbAgenteTransacciones
    ADD MontoCordoba DECIMAL(18,2) NOT NULL
        CONSTRAINT DF_tbAgenteTransacciones_MontoCordoba DEFAULT (0);
END;
GO

UPDATE dbo.tbAgenteTransacciones
SET MontoCordoba = Monto
WHERE MontoCordoba = 0;
GO

ALTER PROCEDURE dbo.SpAgenteTransacciones
(
    @Accion NVARCHAR(20),
    @IdTransaccion BIGINT = NULL,
    @IdAgente INT = NULL,
    @IdTipoOperacion INT = NULL,
    @Monto DECIMAL(18,2) = NULL,
    @Moneda CHAR(3) = 'NIO',
    @TasaCambio DECIMAL(10,4) = 1,
    @NumeroReferencia NVARCHAR(100) = NULL,
    @Observacion NVARCHAR(250) = NULL,
    @IdUsuario INT = NULL,
    @MotivoAnulacion NVARCHAR(250) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @Accion = 'LIST'
    BEGIN
        SELECT TOP (100)
            T.IdTransaccion,
            T.IdAgente,
            T.IdTipoOperacion,
            T.IdCaja,
            T.Monto,
            T.Moneda,
            T.TasaCambio,
            T.MontoCordoba,
            T.NumeroReferencia,
            T.FechaTransaccion,
            T.IdUsuario,
            T.Observacion,
            T.EfectoEfectivo,
            T.EfectoSaldoAgente,
            T.TipoComision,
            T.ValorComision,
            T.MontoComision,
            T.Estado,
            T.IdUsuarioAnula,
            T.FechaAnulacion,
            T.MotivoAnulacion,
            A.CodigoAgente,
            A.NombreAgente,
            O.CodigoOperacion,
            O.NombreOperacion,
            U.NombreCompleto AS NombreUsuario,
            C.NombreCaja
        FROM dbo.tbAgenteTransacciones T
        INNER JOIN dbo.tbAgentesBancarios A
            ON A.IdAgente = T.IdAgente
        INNER JOIN dbo.tbAgenteTipoOperacion O
            ON O.IdTipoOperacion = T.IdTipoOperacion
        INNER JOIN dbo.tbUsuarios U
            ON U.IdUsuario = T.IdUsuario
        INNER JOIN dbo.tbCajas C
            ON C.IdCaja = T.IdCaja
        WHERE T.IdAgente = @IdAgente
        ORDER BY T.IdTransaccion DESC;

        RETURN;
    END;

    IF @Accion = 'RESUMEN'
    BEGIN
        DECLARE @Hoy DATE = CONVERT(DATE, SYSDATETIME());
        DECLARE @SaldoInicial DECIMAL(18,2);

        SELECT @SaldoInicial = SaldoInicial
        FROM dbo.tbAgentesBancarios
        WHERE IdAgente = @IdAgente;

        IF @SaldoInicial IS NULL
        BEGIN
            THROW 51701, 'El agente bancario no existe.', 1;
        END;

        SELECT
            @SaldoInicial +
            ISNULL(SUM(CASE
                WHEN Estado = 1
                THEN MontoCordoba * EfectoSaldoAgente
                ELSE 0 END), 0) AS SaldoActual,
            ISNULL(SUM(CASE
                WHEN Estado = 1
                 AND FechaTransaccion >= @Hoy
                 AND FechaTransaccion < DATEADD(DAY, 1, @Hoy)
                 AND EfectoEfectivo = 1
                THEN MontoCordoba ELSE 0 END), 0) AS EntradasEfectivoHoy,
            ISNULL(SUM(CASE
                WHEN Estado = 1
                 AND FechaTransaccion >= @Hoy
                 AND FechaTransaccion < DATEADD(DAY, 1, @Hoy)
                 AND EfectoEfectivo = -1
                THEN MontoCordoba ELSE 0 END), 0) AS SalidasEfectivoHoy,
            ISNULL(SUM(CASE
                WHEN Estado = 1
                 AND FechaTransaccion >= @Hoy
                 AND FechaTransaccion < DATEADD(DAY, 1, @Hoy)
                THEN MontoComision ELSE 0 END), 0) AS ComisionHoy,
            ISNULL(SUM(CASE
                WHEN Estado = 1
                 AND FechaTransaccion >= @Hoy
                 AND FechaTransaccion < DATEADD(DAY, 1, @Hoy)
                THEN 1 ELSE 0 END), 0) AS OperacionesHoy
        FROM dbo.tbAgenteTransacciones
        WHERE IdAgente = @IdAgente;

        RETURN;
    END;

    IF @Accion = 'INSERT'
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            DECLARE @IdCaja INT;
            DECLARE @EfectoEfectivo SMALLINT;
            DECLARE @EfectoSaldoAgente SMALLINT;
            DECLARE @RequiereReferencia BIT;
            DECLARE @TipoComision TINYINT;
            DECLARE @ValorComision DECIMAL(18,4);
            DECLARE @MontoComision DECIMAL(18,2);
            DECLARE @SaldoInicialAgente DECIMAL(18,2);
            DECLARE @SaldoActual DECIMAL(18,2);
            DECLARE @SaldoDespues DECIMAL(18,2);
            DECLARE @MontoCordoba DECIMAL(18,2);

            SET @Moneda = UPPER(LTRIM(RTRIM(ISNULL(@Moneda, 'NIO'))));

            IF @Moneda NOT IN ('NIO', 'USD')
            BEGIN
                THROW 51710, 'La moneda seleccionada no es valida.', 1;
            END;

            IF @Moneda = 'USD' AND ISNULL(@TasaCambio, 0) <= 0
            BEGIN
                THROW 51711, 'La tasa de cambio no es valida.', 1;
            END;

            IF @Moneda = 'NIO'
            BEGIN
                SET @TasaCambio = 1;
            END;

            SELECT
                @IdCaja = A.IdCaja,
                @SaldoInicialAgente = A.SaldoInicial
            FROM dbo.tbAgentesBancarios A WITH (UPDLOCK, HOLDLOCK)
            WHERE A.IdAgente = @IdAgente
              AND A.Activo = 1;

            IF @IdCaja IS NULL
            BEGIN
                THROW 51702, 'El agente bancario no existe o esta inactivo.', 1;
            END;

            SELECT
                @EfectoEfectivo = O.EfectoEfectivo,
                @EfectoSaldoAgente = O.EfectoSaldoAgente,
                @RequiereReferencia = O.RequiereReferencia,
                @TipoComision = TRY_CONVERT(TINYINT, O.TipoComision),
                @ValorComision = O.ValorComision
            FROM dbo.tbAgenteTipoOperacion O WITH (UPDLOCK, HOLDLOCK)
            WHERE O.IdTipoOperacion = @IdTipoOperacion
              AND O.IdAgente = @IdAgente
              AND O.Activo = 1;

            IF @EfectoEfectivo IS NULL
            BEGIN
                THROW 51703, 'La operacion seleccionada no existe o esta inactiva.', 1;
            END;

            SET @TipoComision = ISNULL(@TipoComision, 0);
            SET @ValorComision = ISNULL(@ValorComision, 0);

            IF NOT EXISTS (
                SELECT 1
                FROM dbo.tbUsuarios
                WHERE IdUsuario = @IdUsuario
                  AND Estado = 1
            )
            BEGIN
                THROW 51704, 'El usuario no esta autorizado.', 1;
            END;

            IF ISNULL(@Monto, 0) <= 0
            BEGIN
                THROW 51705, 'El monto debe ser mayor que cero.', 1;
            END;

            SET @MontoCordoba =
                CASE
                    WHEN @Moneda = 'USD'
                    THEN ROUND(@Monto * @TasaCambio, 2)
                    ELSE @Monto
                END;

            SET @NumeroReferencia =
                NULLIF(LTRIM(RTRIM(@NumeroReferencia)), '');

            IF @NumeroReferencia IS NOT NULL
               AND EXISTS (
                    SELECT 1
                    FROM dbo.tbAgenteTransacciones
                    WHERE IdAgente = @IdAgente
                      AND NumeroReferencia = @NumeroReferencia
                      AND Estado = 1
               )
            BEGIN
                THROW 51707, 'Esta referencia ya fue registrada para este agente.', 1;
            END;

            SELECT @SaldoActual =
                @SaldoInicialAgente +
                ISNULL(SUM(MontoCordoba * EfectoSaldoAgente), 0)
            FROM dbo.tbAgenteTransacciones WITH (UPDLOCK, HOLDLOCK)
            WHERE IdAgente = @IdAgente
              AND Estado = 1;

            SET @SaldoDespues =
                @SaldoActual + (@MontoCordoba * @EfectoSaldoAgente);

            IF @SaldoDespues < 0
            BEGIN
                THROW 51708, 'Saldo insuficiente en el agente para realizar esta operacion.', 1;
            END;

            SET @MontoComision =
                CASE
                    WHEN @TipoComision = 1 THEN ROUND(@ValorComision, 2)
                    WHEN @TipoComision = 2 THEN ROUND(@MontoCordoba * (@ValorComision / 100), 2)
                    ELSE 0
                END;

            INSERT INTO dbo.tbAgenteTransacciones
            (
                IdAgente,
                IdTipoOperacion,
                IdCaja,
                Monto,
                Moneda,
                TasaCambio,
                MontoCordoba,
                NumeroReferencia,
                FechaTransaccion,
                IdUsuario,
                Observacion,
                EfectoEfectivo,
                EfectoSaldoAgente,
                TipoComision,
                ValorComision,
                MontoComision,
                Estado
            )
            VALUES
            (
                @IdAgente,
                @IdTipoOperacion,
                @IdCaja,
                @Monto,
                @Moneda,
                @TasaCambio,
                @MontoCordoba,
                @NumeroReferencia,
                SYSDATETIME(),
                @IdUsuario,
                NULLIF(LTRIM(RTRIM(@Observacion)), ''),
                @EfectoEfectivo,
                @EfectoSaldoAgente,
                @TipoComision,
                @ValorComision,
                @MontoComision,
                1
            );

            DECLARE @NuevaTransaccion BIGINT = SCOPE_IDENTITY();

            COMMIT TRANSACTION;

            SELECT
                @NuevaTransaccion AS IdTransaccion,
                @SaldoDespues AS SaldoAgente,
                @MontoComision AS Comision;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;

            THROW;
        END CATCH;

        RETURN;
    END;

    IF @Accion = 'ANULAR'
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            DECLARE @AnulaIdAgente INT;
            DECLARE @AnulaMontoCordoba DECIMAL(18,2);
            DECLARE @AnulaEfectoSaldo SMALLINT;
            DECLARE @AnulaSaldoInicial DECIMAL(18,2);
            DECLARE @AnulaSaldoActual DECIMAL(18,2);
            DECLARE @SaldoLuegoAnular DECIMAL(18,2);

            SELECT
                @AnulaIdAgente = T.IdAgente,
                @AnulaMontoCordoba = T.MontoCordoba,
                @AnulaEfectoSaldo = T.EfectoSaldoAgente
            FROM dbo.tbAgenteTransacciones T WITH (UPDLOCK, HOLDLOCK)
            WHERE T.IdTransaccion = @IdTransaccion
              AND T.Estado = 1;

            IF @AnulaIdAgente IS NULL
            BEGIN
                THROW 51720, 'La transaccion no existe o ya esta anulada.', 1;
            END;

            IF NOT EXISTS (
                SELECT 1
                FROM dbo.tbUsuarios
                WHERE IdUsuario = @IdUsuario
                  AND Estado = 1
            )
            BEGIN
                THROW 51721, 'El usuario no esta autorizado.', 1;
            END;

            SET @MotivoAnulacion =
                NULLIF(LTRIM(RTRIM(@MotivoAnulacion)), '');

            IF @MotivoAnulacion IS NULL
            BEGIN
                THROW 51722, 'Debe indicar el motivo de la anulacion.', 1;
            END;

            SELECT @AnulaSaldoInicial = SaldoInicial
            FROM dbo.tbAgentesBancarios
            WHERE IdAgente = @AnulaIdAgente;

            SELECT @AnulaSaldoActual =
                @AnulaSaldoInicial +
                ISNULL(SUM(MontoCordoba * EfectoSaldoAgente), 0)
            FROM dbo.tbAgenteTransacciones WITH (UPDLOCK, HOLDLOCK)
            WHERE IdAgente = @AnulaIdAgente
              AND Estado = 1;

            SET @SaldoLuegoAnular =
                @AnulaSaldoActual - (@AnulaMontoCordoba * @AnulaEfectoSaldo);

            IF @SaldoLuegoAnular < 0
            BEGIN
                THROW 51723, 'No puede anular esta operacion porque dejaria el saldo del agente negativo.', 1;
            END;

            UPDATE dbo.tbAgenteTransacciones
            SET
                Estado = 99,
                IdUsuarioAnula = @IdUsuario,
                FechaAnulacion = SYSDATETIME(),
                MotivoAnulacion = @MotivoAnulacion
            WHERE IdTransaccion = @IdTransaccion;

            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;

            THROW;
        END CATCH;

        RETURN;
    END;

    THROW 51799, 'Accion de transaccion de agente no valida.', 1;
END;
GO
