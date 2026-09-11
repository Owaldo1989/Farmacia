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
        private readonly LaboratorioDAL _laboratorioDal;
        private readonly IWebHostEnvironment _environment;

        private const long MaxFotoBytes = 5 * 1024 * 1024;
        private static readonly HashSet<string> ExtensionesFotoPermitidas =
            new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

        public NuevoModel(
            ProductoDAL productoDal,
            CategoriaDAL categoriaDal,
            ProveedorDAL proveedorDal,
            UnidadDAL unidadDal,
            LaboratorioDAL laboratorioDal,
            IWebHostEnvironment environment)
        {
            _productoDal = productoDal;
            _categoriaDal = categoriaDal;
            _proveedorDal = proveedorDal;
            _unidadDal = unidadDal;
            _laboratorioDal = laboratorioDal;
            _environment = environment;
        }

        public List<Producto> Listado { get; set; } = new();

        public List<Categoria> Categorias { get; set; }
        public List<Proveedor> Proveedores { get; set; }
        public List<UnidadMedida> Unidades { get; set; }
        public List<Laboratorio> Laboratorios { get; set; }

        [BindProperty]
        public Producto Producto { get; set; } = new();

        [BindProperty]
        public IFormFile? FotoArchivo { get; set; }

        public string BuscarTexto { get; set; }

        public void OnGet(int? id, int? eliminar, string buscar)
        {
            // ELIMINAR
            if (eliminar != null)
            {
                var productoEliminado = _productoDal.Obtener(eliminar.Value);
                _productoDal.Eliminar(eliminar.Value);
                EliminarFoto(productoEliminado?.Foto);
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

        public async Task<IActionResult> OnPostAsync()
        {
            string? fotoNueva = null;

            try
            {
                bool nuevo = Producto.IdProducto == 0;

                if (Producto.PorcentajeUtilidad < 0)
                    throw new InvalidOperationException(
                        "El porcentaje de utilidad no puede ser negativo.");

                if (Producto.TasaIVA != 0m && Producto.TasaIVA != 15m)
                    throw new InvalidOperationException(
                        "La tasa de IVA debe ser 15% o 0% para un producto exento.");

                var productoAnterior = nuevo
                    ? null
                    : _productoDal.Obtener(Producto.IdProducto);

                if (FotoArchivo is not null && FotoArchivo.Length > 0)
                {
                    var extension = Path.GetExtension(FotoArchivo.FileName);

                    if (!ExtensionesFotoPermitidas.Contains(extension))
                        throw new InvalidOperationException(
                            "La foto debe ser JPG, PNG o WebP.");

                    if (FotoArchivo.Length > MaxFotoBytes)
                        throw new InvalidOperationException(
                            "La foto no puede superar los 5 MB.");

                    var nombreArchivo = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
                    var carpeta = Path.Combine(
                        _environment.WebRootPath,
                        "uploads",
                        "productos");

                    Directory.CreateDirectory(carpeta);
                    var rutaFisica = Path.Combine(carpeta, nombreArchivo);

                    await using (var stream = System.IO.File.Create(rutaFisica))
                    {
                        await FotoArchivo.CopyToAsync(stream);
                    }

                    fotoNueva = $"/uploads/productos/{nombreArchivo}";
                    Producto.Foto = fotoNueva;
                }

                _productoDal.Guardar(Producto);

                if (productoAnterior?.Foto != Producto.Foto)
                    EliminarFoto(productoAnterior?.Foto);

                TempData["Ok"] = nuevo
                    ? "Producto creado correctamente."
                    : "Producto actualizado correctamente.";

                return RedirectToPage("Nuevo");
            }
            catch (Exception ex)
            {
                if (fotoNueva is not null)
                    EliminarFoto(fotoNueva);

                TempData["Error"] = "Error: " + ex.Message;

                CargarCombos();
                Listado = _productoDal.BuscarProductos("");
                return Page();
            }
        }

        private void CargarCombos()
        {
            Categorias = _categoriaDal.Listar();
            Proveedores = _proveedorDal.Listar();
            Unidades = _unidadDal.Listar();
            Laboratorios = _laboratorioDal.Listar();
        }

        private void EliminarFoto(string? rutaFoto)
        {
            const string prefijoPermitido = "/uploads/productos/";

            if (string.IsNullOrWhiteSpace(rutaFoto) ||
                !rutaFoto.StartsWith(prefijoPermitido, StringComparison.OrdinalIgnoreCase))
                return;

            var nombreArchivo = Path.GetFileName(rutaFoto);
            var rutaFisica = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "productos",
                nombreArchivo);

            if (System.IO.File.Exists(rutaFisica))
                System.IO.File.Delete(rutaFisica);
        }
    }
}


