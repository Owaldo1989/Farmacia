namespace Farmacia.Models
{
    public class FacturaDetalleDTO
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public string CodBarra { get; set; }

        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal SubTotal => Cantidad * Precio;
    }
}
