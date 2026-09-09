using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages.Laboratorios
{
    public class GuardarModel : PageModel
    {
        private readonly LaboratorioDAL _dal;

        [BindProperty]
        public Laboratorio Lab { get; set; }

        public GuardarModel(LaboratorioDAL dal)
        {
            _dal = dal;
        }

        public void OnGet(int? id)
        {
            Lab = id == null ? new Laboratorio() : _dal.Obtener(id.Value);
        }

        public IActionResult OnPost()
        {
            _dal.Guardar(Lab);
            return RedirectToPage("Index");
        }
    }
}
