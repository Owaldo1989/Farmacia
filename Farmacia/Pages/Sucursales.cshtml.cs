using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages
{
    public class SucursalesModel : PageModel
    {
        private readonly SucursalDAL _dal;


        public SucursalesModel(
            SucursalDAL dal)
        {
            _dal = dal;
        }


        public List<Sucursal> Listado
        {
            get;
            set;
        } = new();


        [BindProperty]
        public Sucursal Sucursal
        {
            get;
            set;
        } = new();



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
                        "Sucursal desactivada correctamente.";
                }
                catch (Exception ex)
                {
                    TempData["Error"] =
                        ex.Message;
                }


                return RedirectToPage(
                    "/Sucursales"
                );
            }


            if (id.HasValue)
            {
                var sucursal =
                    _dal.Obtener(
                        id.Value
                    );


                if (sucursal != null)
                {
                    Sucursal =
                        sucursal;


                    TempData["Editar"] =
                        "1";
                }
            }


            Listado =
                _dal.Listar();


            return Page();
        }



        public IActionResult OnPost()
        {
            try
            {
                if (
                    string.IsNullOrWhiteSpace(
                        Sucursal.CodigoSucursal
                    )
                )
                {
                    throw new Exception(
                        "Debe especificar el código de la sucursal."
                    );
                }


                if (
                    string.IsNullOrWhiteSpace(
                        Sucursal.NombreSucursal
                    )
                )
                {
                    throw new Exception(
                        "Debe especificar el nombre de la sucursal."
                    );
                }


                bool nuevo =
                    Sucursal.IdSucursal == 0;


                _dal.Guardar(
                    Sucursal
                );


                TempData["Ok"] =
                    nuevo
                        ? "Sucursal creada correctamente."
                        : "Sucursal actualizada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }


            return RedirectToPage(
                "/Sucursales"
            );
        }
    }
}
