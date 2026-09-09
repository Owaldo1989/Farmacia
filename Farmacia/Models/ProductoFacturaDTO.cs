namespace Farmacia.Models
{
    public class ProductoFacturaDTO
    {
        public int IdProducto { get; set; }
        public string CodBarra { get; set; }
        public string NombreProducto { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Inventario { get; set; }
    }
}
