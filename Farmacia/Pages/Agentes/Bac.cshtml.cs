using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages.Agentes
{
    public class BacModel : PageModel
    {
        public IActionResult OnGet()
        {
            return RedirectToPage(
                "/Agentes/Operaciones",
                new
                {
                    codigo = "BAC"
                }
            );
        }
    }
}
