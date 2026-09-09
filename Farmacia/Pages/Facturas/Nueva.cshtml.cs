using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Farmacia.Pages.Facturas
{
    public class NuevaModel : PageModel
    {
        private readonly FacturaDAL _facturaDal;
        private readonly ProductoDAL _productoDal;

        public NuevaModel(FacturaDAL facturaDal, ProductoDAL productoDal)
        {
            _facturaDal = facturaDal;
            _productoDal = productoDal;
        }

        [BindProperty]
        public FacturaDTO Factura { get; set; }

        // JSON con el detalle que viene del front
        [BindProperty]
        public string DetalleJson { get; set; }

        public void OnGet()
        {
            Factura = new FacturaDTO
            {
                IdFacturaCliente = Guid.NewGuid(),
                Fecha = DateTime.Now
            };
        }

        // Handler para buscar productos (devuelve JSON)
        public JsonResult OnGetBuscarProductos(string filtro)
        {
            var lista = _productoDal.BuscarParaFactura(filtro ?? "");
            return new JsonResult(lista);
        }

        public IActionResult OnPost()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(DetalleJson))
                {
                    TempData["Error"] = "La factura no tiene productos.";
                    return Page();
                }

                var detalle = JsonSerializer.Deserialize<List<FacturaDetalleDTO>>(DetalleJson);

                if (detalle == null || detalle.Count == 0)
                {
                    TempData["Error"] = "La factura no tiene productos válidos.";
                    return Page();
                }

                // Calcular total en servidor
                Factura.Total = detalle.Sum(d => d.Cantidad * d.Precio);

                int idFactura = _facturaDal.GuardarFactura(Factura, detalle);

                return RedirectToPage("/Facturas/VerTicket", new { id = idFactura });


                TempData["Ok"] = $"Factura guardada correctamente. No: {idFactura}";
                // Aquí luego puedes redirigir a página de impresión, etc.
                return RedirectToPage("/Facturas/Nueva");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al guardar factura: " + ex.Message;
                return Page();
            }
        }
    }
}
