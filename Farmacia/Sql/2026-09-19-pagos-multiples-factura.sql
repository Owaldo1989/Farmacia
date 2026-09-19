SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF COL_LENGTH('dbo.tbFactura', 'VueltoCordoba') IS NULL
BEGIN
    ALTER TABLE dbo.tbFactura
    ADD VueltoCordoba DECIMAL(18,2) NOT NULL
        CONSTRAINT DF_tbFactura_VueltoCordoba DEFAULT (0);
END;
GO

IF COL_LENGTH('dbo.tbFactura', 'VueltoDolar') IS NULL
BEGIN
    ALTER TABLE dbo.tbFactura
    ADD VueltoDolar DECIMAL(18,2) NOT NULL
        CONSTRAINT DF_tbFactura_VueltoDolar DEFAULT (0);
END;
GO

IF TYPE_ID(N'dbo.TipoPagoFactura') IS NULL
BEGIN
    EXEC(N'
        CREATE TYPE dbo.TipoPagoFactura AS TABLE
        (
            IdFormaPago INT NOT NULL,
            Moneda CHAR(3) NOT NULL,
            Monto DECIMAL(18,2) NOT NULL,
            Referencia NVARCHAR(100) NULL
        );
    ');
END;
GO

ALTER PROCEDURE dbo.SpGuardarFactura
    @IdFacturaCliente UNIQUEIDENTIFIER,
    @Fecha DATETIME,
    @Paciente NVARCHAR(100) = NULL,
    @Total DECIMAL(18,2),
    @PagoCordoba DECIMAL(18,2),
    @PagoDolar DECIMAL(18,2),
    @TasaCambio DECIMAL(10,4),
    @Vuelto DECIMAL(18,2),
    @DetalleFactura dbo.TipoDetalleFactura READONLY,
    @VueltoCordoba DECIMAL(18,2) = 0,
    @VueltoDolar DECIMAL(18,2) = 0,
    @IdTurno INT = NULL,
    @IdUsuario INT = NULL,
    @PagosFactura dbo.TipoPagoFactura READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @IdFactura INT;
    DECLARE @TotalCalculado DECIMAL(18,2);
    DECLARE @PagoTotalCordobas DECIMAL(18,4);
    DECLARE @VueltoTotalCordobas DECIMAL(18,4);
    DECLARE @EfectivoCordoba DECIMAL(18,2);
    DECLARE @EfectivoDolar DECIMAL(18,2);
    DECLARE @EfectivoTotalCordobas DECIMAL(18,4);
    DECLARE @PosTotalCordobas DECIMAL(18,4);

    IF EXISTS (SELECT 1 FROM dbo.tbFactura WHERE IdFacturaCliente = @IdFacturaCliente)
    BEGIN
        SELECT IdFactura
        FROM dbo.tbFactura
        WHERE IdFacturaCliente = @IdFacturaCliente;
        RETURN;
    END;

    IF @TasaCambio <= 0
        THROW 51000, 'La tasa de cambio no es valida.', 1;

    IF NOT EXISTS (SELECT 1 FROM @DetalleFactura)
        THROW 51001, 'La factura no tiene detalle.', 1;

    IF EXISTS (
        SELECT 1
        FROM @DetalleFactura
        WHERE IdProducto <= 0
           OR Cantidad <= 0
           OR Precio <= 0
    )
        THROW 51002, 'El detalle contiene cantidades o precios invalidos.', 1;

    IF EXISTS (
        SELECT IdProducto
        FROM @DetalleFactura
        GROUP BY IdProducto
        HAVING COUNT(*) > 1
    )
        THROW 51003, 'La factura contiene productos duplicados.', 1;

    IF EXISTS (
        SELECT 1
        FROM @DetalleFactura D
        LEFT JOIN dbo.tbProductos P ON P.IdProducto = D.IdProducto
        WHERE P.IdProducto IS NULL
    )
        THROW 51004, 'Uno o mas productos no existen.', 1;

    IF NOT EXISTS (SELECT 1 FROM @PagosFactura)
        THROW 51005, 'Debe registrar al menos una forma de pago.', 1;

    IF EXISTS (
        SELECT 1
        FROM @PagosFactura
        WHERE Monto <= 0
           OR Moneda NOT IN ('NIO', 'USD')
    )
        THROW 51006, 'Los pagos contienen montos o monedas invalidas.', 1;

    IF EXISTS (
        SELECT 1
        FROM @PagosFactura P
        LEFT JOIN dbo.tbFormasPago F ON F.IdFormaPago = P.IdFormaPago
        WHERE F.IdFormaPago IS NULL
           OR F.Activo = 0
    )
        THROW 51007, 'Una forma de pago no existe o esta inactiva.', 1;

    IF EXISTS (
        SELECT 1
        FROM @PagosFactura P
        INNER JOIN dbo.tbFormasPago F ON F.IdFormaPago = P.IdFormaPago
        WHERE NULLIF(LTRIM(RTRIM(P.Referencia)), '') IS NOT NULL
        GROUP BY
            P.IdFormaPago,
            UPPER(LTRIM(RTRIM(P.Referencia)))
        HAVING COUNT(*) > 1
    )
        THROW 51015, 'Una referencia esta repetida en la misma forma de pago.', 1;

    IF EXISTS (
        SELECT 1
        FROM @PagosFactura P
        INNER JOIN dbo.tbFormasPago F ON F.IdFormaPago = P.IdFormaPago
        WHERE F.EsEfectivo = 0
          AND P.Moneda <> 'NIO'
    )
        THROW 51009, 'Los pagos POS deben registrarse en cordobas.', 1;

    SELECT @TotalCalculado =
        CAST(SUM(Cantidad * Precio) AS DECIMAL(18,2))
    FROM @DetalleFactura;

    IF ABS(@TotalCalculado - @Total) > 0.05
        THROW 51010, 'El total enviado no coincide con el detalle.', 1;

    SELECT
        @EfectivoCordoba =
            ISNULL(SUM(CASE
                WHEN F.EsEfectivo = 1 AND P.Moneda = 'NIO'
                THEN P.Monto ELSE 0 END), 0),
        @EfectivoDolar =
            ISNULL(SUM(CASE
                WHEN F.EsEfectivo = 1 AND P.Moneda = 'USD'
                THEN P.Monto ELSE 0 END), 0),
        @PosTotalCordobas =
            ISNULL(SUM(CASE
                WHEN F.EsEfectivo = 0
                THEN P.Monto ELSE 0 END), 0),
        @PagoTotalCordobas =
            ISNULL(SUM(CASE
                WHEN P.Moneda = 'USD'
                THEN P.Monto * @TasaCambio
                ELSE P.Monto END), 0)
    FROM @PagosFactura P
    INNER JOIN dbo.tbFormasPago F ON F.IdFormaPago = P.IdFormaPago;

    SET @VueltoCordoba = ISNULL(@VueltoCordoba, 0);
    SET @VueltoDolar = ISNULL(@VueltoDolar, 0);
    SET @VueltoTotalCordobas =
        @VueltoCordoba + (@VueltoDolar * @TasaCambio);
    SET @EfectivoTotalCordobas =
        @EfectivoCordoba + (@EfectivoDolar * @TasaCambio);

    IF @VueltoCordoba < 0 OR @VueltoDolar < 0
        THROW 51011, 'El vuelto no puede ser negativo.', 1;

    IF @PosTotalCordobas > @TotalCalculado + 0.05
        THROW 51012, 'Los pagos POS no pueden superar el total de la factura.', 1;

    IF @VueltoTotalCordobas > @EfectivoTotalCordobas + 0.05
        THROW 51013, 'Solo el efectivo puede generar vuelto.', 1;

    IF ABS((@PagoTotalCordobas - @VueltoTotalCordobas) - @TotalCalculado) > 0.05
        THROW 51014, 'El pago neto no coincide con el total de la factura.', 1;

    BEGIN TRY
        BEGIN TRAN;

        INSERT INTO dbo.tbFactura
        (
            IdFacturaCliente,
            Fecha,
            Paciente,
            Total,
            PagoCordoba,
            PagoDolar,
            Vuelto,
            TasaCambio,
            IdTurno,
            IdUsuario,
            VueltoCordoba,
            VueltoDolar
        )
        VALUES
        (
            @IdFacturaCliente,
            @Fecha,
            @Paciente,
            @TotalCalculado,
            @EfectivoCordoba,
            @EfectivoDolar,
            CAST(@VueltoTotalCordobas AS DECIMAL(18,2)),
            @TasaCambio,
            @IdTurno,
            @IdUsuario,
            @VueltoCordoba,
            @VueltoDolar
        );

        SET @IdFactura = SCOPE_IDENTITY();

        INSERT INTO dbo.tbFacturaDetalle
        (
            IdFactura,
            IdProducto,
            Cantidad,
            Precio
        )
        SELECT
            @IdFactura,
            IdProducto,
            Cantidad,
            Precio
        FROM @DetalleFactura;

        INSERT INTO dbo.tbFacturaPago
        (
            IdFactura,
            IdFormaPago,
            Moneda,
            Monto,
            TasaCambio,
            Referencia
        )
        SELECT
            @IdFactura,
            IdFormaPago,
            Moneda,
            Monto,
            @TasaCambio,
            NULLIF(LTRIM(RTRIM(Referencia)), '')
        FROM @PagosFactura;

        UPDATE P
        SET P.Inventario = P.Inventario - D.Cantidad
        FROM dbo.tbProductos P
        INNER JOIN @DetalleFactura D ON D.IdProducto = P.IdProducto;

        UPDATE FD
        SET FD.CostoProducto = P.CostoPromedio
        FROM dbo.tbFacturaDetalle FD
        INNER JOIN dbo.tbProductos P ON P.IdProducto = FD.IdProducto
        WHERE FD.IdFactura = @IdFactura;

        COMMIT;

        SELECT @IdFactura AS IdFactura;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW;
    END CATCH;
END;
GO

ALTER PROCEDURE dbo.SpObtenerFacturaCompleta
    @IdFactura INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        'FARMACIA ESPECIALIDADES' AS NombreFarmacia,
        'Frente la Gonper Librerias' AS Direccion,
        '2522-3796' AS Telefono,
        F.IdFactura,
        F.IdFacturaCliente,
        F.Fecha,
        F.Paciente,
        F.Total,
        F.PagoCordoba,
        F.PagoDolar,
        F.Vuelto,
        F.VueltoCordoba,
        F.VueltoDolar,
        F.TasaCambio
    FROM dbo.tbFactura F
    WHERE F.IdFactura = @IdFactura;

    SELECT
        D.IdProducto,
        P.NombreProducto,
        P.CodBarra,
        D.Cantidad,
        D.Precio,
        D.SubTotal
    FROM dbo.tbFacturaDetalle D
    INNER JOIN dbo.tbProductos P ON P.IdProducto = D.IdProducto
    WHERE D.IdFactura = @IdFactura
    ORDER BY D.IdDetalle;

    SELECT
        P.IdFacturaPago,
        P.IdFactura,
        P.IdFormaPago,
        F.Codigo,
        F.Nombre,
        P.Moneda,
        P.Monto,
        P.TasaCambio,
        P.Referencia,
        F.EsEfectivo,
        F.RequiereReferencia,
        F.PermiteVuelto
    FROM dbo.tbFacturaPago P
    INNER JOIN dbo.tbFormasPago F ON F.IdFormaPago = P.IdFormaPago
    WHERE P.IdFactura = @IdFactura
    ORDER BY P.IdFacturaPago;
END;
GO
