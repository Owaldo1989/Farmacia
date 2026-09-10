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


        public NuevaModel(
            FacturaDAL facturaDal,
            ProductoDAL productoDal,
            TipoCambioDAL tipoCambioDal)
        {
            _facturaDal = facturaDal;
            _productoDal = productoDal;
            _tipoCambioDal = tipoCambioDal;
        }


        [BindProperty]
        public FacturaDTO Factura { get; set; }


        [BindProperty]
        public string DetalleJson { get; set; }


        public decimal TasaCambio { get; set; }


        public void OnGet()
        {
            var fecha = DateTime.Now;


            TasaCambio =
                _tipoCambioDal.ObtenerVigente(fecha);


            Factura = new FacturaDTO
            {
                IdFacturaCliente = Guid.NewGuid(),

                Fecha = fecha,

                TasaCambio = TasaCambio
            };
        }


        public JsonResult OnGetBuscarProductos(
            string filtro)
        {
            var lista =
                _productoDal.BuscarParaFactura(
                    filtro ?? ""
                );

            return new JsonResult(lista);
        }


        public IActionResult OnPost()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(
                    DetalleJson))
                {
                    TempData["Error"] =
                        "La factura no tiene productos.";

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
                        "La factura no tiene productos válidos.";

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
                        "La factura contiene cantidades o precios inválidos.";

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


                // ==============================================
                // CALCULAR EL PAGO REAL
                // ==============================================

                decimal totalPagado =
                    Factura.PagoCordoba +
                    (
                        Factura.PagoDolar *
                        Factura.TasaCambio
                    );


                if (totalPagado <
                    Factura.Total)
                {
                    TempData["Error"] =
                        "El monto recibido es insuficiente.";

                    TasaCambio =
                        Factura.TasaCambio;

                    return Page();
                }


                // ==============================================
                // VUELTO CALCULADO POR EL SERVIDOR
                // ==============================================

                Factura.Vuelto =
                    totalPagado -
                    Factura.Total;


                // ==============================================
                // GUARDAR
                // ==============================================

                int idFactura =
                    _facturaDal.GuardarFactura(
                        Factura,
                        detalle
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
                }
                catch
                {
                    TasaCambio = 0;
                }


                return Page();
            }
        }
    }
}