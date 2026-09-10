namespace Farmacia.Models
{
    public class Laboratorio
    {
        public int IdLaboratorio { get; set; }
        [System.ComponentModel.DataAnnotations.Required(
            ErrorMessage = "El nombre del laboratorio es obligatorio.")]
        [System.ComponentModel.DataAnnotations.StringLength(
            150,
            ErrorMessage = "El nombre no puede exceder los 150 caracteres.")]
        public string LaboratorioNombre { get; set; } = string.Empty;
    }
}
