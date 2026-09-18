namespace Farmacia.Models.AgenteBancario
{
    public class AgenteTipoOperacion
    {
        public int IdTipoOperacion { get; set; }

        public int IdAgente { get; set; }

        public string CodigoOperacion { get; set; }
            = string.Empty;

        public string NombreOperacion { get; set; }
            = string.Empty;

        public short EfectoEfectivo { get; set; }

        public short EfectoSaldoAgente { get; set; }

        public bool RequiereReferencia { get; set; } = true;

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; }


        public string CodigoAgente { get; set; }
            = string.Empty;

        public string NombreAgente { get; set; }
            = string.Empty;
    }
}
