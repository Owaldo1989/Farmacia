using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Farmacia.Pages.Facturas
{
    public class NuevaModel : PageModel
    {
        private readonly FacturaDAL _facturaDal;
        private readonly ProductoDAL _productoDal;
        private readonly TipoCambioDAL _tipoCambioDal;
        private readonly CajaEquipoDAL _cajaEquipoDal;
        private readonly CajaTurnoDAL _cajaTurnoDal;


        private const string CookieEquipo =
            "Farmacia.CajaEquipo";


        public NuevaModel(
            FacturaDAL facturaDal,
            ProductoDAL productoDal,
            TipoCambioDAL tipoCambioDal,
            CajaEquipoDAL cajaEquipoDal,
            CajaTurnoDAL cajaTurnoDal)
        {
            _facturaDal = facturaDal;
            _productoDal = productoDal;
            _tipoCambioDal = tipoCambioDal;
            _cajaEquipoDal = cajaEquipoDal;
            _cajaTurnoDal = cajaTurnoDal;
        }


        [BindProperty]
        public FacturaDTO Factura { get; set; }


        [BindProperty]
        public string DetalleJson { get; set; }


        [BindProperty]
        public string PagosJson { get; set; }


        public decimal TasaCambio { get; set; }


        public List<FormaPagoDTO> FormasPago { get; set; } =
            new();


        public CajaEquipo? EquipoActual { get; set; }


        public CajaTurno? TurnoActual { get; set; }


        public IActionResult OnGet()
        {
            if (!ValidarCajaOperativa(
                out string mensaje))
            {
                TempData["ErrorCaja"] =
                    mensaje;

                return RedirectToPage(
                    "/Caja/Apertura"
                );
            }


            var fecha = DateTime.Now;


            TasaCambio =
                _tipoCambioDal.ObtenerVigente(fecha);

            CargarFormasPago();


            Factura = new FacturaDTO
            {
                IdFacturaCliente = Guid.NewGuid(),

                Fecha = fecha,

                TasaCambio = TasaCambio
            };


            return Page();
        }


        public IActionResult OnGetBuscarProductos(
            string filtro)
        {
            if (!ValidarCajaOperativa(
                out string mensaje))
            {
                return new UnauthorizedObjectResult(
                    new
                    {
                        ok = false,
                        mensaje
                    }
                );
            }


            var lista =
                _productoDal.BuscarParaFactura(
                    filtro ?? ""
                );

            return new JsonResult(lista);
        }


        public IActionResult OnPost()
        {
            if (!ValidarCajaOperativa(
                out string mensajeCaja))
            {
                TempData["ErrorCaja"] =
                    mensajeCaja;

                return RedirectToPage(
                    "/Caja/Apertura"
                );
            }


            try
            {
                if (string.IsNullOrWhiteSpace(
                    DetalleJson))
                {
                    TempData["Error"] =
                        "La factura no tiene productos.";

                    PrepararPaginaError();

                    return Page();
                }


                var detalle =
                    JsonSerializer.Deserialize<
                        List<FacturaDetalleDTO>
                    >(DetalleJson);


                if (detalle == null ||
                    detalle.Count == 0)
                {
                    TempData["Error"] =
                        "La factura no tiene productos validos.";

                    PrepararPaginaError();

                    return Page();
                }


                // ==============================================
                // VALIDAR DETALLE
                // ==============================================

                if (detalle.Any(
                    x => x.Cantidad <= 0 ||
                         x.Precio <= 0))
                {
                    TempData["Error"] =
                        "La factura contiene cantidades o precios invalidos.";

                    PrepararPaginaError();

                    return Page();
                }


                var pagos =
                    LeerPagos();


                if (pagos.Count == 0)
                {
                    TempData["Error"] =
                        "Debe registrar al menos una forma de pago.";

                    PrepararPaginaError();

                    return Page();
                }


                if (pagos.Any(
                    x => x.IdFormaPago <= 0 ||
                         x.Monto <= 0 ||
                         string.IsNullOrWhiteSpace(x.Moneda)))
                {
                    TempData["Error"] =
                        "La factura contiene pagos invalidos.";

                    PrepararPaginaError();

                    return Page();
                }


                // ==============================================
                // FECHA DEL SERVIDOR
                // No confiamos en la fecha enviada por navegador
                // ==============================================

                Factura.Fecha =
                    DateTime.Now;


                // ==============================================
                // TOTAL CALCULADO EN SERVIDOR
                // ==============================================

                Factura.Total =
                    detalle.Sum(
                        x =>
                            x.Cantidad *
                            x.Precio
                    );


                // ==============================================
                // TASA DESDE SQL SERVER
                // ==============================================

                Factura.TasaCambio =
                    _tipoCambioDal.ObtenerVigente(
                        Factura.Fecha
                    );


                NormalizarResumenPagos(pagos);


                decimal totalPagado =
                    pagos.Sum(
                        x =>
                            x.Moneda == "USD"
                                ? x.Monto * Factura.TasaCambio
                                : x.Monto
                    );

                decimal vueltoCordobas =
                    Factura.VueltoCordoba +
                    (
                        Factura.VueltoDolar *
                        Factura.TasaCambio
                    );


                decimal neto =
                    totalPagado -
                    vueltoCordobas;


                if (neto + 0.05m <
                    Factura.Total)
                {
                    TempData["Error"] =
                        "El monto recibido es insuficiente.";

                    PrepararPaginaError();

                    return Page();
                }


                Factura.Vuelto =
                    vueltoCordobas;

                Factura.Pagos =
                    pagos;

                Factura.IdTurno =
                    TurnoActual?.IdTurno;

                Factura.IdUsuario =
                    HttpContext.Session.GetInt32(
                        "IdUsuario"
                    );


                // ==============================================
                // GUARDAR
                // ==============================================

                int idFactura =
                    _facturaDal.GuardarFactura(
                        Factura,
                        detalle,
                        pagos
                    );


                return RedirectToPage(
                    "/Facturas/VerTicket",
                    new
                    {
                        id = idFactura
                    }
                );
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "Error al guardar factura: " +
                    ex.Message;


                try
                {
                    TasaCambio =
                        _tipoCambioDal.ObtenerVigente(
                            DateTime.Now
                        );

                    CargarFormasPago();
                }
                catch
                {
                    TasaCambio = 0;
                    FormasPago = new List<FormaPagoDTO>();
                }


                return Page();
            }
        }


        private bool ValidarCajaOperativa(
            out string mensaje)
        {
            mensaje = string.Empty;


            // =============================================
            // USUARIO
            // =============================================

            if (string.IsNullOrWhiteSpace(
                HttpContext.Session.GetString(
                    "Usuario"
                )))
            {
                mensaje =
                    "Debe iniciar sesion.";

                return false;
            }


            // =============================================
            // TOKEN DE ESTA PC
            // =============================================

            string? valorToken =
                Request.Cookies[
                    CookieEquipo
                ];


            if (!Guid.TryParse(
                valorToken,
                out Guid tokenEquipo))
            {
                mensaje =
                    "Esta computadora no esta matriculada en una caja.";

                return false;
            }


            // =============================================
            // PC MATRICULADA
            // =============================================

            EquipoActual =
                _cajaEquipoDal.ObtenerPorToken(
                    tokenEquipo
                );


            if (EquipoActual == null ||
                !EquipoActual.Activo)
            {
                mensaje =
                    "Esta computadora no esta autorizada para operar una caja.";

                return false;
            }


            // =============================================
            // TURNO ABIERTO
            // =============================================

            TurnoActual =
                _cajaTurnoDal.ObtenerActual(
                    tokenEquipo
                );


            if (TurnoActual == null)
            {
                mensaje =
                    $"La caja {EquipoActual.NombreCaja} no tiene un turno abierto.";

                return false;
            }


            // =============================================
            // SEGURIDAD EXTRA
            // =============================================

            if (TurnoActual.IdCaja !=
                EquipoActual.IdCaja)
            {
                mensaje =
                    "El turno abierto no pertenece a la caja asignada a esta computadora.";

                return false;
            }


            return true;
        }


        private List<FacturaPagoDTO> LeerPagos()
        {
            if (string.IsNullOrWhiteSpace(
                PagosJson))
            {
                return new List<FacturaPagoDTO>();
            }


            var pagos =
                JsonSerializer.Deserialize<
                    List<FacturaPagoDTO>
                >(
                    PagosJson,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive =
                            true
                    }
                );


            return pagos?
                .Select(x =>
                {
                    x.Moneda =
                        (x.Moneda ?? "NIO")
                            .Trim()
                            .ToUpperInvariant();

                    x.Referencia =
                        string.IsNullOrWhiteSpace(
                            x.Referencia)
                            ? null
                            : x.Referencia.Trim();

                    return x;
                })
                .ToList()
                ?? new List<FacturaPagoDTO>();
        }


        private void NormalizarResumenPagos(
            List<FacturaPagoDTO> pagos)
        {
            var formas =
                _facturaDal.ListarFormasPagoActivas();


            var efectivoIds =
                formas
                    .Where(x => x.EsEfectivo)
                    .Select(x => x.IdFormaPago)
                    .ToHashSet();


            Factura.PagoCordoba =
                pagos
                    .Where(x =>
                        efectivoIds.Contains(
                            x.IdFormaPago) &&
                        x.Moneda == "NIO")
                    .Sum(x => x.Monto);


            Factura.PagoDolar =
                pagos
                    .Where(x =>
                        efectivoIds.Contains(
                            x.IdFormaPago) &&
                        x.Moneda == "USD")
                    .Sum(x => x.Monto);
        }


        private void CargarFormasPago()
        {
            FormasPago =
                _facturaDal.ListarFormasPagoActivas();
        }


        private void PrepararPaginaError()
        {
            TasaCambio =
                Factura?.TasaCambio > 0
                    ? Factura.TasaCambio
                    : _tipoCambioDal.ObtenerVigente(
                        DateTime.Now
                    );

            CargarFormasPago();

            if (Factura == null)
            {
                Factura =
                    new FacturaDTO
                    {
                        IdFacturaCliente =
                            Guid.NewGuid(),
                        Fecha =
                            DateTime.Now,
                        TasaCambio =
                            TasaCambio
                    };
            }
        }
    }
}
