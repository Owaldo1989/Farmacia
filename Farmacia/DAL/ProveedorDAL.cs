using Farmacia.Models;
using System.Data.SqlClient;
using System.Data;

namespace Farmacia.DAL
{
    public class ProveedorDAL
    {
        private readonly string _cn;

        public ProveedorDAL(IConfiguration config)
        {
            _cn = config.GetConnectionString("BDFarmacia");
        }

        // LISTAR
        public List<Proveedor> Listar()
        {
            List<Proveedor> lista = new();

            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpProveedores", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Accion", "LIST");

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new Proveedor
                    {
                        IdProveedor = Convert.ToInt32(dr["IdProveedor"]),
                        NombreProveedor = dr["NombreProveedor"].ToString(),
                        Contacto = dr["Contacto"]?.ToString(),
                        Telefono = dr["Telefono"]?.ToString()
                    });
                }
            }

            return lista;
        }

        // OBTENER UNO
        public Proveedor Obtener(int id)
        {
            Proveedor p = null;

            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpProveedores", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Accion", "GET");
                cmd.Parameters.AddWithValue("@IdProveedor", id);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    p = new Proveedor
                    {
                        IdProveedor = Convert.ToInt32(dr["IdProveedor"]),
                        NombreProveedor = dr["NombreProveedor"].ToString(),
                        Contacto = dr["Contacto"]?.ToString(),
                        Telefono = dr["Telefono"]?.ToString()
                    };
                }
            }

            return p;
        }

        // INSERTAR / ACTUALIZAR
        public void Guardar(Proveedor p)
        {
            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpProveedores", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Accion", p.IdProveedor == 0 ? "INSERT" : "UPDATE");
                cmd.Parameters.AddWithValue("@IdProveedor", p.IdProveedor);
                cmd.Parameters.AddWithValue("@NombreProveedor", p.NombreProveedor);
                cmd.Parameters.AddWithValue("@Contacto", (object)p.Contacto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Telefono", (object)p.Telefono ?? DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ELIMINAR
        public void Eliminar(int id)
        {
            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpProveedores", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Accion", "DELETE");
                cmd.Parameters.AddWithValue("@IdProveedor", id);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
