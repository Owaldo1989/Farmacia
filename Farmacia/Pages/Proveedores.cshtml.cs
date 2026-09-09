using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages
{
    public class ProveedoresModel : PageModel
    {
        private readonly ProveedorDAL _dal;

        public ProveedoresModel(ProveedorDAL dal)
        {
            _dal = dal;
        }

        public List<Proveedor> Listado { get; set; } = new();

        [BindProperty]
        public Proveedor Proveedor { get; set; } = new();

        public IActionResult OnGet(int? id, int? eliminar)
        {
            // ELIMINAR
            if (eliminar != null)
            {
                _dal.Eliminar(eliminar.Value);
                TempData["Ok"] = "Proveedor eliminado correctamente.";
                return RedirectToPage("/Proveedores");
            }

            // EDITAR
            if (id != null)
            {
                Proveedor = _dal.Obtener(id.Value);
                TempData["Editar"] = "1";
            }

            // LISTAR
            Listado = _dal.Listar();

            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                bool nuevo = Proveedor.IdProveedor == 0;

                _dal.Guardar(Proveedor);

                TempData["Ok"] = nuevo
                    ? "Proveedor creado correctamente."
                    : "Proveedor actualizado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }

            return RedirectToPage("/Proveedores");
        }
    }
}

