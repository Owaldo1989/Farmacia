using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages.Laboratorios
{
    public class EliminarModel : PageModel
    {
        private readonly LaboratorioDAL _dal;
        public Laboratorio Lab { get; set; }

        public EliminarModel(LaboratorioDAL dal)
        {
            _dal = dal;
        }

        public void OnGet(int id)
        {
            Lab = _dal.Obtener(id);
        }

        public IActionResult OnPost(int id)
        {
            _dal.Eliminar(id);
            return RedirectToPage("Index");
        }
    }
}
