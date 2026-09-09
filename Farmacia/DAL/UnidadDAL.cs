using Farmacia.Models;
using System.Data.SqlClient;
using System.Data;

namespace Farmacia.DAL
{
    public class UnidadDAL
    {
        private readonly string _cn;

        public UnidadDAL(IConfiguration config)
        {
            _cn = config.GetConnectionString("BDFarmacia");
        }

        // LISTAR
        public List<UnidadMedida> Listar()
        {
            List<UnidadMedida> lista = new();

            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpUnidadesMedida", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Accion", "LIST");

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new UnidadMedida
                    {
                        IdUnidad = Convert.ToInt32(dr["IdUnidad"]),
                        NombreUnidad = dr["NombreUnidad"].ToString(),
                        Descripcion = dr["Descripcion"]?.ToString()
                    });
                }
            }
            return lista;
        }

        // OBTENER UNO
        public UnidadMedida Obtener(int id)
        {
            UnidadMedida u = null;

            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpUnidadesMedida", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Accion", "GET");
                cmd.Parameters.AddWithValue("@IdUnidad", id);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    u = new UnidadMedida
                    {
                        IdUnidad = Convert.ToInt32(dr["IdUnidad"]),
                        NombreUnidad = dr["NombreUnidad"].ToString(),
                        Descripcion = dr["Descripcion"]?.ToString()
                    };
                }
            }
            return u;
        }

        // INSERTAR / ACTUALIZAR
        public void Guardar(UnidadMedida u)
        {
            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpUnidadesMedida", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Accion", u.IdUnidad == 0 ? "INSERT" : "UPDATE");
                cmd.Parameters.AddWithValue("@IdUnidad", u.IdUnidad);
                cmd.Parameters.AddWithValue("@NombreUnidad", u.NombreUnidad);
                cmd.Parameters.AddWithValue("@Descripcion", (object)u.Descripcion ?? DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ELIMINAR
        public void Eliminar(int id)
        {
            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpUnidadesMedida", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Accion", "DELETE");
                cmd.Parameters.AddWithValue("@IdUnidad", id);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
