using Farmacia.Models.AgenteBancario;
using System.Data;
using System.Data.SqlClient;

namespace Farmacia.DAL
{
    public class AgenteTransaccionDAL
    {
        private readonly string _cn;


        public AgenteTransaccionDAL(
            IConfiguration config)
        {
            _cn =
                config.GetConnectionString(
                    "BDFarmacia"
                );
        }


        // =====================================================
        // LISTAR
        // =====================================================

        public List<AgenteTransaccion> Listar(
            int idAgente)
        {
            List<AgenteTransaccion> lista =
                new();


            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpAgenteTransacciones",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "LIST"
            );


            cmd.Parameters.AddWithValue(
                "@IdAgente",
                idAgente
            );


            cn.Open();


            using var dr =
                cmd.ExecuteReader();


            while (dr.Read())
            {
                lista.Add(
                    Mapear(dr)
                );
            }


            return lista;
        }


        // =====================================================
        // RESUMEN
        // =====================================================

        public AgenteResumen ObtenerResumen(
            int idAgente)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpAgenteTransacciones",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "RESUMEN"
            );


            cmd.Parameters.AddWithValue(
                "@IdAgente",
                idAgente
            );


            cn.Open();


            using var dr =
                cmd.ExecuteReader();


            if (!dr.Read())
            {
                return new AgenteResumen();
            }


            return new AgenteResumen
            {
                SaldoActual =
                    Convert.ToDecimal(
                        dr["SaldoActual"]
                    ),

                EntradasEfectivoHoy =
                    Convert.ToDecimal(
                        dr["EntradasEfectivoHoy"]
                    ),

                SalidasEfectivoHoy =
                    Convert.ToDecimal(
                        dr["SalidasEfectivoHoy"]
                    ),

                ComisionHoy =
                    Convert.ToDecimal(
                        dr["ComisionHoy"]
                    ),

                OperacionesHoy =
                    Convert.ToInt32(
                        dr["OperacionesHoy"]
                    )
            };
        }


        // =====================================================
        // REGISTRAR
        // =====================================================

        public long Registrar(
            int idAgente,
            int idTipoOperacion,
            decimal monto,
            string moneda,
            decimal tasaCambio,
            string? referencia,
            string? observacion,
            int idUsuario)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpAgenteTransacciones",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "INSERT"
            );


            cmd.Parameters.AddWithValue(
                "@IdAgente",
                idAgente
            );


            cmd.Parameters.AddWithValue(
                "@IdTipoOperacion",
                idTipoOperacion
            );


            var pMonto =
                cmd.Parameters.Add(
                    "@Monto",
                    SqlDbType.Decimal
                );

            pMonto.Precision = 18;
            pMonto.Scale = 2;
            pMonto.Value = monto;

            cmd.Parameters.AddWithValue(
                "@Moneda",
                moneda
            );


            var pTasaCambio =
                cmd.Parameters.Add(
                    "@TasaCambio",
                    SqlDbType.Decimal
                );

            pTasaCambio.Precision = 10;
            pTasaCambio.Scale = 4;
            pTasaCambio.Value = tasaCambio;


            cmd.Parameters.AddWithValue(
                "@NumeroReferencia",
                (object?)referencia
                ?? DBNull.Value
            );


            cmd.Parameters.AddWithValue(
                "@Observacion",
                (object?)observacion
                ?? DBNull.Value
            );


            cmd.Parameters.AddWithValue(
                "@IdUsuario",
                idUsuario
            );


            cn.Open();


            using var dr =
                cmd.ExecuteReader();


            if (dr.Read())
            {
                return Convert.ToInt64(
                    dr["IdTransaccion"]
                );
            }


            return 0;
        }


        public AgenteTransaccion? Obtener(
            long idTransaccion)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(@"
                    SELECT
                        T.IdTransaccion,
                        T.IdAgente,
                        T.IdTipoOperacion,
                        T.IdCaja,
                        T.Monto,
                        ISNULL(T.Moneda, 'NIO') AS Moneda,
                        ISNULL(T.TasaCambio, 1) AS TasaCambio,
                        ISNULL(T.MontoCordoba, T.Monto) AS MontoCordoba,
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
                    WHERE T.IdTransaccion = @IdTransaccion;",
                    cn
                );


            cmd.Parameters.AddWithValue(
                "@IdTransaccion",
                idTransaccion
            );


            cn.Open();


            using var dr =
                cmd.ExecuteReader();


            if (!dr.Read())
            {
                return null;
            }


            return Mapear(dr);
        }


        // =====================================================
        // ANULAR
        // =====================================================

        public void Anular(
            long idTransaccion,
            int idUsuario,
            string motivo)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpAgenteTransacciones",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "ANULAR"
            );


            cmd.Parameters.AddWithValue(
                "@IdTransaccion",
                idTransaccion
            );


            cmd.Parameters.AddWithValue(
                "@IdUsuario",
                idUsuario
            );


            cmd.Parameters.AddWithValue(
                "@MotivoAnulacion",
                motivo
            );


            cn.Open();

            cmd.ExecuteNonQuery();
        }


        // =====================================================
        // MAPEO
        // =====================================================

        private static AgenteTransaccion Mapear(
            SqlDataReader dr)
        {
            return new AgenteTransaccion
            {
                IdTransaccion =
                    Convert.ToInt64(
                        dr["IdTransaccion"]
                    ),

                IdAgente =
                    Convert.ToInt32(
                        dr["IdAgente"]
                    ),

                IdTipoOperacion =
                    Convert.ToInt32(
                        dr["IdTipoOperacion"]
                    ),

                IdCaja =
                    Convert.ToInt32(
                        dr["IdCaja"]
                    ),

                Monto =
                    Convert.ToDecimal(
                        dr["Monto"]
                    ),

                Moneda =
                    HasColumn(
                        dr,
                        "Moneda"
                    )
                        ? dr["Moneda"].ToString()?.Trim() ?? "NIO"
                        : "NIO",

                TasaCambio =
                    HasColumn(
                        dr,
                        "TasaCambio"
                    )
                        ? Convert.ToDecimal(
                            dr["TasaCambio"]
                        )
                        : 1,

                MontoCordoba =
                    HasColumn(
                        dr,
                        "MontoCordoba"
                    )
                        ? Convert.ToDecimal(
                            dr["MontoCordoba"]
                        )
                        : Convert.ToDecimal(
                            dr["Monto"]
                        ),

                NumeroReferencia =
                    dr["NumeroReferencia"] == DBNull.Value
                        ? null
                        : dr["NumeroReferencia"].ToString(),

                FechaTransaccion =
                    Convert.ToDateTime(
                        dr["FechaTransaccion"]
                    ),

                IdUsuario =
                    Convert.ToInt32(
                        dr["IdUsuario"]
                    ),

                Observacion =
                    dr["Observacion"] == DBNull.Value
                        ? null
                        : dr["Observacion"].ToString(),

                EfectoEfectivo =
                    Convert.ToInt16(
                        dr["EfectoEfectivo"]
                    ),

                EfectoSaldoAgente =
                    Convert.ToInt16(
                        dr["EfectoSaldoAgente"]
                    ),

                TipoComision =
                    Convert.ToByte(
                        dr["TipoComision"]
                    ),

                ValorComision =
                    Convert.ToDecimal(
                        dr["ValorComision"]
                    ),

                MontoComision =
                    Convert.ToDecimal(
                        dr["MontoComision"]
                    ),

                Estado =
                    Convert.ToByte(
                        dr["Estado"]
                    ),

                IdUsuarioAnula =
                    dr["IdUsuarioAnula"] == DBNull.Value
                        ? null
                        : Convert.ToInt32(
                            dr["IdUsuarioAnula"]
                        ),

                FechaAnulacion =
                    dr["FechaAnulacion"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(
                            dr["FechaAnulacion"]
                        ),

                MotivoAnulacion =
                    dr["MotivoAnulacion"] == DBNull.Value
                        ? null
                        : dr["MotivoAnulacion"].ToString(),

                CodigoAgente =
                    dr["CodigoAgente"].ToString() ?? "",

                NombreAgente =
                    dr["NombreAgente"].ToString() ?? "",

                CodigoOperacion =
                    dr["CodigoOperacion"].ToString() ?? "",

                NombreOperacion =
                    dr["NombreOperacion"].ToString() ?? "",

                NombreUsuario =
                    dr["NombreUsuario"].ToString() ?? "",

                NombreCaja =
                    dr["NombreCaja"].ToString() ?? ""
            };
        }


        private static bool HasColumn(
            IDataRecord reader,
            string columnName)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (string.Equals(
                    reader.GetName(i),
                    columnName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
