using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Farmacia.Pages.Configuracion
{
    public class TipoCambioModel : PageModel
    {
        private readonly TipoCambioDAL _tipoCambioDal;


        public TipoCambioModel(
            TipoCambioDAL tipoCambioDal)
        {
            _tipoCambioDal =
                tipoCambioDal;
        }


        // =====================================================
        // CAMPOS DEL FORMULARIO
        // =====================================================

        [BindProperty]
        public DateTime FechaDesde { get; set; }


        [BindProperty]
        public decimal Tasa { get; set; }



        // =====================================================
        // INFORMACIÓN
        // =====================================================

        public decimal TasaVigente { get; set; }


        public List<TipoCambioDTO> Historial
        {
            get;
            set;
        } = new();



        // =====================================================
        // GET
        // =====================================================

        public IActionResult OnGet()
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            if (!EsAdministrador())
            {
                return RedirectToPage(
                    "/Index"
                );
            }


            FechaDesde =
                DateTime.Today;


            CargarInformacion();


            return Page();
        }



        // =====================================================
        // POST
        // =====================================================

        public IActionResult OnPost()
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            if (!EsAdministrador())
            {
                return RedirectToPage(
                    "/Index"
                );
            }



            // ==============================================
            // VALIDACIONES
            // ==============================================

            if (FechaDesde == default)
            {
                ModelState.AddModelError(
                    nameof(FechaDesde),
                    "Debe seleccionar una fecha."
                );
            }


            if (Tasa <= 0)
            {
                ModelState.AddModelError(
                    nameof(Tasa),
                    "La tasa debe ser mayor que cero."
                );
            }


            if (!ModelState.IsValid)
            {
                CargarInformacion();

                return Page();
            }



            try
            {
                _tipoCambioDal.Guardar(
                    FechaDesde,
                    Tasa
                );


                TempData["Ok"] =
                    $"Tipo de cambio C$ {Tasa:N4} registrado correctamente.";


                return RedirectToPage();
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message
                );


                CargarInformacion();

                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No fue posible guardar el tipo de cambio. " +
                    ex.Message
                );


                CargarInformacion();

                return Page();
            }
        }



        // =====================================================
        // CARGAR DATOS
        // =====================================================

        private void CargarInformacion()
        {
            TasaVigente =
                _tipoCambioDal.ObtenerVigente(
                    DateTime.Now
                );


            Historial =
                _tipoCambioDal.Listar(20);
        }



        // =====================================================
        // SESIÓN
        // =====================================================

        private bool UsuarioAutenticado()
        {
            var usuario =
                HttpContext.Session.GetString(
                    "Usuario"
                );


            return !string.IsNullOrWhiteSpace(
                usuario
            );
        }



        // =====================================================
        // ADMINISTRADOR
        // =====================================================

        private bool EsAdministrador()
        {
            var rol =
                HttpContext.Session.GetString(
                    "Rol"
                );


            return Farmacia.Helpers.RolHelper.EsAdministrador(
                rol
            );
        }
    }
}
