namespace Farmacia.Models
{
    public class FacturaDTO
    {
        public int IdFactura { get; set; }
        public Guid IdFacturaCliente { get; set; }
        public DateTime Fecha { get; set; }

        public string Paciente { get; set; }
        public decimal Total { get; set; }

        public decimal PagoCordoba { get; set; }
        public decimal PagoDolar { get; set; }
        public decimal Vuelto { get; set; }
    }
}
