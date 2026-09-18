namespace Farmacia.Models
{
    public class Bodega
    {
        public int CodBodega { get; set; }

        public int IdSucursal { get; set; }

        public string CodigoBodega { get; set; }
            = string.Empty;

        public string NombreBodega { get; set; }
            = string.Empty;

        public bool Activo { get; set; }
            = true;

        public DateTime FechaRegistro { get; set; }


        // Navegación

        public string NombreSucursal { get; set; }
            = string.Empty;

        public string CodigoSucursal { get; set; }
            = string.Empty;
    }
}
