using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using Farmacia.DAL;
using Farmacia.Models;
using System.IO;

namespace Farmacia.Pages.Facturas
{
    public class TicketModel : PageModel
    {
        private readonly FacturaDAL _facturaDal;

        public TicketModel(FacturaDAL facturaDal)
        {
            _facturaDal = facturaDal;
        }

        public IActionResult OnGet(int id)
        {
            var factura = _facturaDal.ObtenerFacturaCompleta(id);
            var bytes = GenerarPDF(factura);
            return File(bytes, "application/pdf");
        }

        private byte[] GenerarPDF(FacturaCompleta f)
        {
            PdfDocument doc = new PdfDocument();
            PdfPage page = doc.AddPage();

            // ===== RPT004: papel 80mm, imprimible ~72mm =====
            double paperW = MmToPt(80);
            double printableW = MmToPt(72);

            page.Width = paperW;

            // ===== Fuentes (jerarquía) =====
            var fontTitle = new XFont("Verdana", 12, XFontStyle.Bold);
            var fontBold = new XFont("Verdana", 11, XFontStyle.Bold);
            var fontNorm = new XFont("Verdana", 10, XFontStyle.Regular);

            var fontDet = new XFont("Verdana", 10, XFontStyle.Regular);
            var fontCol = new XFont("Verdana", 10, XFontStyle.Bold);

            var gfx = XGraphics.FromPdfPage(page);

            // Alturas reales por fuente (sin GetHeight(gfx))
            double lhTitle = LineHeight(gfx, fontTitle);
            double lhBold = LineHeight(gfx, fontBold);
            double lhNorm = LineHeight(gfx, fontNorm);
            double lhDet = LineHeight(gfx, fontDet);
            double lhCol = LineHeight(gfx, fontCol);

            // ===== Espaciados (ajustados para evitar “montado”) =====
            double gapSmall = 2;   // entre líneas cortas
            double gapBlock = 8;   // entre bloques

            double gapDescToNums = 5;  // ✅ espacio entre descripción y fila numérica
            double gapNumsToNext = 10; // ✅ espacio fuerte entre fila numérica y siguiente producto (anti-montado)

            // ===== Área imprimible centrada =====
            double printableLeft = (page.Width - printableW) / 2.0;
            double printableRight = printableLeft + printableW;

            double pad = 4; // padding interno seguro
            double left = printableLeft + pad;
            double right = printableRight - pad;
            double contentW = right - left;

            var center = new XStringFormat { Alignment = XStringAlignment.Center };
            var rightAlign = new XStringFormat { Alignment = XStringAlignment.Far };

            // ===== Columnas para la fila numérica (en 72mm imprimibles) =====
            // Ajustadas para Verdana 10 y números sin encimarse
            double colGap = 8;
            double qtyW = 36;
            double priceW = 62;

            double qtyX = left;
            double priceX = qtyX + qtyW + colGap;
            double subX = priceX + priceW + colGap;
            double subW = right - subX;

            // ===== Altura dinámica =====
            int items = f?.Detalles?.Count ?? 0;
            // Cada item: 1 línea descripción + 1 línea números + espacios
            double perItem = (lhDet + gapDescToNums) + (lhDet + gapNumsToNext);
            page.Height = System.Math.Max(980, 520 + items * perItem);

            double y = 12;

            // ===== ENCABEZADO =====
            gfx.DrawString(TruncarTexto(gfx, f?.NombreFarmacia ?? "", fontTitle, contentW),
                fontTitle, XBrushes.Black, new XRect(printableLeft, y, printableW, lhTitle), center);
            y += lhTitle + gapSmall;

            gfx.DrawString(TruncarTexto(gfx, f?.Direccion ?? "", fontNorm, contentW),
                fontNorm, XBrushes.Black, new XRect(printableLeft, y, printableW, lhNorm), center);
            y += lhNorm + gapSmall;

            gfx.DrawString(TruncarTexto(gfx, $"Tel: {f?.Telefono}", fontNorm, contentW),
                fontNorm, XBrushes.Black, new XRect(printableLeft, y, printableW, lhNorm), center);
            y += lhNorm + gapBlock;

            DrawLine(gfx, ref y, printableLeft, printableRight);

            // ===== DATOS FACTURA =====
            gfx.DrawString($"No: {f.IdFactura}", fontBold, XBrushes.Black, left, y);
            y += lhBold + gapSmall;

            gfx.DrawString($"FECHA: {f.Fecha:dd/MM/yyyy HH:mm}", fontNorm, XBrushes.Black, left, y);
            y += lhNorm + gapSmall;

            gfx.DrawString(TruncarTexto(gfx, $"PACIENTE: {f.Paciente}", fontNorm, contentW),
                fontNorm, XBrushes.Black, left, y);
            y += lhNorm + gapBlock;

            DrawLine(gfx, ref y, printableLeft, printableRight);

            // ===== CABECERAS =====
            gfx.DrawString("Producto", fontCol, XBrushes.Black, left, y);
            y += lhCol + gapSmall;

            gfx.DrawString("Cant", fontCol, XBrushes.Black, qtyX, y);
            gfx.DrawString("Precio", fontCol, XBrushes.Black, priceX, y);
            gfx.DrawString("SubTot", fontCol, XBrushes.Black, subX, y);
            y += lhCol + gapBlock;

            DrawLine(gfx, ref y, printableLeft, printableRight);

            // ===== DETALLES (2 líneas por item + espaciado extra) =====
            foreach (var d in f.Detalles)
            {
                // 1) Descripción (una sola línea)
                string nombre = TruncarTexto(gfx, d.NombreProducto ?? "", fontDet, contentW);
                gfx.DrawString(nombre, fontDet, XBrushes.Black, left, y);
                y += lhDet + gapDescToNums; // ✅ más aire antes de los números

                // 2) Cant / Precio / SubTot
                gfx.DrawString(d.Cantidad.ToString(), fontDet, XBrushes.Black,
                    new XRect(qtyX, y, qtyW, lhDet), rightAlign);

                gfx.DrawString($"{d.Precio:0.00}", fontDet, XBrushes.Black,
                    new XRect(priceX, y, priceW, lhDet), rightAlign);

                gfx.DrawString($"{d.SubTotal:0.00}", fontDet, XBrushes.Black,
                    new XRect(subX, y, subW, lhDet), rightAlign);

                y += lhDet + gapNumsToNext; // ✅ ESTE espacio evita el “montado” con el siguiente nombre
            }

            DrawLine(gfx, ref y, printableLeft, printableRight);

            // ===== TOTALES =====
            gfx.DrawString($"TOTAL: C${f.Total:0.00}", fontBold, XBrushes.Black, left, y);
            y += lhBold + gapBlock;

            gfx.DrawString($"Pago C$: {f.PagoCordoba:0.00}", fontNorm, XBrushes.Black, left, y);
            y += lhNorm + gapSmall;

            gfx.DrawString($"Pago US$: {f.PagoDolar:0.00}", fontNorm, XBrushes.Black, left, y);
            y += lhNorm + gapSmall;

            gfx.DrawString($"Vuelto: C${f.Vuelto:0.00}", fontBold, XBrushes.Black, left, y);
            y += lhBold + (gapBlock * 2);

            gfx.DrawString("GRACIAS POR SU PREFERENCIA", fontBold, XBrushes.Black,
                new XRect(printableLeft, y, printableW, lhBold), center);

            using var ms = new MemoryStream();
            doc.Save(ms);
            return ms.ToArray();
        }

        private static double MmToPt(double mm) => mm / 25.4 * 72.0;

        private static double LineHeight(XGraphics gfx, XFont font)
            => gfx.MeasureString("Ag", font).Height;

        private static void DrawLine(XGraphics gfx, ref double y, double left, double right)
        {
            // línea fina real (más limpia que guiones)
            gfx.DrawLine(XPens.Black, left, y, right, y);
            y += 10;
        }

        private static string TruncarTexto(XGraphics gfx, string texto, XFont font, double maxWidth)
        {
            if (string.IsNullOrWhiteSpace(texto)) return "";

            if (gfx.MeasureString(texto, font).Width <= maxWidth)
                return texto;

            const string ellipsis = "...";
            string t = texto;

            while (t.Length > 1 && gfx.MeasureString(t + ellipsis, font).Width > maxWidth)
                t = t.Substring(0, t.Length - 1);

            return t + ellipsis;
        }
    }
}
