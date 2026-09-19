namespace Farmacia.Models.Factura
{
    public class FacturaPagoDTO
    {
        public string TipoPago { get; set; } = "";
        public string Moneda { get; set; } = "";

        public decimal Monto { get; set; }

        public decimal TasaCambio { get; set; }

        public string? Referencia { get; set; }
    }
}
