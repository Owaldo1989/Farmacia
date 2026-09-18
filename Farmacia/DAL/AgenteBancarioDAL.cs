using Farmacia.Models.AgenteBancario;
using System.Data;
using System.Data.SqlClient;

namespace Farmacia.DAL
{
    public class AgenteBancarioDAL
    {
        private readonly string _cn;

        public AgenteBancarioDAL(
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

        public List<AgenteBancario> Listar()
        {
            List<AgenteBancario> lista =
                new();


            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpAgentesBancarios",
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
                    Mapear(dr)
                );
            }


            return lista;
        }


        // =====================================================
        // OBTENER
        // =====================================================

        public AgenteBancario? Obtener(
            int idAgente)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpAgentesBancarios",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "GET"
            );


            cmd.Parameters.AddWithValue(
                "@IdAgente",
                idAgente
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
            AgenteBancario agente)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpAgentesBancarios",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                agente.IdAgente == 0
                    ? "INSERT"
                    : "UPDATE"
            );


            cmd.Parameters.AddWithValue(
                "@IdAgente",
                agente.IdAgente
            );


            cmd.Parameters.AddWithValue(
                "@IdCaja",
                agente.IdCaja
            );


            cmd.Parameters.AddWithValue(
                "@CodigoAgente",
                agente.CodigoAgente
            );


            cmd.Parameters.AddWithValue(
                "@NombreAgente",
                agente.NombreAgente
            );


            var pSaldo =
                cmd.Parameters.Add(
                    "@SaldoInicial",
                    SqlDbType.Decimal
                );

            pSaldo.Precision = 18;
            pSaldo.Scale = 2;

            pSaldo.Value =
                agente.SaldoInicial;


            cmd.Parameters.AddWithValue(
                "@Activo",
                agente.Activo
            );


            cn.Open();

            cmd.ExecuteNonQuery();
        }


        // =====================================================
        // DESACTIVAR
        // =====================================================

        public void Desactivar(
            int idAgente)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpAgentesBancarios",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue(
                "@Accion",
                "DELETE"
            );


            cmd.Parameters.AddWithValue(
                "@IdAgente",
                idAgente
            );


            cn.Open();

            cmd.ExecuteNonQuery();
        }


        // =====================================================
        // MAPEAR
        // =====================================================

        private static AgenteBancario Mapear(
            SqlDataReader dr)
        {
            return new AgenteBancario
            {
                IdAgente =Convert.ToInt32(dr["IdAgente"]),
                IdCaja =Convert.ToInt32(dr["IdCaja"]),
                CodigoAgente =dr["CodigoAgente"].ToString()?? "",
                NombreAgente =dr["NombreAgente"].ToString()?? "",
                SaldoInicial =Convert.ToDecimal(dr["SaldoInicial"]),
                Activo =Convert.ToBoolean(dr["Activo"]),
                FechaRegistro =Convert.ToDateTime(dr["FechaRegistro"]),
                CodigoCaja =dr["CodigoCaja"].ToString()?? "",
                NombreCaja =dr["NombreCaja"].ToString()?? "",
                IdSucursal =Convert.ToInt32(dr["IdSucursal"]),
                CodBodega =Convert.ToInt32(dr["CodBodega"]),
                CodigoSucursal =dr["CodigoSucursal"].ToString()?? "",
                NombreSucursal =dr["NombreSucursal"].ToString()?? "",
                CodigoBodega =dr["CodigoBodega"].ToString()?? "",
                NombreBodega =dr["NombreBodega"].ToString()?? ""
            };
        }
    }
}
