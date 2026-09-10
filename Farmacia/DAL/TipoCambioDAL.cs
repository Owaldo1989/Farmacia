using Farmacia.Models;
using System.Data;
using System.Data.SqlClient;

namespace Farmacia.DAL
{
    public class TipoCambioDAL
    {
        private readonly string _cn;

        public TipoCambioDAL(IConfiguration config)
        {
            _cn = config.GetConnectionString("BDFarmacia");
        }


        public decimal ObtenerVigente(DateTime fecha)
        {
            using var cn = new SqlConnection(_cn);

            using var cmd =
                new SqlCommand(
                    "SpTipoCambioObtenerVigente",
                    cn
                );

            cmd.CommandType =
                CommandType.StoredProcedure;

            cmd.Parameters.Add(
                "@Fecha",
                SqlDbType.Date
            ).Value = fecha.Date;


            cn.Open();

            var resultado =
                cmd.ExecuteScalar();


            if (resultado == null ||
                resultado == DBNull.Value)
            {
                throw new Exception(
                    "No existe una tasa de cambio configurada."
                );
            }


            return Convert.ToDecimal(resultado);
        }

        // =====================================================
        // LISTAR HISTORIAL
        // =====================================================

        public List<TipoCambioDTO> Listar(
            int cantidad = 20)
        {
            var lista =
                new List<TipoCambioDTO>();


            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpTipoCambioListar",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.Add(
                "@Cantidad",
                SqlDbType.Int
            ).Value = cantidad;


            cn.Open();


            using var dr =
                cmd.ExecuteReader();


            while (dr.Read())
            {
                lista.Add(
                    new TipoCambioDTO
                    {
                        IdTipoCambio =
                            Convert.ToInt32(
                                dr["IdTipoCambio"]
                            ),

                        FechaDesde =
                            Convert.ToDateTime(
                                dr["FechaDesde"]
                            ),

                        Tasa =
                            Convert.ToDecimal(
                                dr["Tasa"]
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
        // GUARDAR NUEVA TASA
        // =====================================================

        public void Guardar(
            DateTime fechaDesde,
            decimal tasa)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpTipoCambioGuardar",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.Add(
                "@FechaDesde",
                SqlDbType.Date
            ).Value =
                fechaDesde.Date;


            var pTasa =
                cmd.Parameters.Add(
                    "@Tasa",
                    SqlDbType.Decimal
                );


            pTasa.Precision = 10;
            pTasa.Scale = 4;
            pTasa.Value = tasa;


            cn.Open();

            cmd.ExecuteNonQuery();
        }
    }
}
