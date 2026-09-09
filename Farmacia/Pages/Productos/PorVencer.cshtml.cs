using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages.Productos
{
    public class PorVencerModel : PageModel
    {
        private readonly DashboardDAL _dal;

        public List<ProductoVencerDTO> Lista { get; set; }

        public PorVencerModel(DashboardDAL dal)
        {
            _dal = dal;
        }

        public void OnGet()
        {
            Lista = _dal.ObtenerProductosPorVencerDetalle();
        }
    }
}
