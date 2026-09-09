using Farmacia.Models;
using System.Data.SqlClient;
using System.Data;

namespace Farmacia.DAL
{
    public class CategoriaDAL
    {

        private readonly string _cn;

        public CategoriaDAL(IConfiguration config)
        {
            _cn = config.GetConnectionString("BDFarmacia");
        }

        // LISTAR
        public List<Categoria> Listar()
        {
            List<Categoria> lista = new();

            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpCategorias", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Accion", "LIST");

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new Categoria
                    {
                        IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                        NombreCategoria = dr["NombreCategoria"].ToString()
                    });
                }
            }

            return lista;
        }

        // OBTENER UNO
        public Categoria Obtener(int id)
        {
            Categoria c = null;

            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpCategorias", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Accion", "GET");
                cmd.Parameters.AddWithValue("@IdCategoria", id);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    c = new Categoria
                    {
                        IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                        NombreCategoria = dr["NombreCategoria"].ToString()
                    };
                }
            }

            return c;
        }

        // INSERTAR / ACTUALIZAR
        public void Guardar(Categoria c)
        {
            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpCategorias", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Accion", c.IdCategoria == 0 ? "INSERT" : "UPDATE");
                cmd.Parameters.AddWithValue("@IdCategoria", c.IdCategoria);
                cmd.Parameters.AddWithValue("@NombreCategoria", c.NombreCategoria);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ELIMINAR
        public void Eliminar(int id)
        {
            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpCategorias", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Accion", "DELETE");
                cmd.Parameters.AddWithValue("@IdCategoria", id);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }


}

