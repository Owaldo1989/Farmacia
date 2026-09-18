using Farmacia.Models;
using System.Data;
using System.Data.SqlClient;

namespace Farmacia.DAL
{
    public class SucursalDAL
    {
        private readonly string _cn;


        public SucursalDAL(
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

        public List<Sucursal> Listar()
        {
            List<Sucursal> lista =
                new();


            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpSucursales",
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
                    new Sucursal
                    {
                        IdSucursal =
                            Convert.ToInt32(
                                dr["IdSucursal"]
                            ),

                        CodigoSucursal =
                            dr["CodigoSucursal"]
                                .ToString()
                            ?? string.Empty,

                        NombreSucursal =
                            dr["NombreSucursal"]
                                .ToString()
                            ?? string.Empty,

                        Direccion =
                            dr["Direccion"]
                                == DBNull.Value
                                ? null
                                : dr["Direccion"]
                                    .ToString(),

                        Telefono =
                            dr["Telefono"]
                                == DBNull.Value
                                ? null
                                : dr["Telefono"]
                                    .ToString(),

                        Activo =
                            Convert.ToBoolean(
                                dr["Activo"]
                            ),

                        FechaRegistro =
                            Convert.ToDateTime(
                                dr["FechaRegistro"]
                            )
                    }
                );
            }


            return lista;
        }



        // =====================================================
        // OBTENER
        // =====================================================

        public Sucursal? Obtener(
            int id)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpSucursales",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "GET"
            );


            cmd.Parameters.AddWithValue(
                "@IdSucursal",
                id
            );


            cn.Open();


            using var dr =
                cmd.ExecuteReader();


            if (!dr.Read())
            {
                return null;
            }


            return new Sucursal
            {
                IdSucursal =
                    Convert.ToInt32(
                        dr["IdSucursal"]
                    ),

                CodigoSucursal =
                    dr["CodigoSucursal"]
                        .ToString()
                    ?? string.Empty,

                NombreSucursal =
                    dr["NombreSucursal"]
                        .ToString()
                    ?? string.Empty,

                Direccion =
                    dr["Direccion"]
                        == DBNull.Value
                        ? null
                        : dr["Direccion"]
                            .ToString(),

                Telefono =
                    dr["Telefono"]
                        == DBNull.Value
                        ? null
                        : dr["Telefono"]
                            .ToString(),

                Activo =
                    Convert.ToBoolean(
                        dr["Activo"]
                    ),

                FechaRegistro =
                    Convert.ToDateTime(
                        dr["FechaRegistro"]
                    )
            };
        }



        // =====================================================
        // GUARDAR
        // =====================================================

        public void Guardar(
            Sucursal sucursal)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpSucursales",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                sucursal.IdSucursal == 0
                    ? "INSERT"
                    : "UPDATE"
            );


            cmd.Parameters.AddWithValue(
                "@IdSucursal",
                sucursal.IdSucursal
            );


            cmd.Parameters.AddWithValue(
                "@CodigoSucursal",
                sucursal.CodigoSucursal
            );


            cmd.Parameters.AddWithValue(
                "@NombreSucursal",
                sucursal.NombreSucursal
            );


            cmd.Parameters.AddWithValue(
                "@Direccion",
                (object?)sucursal.Direccion
                ?? DBNull.Value
            );


            cmd.Parameters.AddWithValue(
                "@Telefono",
                (object?)sucursal.Telefono
                ?? DBNull.Value
            );


            cmd.Parameters.AddWithValue(
                "@Activo",
                sucursal.Activo
            );


            cn.Open();

            cmd.ExecuteNonQuery();
        }



        // =====================================================
        // DESACTIVAR
        // =====================================================

        public void Eliminar(
            int id)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpSucursales",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "DELETE"
            );


            cmd.Parameters.AddWithValue(
                "@IdSucursal",
                id
            );


            cn.Open();

            cmd.ExecuteNonQuery();
        }
    }
}
