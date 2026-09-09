using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages
{
    public class CategoriasModel : PageModel
    {
        private readonly CategoriaDAL _dal;

        public CategoriasModel(CategoriaDAL dal)
        {
            _dal = dal;
        }

        public List<Categoria> Listado { get; set; } = new();

        [BindProperty]
        public Categoria Categoria { get; set; } = new();

        public IActionResult OnGet(int? id, int? eliminar)
        {
            if (eliminar != null)
            {
                _dal.Eliminar(eliminar.Value);
                TempData["Ok"] = "Categoría eliminada correctamente.";
                return RedirectToPage("/Categorias");
            }

            if (id != null)
            {
                Categoria = _dal.Obtener(id.Value);
                TempData["Editar"] = "1";
            }

            Listado = _dal.Listar();

            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                bool nuevo = Categoria.IdCategoria == 0;

                _dal.Guardar(Categoria);

                TempData["Ok"] = nuevo ?
                    "Categoría creada correctamente." :
                    "Categoría actualizada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }

            return RedirectToPage("/Categorias");
        }
    }
}

