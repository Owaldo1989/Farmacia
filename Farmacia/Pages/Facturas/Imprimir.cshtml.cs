using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages.Facturas
{
    public class ImprimirModel : PageModel
    {
        private readonly FacturaDAL _facturaDal;

        public ImprimirModel(FacturaDAL facturaDal)
        {
            _facturaDal = facturaDal;
        }

        public FacturaCompleta Factura { get; set; }

        public IActionResult OnGet(int id)
        {
            Factura = _facturaDal.ObtenerFacturaCompleta(id);

            if (Factura == null)
                return RedirectToPage("/Facturas/Nueva");

            return Page();
        }
    }
}
