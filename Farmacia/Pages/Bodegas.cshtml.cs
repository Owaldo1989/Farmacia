using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages
{
    public class BodegasModel : PageModel
    {
        private readonly BodegaDAL _dal;

        private readonly SucursalDAL _sucursalDal;


        public BodegasModel(
            BodegaDAL dal,
            SucursalDAL sucursalDal)
        {
            _dal = dal;

            _sucursalDal =
                sucursalDal;
        }



        public List<Bodega> Listado
        {
            get;
            set;
        } = new();


        public List<Sucursal> Sucursales
        {
            get;
            set;
        } = new();


        [BindProperty]
        public Bodega Bodega
        {
            get;
            set;
        } = new();



        // =====================================================
        // GET
        // =====================================================

        public IActionResult OnGet(
            int? id,
            int? eliminar)
        {
            if (
                string.IsNullOrWhiteSpace(
                    HttpContext.Session.GetString(
                        "Usuario"
                    )
                )
            )
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            if (eliminar.HasValue)
            {
                try
                {
                    _dal.Eliminar(
                        eliminar.Value
                    );


                    TempData["Ok"] =
                        "Bodega desactivada correctamente.";
                }
                catch (Exception ex)
                {
                    TempData["Error"] =
                        ex.Message;
                }


                return RedirectToPage(
                    "/Bodegas"
                );
            }


            if (id.HasValue)
            {
                var bodega =
                    _dal.Obtener(
                        id.Value
                    );


                if (bodega != null)
                {
                    Bodega =
                        bodega;


                    TempData["Editar"] =
                        "1";
                }
            }


            CargarDatos();


            return Page();
        }



        // =====================================================
        // POST
        // =====================================================

        public IActionResult OnPost()
        {
            try
            {
                if (
                    Bodega.IdSucursal <= 0
                )
                {
                    throw new Exception(
                        "Debe seleccionar una sucursal."
                    );
                }


                if (
                    string.IsNullOrWhiteSpace(
                        Bodega.CodigoBodega
                    )
                )
                {
                    throw new Exception(
                        "Debe especificar el código de la bodega."
                    );
                }


                if (
                    string.IsNullOrWhiteSpace(
                        Bodega.NombreBodega
                    )
                )
                {
                    throw new Exception(
                        "Debe especificar el nombre de la bodega."
                    );
                }


                bool nueva =
                    Bodega.CodBodega == 0;


                _dal.Guardar(
                    Bodega
                );


                TempData["Ok"] =
                    nueva
                        ? "Bodega creada correctamente."
                        : "Bodega actualizada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }


            return RedirectToPage(
                "/Bodegas"
            );
        }



        // =====================================================
        // CARGAR
        // =====================================================

        private void CargarDatos()
        {
            Listado =
                _dal.Listar();


            Sucursales =
                _sucursalDal
                    .Listar()
                    .Where(
                        x => x.Activo
                    )
                    .OrderBy(
                        x => x.NombreSucursal
                    )
                    .ToList();
        }
    }
}
