using Farmacia.Models;
using System.Data.SqlClient;
using System.Data;

namespace Farmacia.DAL
{
    public class DashboardDAL
    {
        private readonly string _cn;

        public DashboardDAL(IConfiguration config)
        {
            _cn = config.GetConnectionString("BDFarmacia");
        }
        public List<ProductoVencerDTO> ObtenerProductosPorVencerDetalle()
        {
            var lista = new List<ProductoVencerDTO>();

            using var con = new SqlConnection(_cn);
            using var cmd = new SqlCommand("SpProductosPorVencerDetalle", con);

            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();

            using var rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                lista.Add(new ProductoVencerDTO
                {
                    CodBarra = rd.GetString(0),
                    Nombre = rd.GetString(1),
                    FechaVence = rd.GetDateTime(2),
                    DiasRestantes = rd.GetInt32(3),
                    Stock = rd.GetInt32(4)
                });
            }

            return lista;
        }



        // ✅ Obtener ventas del dashboard
        public (decimal dia, decimal semana, decimal mes) ObtenerVentas()
        {
            using var con = new SqlConnection(_cn);
            using var cmd = new SqlCommand("SpDashboardVentas", con);

            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();
            using var rd = cmd.ExecuteReader();

            if (rd.Read())
            {
                return (
                    rd.GetDecimal(0),
                    rd.GetDecimal(1),
                    rd.GetDecimal(2)
                );
            }

            return (0, 0, 0);
        }

        // ✅ Próximos a vencer
        public int ObtenerPorVencer()
        {
            using var con = new SqlConnection(_cn);
            using var cmd = new SqlCommand("SpProductosPorVencer", con);

            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // ✅ Sin stock
        public int ObtenerSinStock()
        {
            using var con = new SqlConnection(_cn);
            using var cmd = new SqlCommand("SpProductosSinStock", con);

            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // ✅ Utilidad del mes
        public (decimal Venta, decimal Costo, decimal Utilidad) ObtenerUtilidadMes()
        {
            using var con = new SqlConnection(_cn);
            using var cmd = new SqlCommand("SpUtilidadMes", con);

            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();

            using var rd = cmd.ExecuteReader();

            if (rd.Read())
            {
                return (
                    rd.IsDBNull(0) ? 0 : rd.GetDecimal(0), // Venta
                    rd.IsDBNull(1) ? 0 : rd.GetDecimal(1), // Costo
                    rd.IsDBNull(2) ? 0 : rd.GetDecimal(2)  // Utilidad
                );
            }

            return (0, 0, 0);
        }


        // ✅ Top productos
        public List<TopProductoDTO> ObtenerTopProductos()
        {
            var lista = new List<TopProductoDTO>();

            using var con = new SqlConnection(_cn);
            using var cmd = new SqlCommand("SpTopProductos", con);

            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();
            using var rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                lista.Add(new TopProductoDTO
                {
                    CodBarra = rd.GetString(0),
                    SubTotal = rd.GetDecimal(1),
                    Unidades = rd.GetInt32(2)
                });
            }

            return lista;
        }
    }
}
