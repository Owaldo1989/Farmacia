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
        public decimal VueltoCordoba { get; set; }
        public decimal VueltoDolar { get; set; }
        public decimal TasaCambio { get; set; }
        public int? IdTurno { get; set; }
        public int? IdUsuario { get; set; }

        public List<FacturaPagoDTO> Pagos { get; set; } = new();
    }
}
