using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages
{
    public class ReportesModel : PageModel
    {
        private readonly ReporteDAL _reporteDal;

        private static readonly HashSet<string> TiposValidos = new()
        {
            "ventas-dia", "ventas-mes", "producto", "categoria",
            "proveedor", "laboratorio", "mas-vendidos", "poca-rotacion"
        };

        public ReportesModel(ReporteDAL reporteDal)
        {
            _reporteDal = reporteDal;
        }

        [BindProperty(SupportsGet = true, Name = "fechaInicio")]
        public DateTime FechaInicio { get; set; }

        [BindProperty(SupportsGet = true, Name = "fechaFin")]
        public DateTime FechaFin { get; set; }

        [BindProperty(SupportsGet = true, Name = "tipo")]
        public string TipoReporte { get; set; } = "ventas-dia";

        [BindProperty(SupportsGet = true, Name = "limiteRotacion")]
        public decimal LimiteRotacion { get; set; } = 5m;

        public ReporteResumenVentasDTO Resumen { get; private set; } = new();
        public List<ReporteVentasFilaDTO> Filas { get; private set; } = new();

        public bool EsReporteProducto =>
            TipoReporte is "producto" or "mas-vendidos" or "poca-rotacion";

        public bool EsReporteTemporal =>
            TipoReporte is "ventas-dia" or "ventas-mes";

        public string TituloReporte => TipoReporte switch
        {
            "ventas-mes" => "Ventas por mes",
            "producto" => "Ventas por producto",
            "categoria" => "Ventas por categoría",
            "proveedor" => "Ventas por proveedor",
            "laboratorio" => "Ventas por laboratorio",
            "mas-vendidos" => "Productos más vendidos",
            "poca-rotacion" => "Productos con poca rotación",
            _ => "Ventas por día"
        };

        public string DescripcionReporte => TipoReporte switch
        {
            "ventas-mes" => "Consolidado mensual de unidades, facturas e ingresos.",
            "producto" => "Comportamiento de venta de cada producto en el período.",
            "categoria" => "Ventas agrupadas por la categoría actual del producto.",
            "proveedor" => "Ventas agrupadas por el proveedor actual del producto.",
            "laboratorio" => "Ventas agrupadas por el laboratorio asignado al producto.",
            "mas-vendidos" => "Los 10 productos con mayor cantidad de unidades vendidas.",
            "poca-rotacion" => "Productos con existencia y pocas o ninguna venta durante el período.",
            _ => "Cierre diario de unidades, facturas e ingresos."
        };

        public void OnGet()
        {
            if (FechaInicio == DateTime.MinValue)
                FechaInicio = DateTime.Today.AddDays(-30);

            if (FechaFin == DateTime.MinValue)
                FechaFin = DateTime.Today;

            if (FechaFin < FechaInicio)
                (FechaInicio, FechaFin) = (FechaFin, FechaInicio);

            if (!TiposValidos.Contains(TipoReporte))
                TipoReporte = "ventas-dia";

            if (LimiteRotacion < 0)
                LimiteRotacion = 0;

            Resumen = _reporteDal.ObtenerResumen(FechaInicio, FechaFin);
            Filas = _reporteDal.ObtenerReporte(
                TipoReporte,
                FechaInicio,
                FechaFin,
                LimiteRotacion);
        }
    }
}
