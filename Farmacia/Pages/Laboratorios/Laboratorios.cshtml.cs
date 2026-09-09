using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages.Laboratorios
{
    public class LaboratoriosModel : PageModel
    {
        private readonly LaboratorioDAL _dal;
        public List<Laboratorio> Lista { get; set; }

        public LaboratoriosModel(LaboratorioDAL dal)
        {
            _dal = dal;
        }

        public void OnGet()
        {
            Lista = _dal.Listar();
        }
    }
}
