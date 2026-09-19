using Farmacia.DAL;
using Farmacia.Models.AgenteBancario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace Farmacia.Pages.Agentes
{
    public class ComprobanteModel : PageModel
    {
        private readonly AgenteTransaccionDAL _transaccionDal;

        public ComprobanteModel(
            AgenteTransaccionDAL transaccionDal)
        {
            _transaccionDal = transaccionDal;
        }

        public IActionResult OnGet(
            long id)
        {
            if (string.IsNullOrWhiteSpace(
                HttpContext.Session.GetString("Usuario")))
            {
                return RedirectToPage("/Login");
            }

            var transaccion =
                _transaccionDal.Obtener(id);

            if (transaccion == null)
            {
                return NotFound();
            }

            return File(
                GenerarPdf(transaccion),
                "application/pdf",
                $"comprobante-agente-{id}.pdf"
            );
        }

        private static byte[] GenerarPdf(
            AgenteTransaccion t)
        {
            using var doc =
                new PdfDocument();

            var page =
                doc.AddPage();

            page.Width =
                MmToPt(80);

            page.Height =
                MmToPt(150);

            var gfx =
                XGraphics.FromPdfPage(page);

            var fontTitle =
                new XFont("Verdana", 12, XFontStyle.Bold);

            var fontBold =
                new XFont("Verdana", 9, XFontStyle.Bold);

            var fontNorm =
                new XFont("Verdana", 8, XFontStyle.Regular);

            var center =
                new XStringFormat
                {
                    Alignment =
                        XStringAlignment.Center
                };

            double left =
                MmToPt(5);

            double right =
                page.Width - MmToPt(5);

            double width =
                right - left;

            double y =
                14;

            DrawCenter(gfx, "COMPROBANTE DE AGENTE", fontTitle, y, page.Width, center);
            y += 18;

            DrawCenter(gfx, t.NombreAgente, fontBold, y, page.Width, center);
            y += 14;

            DrawLine(gfx, ref y, left, right);

            DrawRow(gfx, "No.", t.IdTransaccion.ToString(), fontNorm, fontBold, left, width, ref y);
            DrawRow(gfx, "Fecha", t.FechaTransaccion.ToString("dd/MM/yyyy HH:mm"), fontNorm, fontBold, left, width, ref y);
            DrawRow(gfx, "Operacion", t.NombreOperacion, fontNorm, fontBold, left, width, ref y);
            DrawRow(gfx, "Efectivo", EfectoTexto(t.EfectoEfectivo), fontNorm, fontBold, left, width, ref y);

            DrawLine(gfx, ref y, left, right);

            DrawCenter(gfx, "MONTO ENTREGADO", fontNorm, y, page.Width, center);
            y += 13;

            DrawCenter(
                gfx,
                $"{Simbolo(t.Moneda)} {t.Monto:N2}",
                new XFont("Verdana", 16, XFontStyle.Bold),
                y,
                page.Width,
                center);

            y += 21;

            if (string.Equals(t.Moneda, "USD", StringComparison.OrdinalIgnoreCase))
            {
                DrawCenter(
                    gfx,
                    $"Tasa: {t.TasaCambio:N4}  Eq. C$ {t.MontoCordoba:N2}",
                    fontNorm,
                    y,
                    page.Width,
                    center);

                y += 13;
            }

            DrawLine(gfx, ref y, left, right);

            DrawRow(
                gfx,
                "Referencia",
                string.IsNullOrWhiteSpace(t.NumeroReferencia)
                    ? "SIN REFERENCIA"
                    : t.NumeroReferencia,
                fontNorm,
                fontBold,
                left,
                width,
                ref y);

            DrawRow(gfx, "Usuario", t.NombreUsuario, fontNorm, fontBold, left, width, ref y);
            DrawRow(gfx, "Caja", t.NombreCaja, fontNorm, fontBold, left, width, ref y);

            if (!string.IsNullOrWhiteSpace(t.Observacion))
            {
                DrawRow(gfx, "Obs.", t.Observacion, fontNorm, fontBold, left, width, ref y);
            }

            DrawLine(gfx, ref y, left, right);

            DrawCenter(gfx, "Adjuntar al comprobante emitido por el banco.", fontNorm, y, page.Width, center);
            y += 13;

            DrawCenter(gfx, "Firma cliente: __________________", fontNorm, y, page.Width, center);

            using var ms =
                new MemoryStream();

            doc.Save(ms);

            return ms.ToArray();
        }

        private static void DrawRow(
            XGraphics gfx,
            string label,
            string? value,
            XFont fontLabel,
            XFont fontValue,
            double left,
            double width,
            ref double y)
        {
            gfx.DrawString(label, fontLabel, XBrushes.Black, left, y);
            gfx.DrawString(
                value ?? "",
                fontValue,
                XBrushes.Black,
                new XRect(left + 72, y - 10, width - 72, 12),
                new XStringFormat
                {
                    Alignment =
                        XStringAlignment.Far
                });

            y += 13;
        }

        private static void DrawCenter(
            XGraphics gfx,
            string text,
            XFont font,
            double y,
            double pageWidth,
            XStringFormat center)
        {
            gfx.DrawString(
                text,
                font,
                XBrushes.Black,
                new XRect(0, y - 10, pageWidth, 14),
                center);
        }

        private static void DrawLine(
            XGraphics gfx,
            ref double y,
            double left,
            double right)
        {
            gfx.DrawLine(XPens.Black, left, y, right, y);
            y += 12;
        }

        private static string Simbolo(
            string moneda)
        {
            return string.Equals(moneda, "USD", StringComparison.OrdinalIgnoreCase)
                ? "US$"
                : "C$";
        }

        private static string EfectoTexto(
            short efecto)
        {
            return efecto switch
            {
                1 => "Entrada",
                -1 => "Salida",
                _ => "Sin efecto"
            };
        }

        private static double MmToPt(
            double mm)
        {
            return mm / 25.4 * 72.0;
        }
    }
}
