namespace Farmacia.Models
{
    public class TipoCambioDTO
    {
        public int IdTipoCambio { get; set; }

        public DateTime FechaDesde { get; set; }

        public decimal Tasa { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
