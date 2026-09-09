namespace Farmacia.Models
{
    public class Articulo
    {
        public int IdArticulo { get; set; }
        public string CodBarra { get; set; }
        public string NombreProducto { get; set; }
        public string NombreGenerico { get; set; }
        public int? IdCategoria { get; set; }
        public int? IdUnidad { get; set; }
        public int? IdProveedor { get; set; }
        public string NoLote { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public decimal PrecioCosto { get; set; }
        public decimal CostoPromedio { get; set; }
        public decimal PrecioVenta { get; set; }
        public int Inventario { get; set; }
        public string RecomendadoPara { get; set; }
        public bool RequiereReceta { get; set; }
        public bool Estado { get; set; }
    }
}
