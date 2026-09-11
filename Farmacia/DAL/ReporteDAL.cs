using System.Data;
using System.Data.SqlClient;
using Farmacia.Models;

namespace Farmacia.DAL
{
    public class ReporteDAL
    {
        private readonly string _cn;

        public ReporteDAL(IConfiguration config)
        {
            _cn = config.GetConnectionString("BDFarmacia");
        }

        public ReporteResumenVentasDTO ObtenerResumen(
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            const string sql = @"
                SELECT
                    (SELECT COUNT(*)
                     FROM dbo.tbFactura F
                     WHERE ISNULL(F.Anulada, 0) = 0
                       AND F.Fecha >= @FechaInicio
                       AND F.Fecha < @FechaFinExclusiva) AS Facturas,
                    (SELECT ISNULL(SUM(F.Total), 0)
                     FROM dbo.tbFactura F
                     WHERE ISNULL(F.Anulada, 0) = 0
                       AND F.Fecha >= @FechaInicio
                       AND F.Fecha < @FechaFinExclusiva) AS TotalVentas,
                    (SELECT ISNULL(SUM(D.Cantidad), 0)
                     FROM dbo.tbFacturaDetalle D
                     INNER JOIN dbo.tbFactura F ON F.IdFactura = D.IdFactura
                     WHERE ISNULL(F.Anulada, 0) = 0
                       AND F.Fecha >= @FechaInicio
                       AND F.Fecha < @FechaFinExclusiva) AS UnidadesVendidas,
                    (SELECT COUNT(DISTINCT D.IdProducto)
                     FROM dbo.tbFacturaDetalle D
                     INNER JOIN dbo.tbFactura F ON F.IdFactura = D.IdFactura
                     WHERE ISNULL(F.Anulada, 0) = 0
                       AND F.Fecha >= @FechaInicio
                       AND F.Fecha < @FechaFinExclusiva) AS ProductosVendidos;";

            using var cn = new SqlConnection(_cn);
            using var cmd = new SqlCommand(sql, cn);
            AgregarParametrosPeriodo(cmd, fechaInicio, fechaFin);

            cn.Open();
            using var dr = cmd.ExecuteReader();

            if (!dr.Read())
                return new ReporteResumenVentasDTO();

            return new ReporteResumenVentasDTO
            {
                Facturas = Convert.ToInt32(dr["Facturas"]),
                TotalVentas = Convert.ToDecimal(dr["TotalVentas"]),
                UnidadesVendidas = Convert.ToDecimal(dr["UnidadesVendidas"]),
                ProductosVendidos = Convert.ToInt32(dr["ProductosVendidos"])
            };
        }

        public List<ReporteVentasFilaDTO> ObtenerReporte(
            string tipo,
            DateTime fechaInicio,
            DateTime fechaFin,
            decimal limiteRotacion)
        {
            var sql = tipo switch
            {
                "ventas-mes" => SqlVentasPorMes,
                "producto" => SqlVentasPorProducto,
                "categoria" => SqlVentasPorCategoria,
                "proveedor" => SqlVentasPorProveedor,
                "laboratorio" => SqlVentasPorLaboratorio,
                "mas-vendidos" => SqlProductosMasVendidos,
                "poca-rotacion" => SqlProductosPocaRotacion,
                _ => SqlVentasPorDia
            };

            var lista = new List<ReporteVentasFilaDTO>();

            using var cn = new SqlConnection(_cn);
            using var cmd = new SqlCommand(sql, cn);
            AgregarParametrosPeriodo(cmd, fechaInicio, fechaFin);

            var parametroLimite = cmd.Parameters.Add(
                "@LimiteRotacion",
                SqlDbType.Decimal);
            parametroLimite.Precision = 18;
            parametroLimite.Scale = 2;
            parametroLimite.Value = limiteRotacion;

            cn.Open();
            using var dr = cmd.ExecuteReader();

            while (dr.Read())
                lista.Add(MapearFila(dr));

            return lista;
        }

        private static void AgregarParametrosPeriodo(
            SqlCommand cmd,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            cmd.Parameters.Add("@FechaInicio", SqlDbType.Date).Value =
                fechaInicio.Date;
            cmd.Parameters.Add("@FechaFinExclusiva", SqlDbType.DateTime).Value =
                fechaFin.Date.AddDays(1);
        }

        private static ReporteVentasFilaDTO MapearFila(SqlDataReader dr)
        {
            return new ReporteVentasFilaDTO
            {
                Etiqueta = dr["Etiqueta"]?.ToString() ?? "",
                Codigo = dr["Codigo"] == DBNull.Value
                    ? ""
                    : dr["Codigo"].ToString() ?? "",
                Unidades = Convert.ToDecimal(dr["Unidades"]),
                Facturas = Convert.ToInt32(dr["Facturas"]),
                TotalVenta = Convert.ToDecimal(dr["TotalVenta"]),
                Inventario = Convert.ToDecimal(dr["Inventario"]),
                UltimaVenta = dr["UltimaVenta"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(dr["UltimaVenta"])
            };
        }

        private const string SqlVentasPorDia = @"
            SELECT
                CONVERT(char(10), F.Fecha, 103) AS Etiqueta,
                CAST(NULL AS nvarchar(100)) AS Codigo,
                SUM(D.Cantidad) AS Unidades,
                COUNT(DISTINCT F.IdFactura) AS Facturas,
                SUM(D.SubTotal) AS TotalVenta,
                CAST(0 AS decimal(18,2)) AS Inventario,
                CAST(NULL AS datetime) AS UltimaVenta
            FROM dbo.tbFactura F
            INNER JOIN dbo.tbFacturaDetalle D ON D.IdFactura = F.IdFactura
            WHERE ISNULL(F.Anulada, 0) = 0
              AND F.Fecha >= @FechaInicio
              AND F.Fecha < @FechaFinExclusiva
            GROUP BY CONVERT(date, F.Fecha), CONVERT(char(10), F.Fecha, 103)
            ORDER BY CONVERT(date, F.Fecha) DESC;";

        private const string SqlVentasPorMes = @"
            SELECT
                CONVERT(char(7), F.Fecha, 120) AS Etiqueta,
                CAST(NULL AS nvarchar(100)) AS Codigo,
                SUM(D.Cantidad) AS Unidades,
                COUNT(DISTINCT F.IdFactura) AS Facturas,
                SUM(D.SubTotal) AS TotalVenta,
                CAST(0 AS decimal(18,2)) AS Inventario,
                CAST(NULL AS datetime) AS UltimaVenta
            FROM dbo.tbFactura F
            INNER JOIN dbo.tbFacturaDetalle D ON D.IdFactura = F.IdFactura
            WHERE ISNULL(F.Anulada, 0) = 0
              AND F.Fecha >= @FechaInicio
              AND F.Fecha < @FechaFinExclusiva
            GROUP BY CONVERT(char(7), F.Fecha, 120)
            ORDER BY CONVERT(char(7), F.Fecha, 120) DESC;";

        private const string SqlVentasPorProducto = @"
            SELECT
                P.NombreProducto AS Etiqueta,
                P.CodBarra AS Codigo,
                SUM(D.Cantidad) AS Unidades,
                COUNT(DISTINCT F.IdFactura) AS Facturas,
                SUM(D.SubTotal) AS TotalVenta,
                P.Inventario AS Inventario,
                MAX(F.Fecha) AS UltimaVenta
            FROM dbo.tbFacturaDetalle D
            INNER JOIN dbo.tbFactura F ON F.IdFactura = D.IdFactura
            INNER JOIN dbo.tbProductos P ON P.IdProducto = D.IdProducto
            WHERE ISNULL(F.Anulada, 0) = 0
              AND F.Fecha >= @FechaInicio
              AND F.Fecha < @FechaFinExclusiva
            GROUP BY P.IdProducto, P.NombreProducto, P.CodBarra, P.Inventario
            ORDER BY SUM(D.SubTotal) DESC, SUM(D.Cantidad) DESC;";

        private const string SqlVentasPorCategoria = @"
            SELECT
                COALESCE(C.NombreCategoria, N'Sin categoría') AS Etiqueta,
                CAST(NULL AS nvarchar(100)) AS Codigo,
                SUM(D.Cantidad) AS Unidades,
                COUNT(DISTINCT F.IdFactura) AS Facturas,
                SUM(D.SubTotal) AS TotalVenta,
                CAST(0 AS decimal(18,2)) AS Inventario,
                CAST(NULL AS datetime) AS UltimaVenta
            FROM dbo.tbFacturaDetalle D
            INNER JOIN dbo.tbFactura F ON F.IdFactura = D.IdFactura
            INNER JOIN dbo.tbProductos P ON P.IdProducto = D.IdProducto
            LEFT JOIN dbo.tbCategorias C ON C.IdCategoria = P.IdCategoria
            WHERE ISNULL(F.Anulada, 0) = 0
              AND F.Fecha >= @FechaInicio
              AND F.Fecha < @FechaFinExclusiva
            GROUP BY C.IdCategoria, C.NombreCategoria
            ORDER BY SUM(D.SubTotal) DESC;";

        private const string SqlVentasPorProveedor = @"
            SELECT
                COALESCE(PR.NombreProveedor, N'Sin proveedor') AS Etiqueta,
                CAST(NULL AS nvarchar(100)) AS Codigo,
                SUM(D.Cantidad) AS Unidades,
                COUNT(DISTINCT F.IdFactura) AS Facturas,
                SUM(D.SubTotal) AS TotalVenta,
                CAST(0 AS decimal(18,2)) AS Inventario,
                CAST(NULL AS datetime) AS UltimaVenta
            FROM dbo.tbFacturaDetalle D
            INNER JOIN dbo.tbFactura F ON F.IdFactura = D.IdFactura
            INNER JOIN dbo.tbProductos P ON P.IdProducto = D.IdProducto
            LEFT JOIN dbo.tbProveedor PR ON PR.IdProveedor = P.IdProveedor
            WHERE ISNULL(F.Anulada, 0) = 0
              AND F.Fecha >= @FechaInicio
              AND F.Fecha < @FechaFinExclusiva
            GROUP BY PR.IdProveedor, PR.NombreProveedor
            ORDER BY SUM(D.SubTotal) DESC;";

        private const string SqlVentasPorLaboratorio = @"
            SELECT
                COALESCE(L.Laboratorio, N'Sin laboratorio') AS Etiqueta,
                CAST(NULL AS nvarchar(100)) AS Codigo,
                SUM(D.Cantidad) AS Unidades,
                COUNT(DISTINCT F.IdFactura) AS Facturas,
                SUM(D.SubTotal) AS TotalVenta,
                CAST(0 AS decimal(18,2)) AS Inventario,
                CAST(NULL AS datetime) AS UltimaVenta
            FROM dbo.tbFacturaDetalle D
            INNER JOIN dbo.tbFactura F ON F.IdFactura = D.IdFactura
            INNER JOIN dbo.tbProductos P ON P.IdProducto = D.IdProducto
            LEFT JOIN dbo.tbLaboratorios L ON L.IdLaboratorio = P.IdLaboratorio
            WHERE ISNULL(F.Anulada, 0) = 0
              AND F.Fecha >= @FechaInicio
              AND F.Fecha < @FechaFinExclusiva
            GROUP BY L.IdLaboratorio, L.Laboratorio
            ORDER BY SUM(D.SubTotal) DESC;";

        private const string SqlProductosMasVendidos = @"
            SELECT TOP 10
                P.NombreProducto AS Etiqueta,
                P.CodBarra AS Codigo,
                SUM(D.Cantidad) AS Unidades,
                COUNT(DISTINCT F.IdFactura) AS Facturas,
                SUM(D.SubTotal) AS TotalVenta,
                P.Inventario AS Inventario,
                MAX(F.Fecha) AS UltimaVenta
            FROM dbo.tbFacturaDetalle D
            INNER JOIN dbo.tbFactura F ON F.IdFactura = D.IdFactura
            INNER JOIN dbo.tbProductos P ON P.IdProducto = D.IdProducto
            WHERE ISNULL(F.Anulada, 0) = 0
              AND F.Fecha >= @FechaInicio
              AND F.Fecha < @FechaFinExclusiva
            GROUP BY P.IdProducto, P.NombreProducto, P.CodBarra, P.Inventario
            ORDER BY SUM(D.Cantidad) DESC, SUM(D.SubTotal) DESC;";

        private const string SqlProductosPocaRotacion = @"
            SELECT TOP 100
                P.NombreProducto AS Etiqueta,
                P.CodBarra AS Codigo,
                ISNULL(SUM(CASE WHEN F.IdFactura IS NOT NULL THEN D.Cantidad ELSE 0 END), 0) AS Unidades,
                COUNT(DISTINCT F.IdFactura) AS Facturas,
                ISNULL(SUM(CASE WHEN F.IdFactura IS NOT NULL THEN D.SubTotal ELSE 0 END), 0) AS TotalVenta,
                P.Inventario AS Inventario,
                MAX(F.Fecha) AS UltimaVenta
            FROM dbo.tbProductos P
            LEFT JOIN dbo.tbFacturaDetalle D ON D.IdProducto = P.IdProducto
            LEFT JOIN dbo.tbFactura F ON F.IdFactura = D.IdFactura
                AND ISNULL(F.Anulada, 0) = 0
                AND F.Fecha >= @FechaInicio
                AND F.Fecha < @FechaFinExclusiva
            WHERE P.Inventario > 0
            GROUP BY P.IdProducto, P.NombreProducto, P.CodBarra, P.Inventario
            HAVING ISNULL(SUM(CASE WHEN F.IdFactura IS NOT NULL THEN D.Cantidad ELSE 0 END), 0) <= @LimiteRotacion
            ORDER BY
                ISNULL(SUM(CASE WHEN F.IdFactura IS NOT NULL THEN D.Cantidad ELSE 0 END), 0),
                MAX(F.Fecha),
                P.Inventario DESC;";
    }
}
