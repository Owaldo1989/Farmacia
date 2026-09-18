namespace Farmacia.Models.AgenteBancario
{
    public class AgenteBancario
    {
        public int IdAgente { get; set; }

        public int IdCaja { get; set; }

        public string CodigoAgente { get; set; }
            = string.Empty;

        public string NombreAgente { get; set; }
            = string.Empty;

        public decimal SaldoInicial { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; }


        public string CodigoCaja { get; set; }
            = string.Empty;

        public string NombreCaja { get; set; }
            = string.Empty;

        public int IdSucursal { get; set; }

        public int CodBodega { get; set; }

        public string CodigoSucursal { get; set; }
            = string.Empty;

        public string NombreSucursal { get; set; }
            = string.Empty;

        public string CodigoBodega { get; set; }
            = string.Empty;

        public string NombreBodega { get; set; }
            = string.Empty;
    }
}
