namespace Farmacia.Models
{
    public class CajaEquipo
    {
        public int IdCajaEquipo { get; set; }

        public int IdCaja { get; set; }

        public Guid TokenEquipo { get; set; }

        public string NombreEquipo { get; set; }
            = string.Empty;

        public string? Observacion { get; set; }

        public int? IdUsuarioRegistro { get; set; }

        public DateTime FechaRegistro { get; set; }

        public DateTime? UltimoAcceso { get; set; }

        public bool Activo { get; set; }


        // Datos de Caja

        public string CodigoCaja { get; set; }
            = string.Empty;

        public string NombreCaja { get; set; }
            = string.Empty;

        public decimal FondoFijo { get; set; }

        public int IdSucursal { get; set; }

        public int CodBodega { get; set; }

        public string NombreSucursal { get; set; }
            = string.Empty;

        public string NombreBodega { get; set; }
            = string.Empty;
    }
}
