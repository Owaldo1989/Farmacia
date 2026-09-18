using Farmacia.DAL;
using Farmacia.Models;
using Farmacia.Models.AgenteBancario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CajaModel = Farmacia.Models.Caja;

namespace Farmacia.Pages.Agentes
{
    public class AgentesBancariosModel : PageModel
    {
        private readonly AgenteBancarioDAL _agenteDal;

        private readonly AgenteTipoOperacionDAL _operacionDal;

        private readonly CajaDAL _cajaDal;


        public AgentesBancariosModel(
            AgenteBancarioDAL agenteDal,
            AgenteTipoOperacionDAL operacionDal,
            CajaDAL cajaDal)
        {
            _agenteDal =
                agenteDal;

            _operacionDal =
                operacionDal;

            _cajaDal =
                cajaDal;
        }


        public List<AgenteBancario> Agentes
        {
            get;
            set;
        } = new();


        public List<AgenteTipoOperacion> Operaciones
        {
            get;
            set;
        } = new();


        public List<CajaModel> Cajas
        {
            get;
            set;
        } = new();


        [BindProperty]
        public AgenteBancario Agente
        {
            get;
            set;
        } = new();


        [BindProperty]
        public AgenteTipoOperacion Operacion
        {
            get;
            set;
        } = new();


        [BindProperty]
        public int IdAgenteDesactivar
        {
            get;
            set;
        }


        [BindProperty]
        public int IdOperacionDesactivar
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


            CargarDatos();


            return Page();
        }


        // =====================================================
        // GUARDAR AGENTE
        // =====================================================

        public IActionResult OnPostGuardarAgente()
        {
            if (!SesionValida())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            try
            {
                if (Agente.IdCaja <= 0)
                {
                    throw new Exception(
                        "Debe seleccionar una caja."
                    );
                }


                if (
                    string.IsNullOrWhiteSpace(
                        Agente.CodigoAgente
                    )
                )
                {
                    throw new Exception(
                        "Debe especificar el código del agente."
                    );
                }


                if (
                    string.IsNullOrWhiteSpace(
                        Agente.NombreAgente
                    )
                )
                {
                    throw new Exception(
                        "Debe especificar el nombre del agente."
                    );
                }


                if (Agente.SaldoInicial < 0)
                {
                    throw new Exception(
                        "El saldo inicial no puede ser negativo."
                    );
                }


                bool nuevo =
                    Agente.IdAgente == 0;


                _agenteDal.Guardar(
                    Agente
                );


                TempData["Ok"] =
                    nuevo
                        ? "Agente bancario registrado correctamente."
                        : "Agente bancario actualizado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }


            return RedirectToPage(
                "/AgentesBancarios"
            );
        }


        // =====================================================
        // GUARDAR OPERACIÓN
        // =====================================================

        public IActionResult OnPostGuardarOperacion()
        {
            if (!SesionValida())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            try
            {
                if (Operacion.IdAgente <= 0)
                {
                    throw new Exception(
                        "Debe seleccionar un agente bancario."
                    );
                }


                if (
                    string.IsNullOrWhiteSpace(
                        Operacion.CodigoOperacion
                    )
                )
                {
                    throw new Exception(
                        "Debe especificar el código de la operación."
                    );
                }


                if (
                    string.IsNullOrWhiteSpace(
                        Operacion.NombreOperacion
                    )
                )
                {
                    throw new Exception(
                        "Debe especificar el nombre de la operación."
                    );
                }


                if (
                    Operacion.EfectoEfectivo < -1 ||
                    Operacion.EfectoEfectivo > 1
                )
                {
                    throw new Exception(
                        "El efecto sobre efectivo no es válido."
                    );
                }


                if (
                    Operacion.EfectoSaldoAgente < -1 ||
                    Operacion.EfectoSaldoAgente > 1
                )
                {
                    throw new Exception(
                        "El efecto sobre saldo del agente no es válido."
                    );
                }


                bool nueva =
                    Operacion.IdTipoOperacion == 0;


                _operacionDal.Guardar(
                    Operacion
                );


                TempData["Ok"] =
                    nueva
                        ? "Tipo de operación registrado correctamente."
                        : "Tipo de operación actualizado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }


            return RedirectToPage(
                "/AgentesBancarios"
            );
        }


        // =====================================================
        // DESACTIVAR AGENTE
        // =====================================================

        public IActionResult OnPostDesactivarAgente()
        {
            if (!SesionValida())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            try
            {
                _agenteDal.Desactivar(
                    IdAgenteDesactivar
                );


                TempData["Ok"] =
                    "Agente bancario desactivado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }


            return RedirectToPage(
                "/AgentesBancarios"
            );
        }


        // =====================================================
        // DESACTIVAR OPERACIÓN
        // =====================================================

        public IActionResult OnPostDesactivarOperacion()
        {
            if (!SesionValida())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            try
            {
                _operacionDal.Desactivar(
                    IdOperacionDesactivar
                );


                TempData["Ok"] =
                    "Tipo de operación desactivado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }


            return RedirectToPage(
                "/AgentesBancarios"
            );
        }


        // =====================================================
        // CARGAR
        // =====================================================

        private void CargarDatos()
        {
            Agentes =
                _agenteDal.Listar();


            Operaciones =
                _operacionDal.Listar();


            Cajas =
                _cajaDal
                    .Listar()
                    .Where(
                        x => x.Activo
                    )
                    .OrderBy(
                        x => x.NombreSucursal
                    )
                    .ThenBy(
                        x => x.NombreCaja
                    )
                    .ToList();
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
