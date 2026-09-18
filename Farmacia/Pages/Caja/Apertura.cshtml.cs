using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages.Caja
{
    public class AperturaModel : PageModel
    {
        private readonly CajaEquipoDAL _equipoDal;

        private readonly CajaTurnoDAL _turnoDal;


        private const string CookieEquipo =
            "Farmacia.CajaEquipo";


        public AperturaModel(
            CajaEquipoDAL equipoDal,
            CajaTurnoDAL turnoDal)
        {
            _equipoDal =
                equipoDal;

            _turnoDal =
                turnoDal;
        }



        public CajaEquipo? Equipo
        {
            get;
            set;
        }


        public CajaTurno? Turno
        {
            get;
            set;
        }


        public Guid TokenEquipo
        {
            get;
            set;
        }



        [BindProperty]
        public decimal FondoInicial
        {
            get;
            set;
        }


        [BindProperty]
        public string? Observacion
        {
            get;
            set;
        }



        // =====================================================
        // GET
        // =====================================================

        public IActionResult OnGet()
        {
            if (!SesionValida())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            TokenEquipo =
                ObtenerOCrearToken();


            Equipo =
                _equipoDal.ObtenerPorToken(
                    TokenEquipo
                );


            /*
                PC no matriculada.
            */

            if (Equipo == null)
            {
                return Page();
            }


            /*
                Verificar turno.
            */

            Turno =
                _turnoDal.ObtenerActual(
                    TokenEquipo
                );


            /*
                Si no está abierta,
                sugerimos fondo fijo.
            */

            if (Turno == null)
            {
                FondoInicial =
                    Equipo.FondoFijo;
            }


            return Page();
        }



        // =====================================================
        // ABRIR
        // =====================================================

        public IActionResult OnPostAbrir()
        {
            if (!SesionValida())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            try
            {
                int? idUsuario =
                    HttpContext.Session
                        .GetInt32(
                            "IdUsuario"
                        );


                if (!idUsuario.HasValue)
                {
                    throw new Exception(
                        "No se pudo identificar el usuario conectado. Inicie sesión nuevamente."
                    );
                }


                Guid token =
                    ObtenerOCrearToken();


                var equipo =
                    _equipoDal.ObtenerPorToken(
                        token
                    );


                if (equipo == null)
                {
                    throw new Exception(
                        "Este equipo no se encuentra matriculado en una caja."
                    );
                }


                if (FondoInicial < 0)
                {
                    throw new Exception(
                        "El fondo inicial no puede ser negativo."
                    );
                }


                int idTurno =
                    _turnoDal.Abrir(
                        token,
                        idUsuario.Value,
                        FondoInicial,
                        Observacion
                    );


                TempData["Ok"] =
                    $"Caja abierta correctamente. Turno #{idTurno}.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }


            return RedirectToPage(
                "/Caja/Apertura"
            );
        }



        // =====================================================
        // COOKIE
        // =====================================================

        private Guid ObtenerOCrearToken()
        {
            string? valor =
                Request.Cookies[
                    CookieEquipo
                ];


            if (
                Guid.TryParse(
                    valor,
                    out Guid token
                )
            )
            {
                return token;
            }


            token =
                Guid.NewGuid();


            Response.Cookies.Append(
                CookieEquipo,
                token.ToString(),
                new CookieOptions
                {
                    HttpOnly = true,

                    Secure =
                        Request.IsHttps,

                    SameSite =
                        SameSiteMode.Lax,

                    IsEssential =
                        true,

                    Expires =
                        DateTimeOffset.UtcNow
                            .AddYears(5)
                }
            );


            return token;
        }



        private bool SesionValida()
        {
            return
                !string.IsNullOrWhiteSpace(
                    HttpContext.Session
                        .GetString(
                            "Usuario"
                        )
                );
        }
    }
}
