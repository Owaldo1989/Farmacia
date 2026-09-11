namespace Farmacia.Models
{
    public class IngresoMercaderiaDetalleDTO
    {
        public int IdIngresoDetalle { get; set; }

        public int IdIngreso { get; set; }

        public int IdProducto { get; set; }


        // Información visual
        public string? CodBarra { get; set; }

        public string? NombreProducto { get; set; }


        // Cantidad
        public decimal Cantidad { get; set; }


        // Costos
        public decimal CostoAnterior { get; set; }

        public decimal CostoUnitarioBase { get; set; }

        public decimal TasaIVA { get; set; }

        public decimal CostoUnitarioFinal { get; set; }

        public decimal CostoPromedioAnterior { get; set; }

        public decimal CostoPromedioNuevo { get; set; }


        // Precio venta
        public decimal PrecioVentaAnterior { get; set; }

        
             public decimal PorcentajeUtilidad { get; set; }


        public decimal? PrecioVentaSugerido { get; set; }

        public decimal PrecioVentaNuevo { get; set; }


        // Inventario
        public decimal InventarioAnterior { get; set; }

        public decimal InventarioNuevo { get; set; }


        // Farmacia
        public string? NumeroLote { get; set; }

        public DateTime? FechaVencimiento { get; set; }

        public decimal SubtotalBase
        {
            get
            {
                return Math.Round(
                    Cantidad * CostoUnitarioBase,
                    2
                );
            }
        }

        public decimal MontoIVA
        {
            get
            {
                return Math.Round(
                    SubtotalBase *
                    (TasaIVA / 100m),
                    2
                );
            }
        }

        public decimal TotalLinea
        {
            get
            {
                return SubtotalBase + MontoIVA;
            }
        }

        public bool CostoAumento
        {
            get
            {
                return CostoUnitarioFinal >
                       CostoAnterior;
            }
        }

        public bool PrecioCambio
        {
            get
            {
                return PrecioVentaNuevo !=
                       PrecioVentaAnterior;
            }
        }
    }
}
