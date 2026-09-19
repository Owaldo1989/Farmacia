namespace Farmacia.Helpers
{
    public static class RolHelper
    {
        public static string NormalizarRol(
            string? rol)
        {
            return rol?.Trim()
                   ?? string.Empty;
        }


        public static bool EsAdministrador(
            string? rol)
        {
            var rolNormalizado =
                NormalizarRol(
                    rol
                );


            return string.Equals(
                       rolNormalizado,
                       "Administrador",
                       StringComparison.OrdinalIgnoreCase
                   ) ||
                   string.Equals(
                       rolNormalizado,
                       "Admin",
                       StringComparison.OrdinalIgnoreCase
                   ) ||
                   string.Equals(
                       rolNormalizado,
                       "Aministrador",
                       StringComparison.OrdinalIgnoreCase
                   );
        }
    }
}
