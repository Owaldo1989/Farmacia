using Farmacia.Models;
using System.Data;
using System.Data.SqlClient;

namespace Farmacia.DAL
{
    public class BodegaDAL
    {
        private readonly string _cn;


        public BodegaDAL(
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

        public List<Bodega> Listar()
        {
            return EjecutarLista(
                "LIST",
                null
            );
        }



        // =====================================================
        // LISTAR ACTIVAS
        // =====================================================

        public List<Bodega> ListarActivas()
        {
            return EjecutarLista(
                "LISTACTIVAS",
                null
            );
        }



        // =====================================================
        // POR SUCURSAL
        // =====================================================

        public List<Bodega> ListarPorSucursal(
            int idSucursal)
        {
            return EjecutarLista(
                "BYSUCURSAL",
                idSucursal
            );
        }



        // =====================================================
        // MÉTODO INTERNO DE LISTADO
        // =====================================================

        private List<Bodega> EjecutarLista(
            string accion,
            int? idSucursal)
        {
            List<Bodega> lista =
                new();


            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpBodegas",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                accion
            );


            if (idSucursal.HasValue)
            {
                cmd.Parameters.AddWithValue(
                    "@IdSucursal",
                    idSucursal.Value
                );
            }


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
        // OBTENER
        // =====================================================

        public Bodega? Obtener(
            int codBodega)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpBodegas",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "GET"
            );


            cmd.Parameters.AddWithValue(
                "@CodBodega",
                codBodega
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
        // GUARDAR
        // =====================================================

        public void Guardar(
            Bodega bodega)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpBodegas",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                bodega.CodBodega == 0
                    ? "INSERT"
                    : "UPDATE"
            );


            cmd.Parameters.AddWithValue(
                "@CodBodega",
                bodega.CodBodega
            );


            cmd.Parameters.AddWithValue(
                "@IdSucursal",
                bodega.IdSucursal
            );


            cmd.Parameters.AddWithValue(
                "@CodigoBodega",
                bodega.CodigoBodega
            );


            cmd.Parameters.AddWithValue(
                "@NombreBodega",
                bodega.NombreBodega
            );


            cmd.Parameters.AddWithValue(
                "@Activo",
                bodega.Activo
            );


            cn.Open();

            cmd.ExecuteNonQuery();
        }



        // =====================================================
        // DESACTIVAR
        // =====================================================

        public void Eliminar(
            int codBodega)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpBodegas",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "DELETE"
            );


            cmd.Parameters.AddWithValue(
                "@CodBodega",
                codBodega
            );


            cn.Open();

            cmd.ExecuteNonQuery();
        }



        // =====================================================
        // MAPEAR
        // =====================================================

        private static Bodega Mapear(
            SqlDataReader dr)
        {
            return new Bodega
            {
                CodBodega =
                    Convert.ToInt32(
                        dr["CodBodega"]
                    ),

                IdSucursal =
                    Convert.ToInt32(
                        dr["IdSucursal"]
                    ),

                CodigoBodega =
                    dr["CodigoBodega"]
                        .ToString()
                    ?? string.Empty,

                NombreBodega =
                    dr["NombreBodega"]
                        .ToString()
                    ?? string.Empty,

                Activo =
                    Convert.ToBoolean(
                        dr["Activo"]
                    ),

                FechaRegistro =
                    Convert.ToDateTime(
                        dr["FechaRegistro"]
                    ),

                NombreSucursal =
                    dr["NombreSucursal"]
                        .ToString()
                    ?? string.Empty,

                CodigoSucursal =
                    dr["CodigoSucursal"]
                        .ToString()
                    ?? string.Empty
            };
        }
    }
}
