using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages.Facturas
{
    public class ListaModel : PageModel
    {
        private readonly FacturaDAL _dal;

        public ListaModel(FacturaDAL dal)
        {
            _dal = dal;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime FechaInicio { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime FechaFin { get; set; }

        public List<FacturaDTO> Listado { get; set; }
        public JsonResult OnGetDetalle(int id)
        {
            var factura = _dal.ObtenerFacturaCompleta(id);

            return new JsonResult(new
            {
                fecha = factura.Fecha.ToString("dd/MM/yyyy hh:mm tt"),
                paciente = factura.Paciente,
                total = factura.Total.ToString("0.00"),
                pagoCordoba = factura.PagoCordoba.ToString("0.00"),
                pagoDolar = factura.PagoDolar.ToString("0.00"),
                vuelto = factura.Vuelto.ToString("0.00"),
                detalles = factura.Detalles.Select(d => new {
                    nombreProducto = d.NombreProducto,
                    codBarra = d.CodBarra,
                    cantidad = d.Cantidad.ToString("0.00"),
                    precio = d.Precio.ToString("0.00"),
                    subTotal = d.SubTotal.ToString("0.00")
                })
            });
        }


        public void OnGet()
        {
            if (FechaInicio == DateTime.MinValue)
                FechaInicio = DateTime.Today;

            if (FechaFin == DateTime.MinValue)
                FechaFin = DateTime.Today;

            Listado = _dal.ListarFacturas(FechaInicio, FechaFin);
        }

        public IActionResult OnGetAnular(int id)
        {
            // LUEGO HACEMOS ESTE MÉTODO
            return RedirectToPage();
        }
    }
}
