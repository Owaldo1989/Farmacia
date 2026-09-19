using System.Data;
using System.Data.SqlClient;
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

        public List<FormaPagoDTO> ListarFormasPagoActivas()
        {
            var lista = new List<FormaPagoDTO>();

            using var cn = new SqlConnection(_cn);
            using var cmd = new SqlCommand(@"
                SELECT IdFormaPago, Codigo, Nombre, EsEfectivo, RequiereReferencia, PermiteVuelto, Activo
                FROM dbo.tbFormasPago
                WHERE Activo = 1
                ORDER BY
                    CASE WHEN EsEfectivo = 1 THEN 0 ELSE 1 END,
                    Nombre;", cn);

            cn.Open();
            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new FormaPagoDTO
                {
                    IdFormaPago = dr.GetInt32(dr.GetOrdinal("IdFormaPago")),
                    Codigo = dr["Codigo"]?.ToString() ?? string.Empty,
                    Nombre = dr["Nombre"]?.ToString() ?? string.Empty,
                    EsEfectivo = dr.GetBoolean(dr.GetOrdinal("EsEfectivo")),
                    RequiereReferencia = dr.GetBoolean(dr.GetOrdinal("RequiereReferencia")),
                    PermiteVuelto = dr.GetBoolean(dr.GetOrdinal("PermiteVuelto")),
                    Activo = dr.GetBoolean(dr.GetOrdinal("Activo"))
                });
            }

            return lista;
        }

        public List<FacturaDetalleDTO> ObtenerDetalle(int idFactura)
        {
            var lista = new List<FacturaDetalleDTO>();

            using var cn = new SqlConnection(_cn);
            using var cmd = new SqlCommand(@"
                SELECT D.IdProducto, P.NombreProducto, P.CodBarra, D.Cantidad, D.Precio
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
                SELECT IdFactura, IdFacturaCliente, Fecha, Paciente, Total, PagoCordoba, PagoDolar, Vuelto, TasaCambio
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
                    Vuelto = dr.GetDecimal(7),
                    TasaCambio = dr.GetDecimal(8)
                });
            }

            return lista;
        }

        public FacturaCompleta ObtenerFacturaCompleta(int idFactura)
        {
            var factura = new FacturaCompleta();

            using var cn = new SqlConnection(_cn);
            using var cmd = new SqlCommand("SpObtenerFacturaCompleta", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdFactura", idFactura);

            cn.Open();

            using var dr = cmd.ExecuteReader();

            if (!dr.Read())
            {
                throw new Exception("No hay datos del encabezado.");
            }

            factura.NombreFarmacia = dr["NombreFarmacia"].ToString();
            factura.Direccion = dr["Direccion"].ToString();
            factura.Telefono = dr["Telefono"].ToString();
            factura.IdFactura = dr.GetInt32(dr.GetOrdinal("IdFactura"));
            factura.Fecha = dr.GetDateTime(dr.GetOrdinal("Fecha"));
            factura.TasaCambio = dr.GetDecimal(dr.GetOrdinal("TasaCambio"));
            factura.Paciente = dr["Paciente"].ToString();
            factura.Total = dr.GetDecimal(dr.GetOrdinal("Total"));
            factura.PagoCordoba = dr.GetDecimal(dr.GetOrdinal("PagoCordoba"));
            factura.PagoDolar = dr.GetDecimal(dr.GetOrdinal("PagoDolar"));
            factura.Vuelto = dr.GetDecimal(dr.GetOrdinal("Vuelto"));
            factura.VueltoCordoba = HasColumn(dr, "VueltoCordoba")
                ? dr.GetDecimal(dr.GetOrdinal("VueltoCordoba"))
                : factura.Vuelto;
            factura.VueltoDolar = HasColumn(dr, "VueltoDolar")
                ? dr.GetDecimal(dr.GetOrdinal("VueltoDolar"))
                : 0;

            if (!dr.NextResult())
            {
                throw new Exception("No devolvio detalle.");
            }

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

            if (dr.NextResult())
            {
                while (dr.Read())
                {
                    factura.Pagos.Add(new FacturaPagoDTO
                    {
                        IdFacturaPago = dr.GetInt64(dr.GetOrdinal("IdFacturaPago")),
                        IdFactura = dr.GetInt32(dr.GetOrdinal("IdFactura")),
                        IdFormaPago = dr.GetInt32(dr.GetOrdinal("IdFormaPago")),
                        CodigoFormaPago = dr["Codigo"]?.ToString() ?? string.Empty,
                        NombreFormaPago = dr["Nombre"]?.ToString() ?? string.Empty,
                        Moneda = dr["Moneda"]?.ToString()?.Trim() ?? "NIO",
                        Monto = dr.GetDecimal(dr.GetOrdinal("Monto")),
                        TasaCambio = dr.GetDecimal(dr.GetOrdinal("TasaCambio")),
                        Referencia = dr["Referencia"] == DBNull.Value ? null : dr["Referencia"].ToString(),
                        EsEfectivo = dr.GetBoolean(dr.GetOrdinal("EsEfectivo")),
                        RequiereReferencia = dr.GetBoolean(dr.GetOrdinal("RequiereReferencia")),
                        PermiteVuelto = dr.GetBoolean(dr.GetOrdinal("PermiteVuelto"))
                    });
                }
            }

            return factura;
        }

        public int GuardarFactura(
            FacturaDTO factura,
            List<FacturaDetalleDTO> detalle,
            List<FacturaPagoDTO> pagos)
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
            cmd.Parameters.AddWithValue("@VueltoCordoba", factura.VueltoCordoba);
            cmd.Parameters.AddWithValue("@VueltoDolar", factura.VueltoDolar);
            cmd.Parameters.AddWithValue("@IdTurno", factura.IdTurno ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IdUsuario", factura.IdUsuario ?? (object)DBNull.Value);

            var tasaParam = cmd.Parameters.Add("@TasaCambio", SqlDbType.Decimal);
            tasaParam.Precision = 10;
            tasaParam.Scale = 4;
            tasaParam.Value = factura.TasaCambio;

            var detalleTvp = new DataTable();
            detalleTvp.Columns.Add("IdProducto", typeof(int));
            detalleTvp.Columns.Add("Cantidad", typeof(decimal));
            detalleTvp.Columns.Add("Precio", typeof(decimal));

            foreach (var d in detalle)
            {
                detalleTvp.Rows.Add(d.IdProducto, d.Cantidad, d.Precio);
            }

            var detalleParam = cmd.Parameters.AddWithValue("@DetalleFactura", detalleTvp);
            detalleParam.SqlDbType = SqlDbType.Structured;
            detalleParam.TypeName = "dbo.TipoDetalleFactura";

            var pagosTvp = new DataTable();
            pagosTvp.Columns.Add("IdFormaPago", typeof(int));
            pagosTvp.Columns.Add("Moneda", typeof(string));
            pagosTvp.Columns.Add("Monto", typeof(decimal));
            pagosTvp.Columns.Add("Referencia", typeof(string));

            foreach (var p in pagos)
            {
                pagosTvp.Rows.Add(
                    p.IdFormaPago,
                    (p.Moneda ?? "NIO").Trim().ToUpperInvariant(),
                    p.Monto,
                    string.IsNullOrWhiteSpace(p.Referencia)
                        ? DBNull.Value
                        : p.Referencia.Trim());
            }

            var pagosParam = cmd.Parameters.AddWithValue("@PagosFactura", pagosTvp);
            pagosParam.SqlDbType = SqlDbType.Structured;
            pagosParam.TypeName = "dbo.TipoPagoFactura";

            cn.Open();
            var result = cmd.ExecuteScalar();

            return Convert.ToInt32(result);
        }

        private static bool HasColumn(IDataRecord reader, string columnName)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (string.Equals(reader.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
