using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages.Productos
{
    public class NuevoModel : PageModel
    {
        private readonly ProductoDAL _productoDal;
        private readonly CategoriaDAL _categoriaDal;
        private readonly ProveedorDAL _proveedorDal;
        private readonly UnidadDAL _unidadDal;

        public NuevoModel(ProductoDAL productoDal, CategoriaDAL categoriaDal, ProveedorDAL proveedorDal, UnidadDAL unidadDal)
        {
            _productoDal = productoDal;
            _categoriaDal = categoriaDal;
            _proveedorDal = proveedorDal;
            _unidadDal = unidadDal;
        }

        public List<Producto> Listado { get; set; } = new();

        public List<Categoria> Categorias { get; set; }
        public List<Proveedor> Proveedores { get; set; }
        public List<UnidadMedida> Unidades { get; set; }

        [BindProperty]
        public Producto Producto { get; set; } = new();

        public string BuscarTexto { get; set; }

        public void OnGet(int? id, int? eliminar, string buscar)
        {
            // ELIMINAR
            if (eliminar != null)
            {
                _productoDal.Eliminar(eliminar.Value);
                TempData["Ok"] = "Producto eliminado correctamente.";
                Response.Redirect("/Productos/Nuevo");
                return;
            }

            // EDITAR
            if (id != null)
            {
                Producto = _productoDal.Obtener(id.Value);
                TempData["Editar"] = "1";
            }

            // FILTRO
            BuscarTexto = buscar ?? "";

            if (!string.IsNullOrEmpty(BuscarTexto))
                Listado = _productoDal.BuscarProductos(BuscarTexto);
            else
                Listado = _productoDal.BuscarProductos("");

            // COMBOS
            CargarCombos();
        }

        public IActionResult OnPost()
        {
            try
            {
                bool nuevo = Producto.IdProducto == 0;

                _productoDal.Guardar(Producto);

                TempData["Ok"] = nuevo
                    ? "Producto creado correctamente."
                    : "Producto actualizado correctamente.";

                return RedirectToPage("Nuevo");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;

                CargarCombos();
                return Page();
            }
        }

        private void CargarCombos()
        {
            Categorias = _categoriaDal.Listar();
            Proveedores = _proveedorDal.Listar();
            Unidades = _unidadDal.Listar();
        }
    }
}


