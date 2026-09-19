using Farmacia.DAL;
using Farmacia.Models.AgenteBancario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages.Agentes
{
    public class OperacionesModel : PageModel
    {
        private readonly AgenteBancarioDAL _agenteDal;
        private readonly AgenteTipoOperacionDAL _tipoDal;
        private readonly AgenteTransaccionDAL _transaccionDal;


        public OperacionesModel(
            AgenteBancarioDAL agenteDal,
            AgenteTipoOperacionDAL tipoDal,
            AgenteTransaccionDAL transaccionDal)
        {
            _agenteDal = agenteDal;
            _tipoDal = tipoDal;
            _transaccionDal = transaccionDal;
        }


        public AgenteBancario? Agente
        {
            get;
            set;
        }


        public AgenteResumen Resumen
        {
            get;
            set;
        } = new();


        public List<AgenteTipoOperacion> Operaciones
        {
            get;
            set;
        } = new();


        public List<AgenteTransaccion> Transacciones
        {
            get;
            set;
        } = new();


        [BindProperty]
        public int IdTipoOperacion
        {
            get;
            set;
        }


        [BindProperty]
        public decimal Monto
        {
            get;
            set;
        }


        [BindProperty]
        public string? NumeroReferencia
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


        [BindProperty]
        public long IdTransaccionAnular
        {
            get;
            set;
        }


        [BindProperty]
        public string? MotivoAnulacion
        {
            get;
            set;
        }


        // =====================================================
        // GET
        // =====================================================

        public IActionResult OnGet(
            string codigo)
        {
            if (!SesionValida())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            if (!CargarAgente(codigo))
            {
                return NotFound();
            }


            return Page();
        }


        // =====================================================
        // REGISTRAR
        // =====================================================

        public IActionResult OnPostRegistrar(
            string codigo)
        {
            if (!SesionValida())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            try
            {
                var agente =
                    BuscarAgente(codigo);


                if (agente == null)
                {
                    throw new Exception(
                        "El agente bancario no existe."
                    );
                }


                int? idUsuario =
                    HttpContext.Session.GetInt32(
                        "IdUsuario"
                    );


                if (!idUsuario.HasValue)
                {
                    throw new Exception(
                        "No se pudo identificar el usuario. Inicie sesión nuevamente."
                    );
                }


                if (IdTipoOperacion <= 0)
                {
                    throw new Exception(
                        "Debe seleccionar una operación."
                    );
                }


                if (Monto <= 0)
                {
                    throw new Exception(
                        "El monto debe ser mayor que cero."
                    );
                }


                long id =
                    _transaccionDal.Registrar(
                        agente.IdAgente,
                        IdTipoOperacion,
                        Monto,
                        NumeroReferencia,
                        Observacion,
                        idUsuario.Value
                    );


                TempData["Ok"] =
                    $"Operación #{id} registrada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }


            return RedirectToPage(
                "/Agentes/Operaciones",
                new
                {
                    codigo
                }
            );
        }


        // =====================================================
        // ANULAR
        // =====================================================

        public IActionResult OnPostAnular(
            string codigo)
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
                    HttpContext.Session.GetInt32(
                        "IdUsuario"
                    );


                if (!idUsuario.HasValue)
                {
                    throw new Exception(
                        "No se pudo identificar el usuario."
                    );
                }


                if (
                    string.IsNullOrWhiteSpace(
                        MotivoAnulacion
                    )
                )
                {
                    throw new Exception(
                        "Debe indicar el motivo de la anulación."
                    );
                }


                _transaccionDal.Anular(
                    IdTransaccionAnular,
                    idUsuario.Value,
                    MotivoAnulacion
                );


                TempData["Ok"] =
                    "Operación anulada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }


            return RedirectToPage(
                "/Agentes/Operaciones",
                new
                {
                    codigo
                }
            );
        }


        // =====================================================
        // CARGAR
        // =====================================================

        private bool CargarAgente(
            string codigo)
        {
            Agente =
                BuscarAgente(
                    codigo
                );


            if (Agente == null)
            {
                return false;
            }


            Operaciones =
                _tipoDal
                    .Listar(
                        Agente.IdAgente
                    )
                    .Where(
                        x => x.Activo
                    )
                    .OrderBy(
                        x => x.NombreOperacion
                    )
                    .ToList();


            Transacciones =
                _transaccionDal.Listar(
                    Agente.IdAgente
                );


            Resumen =
                _transaccionDal.ObtenerResumen(
                    Agente.IdAgente
                );


            return true;
        }


        private AgenteBancario? BuscarAgente(
            string codigo)
        {
            if (
                string.IsNullOrWhiteSpace(
                    codigo
                )
            )
            {
                return null;
            }


            return _agenteDal
                .Listar()
                .FirstOrDefault(
                    x =>
                        x.Activo
                        &&
                        string.Equals(
                            x.CodigoAgente,
                            codigo,
                            StringComparison.OrdinalIgnoreCase
                        )
                );
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
