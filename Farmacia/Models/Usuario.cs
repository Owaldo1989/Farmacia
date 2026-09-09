namespace Farmacia.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string UsuarioNombre { get; set; }
        public string Clave { get; set; }
        public string NombreCompleto { get; set; }
        public string Rol { get; set; }
        public bool Estado { get; set; }
    }
}
