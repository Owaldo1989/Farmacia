using Farmacia.DAL;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace Farmacia.Pages.Ingresos
{
    public class ReporteModel : PageModel
    {
        private readonly IngresoMercaderiaDAL _dal;

        public ReporteModel(
            IngresoMercaderiaDAL dal)
        {
            _dal = dal;
        }


        public IActionResult OnGet(int id)
        {
            if (
                string.IsNullOrWhiteSpace(
                    HttpContext.Session.GetString(
                        "Usuario"
                    )
                )
            )
            {
                return RedirectToPage(
                    "/Login"
                );
            }


            var ingreso =
                _dal.Obtener(id);


            if (ingreso == null)
            {
                return NotFound();
            }


            var pdf =
                GenerarPDF(ingreso);


            return File(
                pdf,
                "application/pdf",
                $"IngresoMercaderia_{id}.pdf"
            );
        }



        // =====================================================
        // GENERAR PDF
        // =====================================================

        private byte[] GenerarPDF(
            IngresoMercaderiaCompletoDTO data)
        {
            var documento =
                new PdfDocument();


            documento.Info.Title =
                $"Ingreso de Mercadería #{data.Ingreso.IdIngreso}";


            // =================================================
            // FUENTES
            // =================================================

            var fontTitulo =
                new XFont(
                    "Verdana",
                    16,
                    XFontStyle.Bold
                );


            var fontSubtitulo =
                new XFont(
                    "Verdana",
                    9,
                    XFontStyle.Regular
                );


            var fontSeccion =
                new XFont(
                    "Verdana",
                    10,
                    XFontStyle.Bold
                );


            var fontNormal =
                new XFont(
                    "Verdana",
                    8,
                    XFontStyle.Regular
                );


            var fontBold =
                new XFont(
                    "Verdana",
                    8,
                    XFontStyle.Bold
                );


            var fontTabla =
                new XFont(
                    "Verdana",
                    7,
                    XFontStyle.Regular
                );


            var fontTablaBold =
                new XFont(
                    "Verdana",
                    7,
                    XFontStyle.Bold
                );


            // =================================================
            // COLORES
            // =================================================

            var verde =
                XColor.FromArgb(
                    7,
                    150,
                    103
                );


            var verdeOscuro =
                XColor.FromArgb(
                    5,
                    122,
                    85
                );


            var verdeSuave =
                XColor.FromArgb(
                    229,
                    246,
                    240
                );


            var grisFondo =
                XColor.FromArgb(
                    247,
                    249,
                    248
                );


            var grisLinea =
                XColor.FromArgb(
                    220,
                    228,
                    225
                );


            var rojoSuave =
                XColor.FromArgb(
                    255,
                    240,
                    241
                );


            var rojo =
                XColor.FromArgb(
                    190,
                    65,
                    75
                );


            var brushVerde =
                new XSolidBrush(
                    verde
                );


            var brushVerdeOscuro =
                new XSolidBrush(
                    verdeOscuro
                );


            var brushVerdeSuave =
                new XSolidBrush(
                    verdeSuave
                );


            var brushGris =
                new XSolidBrush(
                    grisFondo
                );


            var brushRojoSuave =
                new XSolidBrush(
                    rojoSuave
                );


            var brushRojo =
                new XSolidBrush(
                    rojo
                );


            var penLinea =
                new XPen(
                    grisLinea,
                    0.7
                );



            // =================================================
            // PÁGINA
            // =================================================

            PdfPage pagina =
                CrearPagina(documento);


            XGraphics gfx =
                XGraphics.FromPdfPage(
                    pagina
                );


            double margen =
                28;


            double anchoContenido =
                pagina.Width -
                margen * 2;


            double y =
                26;



            // =================================================
            // HEADER
            // =================================================

            gfx.DrawRectangle(
                brushVerde,
                0,
                0,
                pagina.Width,
                68
            );


            gfx.DrawString(
                "FARMACIA ESPECIALIDADES",
                fontTitulo,
                XBrushes.White,
                margen,
                25
            );


            gfx.DrawString(
                "REPORTE DE INGRESO DE MERCADERÍA",
                fontSubtitulo,
                XBrushes.White,
                margen,
                44
            );


            gfx.DrawString(
                $"Ingreso #{data.Ingreso.IdIngreso}",
                fontSeccion,
                XBrushes.White,
                new XRect(
                    margen,
                    24,
                    anchoContenido,
                    20
                ),
                new XStringFormat
                {
                    Alignment =
                        XStringAlignment.Far
                }
            );


            y = 85;



            // =================================================
            // DATOS DE FACTURA
            // =================================================

            DibujarTituloSeccion(
                gfx,
                "INFORMACIÓN DEL INGRESO",
                margen,
                ref y,
                anchoContenido,
                brushVerdeSuave,
                brushVerdeOscuro,
                fontSeccion
            );


            y += 6;


            double mitad =
                anchoContenido / 2;


            DibujarDato(
                gfx,
                "Proveedor:",
                data.Ingreso.NombreProveedor ?? "",
                margen,
                y,
                110,
                fontNormal,
                fontBold
            );


            DibujarDato(
                gfx,
                "Factura proveedor:",
                data.Ingreso.NumeroFactura,
                margen + mitad,
                y,
                105,
                fontNormal,
                fontBold
            );


            y += 18;


            DibujarDato(
                gfx,
                "Fecha factura:",
                data.Ingreso.FechaFactura
                    .ToString("dd/MM/yyyy"),
                margen,
                y,
                110,
                fontNormal,
                fontBold
            );


            DibujarDato(
                gfx,
                "Fecha de ingreso:",
                data.Ingreso.FechaIngreso
                    .ToString("dd/MM/yyyy HH:mm"),
                margen + mitad,
                y,
                105,
                fontNormal,
                fontBold
            );


            y += 18;


            DibujarDato(
                gfx,
                "Registrado por:",
                string.IsNullOrWhiteSpace(
                    data.Ingreso.UsuarioRegistro
                )
                    ? "-"
                    : data.Ingreso.UsuarioRegistro,
                margen,
                y,
                110,
                fontNormal,
                fontBold
            );


            DibujarDato(
                gfx,
                "Estado:",
                data.Ingreso.Estado == 1
                    ? "CONFIRMADO"
                    : data.Ingreso.Estado == 99
                        ? "ANULADO"
                        : "PENDIENTE",
                margen + mitad,
                y,
                105,
                fontNormal,
                fontBold
            );


            y += 30;



            // =================================================
            // TOTALES SUPERIORES
            // =================================================

            double separacion =
                10;


            double anchoCaja =
                (
                    anchoContenido -
                    separacion * 2
                ) / 3;


            DibujarCajaTotal(
                gfx,
                "SUBTOTAL",
                data.Ingreso.SubtotalFacturaProveedor,
                margen,
                y,
                anchoCaja,
                brushGris,
                XBrushes.Black,
                fontNormal,
                fontSeccion
            );


            DibujarCajaTotal(
                gfx,
                "IVA",
                data.Ingreso.IVAFacturaProveedor,
                margen +
                anchoCaja +
                separacion,
                y,
                anchoCaja,
                brushGris,
                XBrushes.Black,
                fontNormal,
                fontSeccion
            );


            DibujarCajaTotal(
                gfx,
                "TOTAL FACTURA",
                data.Ingreso.TotalFacturaProveedor,
                margen +
                (
                    anchoCaja +
                    separacion
                ) * 2,
                y,
                anchoCaja,
                brushVerdeSuave,
                brushVerdeOscuro,
                fontNormal,
                fontSeccion
            );


            y += 60;



            // =================================================
            // DETALLE
            // =================================================

            DibujarTituloSeccion(
                gfx,
                "PRODUCTOS RECIBIDOS",
                margen,
                ref y,
                anchoContenido,
                brushVerdeSuave,
                brushVerdeOscuro,
                fontSeccion
            );


            y += 7;



            // Anchos de columnas

            double[] columnas =
            {
                165, // producto
                38,  // cantidad
                60,  // costo anterior
                60,  // costo base
                35,  // iva
                62,  // costo real
                42,  // utilidad
                62,  // precio anterior
                62,  // sugerido
                62,  // nuevo
                72   // total
            };


            string[] encabezados =
            {
                "Producto",
                "Cant.",
                "Costo Ant.",
                "Costo Base",
                "IVA",
                "Costo Real",
                "Util.",
                "P. Ant.",
                "Sugerido",
                "P. Nuevo",
                "Total"
            };


            DibujarCabeceraTabla(
                gfx,
                margen,
                ref y,
                columnas,
                encabezados,
                brushVerde,
                fontTablaBold
            );



            // =================================================
            // FILAS
            // =================================================

            foreach (
                var item in data.Detalles
            )
            {
                const double alturaFila =
                    27;


                /*
                    Saltar de página cuando
                    ya no hay espacio.
                */

                if (
                    y + alturaFila >
                    pagina.Height - 55
                )
                {
                    gfx.Dispose();


                    pagina =
                        CrearPagina(
                            documento
                        );


                    gfx =
                        XGraphics.FromPdfPage(
                            pagina
                        );


                    y =
                        30;


                    gfx.DrawString(
                        $"Ingreso #{data.Ingreso.IdIngreso} - continuación",
                        fontSeccion,
                        brushVerdeOscuro,
                        margen,
                        y
                    );


                    y += 20;


                    DibujarCabeceraTabla(
                        gfx,
                        margen,
                        ref y,
                        columnas,
                        encabezados,
                        brushVerde,
                        fontTablaBold
                    );
                }


                double x =
                    margen;


                bool aumentoCosto =
                    item.CostoUnitarioFinal >
                    item.CostoAnterior;


                XBrush fondoFila =
                    aumentoCosto
                        ? brushRojoSuave
                        : XBrushes.White;


                // Fondo

                gfx.DrawRectangle(
                    fondoFila,
                    margen,
                    y,
                    anchoContenido,
                    alturaFila
                );



                // PRODUCTO

                string producto =
                    TruncarTexto(
                        gfx,
                        item.NombreProducto ?? "",
                        fontTabla,
                        columnas[0] - 8
                    );


                gfx.DrawString(
                    producto,
                    fontTablaBold,
                    XBrushes.Black,
                    new XRect(
                        x + 4,
                        y + 5,
                        columnas[0] - 8,
                        10
                    )
                );


                string codigo =
                    TruncarTexto(
                        gfx,
                        item.CodBarra ?? "",
                        fontTabla,
                        columnas[0] - 8
                    );


                gfx.DrawString(
                    codigo,
                    fontTabla,
                    XBrushes.Gray,
                    new XRect(
                        x + 4,
                        y + 15,
                        columnas[0] - 8,
                        9
                    )
                );


                x += columnas[0];



                DibujarCeldaNumero(
                    gfx,
                    item.Cantidad.ToString("0.##"),
                    x,
                    y,
                    columnas[1],
                    alturaFila,
                    fontTabla
                );


                x += columnas[1];



                DibujarCeldaNumero(
                    gfx,
                    item.CostoAnterior.ToString("N2"),
                    x,
                    y,
                    columnas[2],
                    alturaFila,
                    fontTabla
                );


                x += columnas[2];



                DibujarCeldaNumero(
                    gfx,
                    item.CostoUnitarioBase.ToString("N2"),
                    x,
                    y,
                    columnas[3],
                    alturaFila,
                    fontTabla
                );


                x += columnas[3];



                DibujarCeldaCentro(
                    gfx,
                    item.TasaIVA.ToString("0") + "%",
                    x,
                    y,
                    columnas[4],
                    alturaFila,
                    fontTabla
                );


                x += columnas[4];



                DibujarCeldaNumero(
                    gfx,
                    item.CostoUnitarioFinal.ToString("N2"),
                    x,
                    y,
                    columnas[5],
                    alturaFila,
                    aumentoCosto
                        ? fontTablaBold
                        : fontTabla,
                    aumentoCosto
                        ? brushRojo
                        : XBrushes.Black
                );


                x += columnas[5];



                DibujarCeldaCentro(
                    gfx,
                    item.PorcentajeUtilidad
                        .ToString("0.##") + "%",
                    x,
                    y,
                    columnas[6],
                    alturaFila,
                    fontTabla
                );


                x += columnas[6];



                DibujarCeldaNumero(
                    gfx,
                    item.PrecioVentaAnterior
                        .ToString("N2"),
                    x,
                    y,
                    columnas[7],
                    alturaFila,
                    fontTabla
                );


                x += columnas[7];



                DibujarCeldaNumero(
                    gfx,
                    (
                        item.PrecioVentaSugerido
                        ?? 0
                    ).ToString("N2"),
                    x,
                    y,
                    columnas[8],
                    alturaFila,
                    fontTabla
                );


                x += columnas[8];



                DibujarCeldaNumero(
                    gfx,
                    item.PrecioVentaNuevo
                        .ToString("N2"),
                    x,
                    y,
                    columnas[9],
                    alturaFila,
                    fontTablaBold,
                    brushVerdeOscuro
                );


                x += columnas[9];



                decimal totalLinea =
                    Math.Round(
                        item.Cantidad *
                        item.CostoUnitarioFinal,
                        2
                    );


                DibujarCeldaNumero(
                    gfx,
                    totalLinea.ToString("N2"),
                    x,
                    y,
                    columnas[10],
                    alturaFila,
                    fontTablaBold
                );



                // Línea inferior

                gfx.DrawLine(
                    penLinea,
                    margen,
                    y + alturaFila,
                    margen + anchoContenido,
                    y + alturaFila
                );


                y += alturaFila;
            }



            // =================================================
            // RESUMEN FINAL
            // =================================================

            if (
                y + 105 >
                pagina.Height - 30
            )
            {
                gfx.Dispose();


                pagina =
                    CrearPagina(
                        documento
                    );


                gfx =
                    XGraphics.FromPdfPage(
                        pagina
                    );


                y =
                    35;
            }


            y += 15;



            double resumenX =
                pagina.Width -
                margen -
                250;


            gfx.DrawRectangle(
                brushGris,
                resumenX,
                y,
                250,
                82
            );


            DibujarResumen(
                gfx,
                "Subtotal:",
                data.Ingreso.SubtotalFacturaProveedor,
                resumenX,
                ref y,
                250,
                fontNormal,
                fontBold
            );


            DibujarResumen(
                gfx,
                "IVA:",
                data.Ingreso.IVAFacturaProveedor,
                resumenX,
                ref y,
                250,
                fontNormal,
                fontBold
            );


            DibujarResumen(
                gfx,
                "TOTAL:",
                data.Ingreso.TotalFacturaProveedor,
                resumenX,
                ref y,
                250,
                fontSeccion,
                fontSeccion,
                brushVerdeOscuro
            );



            // =================================================
            // OBSERVACIÓN
            // =================================================

            if (
                !string.IsNullOrWhiteSpace(
                    data.Ingreso.Observacion
                )
            )
            {
                y += 20;


                gfx.DrawString(
                    "Observación:",
                    fontBold,
                    XBrushes.Black,
                    margen,
                    y
                );


                y += 13;


                gfx.DrawString(
                    TruncarTexto(
                        gfx,
                        data.Ingreso.Observacion,
                        fontNormal,
                        anchoContenido
                    ),
                    fontNormal,
                    XBrushes.Black,
                    margen,
                    y
                );
            }



            // =================================================
            // PIE
            // =================================================

            DibujarPie(
                gfx,
                pagina,
                data.Ingreso.IdIngreso,
                fontNormal
            );


            gfx.Dispose();


            using var ms =
                new MemoryStream();


            documento.Save(ms);


            return ms.ToArray();
        }



        // =====================================================
        // CREAR A4 HORIZONTAL
        // =====================================================

        private static PdfPage CrearPagina(
            PdfDocument documento)
        {
            var pagina =
                documento.AddPage();


            /*
                A4 horizontal en puntos:
                297mm x 210mm
            */

            pagina.Width =
                842;


            pagina.Height =
                595;


            return pagina;
        }



        // =====================================================
        // TÍTULO SECCIÓN
        // =====================================================

        private static void DibujarTituloSeccion(
            XGraphics gfx,
            string titulo,
            double x,
            ref double y,
            double ancho,
            XBrush fondo,
            XBrush texto,
            XFont fuente)
        {
            gfx.DrawRectangle(
                fondo,
                x,
                y,
                ancho,
                24
            );


            gfx.DrawString(
                titulo,
                fuente,
                texto,
                new XRect(
                    x + 8,
                    y,
                    ancho - 16,
                    24
                ),
                new XStringFormat
                {
                    LineAlignment =
                        XLineAlignment.Center
                }
            );


            y += 24;
        }



        // =====================================================
        // DATOS
        // =====================================================

        private static void DibujarDato(
            XGraphics gfx,
            string etiqueta,
            string valor,
            double x,
            double y,
            double anchoEtiqueta,
            XFont normal,
            XFont bold)
        {
            gfx.DrawString(
                etiqueta,
                normal,
                XBrushes.Gray,
                x,
                y
            );


            gfx.DrawString(
                valor,
                bold,
                XBrushes.Black,
                x + anchoEtiqueta,
                y
            );
        }



        // =====================================================
        // CAJAS TOTAL
        // =====================================================

        private static void DibujarCajaTotal(
            XGraphics gfx,
            string titulo,
            decimal valor,
            double x,
            double y,
            double ancho,
            XBrush fondo,
            XBrush texto,
            XFont fontTitulo,
            XFont fontValor)
        {
            gfx.DrawRectangle(
                fondo,
                x,
                y,
                ancho,
                46
            );


            gfx.DrawString(
                titulo,
                fontTitulo,
                XBrushes.Gray,
                x + 10,
                y + 13
            );


            gfx.DrawString(
                $"C$ {valor:N2}",
                fontValor,
                texto,
                new XRect(
                    x + 10,
                    y + 23,
                    ancho - 20,
                    18
                ),
                new XStringFormat
                {
                    Alignment =
                        XStringAlignment.Far
                }
            );
        }



        // =====================================================
        // CABECERA TABLA
        // =====================================================

        private static void DibujarCabeceraTabla(
            XGraphics gfx,
            double xInicial,
            ref double y,
            double[] columnas,
            string[] encabezados,
            XBrush fondo,
            XFont fuente)
        {
            const double alto =
                25;


            double x =
                xInicial;


            for (
                int i = 0;
                i < columnas.Length;
                i++
            )
            {
                gfx.DrawRectangle(
                    fondo,
                    x,
                    y,
                    columnas[i],
                    alto
                );


                gfx.DrawString(
                    encabezados[i],
                    fuente,
                    XBrushes.White,
                    new XRect(
                        x + 3,
                        y,
                        columnas[i] - 6,
                        alto
                    ),
                    new XStringFormat
                    {
                        Alignment =
                            i == 0
                                ? XStringAlignment.Near
                                : XStringAlignment.Center,

                        LineAlignment =
                            XLineAlignment.Center
                    }
                );


                x += columnas[i];
            }


            y += alto;
        }



        // =====================================================
        // CELDA NUMÉRICA
        // =====================================================

        private static void DibujarCeldaNumero(
            XGraphics gfx,
            string texto,
            double x,
            double y,
            double ancho,
            double alto,
            XFont fuente,
            XBrush? brush = null)
        {
            gfx.DrawString(
                texto,
                fuente,
                brush ?? XBrushes.Black,
                new XRect(
                    x + 3,
                    y,
                    ancho - 6,
                    alto
                ),
                new XStringFormat
                {
                    Alignment =
                        XStringAlignment.Far,

                    LineAlignment =
                        XLineAlignment.Center
                }
            );
        }



        private static void DibujarCeldaCentro(
            XGraphics gfx,
            string texto,
            double x,
            double y,
            double ancho,
            double alto,
            XFont fuente)
        {
            gfx.DrawString(
                texto,
                fuente,
                XBrushes.Black,
                new XRect(
                    x,
                    y,
                    ancho,
                    alto
                ),
                new XStringFormat
                {
                    Alignment =
                        XStringAlignment.Center,

                    LineAlignment =
                        XLineAlignment.Center
                }
            );
        }



        // =====================================================
        // RESUMEN
        // =====================================================

        private static void DibujarResumen(
            XGraphics gfx,
            string etiqueta,
            decimal valor,
            double x,
            ref double y,
            double ancho,
            XFont fontEtiqueta,
            XFont fontValor,
            XBrush? brush = null)
        {
            gfx.DrawString(
                etiqueta,
                fontEtiqueta,
                XBrushes.Gray,
                x + 12,
                y + 15
            );


            gfx.DrawString(
                $"C$ {valor:N2}",
                fontValor,
                brush ?? XBrushes.Black,
                new XRect(
                    x,
                    y + 7,
                    ancho - 12,
                    18
                ),
                new XStringFormat
                {
                    Alignment =
                        XStringAlignment.Far
                }
            );


            y += 22;
        }



        // =====================================================
        // PIE
        // =====================================================

        private static void DibujarPie(
            XGraphics gfx,
            PdfPage pagina,
            int idIngreso,
            XFont fuente)
        {
            const double margen =
                28;


            double y =
                pagina.Height - 25;


            gfx.DrawLine(
                XPens.LightGray,
                margen,
                y - 7,
                pagina.Width - margen,
                y - 7
            );


            gfx.DrawString(
                $"Ingreso #{idIngreso} | Generado {DateTime.Now:dd/MM/yyyy HH:mm}",
                fuente,
                XBrushes.Gray,
                margen,
                y
            );
        }



        // =====================================================
        // TRUNCAR
        // =====================================================

        private static string TruncarTexto(
            XGraphics gfx,
            string texto,
            XFont fuente,
            double anchoMaximo)
        {
            if (
                string.IsNullOrWhiteSpace(
                    texto
                )
            )
            {
                return "";
            }


            if (
                gfx.MeasureString(
                    texto,
                    fuente
                ).Width <= anchoMaximo
            )
            {
                return texto;
            }


            const string puntos =
                "...";


            string resultado =
                texto;


            while (
                resultado.Length > 1 &&
                gfx.MeasureString(
                    resultado + puntos,
                    fuente
                ).Width > anchoMaximo
            )
            {
                resultado =
                    resultado.Substring(
                        0,
                        resultado.Length - 1
                    );
            }


            return resultado +
                   puntos;
        }
    }
}
