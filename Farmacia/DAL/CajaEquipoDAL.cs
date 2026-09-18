using Farmacia.Models;
using System.Data;
using System.Data.SqlClient;

namespace Farmacia.DAL
{
    public class CajaEquipoDAL
    {
        private readonly string _cn;


        public CajaEquipoDAL(
            IConfiguration config)
        {
            _cn =
                config.GetConnectionString(
                    "BDFarmacia"
                );
        }



        public CajaEquipo? ObtenerPorToken(
            Guid token)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpCajaEquipos",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "GETTOKEN"
            );


            cmd.Parameters.AddWithValue(
                "@TokenEquipo",
                token
            );


            cn.Open();


            using var dr =
                cmd.ExecuteReader();


            if (!dr.Read())
            {
                return null;
            }


            return new CajaEquipo
            {
                IdCajaEquipo =
                    Convert.ToInt32(
                        dr["IdCajaEquipo"]
                    ),

                IdCaja =
                    Convert.ToInt32(
                        dr["IdCaja"]
                    ),

                TokenEquipo =
                    (Guid)dr["TokenEquipo"],

                NombreEquipo =
                    dr["NombreEquipo"]
                        .ToString()
                    ?? "",

                Observacion =
                    dr["Observacion"]
                    == DBNull.Value
                        ? null
                        : dr["Observacion"]
                            .ToString(),

                IdUsuarioRegistro =
                    dr["IdUsuarioRegistro"]
                    == DBNull.Value
                        ? null
                        : Convert.ToInt32(
                            dr["IdUsuarioRegistro"]
                        ),

                FechaRegistro =
                    Convert.ToDateTime(
                        dr["FechaRegistro"]
                    ),

                UltimoAcceso =
                    dr["UltimoAcceso"]
                    == DBNull.Value
                        ? null
                        : Convert.ToDateTime(
                            dr["UltimoAcceso"]
                        ),

                Activo =
                    Convert.ToBoolean(
                        dr["Activo"]
                    ),

                CodigoCaja =
                    dr["CodigoCaja"]
                        .ToString()
                    ?? "",

                NombreCaja =
                    dr["NombreCaja"]
                        .ToString()
                    ?? "",

                FondoFijo =
                    Convert.ToDecimal(
                        dr["FondoFijo"]
                    ),

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
                    ?? ""
            };
        }



        public void Matricular(
            int idCaja,
            Guid token,
            string nombreEquipo,
            string? observacion,
            int? idUsuario)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpCajaEquipos",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "MATRICULAR"
            );


            cmd.Parameters.AddWithValue(
                "@IdCaja",
                idCaja
            );


            cmd.Parameters.AddWithValue(
                "@TokenEquipo",
                token
            );


            cmd.Parameters.AddWithValue(
                "@NombreEquipo",
                nombreEquipo
            );


            cmd.Parameters.AddWithValue(
                "@Observacion",
                (object?)observacion
                ?? DBNull.Value
            );


            cmd.Parameters.AddWithValue(
                "@IdUsuarioRegistro",
                (object?)idUsuario
                ?? DBNull.Value
            );


            cn.Open();

            cmd.ExecuteNonQuery();
        }



        public void Desmatricular(
            int idCajaEquipo)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpCajaEquipos",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "DELETE"
            );


            cmd.Parameters.AddWithValue(
                "@IdCajaEquipo",
                idCajaEquipo
            );


            cn.Open();

            cmd.ExecuteNonQuery();
        }



        public void RegistrarAcceso(
            Guid token)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpCajaEquipos",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "ACCESO"
            );


            cmd.Parameters.AddWithValue(
                "@TokenEquipo",
                token
            );


            cn.Open();

            cmd.ExecuteNonQuery();
        }
    }
}
