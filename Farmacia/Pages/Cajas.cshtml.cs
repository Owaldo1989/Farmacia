using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CajaModel = Farmacia.Models.Caja;

namespace Farmacia.Pages
{
    public class CajasModel : PageModel
    {
        private readonly CajaDAL _cajaDal;

        private readonly CajaEquipoDAL _equipoDal;

        private readonly SucursalDAL _sucursalDal;

        private readonly BodegaDAL _bodegaDal;


        private const string CookieEquipo =
            "Farmacia.CajaEquipo";


        public CajasModel(
            CajaDAL cajaDal,
            CajaEquipoDAL equipoDal,
            SucursalDAL sucursalDal,
            BodegaDAL bodegaDal)
        {
            _cajaDal =
                cajaDal;

            _equipoDal =
                equipoDal;

            _sucursalDal =
                sucursalDal;

            _bodegaDal =
                bodegaDal;
        }



        public List<CajaModel> Listado
        {
            get;
            set;
        } = new();


        public List<Sucursal> Sucursales
        {
            get;
            set;
        } = new();


        public List<Bodega> Bodegas
        {
            get;
            set;
        } = new();


        [BindProperty]
        public CajaModel Caja
        {
            get;
            set;
        } = new();



        [BindProperty]
        public int MatriculaIdCaja
        {
            get;
            set;
        }



        [BindProperty]
        public string NombreEquipo
        {
            get;
            set;
        } = string.Empty;



        [BindProperty]
        public string? ObservacionEquipo
        {
            get;
            set;
        }



        public Guid TokenEquipoActual
        {
            get;
            set;
        }


        public CajaEquipo? EquipoActual
        {
            get;
            set;
        }



        // =====================================================
        // GET
        // =====================================================

        public IActionResult OnGet(
            int? id,
            int? desactivar,
            int? desmatricular)
        {
            if (!SesionValida())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            TokenEquipoActual =
                ObtenerOCrearTokenEquipo();


            // =============================================
            // DESACTIVAR CAJA
            // =============================================

            if (desactivar.HasValue)
            {
                try
                {
                    _cajaDal.Desactivar(
                        desactivar.Value
                    );


                    TempData["Ok"] =
                        "Caja desactivada correctamente.";
                }
                catch (Exception ex)
                {
                    TempData["Error"] =
                        ex.Message;
                }


                return RedirectToPage(
                    "/Cajas"
                );
            }



            // =============================================
            // DESMATRICULAR PC
            // =============================================

            if (desmatricular.HasValue)
            {
                try
                {
                    _equipoDal.Desmatricular(
                        desmatricular.Value
                    );


                    TempData["Ok"] =
                        "Equipo desmatriculado correctamente.";
                }
                catch (Exception ex)
                {
                    TempData["Error"] =
                        ex.Message;
                }


                return RedirectToPage(
                    "/Cajas"
                );
            }



            // =============================================
            // EDITAR
            // =============================================

            if (id.HasValue)
            {
                var caja =
                    _cajaDal.Obtener(
                        id.Value
                    );


                if (caja != null)
                {
                    Caja =
                        caja;


                    Bodegas =
                        _bodegaDal
                            .ListarPorSucursal(
                                caja.IdSucursal
                            );


                    TempData["Editar"] =
                        "1";
                }
            }



            CargarDatos();


            EquipoActual =
                _equipoDal.ObtenerPorToken(
                    TokenEquipoActual
                );


            return Page();
        }



        // =====================================================
        // GUARDAR CAJA
        // =====================================================

        public IActionResult OnPost()
        {
            if (!SesionValida())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            try
            {
                if (
                    Caja.IdSucursal <= 0
                )
                {
                    throw new Exception(
                        "Debe seleccionar una sucursal."
                    );
                }


                if (
                    Caja.CodBodega <= 0
                )
                {
                    throw new Exception(
                        "Debe seleccionar una bodega."
                    );
                }


                if (
                    string.IsNullOrWhiteSpace(
                        Caja.CodigoCaja
                    )
                )
                {
                    throw new Exception(
                        "Debe especificar el código de la caja."
                    );
                }


                if (
                    string.IsNullOrWhiteSpace(
                        Caja.NombreCaja
                    )
                )
                {
                    throw new Exception(
                        "Debe especificar el nombre de la caja."
                    );
                }


                if (
                    Caja.FondoFijo < 0
                )
                {
                    throw new Exception(
                        "El fondo fijo no puede ser negativo."
                    );
                }


                bool nueva =
                    Caja.IdCaja == 0;


                _cajaDal.Guardar(
                    Caja
                );


                TempData["Ok"] =
                    nueva
                        ? "Caja creada correctamente."
                        : "Caja actualizada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }


            return RedirectToPage(
                "/Cajas"
            );
        }



        // =====================================================
        // MATRICULAR ESTA PC
        // =====================================================

        public IActionResult OnPostMatricular()
        {
            if (!SesionValida())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            try
            {
                if (
                    MatriculaIdCaja <= 0
                )
                {
                    throw new Exception(
                        "Debe seleccionar una caja."
                    );
                }


                if (
                    string.IsNullOrWhiteSpace(
                        NombreEquipo
                    )
                )
                {
                    throw new Exception(
                        "Debe indicar un nombre para el equipo."
                    );
                }


                Guid token =
                    ObtenerOCrearTokenEquipo();


                int? idUsuario =
                    HttpContext.Session
                        .GetInt32(
                            "IdUsuario"
                        );


                _equipoDal.Matricular(
                    MatriculaIdCaja,
                    token,
                    NombreEquipo,
                    ObservacionEquipo,
                    idUsuario
                );


                TempData["Ok"] =
                    "Equipo matriculado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }


            return RedirectToPage(
                "/Cajas"
            );
        }



        // =====================================================
        // AJAX BODEGAS POR SUCURSAL
        // =====================================================

        public IActionResult OnGetBodegas(
            int idSucursal)
        {
            if (!SesionValida())
            {
                return new UnauthorizedResult();
            }


            var bodegas =
                _bodegaDal
                    .ListarPorSucursal(
                        idSucursal
                    )
                    .Where(
                        x => x.Activo
                    )
                    .Select(
                        x => new
                        {
                            x.CodBodega,
                            x.CodigoBodega,
                            x.NombreBodega
                        }
                    )
                    .ToList();


            return new JsonResult(
                bodegas
            );
        }



        // =====================================================
        // CARGAR DATOS
        // =====================================================

        private void CargarDatos()
        {
            Listado =
                _cajaDal.Listar();


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



        // =====================================================
        // TOKEN PC
        // =====================================================

        private Guid ObtenerOCrearTokenEquipo()
        {
            string? valor =
                Request.Cookies[
                    CookieEquipo
                ];


            if (
                Guid.TryParse(
                    valor,
                    out Guid tokenExistente
                )
            )
            {
                return tokenExistente;
            }


            Guid token =
                Guid.NewGuid();


            Response.Cookies.Append(
                CookieEquipo,
                token.ToString(),
                new CookieOptions
                {
                    HttpOnly =
                        true,

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
            return !string.IsNullOrWhiteSpace(
                HttpContext.Session.GetString(
                    "Usuario"
                )
            );
        }
    }
}
