namespace Farmacia.Models
{
    public class ProductoIngresoDTO
    {
        public int IdProducto { get; set; }

        public string CodBarra { get; set; } = string.Empty;

        public string NombreProducto { get; set; } = string.Empty;

        public string? NombreGenerico { get; set; }

        public decimal PrecioCosto { get; set; }

        public decimal CostoPromedio { get; set; }

        public decimal PrecioVenta { get; set; }

        public decimal Inventario { get; set; }

        public decimal TasaIVA { get; set; }
        public decimal PorcentajeUtilidad { get; set; }
    }
}
