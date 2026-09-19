namespace Farmacia.Models
{
    public class Denominacion
    {
        public int IdDenominacion { get; set; }

        public string Moneda { get; set; } = "NIO";

        public string Tipo { get; set; } = "B";

        public decimal Valor { get; set; }

        public string Descripcion { get; set; } = string.Empty;

        public int Orden { get; set; }

        public bool Activo { get; set; } = true;
    }
}
