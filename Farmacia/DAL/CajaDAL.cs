using Farmacia.Models;
using System.Data;
using System.Data.SqlClient;

namespace Farmacia.DAL
{
    public class CajaDAL
    {
        private readonly string _cn;


        public CajaDAL(
            IConfiguration config)
        {
            _cn =
                config.GetConnectionString(
                    "BDFarmacia"
                );
        }



        public List<Caja> Listar()
        {
            List<Caja> lista =
                new();


            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpCajas",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "LIST"
            );


            cn.Open();


            using var dr =
                cmd.ExecuteReader();


            while (dr.Read())
            {
                lista.Add(
                    MapearLista(dr)
                );
            }


            return lista;
        }



        public Caja? Obtener(
            int idCaja)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpCajas",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "GET"
            );


            cmd.Parameters.AddWithValue(
                "@IdCaja",
                idCaja
            );


            cn.Open();


            using var dr =
                cmd.ExecuteReader();


            if (!dr.Read())
            {
                return null;
            }


            return new Caja
            {
                IdCaja =
                    Convert.ToInt32(
                        dr["IdCaja"]
                    ),

                CodigoCaja =
                    dr["CodigoCaja"].ToString()
                    ?? "",

                NombreCaja =
                    dr["NombreCaja"].ToString()
                    ?? "",

                IdSucursal =
                    Convert.ToInt32(
                        dr["IdSucursal"]
                    ),

                CodBodega =
                    Convert.ToInt32(
                        dr["CodBodega"]
                    ),

                FondoFijo =
                    Convert.ToDecimal(
                        dr["FondoFijo"]
                    ),

                Activo =
                    Convert.ToBoolean(
                        dr["Activo"]
                    ),

                FechaRegistro =
                    Convert.ToDateTime(
                        dr["FechaRegistro"]
                    ),

                NombreSucursal =
                    dr["NombreSucursal"].ToString()
                    ?? "",

                CodigoSucursal =
                    dr["CodigoSucursal"].ToString()
                    ?? "",

                NombreBodega =
                    dr["NombreBodega"].ToString()
                    ?? "",

                CodigoBodega =
                    dr["CodigoBodega"].ToString()
                    ?? ""
            };
        }



        public void Guardar(
            Caja caja)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpCajas",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                caja.IdCaja == 0
                    ? "INSERT"
                    : "UPDATE"
            );


            cmd.Parameters.AddWithValue(
                "@IdCaja",
                caja.IdCaja
            );


            cmd.Parameters.AddWithValue(
                "@CodigoCaja",
                caja.CodigoCaja
            );


            cmd.Parameters.AddWithValue(
                "@NombreCaja",
                caja.NombreCaja
            );


            cmd.Parameters.AddWithValue(
                "@IdSucursal",
                caja.IdSucursal
            );


            cmd.Parameters.AddWithValue(
                "@CodBodega",
                caja.CodBodega
            );


            cmd.Parameters.AddWithValue(
                "@FondoFijo",
                caja.FondoFijo
            );


            cmd.Parameters.AddWithValue(
                "@Activo",
                caja.Activo
            );


            cn.Open();

            cmd.ExecuteNonQuery();
        }



        public void Desactivar(
            int idCaja)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpCajas",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "DELETE"
            );


            cmd.Parameters.AddWithValue(
                "@IdCaja",
                idCaja
            );


            cn.Open();

            cmd.ExecuteNonQuery();
        }



        private static Caja MapearLista(
            SqlDataReader dr)
        {
            return new Caja
            {
                IdCaja =
                    Convert.ToInt32(
                        dr["IdCaja"]
                    ),

                CodigoCaja =
                    dr["CodigoCaja"].ToString()
                    ?? "",

                NombreCaja =
                    dr["NombreCaja"].ToString()
                    ?? "",

                IdSucursal =
                    Convert.ToInt32(
                        dr["IdSucursal"]
                    ),

                CodBodega =
                    Convert.ToInt32(
                        dr["CodBodega"]
                    ),

                FondoFijo =
                    Convert.ToDecimal(
                        dr["FondoFijo"]
                    ),

                Activo =
                    Convert.ToBoolean(
                        dr["Activo"]
                    ),

                FechaRegistro =
                    Convert.ToDateTime(
                        dr["FechaRegistro"]
                    ),

                NombreSucursal =
                    dr["NombreSucursal"].ToString()
                    ?? "",

                CodigoSucursal =
                    dr["CodigoSucursal"].ToString()
                    ?? "",

                NombreBodega =
                    dr["NombreBodega"].ToString()
                    ?? "",

                CodigoBodega =
                    dr["CodigoBodega"].ToString()
                    ?? "",

                IdCajaEquipo =
                    dr["IdCajaEquipo"]
                    == DBNull.Value
                        ? null
                        : Convert.ToInt32(
                            dr["IdCajaEquipo"]
                        ),

                NombreEquipo =
                    dr["NombreEquipo"]
                    == DBNull.Value
                        ? null
                        : dr["NombreEquipo"]
                            .ToString(),

                TokenEquipo =
                    dr["TokenEquipo"]
                    == DBNull.Value
                        ? null
                        : (Guid)dr["TokenEquipo"],

                UltimoAcceso =
                    dr["UltimoAcceso"]
                    == DBNull.Value
                        ? null
                        : Convert.ToDateTime(
                            dr["UltimoAcceso"]
                        )
            };
        }
    }
}
