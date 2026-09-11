using Farmacia.Models;
using System.Data;
using System.Data.SqlClient;

namespace Farmacia.DAL
{
    public class IngresoMercaderiaDAL
    {
        private readonly string _cn;

        public IngresoMercaderiaDAL(
            IConfiguration config)
        {
            _cn = config.GetConnectionString(
                "BDFarmacia"
            );
        }


        // =====================================================
        // BUSCAR PRODUCTOS PARA INGRESO
        // =====================================================

        public List<ProductoIngresoDTO> BuscarProductos(
            string filtro)
        {
            var lista =
                new List<ProductoIngresoDTO>();


            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpIngresoMercaderiaBuscarProducto",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.Add(
                "@Filtro",
                SqlDbType.NVarChar,
                150
            ).Value =
                filtro ?? string.Empty;


            cn.Open();


            using var dr =
                cmd.ExecuteReader();


            while (dr.Read())
            {
                lista.Add(
                    new ProductoIngresoDTO
                    {
                        IdProducto =
                            Convert.ToInt32(
                                dr["IdProducto"]
                            ),

                        CodBarra =
                            dr["CodBarra"]
                                ?.ToString()
                            ?? string.Empty,

                        NombreProducto =
                            dr["NombreProducto"]
                                ?.ToString()
                            ?? string.Empty,

                        NombreGenerico =
                            dr["NombreGenerico"]
                                ?.ToString(),

                        PrecioCosto =
                            Convert.ToDecimal(
                                dr["PrecioCosto"]
                            ),

                        CostoPromedio =
                            Convert.ToDecimal(
                                dr["CostoPromedio"]
                            ),

                        PrecioVenta =
                            Convert.ToDecimal(
                                dr["PrecioVenta"]
                            ),

                        Inventario =
                            Convert.ToDecimal(
                                dr["Inventario"]
                            ),

                        TasaIVA =
                            Convert.ToDecimal(
                                dr["TasaIVA"]
                            ),
                        PorcentajeUtilidad =
    Convert.ToDecimal(
        dr["PorcentajeUtilidad"]
    )
                    }
                );
            }


            return lista;
        }


        // =====================================================
        // CONFIRMAR INGRESO
        // =====================================================

        public int ConfirmarIngreso(
            IngresoMercaderiaDTO ingreso,
            List<IngresoMercaderiaDetalleDTO> detalle)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpIngresoMercaderiaConfirmar",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;



            // =================================================
            // ENCABEZADO
            // =================================================

            cmd.Parameters.Add(
                "@IdProveedor",
                SqlDbType.Int
            ).Value =
                ingreso.IdProveedor;


            cmd.Parameters.Add(
                "@NumeroFactura",
                SqlDbType.NVarChar,
                50
            ).Value =
                ingreso.NumeroFactura;


            cmd.Parameters.Add(
                "@FechaFactura",
                SqlDbType.Date
            ).Value =
                ingreso.FechaFactura.Date;



            AgregarDecimal(
                cmd,
                "@SubtotalFacturaProveedor",
                ingreso.SubtotalFacturaProveedor
            );


            AgregarDecimal(
                cmd,
                "@IVAFacturaProveedor",
                ingreso.IVAFacturaProveedor
            );


            AgregarDecimal(
                cmd,
                "@TotalFacturaProveedor",
                ingreso.TotalFacturaProveedor
            );


            cmd.Parameters.Add(
                "@UsuarioRegistro",
                SqlDbType.NVarChar,
                50
            ).Value =
                string.IsNullOrWhiteSpace(
                    ingreso.UsuarioRegistro
                )
                    ? DBNull.Value
                    : ingreso.UsuarioRegistro;


            cmd.Parameters.Add(
                "@Observacion",
                SqlDbType.NVarChar,
                500
            ).Value =
                string.IsNullOrWhiteSpace(
                    ingreso.Observacion
                )
                    ? DBNull.Value
                    : ingreso.Observacion;



            // =================================================
            // TVP
            // =================================================

            var tvp =
                new DataTable();


            tvp.Columns.Add(
                "IdProducto",
                typeof(int)
            );

            tvp.Columns.Add(
                "Cantidad",
                typeof(decimal)
            );

            tvp.Columns.Add(
                "CostoUnitarioBase",
                typeof(decimal)
            );

            tvp.Columns.Add(
                "TasaIVA",
                typeof(decimal)
            );

            tvp.Columns.Add(
                "PrecioVentaSugerido",
                typeof(decimal)
            );

            tvp.Columns.Add(
                "PrecioVentaNuevo",
                typeof(decimal)
            );

            tvp.Columns.Add(
                "NumeroLote",
                typeof(string)
            );

            tvp.Columns.Add(
                "FechaVencimiento",
                typeof(DateTime)
            );



            foreach (var item in detalle)
            {
                tvp.Rows.Add(
                    item.IdProducto,
                    item.Cantidad,
                    item.CostoUnitarioBase,
                    item.TasaIVA,

                    item.PrecioVentaSugerido
                        ?? (object)DBNull.Value,

                    item.PrecioVentaNuevo,

                    string.IsNullOrWhiteSpace(
                        item.NumeroLote
                    )
                        ? DBNull.Value
                        : item.NumeroLote,

                    item.FechaVencimiento
                        ?? (object)DBNull.Value
                );
            }



            var pDetalle =
                cmd.Parameters.AddWithValue(
                    "@Detalle",
                    tvp
                );


            pDetalle.SqlDbType =
                SqlDbType.Structured;


            pDetalle.TypeName =
                "dbo.TipoIngresoMercaderiaDetalle";



            cn.Open();


            var resultado =
                cmd.ExecuteScalar();


            return Convert.ToInt32(
                resultado
            );
        }



        // =====================================================
        // DECIMAL
        // =====================================================

        private static void AgregarDecimal(
            SqlCommand cmd,
            string nombre,
            decimal valor)
        {
            var parametro =
                cmd.Parameters.Add(
                    nombre,
                    SqlDbType.Decimal
                );


            parametro.Precision = 18;

            parametro.Scale = 2;

            parametro.Value = valor;
        }

        // =====================================================
        // LISTAR INGRESOS
        // =====================================================

        public List<IngresoMercaderiaDTO> Listar(
            DateTime fechaInicio,
            DateTime fechaFin,
            int? idProveedor,
            string? numeroFactura)
        {
            var lista =
                new List<IngresoMercaderiaDTO>();


            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpIngresoMercaderiaListar",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.Add(
                "@FechaInicio",
                SqlDbType.Date
            ).Value =
                fechaInicio.Date;


            cmd.Parameters.Add(
                "@FechaFin",
                SqlDbType.Date
            ).Value =
                fechaFin.Date;


            cmd.Parameters.Add(
                "@IdProveedor",
                SqlDbType.Int
            ).Value =
                idProveedor.HasValue &&
                idProveedor.Value > 0
                    ? idProveedor.Value
                    : DBNull.Value;


            cmd.Parameters.Add(
                "@NumeroFactura",
                SqlDbType.NVarChar,
                50
            ).Value =
                string.IsNullOrWhiteSpace(
                    numeroFactura
                )
                    ? DBNull.Value
                    : numeroFactura.Trim();


            cn.Open();


            using var dr =
                cmd.ExecuteReader();


            while (dr.Read())
            {
                lista.Add(
                    new IngresoMercaderiaDTO
                    {
                        IdIngreso =
                            Convert.ToInt32(
                                dr["IdIngreso"]
                            ),

                        IdProveedor =
                            Convert.ToInt32(
                                dr["IdProveedor"]
                            ),

                        NombreProveedor =
                            dr["NombreProveedor"]
                                ?.ToString(),

                        NumeroFactura =
                            dr["NumeroFactura"]
                                ?.ToString()
                            ?? string.Empty,

                        FechaFactura =
                            Convert.ToDateTime(
                                dr["FechaFactura"]
                            ),

                        FechaIngreso =
                            Convert.ToDateTime(
                                dr["FechaIngreso"]
                            ),

                        SubtotalFacturaProveedor =
                            Convert.ToDecimal(
                                dr["SubtotalFacturaProveedor"]
                            ),

                        IVAFacturaProveedor =
                            Convert.ToDecimal(
                                dr["IVAFacturaProveedor"]
                            ),

                        TotalFacturaProveedor =
                            Convert.ToDecimal(
                                dr["TotalFacturaProveedor"]
                            ),

                        Estado =
                            Convert.ToByte(
                                dr["Estado"]
                            ),

                        UsuarioRegistro =
                            dr["UsuarioRegistro"]
                                == DBNull.Value
                                ? null
                                : dr["UsuarioRegistro"]
                                    .ToString(),

                        FechaConfirmacion =
                            dr["FechaConfirmacion"]
                                == DBNull.Value
                                ? null
                                : Convert.ToDateTime(
                                    dr["FechaConfirmacion"]
                                ),

                        Observacion =
                            dr["Observacion"]
                                == DBNull.Value
                                ? null
                                : dr["Observacion"]
                                    .ToString(),

                        CantidadLineas =
                            Convert.ToInt32(
                                dr["CantidadLineas"]
                            ),

                        CantidadUnidades =
                            Convert.ToDecimal(
                                dr["CantidadUnidades"]
                            )
                    }
                );
            }


            return lista;
        }


        // =====================================================
        // OBTENER INGRESO COMPLETO
        // =====================================================

        public IngresoMercaderiaCompletoDTO? Obtener(
            int idIngreso)
        {
            using var cn =
                new SqlConnection(_cn);


            using var cmd =
                new SqlCommand(
                    "SpIngresoMercaderiaObtener",
                    cn
                );


            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.Add(
                "@IdIngreso",
                SqlDbType.Int
            ).Value =
                idIngreso;


            cn.Open();


            using var dr =
                cmd.ExecuteReader();


            if (!dr.Read())
            {
                return null;
            }


            var resultado =
                new IngresoMercaderiaCompletoDTO();


            resultado.Ingreso =
                new IngresoMercaderiaDTO
                {
                    IdIngreso =
                        Convert.ToInt32(
                            dr["IdIngreso"]
                        ),

                    IdProveedor =
                        Convert.ToInt32(
                            dr["IdProveedor"]
                        ),

                    NombreProveedor =
                        dr["NombreProveedor"]
                            ?.ToString(),

                    NumeroFactura =
                        dr["NumeroFactura"]
                            ?.ToString()
                        ?? string.Empty,

                    FechaFactura =
                        Convert.ToDateTime(
                            dr["FechaFactura"]
                        ),

                    FechaIngreso =
                        Convert.ToDateTime(
                            dr["FechaIngreso"]
                        ),

                    SubtotalFacturaProveedor =
                        Convert.ToDecimal(
                            dr["SubtotalFacturaProveedor"]
                        ),

                    IVAFacturaProveedor =
                        Convert.ToDecimal(
                            dr["IVAFacturaProveedor"]
                        ),

                    TotalFacturaProveedor =
                        Convert.ToDecimal(
                            dr["TotalFacturaProveedor"]
                        ),

                    Estado =
                        Convert.ToByte(
                            dr["Estado"]
                        ),

                    UsuarioRegistro =
                        dr["UsuarioRegistro"]
                            == DBNull.Value
                            ? null
                            : dr["UsuarioRegistro"]
                                .ToString(),

                    FechaConfirmacion =
                        dr["FechaConfirmacion"]
                            == DBNull.Value
                            ? null
                            : Convert.ToDateTime(
                                dr["FechaConfirmacion"]
                            ),

                    Observacion =
                        dr["Observacion"]
                            == DBNull.Value
                            ? null
                            : dr["Observacion"]
                                .ToString()
                };


            if (!dr.NextResult())
            {
                return resultado;
            }


            while (dr.Read())
            {
                resultado.Detalles.Add(
                    new IngresoMercaderiaDetalleDTO
                    {
                        IdIngresoDetalle =
                            Convert.ToInt32(
                                dr["IdIngresoDetalle"]
                            ),

                        IdIngreso =
                            Convert.ToInt32(
                                dr["IdIngreso"]
                            ),

                        IdProducto =
                            Convert.ToInt32(
                                dr["IdProducto"]
                            ),

                        CodBarra =
                            dr["CodBarra"]
                                ?.ToString(),

                        NombreProducto =
                            dr["NombreProducto"]
                                ?.ToString(),

                        Cantidad =
                            Convert.ToDecimal(
                                dr["Cantidad"]
                            ),

                        CostoAnterior =
                            Convert.ToDecimal(
                                dr["CostoAnterior"]
                            ),

                        CostoUnitarioBase =
                            Convert.ToDecimal(
                                dr["CostoUnitarioBase"]
                            ),

                        TasaIVA =
                            Convert.ToDecimal(
                                dr["TasaIVA"]
                            ),

                        CostoUnitarioFinal =
                            Convert.ToDecimal(
                                dr["CostoUnitarioFinal"]
                            ),

                        CostoPromedioAnterior =
                            Convert.ToDecimal(
                                dr["CostoPromedioAnterior"]
                            ),

                        CostoPromedioNuevo =
                            Convert.ToDecimal(
                                dr["CostoPromedioNuevo"]
                            ),

                        PrecioVentaAnterior =
                            Convert.ToDecimal(
                                dr["PrecioVentaAnterior"]
                            ),

                        PrecioVentaSugerido =
                            dr["PrecioVentaSugerido"]
                                == DBNull.Value
                                ? null
                                : Convert.ToDecimal(
                                    dr["PrecioVentaSugerido"]
                                ),

                        PrecioVentaNuevo =
                            Convert.ToDecimal(
                                dr["PrecioVentaNuevo"]
                            ),

                        InventarioAnterior =
                            Convert.ToDecimal(
                                dr["InventarioAnterior"]
                            ),

                        InventarioNuevo =
                            Convert.ToDecimal(
                                dr["InventarioNuevo"]
                            ),

                        NumeroLote =
                            dr["NumeroLote"]
                                == DBNull.Value
                                ? null
                                : dr["NumeroLote"]
                                    .ToString(),

                        FechaVencimiento =
                            dr["FechaVencimiento"]
                                == DBNull.Value
                                ? null
                                : Convert.ToDateTime(
                                    dr["FechaVencimiento"]
                                )
                    }
                );
            }


            return resultado;
        }
    }
}
