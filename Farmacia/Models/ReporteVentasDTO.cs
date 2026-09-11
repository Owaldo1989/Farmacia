namespace Farmacia.Models
{
    public class ReporteResumenVentasDTO
    {
        public int Facturas { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal UnidadesVendidas { get; set; }
        public int ProductosVendidos { get; set; }
    }

    public class ReporteVentasFilaDTO
    {
        public string Etiqueta { get; set; } = "";
        public string Codigo { get; set; } = "";
        public decimal Unidades { get; set; }
        public int Facturas { get; set; }
        public decimal TotalVenta { get; set; }
        public decimal Inventario { get; set; }
        public DateTime? UltimaVenta { get; set; }
    }
}
