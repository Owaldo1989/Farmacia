using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages
{
    public class DenominacionesModel : PageModel
    {
        private readonly DenominacionDAL _dal;


        public DenominacionesModel(
            DenominacionDAL dal)
        {
            _dal = dal;
        }


        public List<Denominacion> Listado
        {
            get;
            set;
        } = new();


        [BindProperty]
        public Denominacion Denominacion
        {
            get;
            set;
        } = new()
        {
            Moneda = "NIO",
            Tipo = "B",
            Activo = true
        };


        public IActionResult OnGet(
            int? id)
        {
            if (!EsAdministrador())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            if (id.HasValue)
            {
                var denominacion =
                    _dal.Obtener(
                        id.Value
                    );


                if (denominacion != null)
                {
                    Denominacion =
                        denominacion;

                    TempData["Editar"] =
                        "1";
                }
            }


            Listado =
                _dal.Listar();


            return Page();
        }


        public IActionResult OnPostGuardar()
        {
            if (!EsAdministrador())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            try
            {
                ValidarDenominacion();


                bool nuevo =
                    Denominacion.IdDenominacion == 0;


                _dal.Guardar(
                    Denominacion
                );


                TempData["Ok"] =
                    nuevo
                        ? "Denominación creada correctamente."
                        : "Denominación actualizada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }


            return RedirectToPage(
                "/Denominaciones"
            );
        }


        public IActionResult OnPostCambiarEstado(
            int id,
            bool activo)
        {
            if (!EsAdministrador())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            try
            {
                _dal.CambiarEstado(
                    id,
                    activo
                );


                TempData["Ok"] =
                    activo
                        ? "Denominación activada correctamente."
                        : "Denominación inactivada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }


            return RedirectToPage(
                "/Denominaciones"
            );
        }


        private void ValidarDenominacion()
        {
            Denominacion.Moneda =
                (Denominacion.Moneda ?? "")
                .Trim()
                .ToUpperInvariant();

            Denominacion.Tipo =
                (Denominacion.Tipo ?? "")
                .Trim()
                .ToUpperInvariant();

            Denominacion.Descripcion =
                (Denominacion.Descripcion ?? "")
                .Trim();


            if (Denominacion.Moneda.Length != 3)
            {
                throw new Exception(
                    "Debe seleccionar una moneda válida."
                );
            }


            if (Denominacion.Tipo != "B" &&
                Denominacion.Tipo != "M")
            {
                throw new Exception(
                    "Debe seleccionar si la denominación es billete o moneda."
                );
            }


            if (Denominacion.Valor <= 0)
            {
                throw new Exception(
                    "El valor de la denominación debe ser mayor que cero."
                );
            }


            if (string.IsNullOrWhiteSpace(
                Denominacion.Descripcion))
            {
                throw new Exception(
                    "Debe indicar la descripción de la denominación."
                );
            }


            if (Denominacion.Descripcion.Length > 50)
            {
                throw new Exception(
                    "La descripción no puede superar 50 caracteres."
                );
            }
        }


        private bool EsAdministrador()
        {
            return Farmacia.Helpers.RolHelper.EsAdministrador(
                HttpContext.Session.GetString(
                    "Rol"
                )
            );
        }
    }
}
