using Farmacia.Models;
using System.Data;
using System.Data.SqlClient;

namespace Farmacia.DAL
{
    public class DenominacionDAL
    {
        private readonly string _cn;


        public DenominacionDAL(
            IConfiguration config)
        {
            _cn =
                config.GetConnectionString(
                    "BDFarmacia"
                );
        }


        public List<Denominacion> Listar()
        {
            List<Denominacion> lista =
                new();


            using SqlConnection cn =
                new(_cn);


            using SqlCommand cmd =
                new(
                    @"SELECT
                          IdDenominacion,
                          Moneda,
                          Tipo,
                          Valor,
                          Descripcion,
                          Orden,
                          Activo
                      FROM dbo.tbDenominaciones
                      ORDER BY
                          Moneda,
                          Tipo,
                          Orden,
                          Valor DESC;",
                    cn
                );


            cn.Open();


            using SqlDataReader dr =
                cmd.ExecuteReader();


            while (dr.Read())
            {
                lista.Add(
                    Mapear(dr)
                );
            }


            return lista;
        }


        public Denominacion? Obtener(
            int id)
        {
            using SqlConnection cn =
                new(_cn);


            using SqlCommand cmd =
                new(
                    @"SELECT
                          IdDenominacion,
                          Moneda,
                          Tipo,
                          Valor,
                          Descripcion,
                          Orden,
                          Activo
                      FROM dbo.tbDenominaciones
                      WHERE IdDenominacion = @IdDenominacion;",
                    cn
                );


            cmd.Parameters.AddWithValue(
                "@IdDenominacion",
                id
            );


            cn.Open();


            using SqlDataReader dr =
                cmd.ExecuteReader();


            if (!dr.Read())
            {
                return null;
            }


            return Mapear(dr);
        }


        public void Guardar(
            Denominacion denominacion)
        {
            Normalizar(
                denominacion
            );


            if (ExisteDuplicada(
                denominacion))
            {
                throw new Exception(
                    "Ya existe una denominación con la misma moneda, tipo y valor."
                );
            }


            if (denominacion.Orden <= 0)
            {
                denominacion.Orden =
                    ObtenerSiguienteOrden(
                        denominacion.Moneda
                    );
            }


            using SqlConnection cn =
                new(_cn);


            using SqlCommand cmd =
                cn.CreateCommand();


            if (denominacion.IdDenominacion == 0)
            {
                cmd.CommandText =
                    @"INSERT INTO dbo.tbDenominaciones
                      (
                          Moneda,
                          Tipo,
                          Valor,
                          Descripcion,
                          Orden,
                          Activo
                      )
                      VALUES
                      (
                          @Moneda,
                          @Tipo,
                          @Valor,
                          @Descripcion,
                          @Orden,
                          @Activo
                      );";
            }
            else
            {
                cmd.CommandText =
                    @"UPDATE dbo.tbDenominaciones
                      SET
                          Moneda = @Moneda,
                          Tipo = @Tipo,
                          Valor = @Valor,
                          Descripcion = @Descripcion,
                          Orden = @Orden,
                          Activo = @Activo
                      WHERE IdDenominacion = @IdDenominacion;";


                cmd.Parameters.AddWithValue(
                    "@IdDenominacion",
                    denominacion.IdDenominacion
                );
            }


            AgregarParametros(
                cmd,
                denominacion
            );


            cn.Open();
            cmd.ExecuteNonQuery();
        }


        public void CambiarEstado(
            int id,
            bool activo)
        {
            using SqlConnection cn =
                new(_cn);


            using SqlCommand cmd =
                new(
                    @"UPDATE dbo.tbDenominaciones
                      SET Activo = @Activo
                      WHERE IdDenominacion = @IdDenominacion;",
                    cn
                );


            cmd.Parameters.AddWithValue(
                "@IdDenominacion",
                id
            );


            cmd.Parameters.AddWithValue(
                "@Activo",
                activo
            );


            cn.Open();


            if (cmd.ExecuteNonQuery() == 0)
            {
                throw new Exception(
                    "La denominación seleccionada no existe."
                );
            }
        }


        private bool ExisteDuplicada(
            Denominacion denominacion)
        {
            using SqlConnection cn =
                new(_cn);


            using SqlCommand cmd =
                new(
                    @"SELECT COUNT(1)
                      FROM dbo.tbDenominaciones
                      WHERE Moneda = @Moneda
                        AND Tipo = @Tipo
                        AND Valor = @Valor
                        AND IdDenominacion <> @IdDenominacion;",
                    cn
                );


            cmd.Parameters.AddWithValue(
                "@Moneda",
                denominacion.Moneda
            );


            cmd.Parameters.AddWithValue(
                "@Tipo",
                denominacion.Tipo
            );


            var pValor =
                cmd.Parameters.Add(
                    "@Valor",
                    SqlDbType.Decimal
                );

            pValor.Precision = 18;
            pValor.Scale = 2;
            pValor.Value = denominacion.Valor;


            cmd.Parameters.AddWithValue(
                "@IdDenominacion",
                denominacion.IdDenominacion
            );


            cn.Open();


            return Convert.ToInt32(
                cmd.ExecuteScalar()
            ) > 0;
        }


        private int ObtenerSiguienteOrden(
            string moneda)
        {
            using SqlConnection cn =
                new(_cn);


            using SqlCommand cmd =
                new(
                    @"SELECT ISNULL(MAX(Orden), 0) + 1
                      FROM dbo.tbDenominaciones
                      WHERE Moneda = @Moneda;",
                    cn
                );


            cmd.Parameters.AddWithValue(
                "@Moneda",
                moneda
            );


            cn.Open();


            return Convert.ToInt32(
                cmd.ExecuteScalar()
            );
        }


        private static void AgregarParametros(
            SqlCommand cmd,
            Denominacion denominacion)
        {
            cmd.Parameters.AddWithValue(
                "@Moneda",
                denominacion.Moneda
            );


            cmd.Parameters.AddWithValue(
                "@Tipo",
                denominacion.Tipo
            );


            var pValor =
                cmd.Parameters.Add(
                    "@Valor",
                    SqlDbType.Decimal
                );

            pValor.Precision = 18;
            pValor.Scale = 2;
            pValor.Value = denominacion.Valor;


            cmd.Parameters.AddWithValue(
                "@Descripcion",
                denominacion.Descripcion
            );


            cmd.Parameters.AddWithValue(
                "@Orden",
                denominacion.Orden
            );


            cmd.Parameters.AddWithValue(
                "@Activo",
                denominacion.Activo
            );
        }


        private static void Normalizar(
            Denominacion denominacion)
        {
            denominacion.Moneda =
                (denominacion.Moneda ?? "")
                .Trim()
                .ToUpperInvariant();

            denominacion.Tipo =
                (denominacion.Tipo ?? "")
                .Trim()
                .ToUpperInvariant();

            denominacion.Descripcion =
                (denominacion.Descripcion ?? "")
                .Trim();
        }


        private static Denominacion Mapear(
            SqlDataReader dr)
        {
            return new Denominacion
            {
                IdDenominacion =
                    Convert.ToInt32(
                        dr["IdDenominacion"]
                    ),

                Moneda =
                    dr["Moneda"].ToString()?.Trim()
                    ?? string.Empty,

                Tipo =
                    dr["Tipo"].ToString()?.Trim()
                    ?? string.Empty,

                Valor =
                    Convert.ToDecimal(
                        dr["Valor"]
                    ),

                Descripcion =
                    dr["Descripcion"].ToString()
                    ?? string.Empty,

                Orden =
                    Convert.ToInt32(
                        dr["Orden"]
                    ),

                Activo =
                    Convert.ToBoolean(
                        dr["Activo"]
                    )
            };
        }
    }
}
