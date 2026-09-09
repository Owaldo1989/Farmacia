using System.Data.SqlClient;
using System.Data;
using Farmacia.Models;

namespace Farmacia.DAL
{
    public class FacturaDAL
    {
        private readonly string _cn;

        public FacturaDAL(IConfiguration config)
        {
            _cn = config.GetConnectionString("BDFarmacia");
        }

        public void AnularFactura(int idFactura)
        {
            using var cn = new SqlConnection(_cn);
            using var cmd = new SqlCommand("SpFacturaAnular", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdFactura", idFactura);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public List<FacturaDetalleDTO> ObtenerDetalle(int idFactura)
        {
            var lista = new List<FacturaDetalleDTO>();

            using var cn = new SqlConnection(_cn);
            using var cmd = new SqlCommand(@"
        SELECT D.IdProducto, P.NombreProducto, P.CodBarra,
               D.Cantidad, D.Precio
        FROM tbFacturaDetalle D
        INNER JOIN tbProductos P ON D.IdProducto = P.IdProducto
        WHERE D.IdFactura = @ID", cn);

            cmd.Parameters.AddWithValue("@ID", idFactura);

            cn.Open();
            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new FacturaDetalleDTO
                {
                    IdProducto = dr.GetInt32(0),
                    NombreProducto = dr["NombreProducto"].ToString(),
                    CodBarra = dr["CodBarra"].ToString(),
                    Cantidad = dr.GetDecimal(3),
                    Precio = dr.GetDecimal(4)
                });
            }
            return lista;
        }

        public List<FacturaDTO> ListarFacturas(DateTime fechaInicio, DateTime fechaFin)
        {
            var lista = new List<FacturaDTO>();

            using var cn = new SqlConnection(_cn);
            using var cmd = new SqlCommand(@"
        SELECT IdFactura, IdFacturaCliente, Fecha, Paciente, Total, PagoCordoba, PagoDolar, Vuelto
        FROM tbFactura
        WHERE CONVERT(date, Fecha) BETWEEN @F1 AND @F2
        ORDER BY Fecha DESC", cn);

            cmd.Parameters.AddWithValue("@F1", fechaInicio);
            cmd.Parameters.AddWithValue("@F2", fechaFin);

            cn.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new FacturaDTO
                {
                    IdFactura = dr.GetInt32(0),
                    IdFacturaCliente = dr.GetGuid(1),
                    Fecha = dr.GetDateTime(2),
                    Paciente = dr["Paciente"]?.ToString(),
                    Total = dr.GetDecimal(4),
                    PagoCordoba = dr.GetDecimal(5),
                    PagoDolar = dr.GetDecimal(6),
                    Vuelto = dr.GetDecimal(7)
                });
            }

            return lista;
        }

        public FacturaCompleta ObtenerFacturaCompleta(int idFactura)
        {
            Console.WriteLine("→ Entrando a ObtenerFacturaCompleta. ID = " + idFactura);

            var factura = new FacturaCompleta();

            using var cn = new SqlConnection(_cn);
            using var cmd = new SqlCommand("SpObtenerFacturaCompleta", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdFactura", idFactura);

            cn.Open();

            using (var dr = cmd.ExecuteReader())
            {
                if (!dr.Read())
                {
                    Console.WriteLine("❌ NO SE ENCONTRÓ ENCABEZADO");
                    throw new Exception("No hay datos del encabezado.");
                }

                Console.WriteLine("✔ Encabezado leído correctamente");
                factura.NombreFarmacia = dr["NombreFarmacia"].ToString();
                factura.Direccion = dr["Direccion"].ToString();
                factura.Telefono = dr["Telefono"].ToString();
                factura.IdFactura = dr.GetInt32(dr.GetOrdinal("IdFactura"));
                factura.Fecha = dr.GetDateTime(dr.GetOrdinal("Fecha"));
                factura.Paciente = dr["Paciente"].ToString();
                factura.Total = dr.GetDecimal(dr.GetOrdinal("Total"));
                factura.PagoCordoba = dr.GetDecimal(dr.GetOrdinal("PagoCordoba"));
                factura.PagoDolar = dr.GetDecimal(dr.GetOrdinal("PagoDolar"));
                factura.Vuelto = dr.GetDecimal(dr.GetOrdinal("Vuelto"));
                Console.WriteLine($"→ Paciente: {factura.Paciente}, Total: {factura.Total}");

                if (!dr.NextResult())
                {
                    Console.WriteLine("❌ NO HAY DETALLE");
                    throw new Exception("No devolvió detalle.");
                }

                Console.WriteLine("✔ Leyendo detalle…");

                while (dr.Read())
                {
                    factura.Detalles.Add(new FacturaDetalleDTO
                    {
                        IdProducto = dr.GetInt32(dr.GetOrdinal("IdProducto")),
                        NombreProducto = dr["NombreProducto"].ToString(),
                        Cantidad = dr.GetDecimal(dr.GetOrdinal("Cantidad")),
                        Precio = dr.GetDecimal(dr.GetOrdinal("Precio"))
                    });
                }
            }

            Console.WriteLine($"✔ Factura cargada. Detalles = {factura.Detalles.Count}");

            return factura;
        }



        public int GuardarFactura(FacturaDTO factura, List<FacturaDetalleDTO> detalle)
        {
            using var cn = new SqlConnection(_cn);
            using var cmd = new SqlCommand("SpGuardarFactura", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdFacturaCliente", factura.IdFacturaCliente);
            cmd.Parameters.AddWithValue("@Fecha", factura.Fecha);
            cmd.Parameters.AddWithValue("@Paciente", factura.Paciente ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Total", factura.Total);
            cmd.Parameters.AddWithValue("@PagoCordoba", factura.PagoCordoba);
            cmd.Parameters.AddWithValue("@PagoDolar", factura.PagoDolar);
            cmd.Parameters.AddWithValue("@Vuelto", factura.Vuelto);

            // Tabla tipo para detalle
            var tvp = new DataTable();
            tvp.Columns.Add("IdProducto", typeof(int));
            tvp.Columns.Add("Cantidad", typeof(decimal));
            tvp.Columns.Add("Precio", typeof(decimal));

            foreach (var d in detalle)
            {
                tvp.Rows.Add(d.IdProducto, d.Cantidad, d.Precio);
            }

            var param = cmd.Parameters.AddWithValue("@DetalleFactura", tvp);
            param.SqlDbType = SqlDbType.Structured;
            param.TypeName = "dbo.TipoDetalleFactura";

            cn.Open();
            var result = cmd.ExecuteScalar();  // el SP devuelve IdFactura

            return Convert.ToInt32(result);
        }
    }
}
