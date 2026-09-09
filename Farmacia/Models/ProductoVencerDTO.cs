namespace Farmacia.Models
{
    public class ProductoVencerDTO
    {
        public string CodBarra { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaVence { get; set; }
        public int DiasRestantes { get; set; }
        public int Stock { get; set; }
    }
}
