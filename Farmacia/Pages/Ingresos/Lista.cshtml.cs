using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages.Ingresos
{
    public class ListaModel : PageModel
    {
        private readonly IngresoMercaderiaDAL _ingresoDal;
        private readonly ProveedorDAL _proveedorDal;


        public ListaModel(
            IngresoMercaderiaDAL ingresoDal,
            ProveedorDAL proveedorDal)
        {
            _ingresoDal =
                ingresoDal;

            _proveedorDal =
                proveedorDal;
        }


        [BindProperty(SupportsGet = true)]
        public DateTime FechaInicio
        {
            get;
            set;
        }


        [BindProperty(SupportsGet = true)]
        public DateTime FechaFin
        {
            get;
            set;
        }


        [BindProperty(SupportsGet = true)]
        public int? IdProveedor
        {
            get;
            set;
        }


        [BindProperty(SupportsGet = true)]
        public string? NumeroFactura
        {
            get;
            set;
        }


        public List<IngresoMercaderiaDTO> Listado
        {
            get;
            set;
        } = new();


        public List<Proveedor> Proveedores
        {
            get;
            set;
        } = new();



        public IActionResult OnGet()
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            if (FechaInicio == default)
            {
                FechaInicio =
                    new DateTime(
                        DateTime.Today.Year,
                        DateTime.Today.Month,
                        1
                    );
            }


            if (FechaFin == default)
            {
                FechaFin =
                    DateTime.Today;
            }


            Proveedores =
                _proveedorDal.Listar();


            Listado =
                _ingresoDal.Listar(
                    FechaInicio,
                    FechaFin,
                    IdProveedor,
                    NumeroFactura
                );


            return Page();
        }



        public JsonResult OnGetDetalle(
            int id)
        {
            var ingreso =
                _ingresoDal.Obtener(id);


            if (ingreso == null)
            {
                Response.StatusCode = 404;

                return new JsonResult(
                    new
                    {
                        mensaje =
                            "El ingreso no existe."
                    }
                );
            }


            return new JsonResult(
                new
                {
                    ingreso =
                        ingreso.Ingreso,

                    detalles =
                        ingreso.Detalles.Select(
                            x => new
                            {
                                x.IdProducto,
                                x.CodBarra,
                                x.NombreProducto,

                                x.Cantidad,

                                x.CostoAnterior,

                                x.CostoUnitarioBase,

                                x.TasaIVA,

                                x.CostoUnitarioFinal,

                                x.CostoPromedioAnterior,

                                x.CostoPromedioNuevo,

                                x.PrecioVentaAnterior,

                                x.PrecioVentaSugerido,

                                x.PrecioVentaNuevo,

                                x.InventarioAnterior,

                                x.InventarioNuevo,

                                x.NumeroLote,

                                fechaVencimiento =
                                    x.FechaVencimiento?
                                        .ToString(
                                            "dd/MM/yyyy"
                                        ),

                                x.SubtotalBase,

                                x.MontoIVA,

                                x.TotalLinea,

                                x.CostoAumento,

                                x.PrecioCambio
                            }
                        )
                }
            );
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
