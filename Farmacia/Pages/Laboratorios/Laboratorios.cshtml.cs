using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmacia.Pages.Laboratorios
{
    public class LaboratoriosModel : PageModel
    {
        private readonly LaboratorioDAL _dal;

        public LaboratoriosModel(LaboratorioDAL dal)
        {
            _dal = dal;
        }

        public List<Laboratorio> Listado { get; set; } = new();

        // Mantiene compatible la vista histórica /Laboratorios.
        public List<Laboratorio> Lista => Listado;

        [BindProperty]
        public Laboratorio Laboratorio { get; set; } = new();

        public IActionResult OnGet(int? id, int? eliminar)
        {
            try
            {
                if (eliminar.HasValue)
                {
                    _dal.Eliminar(eliminar.Value);
                    TempData["Ok"] = "Laboratorio eliminado correctamente.";
                    return RedirectToPage("/Laboratorios/Laboratorios");
                }

                if (id.HasValue)
                {
                    var laboratorio = _dal.Obtener(id.Value);

                    if (laboratorio == null)
                    {
                        TempData["Error"] = "No se encontró el laboratorio solicitado.";
                        return RedirectToPage("/Laboratorios/Laboratorios");
                    }

                    Laboratorio = laboratorio;
                    TempData["Editar"] = "1";
                }

                Listado = _dal.Listar();
                return Page();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error: " + ex.Message;
                return RedirectToPage("/Laboratorios/Laboratorios");
            }
        }

        public IActionResult OnPost()
        {
            Laboratorio.LaboratorioNombre = Laboratorio.LaboratorioNombre?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(Laboratorio.LaboratorioNombre))
            {
                ModelState.AddModelError(
                    "Laboratorio.LaboratorioNombre",
                    "El nombre del laboratorio es obligatorio.");
            }

            if (!ModelState.IsValid)
            {
                Listado = _dal.Listar();
                TempData["Editar"] = "1";
                return Page();
            }

            try
            {
                var esNuevo = Laboratorio.IdLaboratorio == 0;
                _dal.Guardar(Laboratorio);

                TempData["Ok"] = esNuevo
                    ? "Laboratorio creado correctamente."
                    : "Laboratorio actualizado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error: " + ex.Message;
            }

            return RedirectToPage("/Laboratorios/Laboratorios");
        }
    }
}
