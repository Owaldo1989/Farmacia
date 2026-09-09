using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Farmacia.Pages
{
    public class IndexModel : PageModel
    {

        public decimal UtilVenta { get; set; }
        public decimal UtilCosto { get; set; }
        public decimal UtilUtilidad { get; set; }

        private readonly DashboardDAL _dashboardDal;

        public decimal VentaDia { get; set; }
        public decimal VentaSemana { get; set; }
        public decimal VentaMes { get; set; }

        public int PorVencer { get; set; }
        public int SinStock { get; set; }
        public List<TopProductoDTO> TopProductos { get; set; }

        // ⭐ Para el gráfico
        public string TopLabelsJson { get; set; }
        public string TopDataJson { get; set; }

        // ⭐ JSON para la gráfica de dona
        public string DonaDataJson { get; set; }
        public string DonaLabelsJson { get; set; }

        public IndexModel(DashboardDAL dashboardDal)
        {
            _dashboardDal = dashboardDal;
        }

        public IActionResult OnGet()
        {

            var usuario = HttpContext.Session.GetString("Usuario");

            if (string.IsNullOrEmpty(usuario))
                return RedirectToPage("/Login");

            var ventas = _dashboardDal.ObtenerVentas();
            VentaDia = ventas.dia;
            VentaSemana = ventas.semana;
            VentaMes = ventas.mes;

            PorVencer = _dashboardDal.ObtenerPorVencer();
            SinStock = _dashboardDal.ObtenerSinStock();
            TopProductos = _dashboardDal.ObtenerTopProductos();

            var util = _dashboardDal.ObtenerUtilidadMes();
            UtilVenta = util.Venta;
            UtilCosto = util.Costo;
            UtilUtilidad = util.Utilidad;

            return Page();
        }
    }



    }
