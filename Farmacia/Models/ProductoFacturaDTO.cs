namespace Farmacia.Models
{
    public class ProductoFacturaDTO
    {
        public int IdProducto { get; set; }
        public string CodBarra { get; set; }
        public string NombreProducto { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Inventario { get; set; }
        public string? Foto { get; set; }
        public string? NombreGenerico { get; set; }
        public string? RecomendadoPara { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string? CategoriaNombre { get; set; }
        public string? ProveedorNombre { get; set; }
        public string? UnidadNombre { get; set; }
        public string? LaboratorioNombre { get; set; }
    }
}
