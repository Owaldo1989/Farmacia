namespace Farmacia.Models.AgenteBancario
{
    public class AgenteTransaccion
    {
        public long IdTransaccion { get; set; }

        public int IdAgente { get; set; }

        public int IdTipoOperacion { get; set; }

        public int IdCaja { get; set; }

        public decimal Monto { get; set; }

        public string? NumeroReferencia { get; set; }

        public DateTime FechaTransaccion { get; set; }

        public int IdUsuario { get; set; }

        public string? Observacion { get; set; }

        public short EfectoEfectivo { get; set; }

        public short EfectoSaldoAgente { get; set; }

        public byte TipoComision { get; set; }

        public decimal ValorComision { get; set; }

        public decimal MontoComision { get; set; }

        public byte Estado { get; set; }

        public int? IdUsuarioAnula { get; set; }

        public DateTime? FechaAnulacion { get; set; }

        public string? MotivoAnulacion { get; set; }


        public string CodigoAgente { get; set; }
            = string.Empty;

        public string NombreAgente { get; set; }
            = string.Empty;

        public string CodigoOperacion { get; set; }
            = string.Empty;

        public string NombreOperacion { get; set; }
            = string.Empty;

        public string NombreUsuario { get; set; }
            = string.Empty;

        public string NombreCaja { get; set; }
            = string.Empty;
    }
}
