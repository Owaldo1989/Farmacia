using Farmacia.Models;
using System.Data.SqlClient;
using System.Data;

namespace Farmacia.DAL
{
    public class ProductoDAL
    {

        private readonly string _cn;

        public ProductoDAL(IConfiguration config)
        {
            _cn = config.GetConnectionString("BDFarmacia");
        }

        public List<Producto> Listar()
        {
            List<Producto> lista = new();

            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpProductos", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Accion", "LIST");

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new Producto
                    {
                        IdProducto = Convert.ToInt32(dr["IdProducto"]),
                        CodBarra = dr["CodBarra"].ToString(),
                        NombreProducto = dr["NombreProducto"].ToString(),
                        NombreGenerico = dr["NombreGenerico"]?.ToString(),
                        FechaVencimiento = dr["FechaVencimiento"] as DateTime?,
                        RecomendadoPara = dr["RecomendadoPara"]?.ToString(),
                        PrecioCosto = Convert.ToDecimal(dr["PrecioCosto"]),
                        CostoPromedio = Convert.ToDecimal(dr["CostoPromedio"]),
                        PrecioVenta = Convert.ToDecimal(dr["PrecioVenta"]),
                        PorcentajeUtilidad = Convert.ToDecimal(dr["PorcentajeUtilidad"]),
                        TasaIVA = Convert.ToDecimal(dr["TasaIVA"]),
                        Inventario = Convert.ToDecimal(dr["Inventario"]),
                        IdProveedor = Convert.ToInt32(dr["IdProveedor"]),
                        IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                        IdUnidad = Convert.ToInt32(dr["IdUnidad"]),
                        IdLaboratorio = dr["IdLaboratorio"] == DBNull.Value
                            ? null
                            : Convert.ToInt32(dr["IdLaboratorio"]),
                        Foto = dr["Foto"] == DBNull.Value
                            ? null
                            : dr["Foto"].ToString(),
                        CategoriaNombre = dr["NombreCategoria"].ToString(),
                        ProveedorNombre = dr["NombreProveedor"].ToString(),
                        UnidadNombre = dr["NombreUnidad"].ToString()
                    });
                }
            }

            return lista;
        }
        public List<ProductoFacturaDTO> BuscarParaFactura(string filtro)
        {
            List<ProductoFacturaDTO> lista = new();

            using var cn = new SqlConnection(_cn);
            using var cmd = new SqlCommand("SpProductosBuscar", cn); // Usa tu SP de productos
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Filtro", filtro ?? "");

            cn.Open();
            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new ProductoFacturaDTO
                {
                    IdProducto = Convert.ToInt32(dr["IdProducto"]),
                    CodBarra = dr["CodBarra"].ToString(),
                    NombreProducto = dr["NombreProducto"].ToString(),
                    PrecioVenta = Convert.ToDecimal(dr["PrecioVenta"]),
                    Inventario = Convert.ToDecimal(dr["Inventario"]),
                    Foto = dr["Foto"] == DBNull.Value
                        ? null
                        : dr["Foto"].ToString(),
                    NombreGenerico = dr["NombreGenerico"] == DBNull.Value
                        ? null
                        : dr["NombreGenerico"].ToString(),
                    RecomendadoPara = dr["RecomendadoPara"] == DBNull.Value
                        ? null
                        : dr["RecomendadoPara"].ToString(),
                    FechaVencimiento = dr["FechaVencimiento"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(dr["FechaVencimiento"]),
                    CategoriaNombre = dr["CategoriaNombre"] == DBNull.Value
                        ? null
                        : dr["CategoriaNombre"].ToString(),
                    ProveedorNombre = dr["ProveedorNombre"] == DBNull.Value
                        ? null
                        : dr["ProveedorNombre"].ToString(),
                    UnidadNombre = dr["UnidadNombre"] == DBNull.Value
                        ? null
                        : dr["UnidadNombre"].ToString(),
                    LaboratorioNombre = dr["LaboratorioNombre"] == DBNull.Value
                        ? null
                        : dr["LaboratorioNombre"].ToString()
                });
            }

            return lista;
        }
        public List<Producto> BuscarProductos(string filtro)
        {
            List<Producto> lista = new();

            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpProductosBuscar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Filtro", filtro ?? "");

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new Producto
                    {
                        IdProducto = Convert.ToInt32(dr["IdProducto"]),
                        CodBarra = dr["CodBarra"].ToString(),
                        NombreProducto = dr["NombreProducto"].ToString(),
                        NombreGenerico = dr["NombreGenerico"].ToString(),
                        FechaVencimiento = dr["FechaVencimiento"] as DateTime?,
                        RecomendadoPara = dr["RecomendadoPara"].ToString(),
                        PrecioVenta = Convert.ToDecimal(dr["PrecioVenta"]),
                        Inventario = Convert.ToDecimal(dr["Inventario"]),
                        Foto = dr["Foto"] == DBNull.Value
                            ? null
                            : dr["Foto"].ToString(),
                        CategoriaNombre = dr["CategoriaNombre"].ToString(),
                        ProveedorNombre = dr["ProveedorNombre"].ToString(),
                        UnidadNombre = dr["UnidadNombre"].ToString()
                    });
                }
            }

            return lista;
        }


        public Producto Obtener(int id)
        {
            Producto p = null;

            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpProductos", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Accion", "GET");
                cmd.Parameters.AddWithValue("@IdProducto", id);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    p = new Producto
                    {
                        IdProducto = Convert.ToInt32(dr["IdProducto"]),
                        CodBarra = dr["CodBarra"].ToString(),
                        NombreProducto = dr["NombreProducto"].ToString(),
                        NombreGenerico = dr["NombreGenerico"]?.ToString(),
                        FechaVencimiento = dr["FechaVencimiento"] as DateTime?,
                        RecomendadoPara = dr["RecomendadoPara"]?.ToString(),
                        PrecioCosto = Convert.ToDecimal(dr["PrecioCosto"]),
                        CostoPromedio = Convert.ToDecimal(dr["CostoPromedio"]),
                        PrecioVenta = Convert.ToDecimal(dr["PrecioVenta"]),
                        PorcentajeUtilidad = Convert.ToDecimal(dr["PorcentajeUtilidad"]),
                        TasaIVA = Convert.ToDecimal(dr["TasaIVA"]),
                        Inventario = Convert.ToDecimal(dr["Inventario"]),
                        IdProveedor = Convert.ToInt32(dr["IdProveedor"]),
                        IdCategoria = Convert.ToInt32(dr["IdCategoria"]),
                        IdUnidad = Convert.ToInt32(dr["IdUnidad"]),
                        IdLaboratorio = dr["IdLaboratorio"] == DBNull.Value
                            ? null
                            : Convert.ToInt32(dr["IdLaboratorio"]),
                        Foto = dr["Foto"] == DBNull.Value
                            ? null
                            : dr["Foto"].ToString()
                    };
                }
            }

            return p;
        }

        public void Guardar(Producto p)
        {
            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpProductos", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Accion", p.IdProducto == 0 ? "INSERT" : "UPDATE");
                cmd.Parameters.AddWithValue("@IdProducto", p.IdProducto);
                cmd.Parameters.AddWithValue("@CodBarra", p.CodBarra);
                cmd.Parameters.AddWithValue("@NombreProducto", p.NombreProducto);
                cmd.Parameters.AddWithValue("@NombreGenerico", (object)p.NombreGenerico ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaVencimiento", (object)p.FechaVencimiento ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RecomendadoPara", (object)p.RecomendadoPara ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PrecioCosto", p.PrecioCosto);
                cmd.Parameters.AddWithValue("@CostoPromedio", p.CostoPromedio);
                cmd.Parameters.AddWithValue("@PrecioVenta", p.PrecioVenta);
                cmd.Parameters.AddWithValue("@PorcentajeUtilidad", p.PorcentajeUtilidad);
                cmd.Parameters.AddWithValue("@TasaIVA", p.TasaIVA);
                cmd.Parameters.AddWithValue("@Inventario", p.Inventario);
                cmd.Parameters.AddWithValue("@IdProveedor", p.IdProveedor);
                cmd.Parameters.AddWithValue("@IdCategoria", p.IdCategoria);
                cmd.Parameters.AddWithValue("@IdUnidad", p.IdUnidad);
                cmd.Parameters.AddWithValue(
                    "@IdLaboratorio",
                    (object?)p.IdLaboratorio ?? DBNull.Value);
                cmd.Parameters.AddWithValue(
                    "@Foto",
                    (object?)p.Foto ?? DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Eliminar(int id)
        {
            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpProductos", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Accion", "DELETE");
                cmd.Parameters.AddWithValue("@IdProducto", id);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

    }
}
