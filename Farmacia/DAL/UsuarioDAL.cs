using Farmacia.Models;
using System.Data.SqlClient;
using System.Data;

namespace Farmacia.DAL
{
    public class UsuarioDAL
    {
        private readonly string _cn;

        public UsuarioDAL(IConfiguration config)
        {
            _cn = config.GetConnectionString("BDFarmacia");
        }

        public Usuario Login(string usuario, string clave)
        {
            Usuario u = null;

            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpLoginUsuario", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Usuario", usuario);
                cmd.Parameters.AddWithValue("@Clave", clave);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    u = new Usuario
                    {
                        IdUsuario = (int)dr["IdUsuario"],
                        UsuarioNombre = dr["UsuarioNombre"].ToString(),
                        Clave = dr["Clave"].ToString(),
                        NombreCompleto = dr["NombreCompleto"].ToString(),
                        Rol = dr["Rol"].ToString(),
                        Estado = (bool)dr["Estado"]
                    };
                }
            }

            return u;
        }


    }
}
