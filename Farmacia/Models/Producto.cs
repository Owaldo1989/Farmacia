namespace Farmacia.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string CodBarra { get; set; }
        public string NombreProducto { get; set; }
        public string NombreGenerico { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string RecomendadoPara { get; set; }
        public decimal PrecioCosto { get; set; }
        public decimal CostoPromedio { get; set; }
        public decimal PrecioVenta { get; set; }

        public decimal PorcentajeUtilidad { get; set; }
        public decimal TasaIVA { get; set; } = 15m;
        public decimal Inventario { get; set; }
        public int IdProveedor { get; set; }
        public int IdCategoria { get; set; }
        public int IdUnidad { get; set; }
        public int? IdLaboratorio { get; set; }
        public string? Foto { get; set; }

        // Campos de navegación (opcional)
        public string CategoriaNombre { get; set; }
        public string ProveedorNombre { get; set; }
        public string UnidadNombre { get; set; }
    }
}
