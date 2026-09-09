using Farmacia.Models;
using System.Data.SqlClient;
using System.Data;

namespace Farmacia.DAL
{
    public class LaboratorioDAL
    {
        private readonly string _cn;

        public LaboratorioDAL(IConfiguration config)
        {
            _cn = config.GetConnectionString("BDFarmacia");
        }

        // LISTAR
        public List<Laboratorio> Listar()
        {
            List<Laboratorio> lista = new();

            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpLaboratorios", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Accion", "LIST");

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new Laboratorio
                    {
                        IdLaboratorio = Convert.ToInt32(dr["IdLaboratorio"]),
                        LaboratorioNombre = dr["Laboratorio"].ToString()
                    });
                }
            }

            return lista;
        }

        // OBTENER
        public Laboratorio Obtener(int id)
        {
            Laboratorio l = null;

            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpLaboratorios", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Accion", "GET");
                cmd.Parameters.AddWithValue("@IdLaboratorio", id);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    l = new Laboratorio
                    {
                        IdLaboratorio = Convert.ToInt32(dr["IdLaboratorio"]),
                        LaboratorioNombre = dr["Laboratorio"].ToString()
                    };
                }
            }

            return l;
        }

        // GUARDAR (INSERT/UPDATE)
        public void Guardar(Laboratorio l)
        {
            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpLaboratorios", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Accion", l.IdLaboratorio == 0 ? "INSERT" : "UPDATE");
                cmd.Parameters.AddWithValue("@IdLaboratorio", l.IdLaboratorio);
                cmd.Parameters.AddWithValue("@Laboratorio", l.LaboratorioNombre);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ELIMINAR
        public void Eliminar(int id)
        {
            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpLaboratorios", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Accion", "DELETE");
                cmd.Parameters.AddWithValue("@IdLaboratorio", id);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
