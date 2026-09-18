namespace Farmacia.Models
{
    public class Caja
    {
        public int IdCaja { get; set; }

        public string CodigoCaja { get; set; }
            = string.Empty;

        public string NombreCaja { get; set; }
            = string.Empty;

        public int IdSucursal { get; set; }

        public int CodBodega { get; set; }

        public decimal FondoFijo { get; set; }

        public bool Activo { get; set; }
            = true;

        public DateTime FechaRegistro { get; set; }


        // Navegación

        public string NombreSucursal { get; set; }
            = string.Empty;

        public string CodigoSucursal { get; set; }
            = string.Empty;

        public string NombreBodega { get; set; }
            = string.Empty;

        public string CodigoBodega { get; set; }
            = string.Empty;


        // Equipo activo

        public int? IdCajaEquipo { get; set; }

        public string? NombreEquipo { get; set; }

        public Guid? TokenEquipo { get; set; }

        public DateTime? UltimoAcceso { get; set; }
    }
}
