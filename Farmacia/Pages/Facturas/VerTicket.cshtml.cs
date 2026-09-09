using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages.Facturas
{
    public class VerTicketModel : PageModel
    {
        public int FacturaId { get; set; }

        public void OnGet(int id)
        {
            FacturaId = id;
        }
    }
}
