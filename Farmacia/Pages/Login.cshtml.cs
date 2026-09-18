using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UsuarioDAL _userDal;

        public LoginModel(UsuarioDAL userDal)
        {
            _userDal = userDal;
        }

        [BindProperty]
        public string Usuario { get; set; }

        [BindProperty]
        public string Clave { get; set; }

        public string Mensaje { get; set; }

        public void OnGet()
        {
            // Cerrar sesión si ya estaba abierta
            HttpContext.Session.Clear();
        }

        public IActionResult OnPost()
        {
            var u = _userDal.Login(Usuario, Clave);

            if (u == null)
            {
                Mensaje = "Usuario o clave incorrectos.";
                return Page();
            }

            HttpContext.Session.SetString("Usuario", u.UsuarioNombre);
            HttpContext.Session.SetString("Nombre", u.NombreCompleto);
            HttpContext.Session.SetString("Rol", u.Rol);
            HttpContext.Session.SetInt32( "IdUsuario", u.IdUsuario
);

            return RedirectToPage("/Index");
        }
    }
}
