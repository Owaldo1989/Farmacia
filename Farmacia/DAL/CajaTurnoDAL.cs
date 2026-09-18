using Farmacia.Models;
using System.Data;
using System.Data.SqlClient;

namespace Farmacia.DAL
{
    public class CajaTurnoDAL
    {
        private readonly string _cn;


        public CajaTurnoDAL(
            IConfiguration config)
        {
            _cn =
                config.GetConnectionString(
                    "BDFarmacia"
                );
        }



        // =====================================================
        // TURNO ACTUAL
        // =====================================================

        public CajaTurno? ObtenerActual(
            Guid tokenEquipo)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpCajaTurnos",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "GETACTUAL"
            );


            cmd.Parameters.AddWithValue(
                "@TokenEquipo",
                tokenEquipo
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
        // APERTURAR
        // =====================================================

        public int Abrir(
            Guid tokenEquipo,
            int idUsuario,
            decimal fondoInicial,
            string? observacion)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpCajaTurnos",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "APERTURAR"
            );


            cmd.Parameters.AddWithValue(
                "@TokenEquipo",
                tokenEquipo
            );


            cmd.Parameters.AddWithValue(
                "@IdUsuario",
                idUsuario
            );


            cmd.Parameters.AddWithValue(
                "@FondoInicial",
                fondoInicial
            );


            cmd.Parameters.AddWithValue(
                "@Observacion",
                (object?)observacion
                ?? DBNull.Value
            );


            cn.Open();


            object? resultado =
                cmd.ExecuteScalar();


            return Convert.ToInt32(
                resultado
            );
        }



        // =====================================================
        // MAPEAR
        // =====================================================

        private static CajaTurno Mapear(
            SqlDataReader dr)
        {
            return new CajaTurno
            {
                IdTurno =
                    Convert.ToInt32(
                        dr["IdTurno"]
                    ),

                IdCaja =
                    Convert.ToInt32(
                        dr["IdCaja"]
                    ),

                IdCajaEquipo =
                    Convert.ToInt32(
                        dr["IdCajaEquipo"]
                    ),

                IdUsuarioApertura =
                    Convert.ToInt32(
                        dr["IdUsuarioApertura"]
                    ),

                FechaApertura =
                    Convert.ToDateTime(
                        dr["FechaApertura"]
                    ),

                FondoConfigurado =
                    Convert.ToDecimal(
                        dr["FondoConfigurado"]
                    ),

                FondoInicial =
                    Convert.ToDecimal(
                        dr["FondoInicial"]
                    ),

                ObservacionApertura =
                    dr["ObservacionApertura"]
                        == DBNull.Value
                        ? null
                        : dr["ObservacionApertura"]
                            .ToString(),

                Estado =
                    Convert.ToByte(
                        dr["Estado"]
                    ),

                CodigoCaja =
                    dr["CodigoCaja"]
                        .ToString()
                    ?? "",

                NombreCaja =
                    dr["NombreCaja"]
                        .ToString()
                    ?? "",

                IdSucursal =
                    Convert.ToInt32(
                        dr["IdSucursal"]
                    ),

                CodBodega =
                    Convert.ToInt32(
                        dr["CodBodega"]
                    ),

                NombreSucursal =
                    dr["NombreSucursal"]
                        .ToString()
                    ?? "",

                NombreBodega =
                    dr["NombreBodega"]
                        .ToString()
                    ?? "",

                UsuarioApertura =
                    dr["UsuarioApertura"]
                        .ToString()
                    ?? ""
            };
        }
    }
}
