namespace Farmacia.Models
{
    public class FacturaPagoDTO
    {
        public long IdFacturaPago { get; set; }

        public int IdFactura { get; set; }

        public int IdFormaPago { get; set; }

        public string CodigoFormaPago { get; set; } = string.Empty;

        public string NombreFormaPago { get; set; } = string.Empty;

        public string Moneda { get; set; } = "NIO";

        public decimal Monto { get; set; }

        public decimal TasaCambio { get; set; }

        public string? Referencia { get; set; }

        public bool EsEfectivo { get; set; }

        public bool RequiereReferencia { get; set; }

        public bool PermiteVuelto { get; set; }

        public decimal MontoCordoba =>
            Moneda == "USD"
                ? Monto * TasaCambio
                : Monto;
    }
}
