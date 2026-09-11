namespace Farmacia.Models
{
    public class IngresoMercaderiaDTO
    {
        public int IdIngreso { get; set; }

        public int IdProveedor { get; set; }

        public string NumeroFactura { get; set; } = string.Empty;

        public DateTime FechaFactura { get; set; }

        public DateTime FechaIngreso { get; set; }

        public decimal SubtotalFacturaProveedor { get; set; }

        public decimal IVAFacturaProveedor { get; set; }

        public decimal TotalFacturaProveedor { get; set; }

        public byte Estado { get; set; }

        public string? UsuarioRegistro { get; set; }

        public DateTime? FechaConfirmacion { get; set; }

        public string? Observacion { get; set; }
        public int CantidadLineas { get; set; }

        public decimal CantidadUnidades { get; set; }


        // Campos auxiliares para consultas
        public string? NombreProveedor { get; set; }
    }
}
