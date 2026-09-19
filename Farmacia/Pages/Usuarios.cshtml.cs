using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages
{
    public class UsuariosModel : PageModel
    {
        private readonly UsuarioDAL _usuarioDal;
        private readonly SucursalDAL _sucursalDal;


        public UsuariosModel(
            UsuarioDAL usuarioDal,
            SucursalDAL sucursalDal)
        {
            _usuarioDal = usuarioDal;
            _sucursalDal = sucursalDal;
        }


        public List<Usuario> Listado
        {
            get;
            set;
        } = new();


        public List<Sucursal> Sucursales
        {
            get;
            set;
        } = new();


        [BindProperty]
        public Usuario Usuario
        {
            get;
            set;
        } = new()
        {
            Estado = true,
            Rol = "Empleado"
        };


        [BindProperty]
        public string? ClaveInicial
        {
            get;
            set;
        }


        [BindProperty]
        public int IdUsuarioClave
        {
            get;
            set;
        }


        [BindProperty]
        public string? NuevaClave
        {
            get;
            set;
        }


        [BindProperty]
        public string? ConfirmarClave
        {
            get;
            set;
        }


        public IActionResult OnGet(
            int? id)
        {
            if (!EsAdministrador())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            CargarDatos();


            if (id.HasValue)
            {
                var usuario =
                    _usuarioDal.Obtener(
                        id.Value
                    );


                if (usuario != null)
                {
                    Usuario =
                        usuario;


                    TempData["Editar"] =
                        "1";
                }
            }


            return Page();
        }


        public IActionResult OnPostGuardar()
        {
            if (!EsAdministrador())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            try
            {
                ValidarUsuario();


                bool nuevo =
                    Usuario.IdUsuario == 0;


                _usuarioDal.Guardar(
                    Usuario,
                    ClaveInicial
                );


                TempData["Ok"] =
                    nuevo
                        ? "Usuario creado correctamente."
                        : "Usuario actualizado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }


            return RedirectToPage(
                "/Usuarios"
            );
        }


        public IActionResult OnPostCambiarClave()
        {
            if (!EsAdministrador())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            try
            {
                if (IdUsuarioClave <= 0)
                {
                    throw new Exception(
                        "Debe seleccionar un usuario."
                    );
                }


                if (string.IsNullOrWhiteSpace(
                    NuevaClave))
                {
                    throw new Exception(
                        "Debe indicar la nueva contraseña."
                    );
                }


                if (NuevaClave.Trim().Length < 4)
                {
                    throw new Exception(
                        "La contraseña debe tener al menos 4 caracteres."
                    );
                }


                if (NuevaClave != ConfirmarClave)
                {
                    throw new Exception(
                        "La confirmación de contraseña no coincide."
                    );
                }


                _usuarioDal.CambiarClave(
                    IdUsuarioClave,
                    NuevaClave
                );


                TempData["Ok"] =
                    "Contraseña actualizada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }


            return RedirectToPage(
                "/Usuarios"
            );
        }


        public IActionResult OnPostCambiarEstado(
            int id,
            bool estado)
        {
            if (!EsAdministrador())
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            try
            {
                int? idSesion =
                    HttpContext.Session.GetInt32(
                        "IdUsuario"
                    );


                if (idSesion == id &&
                    !estado)
                {
                    throw new Exception(
                        "No puede desactivar el usuario con el que inició sesión."
                    );
                }


                _usuarioDal.CambiarEstado(
                    id,
                    estado
                );


                TempData["Ok"] =
                    estado
                        ? "Usuario activado correctamente."
                        : "Usuario desactivado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }


            return RedirectToPage(
                "/Usuarios"
            );
        }


        private void ValidarUsuario()
        {
            if (string.IsNullOrWhiteSpace(
                Usuario.UsuarioNombre))
            {
                throw new Exception(
                    "Debe indicar el nombre de usuario."
                );
            }


            if (string.IsNullOrWhiteSpace(
                Usuario.NombreCompleto))
            {
                throw new Exception(
                    "Debe indicar el nombre completo."
                );
            }


            if (string.IsNullOrWhiteSpace(
                Usuario.Rol))
            {
                throw new Exception(
                    "Debe seleccionar un rol."
                );
            }


            if (Usuario.IdUsuario == 0 &&
                (
                    string.IsNullOrWhiteSpace(
                        ClaveInicial
                    ) ||
                    ClaveInicial.Trim().Length < 4
                ))
            {
                throw new Exception(
                    "La contraseña inicial debe tener al menos 4 caracteres."
                );
            }
        }


        private void CargarDatos()
        {
            Listado =
                _usuarioDal.Listar();


            Sucursales =
                _sucursalDal
                    .Listar()
                    .Where(x => x.Activo)
                    .ToList();
        }


        private bool EsAdministrador()
        {
            return Farmacia.Helpers.RolHelper.EsAdministrador(
                HttpContext.Session.GetString(
                    "Rol"
                )
            );
        }
    }
}
