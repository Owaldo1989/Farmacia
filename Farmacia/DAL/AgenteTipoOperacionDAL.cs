using Farmacia.Models.AgenteBancario;
using System.Data;
using System.Data.SqlClient;

namespace Farmacia.DAL
{
    public class AgenteTipoOperacionDAL
    {
        private readonly string _cn;


        public AgenteTipoOperacionDAL(
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

        public List<AgenteTipoOperacion> Listar(
            int? idAgente = null)
        {
            List<AgenteTipoOperacion> lista =
                new();


            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpAgenteTipoOperacion",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "LIST"
            );


            cmd.Parameters.AddWithValue(
                "@IdAgente",
                (object?)idAgente
                ?? DBNull.Value
            );


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

        public AgenteTipoOperacion? Obtener(
            int idTipoOperacion)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpAgenteTipoOperacion",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "GET"
            );


            cmd.Parameters.AddWithValue(
                "@IdTipoOperacion",
                idTipoOperacion
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
            AgenteTipoOperacion operacion)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpAgenteTipoOperacion",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                operacion.IdTipoOperacion == 0
                    ? "INSERT"
                    : "UPDATE"
            );


            cmd.Parameters.AddWithValue(
                "@IdTipoOperacion",
                operacion.IdTipoOperacion
            );


            cmd.Parameters.AddWithValue(
                "@IdAgente",
                operacion.IdAgente
            );


            cmd.Parameters.AddWithValue(
                "@CodigoOperacion",
                operacion.CodigoOperacion
            );


            cmd.Parameters.AddWithValue(
                "@NombreOperacion",
                operacion.NombreOperacion
            );


            cmd.Parameters.AddWithValue(
                "@EfectoEfectivo",
                operacion.EfectoEfectivo
            );


            cmd.Parameters.AddWithValue(
                "@EfectoSaldoAgente",
                operacion.EfectoSaldoAgente
            );


            cmd.Parameters.AddWithValue(
                "@RequiereReferencia",
                operacion.RequiereReferencia
            );


            cmd.Parameters.AddWithValue(
                "@Activo",
                operacion.Activo
            );


            cn.Open();

            cmd.ExecuteNonQuery();
        }


        // =====================================================
        // DESACTIVAR
        // =====================================================

        public void Desactivar(
            int idTipoOperacion)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpAgenteTipoOperacion",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "DELETE"
            );


            cmd.Parameters.AddWithValue(
                "@IdTipoOperacion",
                idTipoOperacion
            );


            cn.Open();

            cmd.ExecuteNonQuery();
        }


        // =====================================================
        // MAPEAR
        // =====================================================

        private static AgenteTipoOperacion Mapear(
            SqlDataReader dr)
        {
            return new AgenteTipoOperacion
            {
                IdTipoOperacion =
                    Convert.ToInt32(
                        dr["IdTipoOperacion"]
                    ),

                IdAgente =
                    Convert.ToInt32(
                        dr["IdAgente"]
                    ),

                CodigoOperacion =
                    dr["CodigoOperacion"]
                        .ToString()
                    ?? "",

                NombreOperacion =
                    dr["NombreOperacion"]
                        .ToString()
                    ?? "",

                EfectoEfectivo =
                    Convert.ToInt16(
                        dr["EfectoEfectivo"]
                    ),

                EfectoSaldoAgente =
                    Convert.ToInt16(
                        dr["EfectoSaldoAgente"]
                    ),

                RequiereReferencia =
                    Convert.ToBoolean(
                        dr["RequiereReferencia"]
                    ),

                Activo =
                    Convert.ToBoolean(
                        dr["Activo"]
                    ),

                FechaRegistro =
                    Convert.ToDateTime(
                        dr["FechaRegistro"]
                    ),

                CodigoAgente =
                    dr["CodigoAgente"]
                        .ToString()
                    ?? "",

                NombreAgente =
                    dr["NombreAgente"]
                        .ToString()
                    ?? ""
            };
        }
    }
}
