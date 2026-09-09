using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages
{
    public class UnidadesModel : PageModel
    {
        private readonly UnidadDAL _dal;

        public UnidadesModel(UnidadDAL dal)
        {
            _dal = dal;
        }

        public List<UnidadMedida> Listado { get; set; } = new();

        [BindProperty]
        public UnidadMedida Unidad { get; set; } = new();

        public IActionResult OnGet(int? id, int? eliminar)
        {
            // ELIMINAR
            if (eliminar != null)
            {
                _dal.Eliminar(eliminar.Value);
                TempData["Ok"] = "Unidad eliminada correctamente.";
                return RedirectToPage("/Unidades");
            }

            // EDITAR (CARGAR DATOS)
            if (id != null)
            {
                Unidad = _dal.Obtener(id.Value);
                TempData["Editar"] = "1"; // Para abrir modal en la vista
            }

            // LISTADO SIEMPRE
            Listado = _dal.Listar();

            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                bool esNuevo = Unidad.IdUnidad == 0;

                _dal.Guardar(Unidad);

                TempData["Ok"] = esNuevo
                    ? "Unidad creada correctamente."
                    : "Unidad actualizada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error: " + ex.Message;
            }

            return RedirectToPage("/Unidades");
        }

    }
}
