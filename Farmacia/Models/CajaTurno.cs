namespace Farmacia.Models
{
    public class CajaTurno
    {
        public int IdTurno { get; set; }

        public int IdCaja { get; set; }

        public int IdCajaEquipo { get; set; }

        public int IdUsuarioApertura { get; set; }

        public DateTime FechaApertura { get; set; }

        public decimal FondoConfigurado { get; set; }

        public decimal FondoInicial { get; set; }

        public string? ObservacionApertura { get; set; }


        public int? IdUsuarioCierre { get; set; }

        public DateTime? FechaCierre { get; set; }

        public decimal? EfectivoEsperado { get; set; }

        public decimal? EfectivoContado { get; set; }

        public decimal? Diferencia { get; set; }

        public string? ObservacionCierre { get; set; }


        public byte Estado { get; set; }


        // =============================================
        // INFORMACIÓN
        // =============================================

        public string CodigoCaja { get; set; }
            = string.Empty;

        public string NombreCaja { get; set; }
            = string.Empty;

        public int IdSucursal { get; set; }

        public int CodBodega { get; set; }

        public string NombreSucursal { get; set; }
            = string.Empty;

        public string NombreBodega { get; set; }
            = string.Empty;

        public string UsuarioApertura { get; set; }
            = string.Empty;
    }
}
