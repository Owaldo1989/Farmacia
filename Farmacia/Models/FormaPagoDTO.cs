namespace Farmacia.Models
{
    public class FormaPagoDTO
    {
        public int IdFormaPago { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public bool EsEfectivo { get; set; }

        public bool RequiereReferencia { get; set; }

        public bool PermiteVuelto { get; set; }

        public bool Activo { get; set; }
    }
}
