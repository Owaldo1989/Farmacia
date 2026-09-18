namespace Farmacia.Models
{
    public class Sucursal
    {
        public int IdSucursal { get; set; }

        public string CodigoSucursal { get; set; }
            = string.Empty;

        public string NombreSucursal { get; set; }
            = string.Empty;

        public string? Direccion { get; set; }

        public string? Telefono { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; }
    }
}
