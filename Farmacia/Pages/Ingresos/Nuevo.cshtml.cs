using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Text.Json;

namespace Farmacia.Pages.Ingresos
{
    public class NuevoModel : PageModel
    {
        private readonly IngresoMercaderiaDAL _ingresoDal;
        private readonly ProveedorDAL _proveedorDal;


        public NuevoModel(
            IngresoMercaderiaDAL ingresoDal,
            ProveedorDAL proveedorDal)
        {
            _ingresoDal =
                ingresoDal;

            _proveedorDal =
                proveedorDal;
        }


        [BindProperty]
        public IngresoMercaderiaDTO Ingreso
        {
            get;
            set;
        } = new();


        [BindProperty]
        public string DetalleJson
        {
            get;
            set;
        } = string.Empty;


        public List<Proveedor> Proveedores
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


            Ingreso =
                new IngresoMercaderiaDTO
                {
                    FechaFactura =
                        DateTime.Today
                };


            CargarCombos();


            return Page();
        }



        // =====================================================
        // BUSCAR PRODUCTOS
        // =====================================================

        public JsonResult OnGetBuscarProductos(
            string filtro)
        {
            var lista =
                _ingresoDal.BuscarProductos(
                    filtro ?? string.Empty
                );


            return new JsonResult(
                lista
            );
        }



        // =====================================================
        // CONFIRMAR
        // =====================================================

        public IActionResult OnPostConfirmar()
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            CargarCombos();


            // =================================================
            // ENCABEZADO
            // =================================================

            Ingreso.NumeroFactura =
                Ingreso.NumeroFactura
                    ?.Trim()
                ?? string.Empty;


            if (Ingreso.IdProveedor <= 0)
            {
                ModelState.AddModelError(
                    "Ingreso.IdProveedor",
                    "Debe seleccionar un proveedor."
                );
            }


            if (
                string.IsNullOrWhiteSpace(
                    Ingreso.NumeroFactura
                )
            )
            {
                ModelState.AddModelError(
                    "Ingreso.NumeroFactura",
                    "Debe ingresar el número de factura."
                );
            }


            if (
                Ingreso.FechaFactura ==
                default
            )
            {
                ModelState.AddModelError(
                    "Ingreso.FechaFactura",
                    "Debe seleccionar la fecha de la factura."
                );
            }


            if (
                Ingreso.SubtotalFacturaProveedor < 0 ||
                Ingreso.IVAFacturaProveedor < 0 ||
                Ingreso.TotalFacturaProveedor <= 0
            )
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Los valores de la factura no son válidos."
                );
            }



            // =================================================
            // DETALLE
            // =================================================

            List<IngresoMercaderiaDetalleDTO>
                detalle =
                    new();


            if (
                string.IsNullOrWhiteSpace(
                    DetalleJson
                )
            )
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Debe agregar al menos un producto."
                );
            }
            else
            {
                try
                {
                    detalle =
                        JsonSerializer.Deserialize<
                            List<IngresoMercaderiaDetalleDTO>
                        >(
                            DetalleJson,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive =
                                    true
                            }
                        )
                        ?? new();
                }
                catch
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "No fue posible leer el detalle del ingreso."
                    );
                }
            }


            if (
                detalle.Count == 0
            )
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Debe agregar al menos un producto."
                );
            }


            if (
                detalle.Any(
                    x =>
                        x.Cantidad <= 0 ||
                        x.CostoUnitarioBase < 0 ||
                        x.PrecioVentaNuevo <= 0
                )
            )
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Existen productos con cantidades, costos o precios inválidos."
                );
            }



            if (!ModelState.IsValid)
            {
                return Page();
            }



            // =================================================
            // USUARIO
            // =================================================

            Ingreso.UsuarioRegistro =
                HttpContext.Session.GetString(
                    "Usuario"
                );


            try
            {
                int idIngreso =
                    _ingresoDal.ConfirmarIngreso(
                        Ingreso,
                        detalle
                    );


                TempData["Ok"] =
                    $"Ingreso #{idIngreso} confirmado correctamente.";


                return RedirectToPage(
                    "/Ingresos/Nuevo"
                );
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message
                );


                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No fue posible registrar el ingreso. " +
                    ex.Message
                );


                return Page();
            }
        }



        private void CargarCombos()
        {
            Proveedores =
                _proveedorDal.Listar();
        }



        private bool UsuarioAutenticado()
        {
            return !string.IsNullOrWhiteSpace(
                HttpContext.Session.GetString(
                    "Usuario"
                )
            );
        }
    }
}
