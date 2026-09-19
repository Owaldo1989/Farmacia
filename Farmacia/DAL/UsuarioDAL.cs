using Farmacia.Models;
using System.Data.SqlClient;
using System.Data;

namespace Farmacia.DAL
{
    public class UsuarioDAL
    {
        private readonly string _cn;

        public UsuarioDAL(IConfiguration config)
        {
            _cn = config.GetConnectionString("BDFarmacia");
        }

        public Usuario? Login(string usuario, string clave)
        {
            Usuario? u = null;

            using (SqlConnection cn = new SqlConnection(_cn))
            using (SqlCommand cmd = new SqlCommand("SpLoginUsuario", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Usuario", usuario);
                cmd.Parameters.AddWithValue("@Clave", clave);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    u = new Usuario
                    {
                        IdUsuario =
                            (int)dr["IdUsuario"],

                        UsuarioNombre =
                            LeerTexto(
                                dr,
                                "UsuarioNombre",
                                "Usuario"
                            ),

                        Clave =
                            dr["Clave"].ToString()
                            ?? string.Empty,

                        NombreCompleto =
                            dr["NombreCompleto"].ToString()
                            ?? string.Empty,

                        Rol =
                            dr["Rol"].ToString()
                            ?? string.Empty,

                        Estado =
                            Convert.ToBoolean(
                                dr["Estado"]
                            ),

                        IdSucursal =
                            TieneColumna(
                                dr,
                                "IdSucursal"
                            ) &&
                            dr["IdSucursal"] != DBNull.Value
                                ? Convert.ToInt32(
                                    dr["IdSucursal"]
                                )
                                : null
                    };
                }
            }

            return u;
        }


        public List<Usuario> Listar()
        {
            List<Usuario> lista =
                new();


            using SqlConnection cn =
                new(_cn);


            using SqlCommand cmd =
                new(
                    @"SELECT
                          IdUsuario,
                          Usuario,
                          NombreCompleto,
                          Rol,
                          Estado,
                          IdSucursal
                      FROM dbo.tbUsuarios
                      ORDER BY
                          Estado DESC,
                          NombreCompleto,
                          Usuario;",
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


        public Usuario? Obtener(
            int idUsuario)
        {
            using SqlConnection cn =
                new(_cn);


            using SqlCommand cmd =
                new(
                    @"SELECT
                          IdUsuario,
                          Usuario,
                          NombreCompleto,
                          Rol,
                          Estado,
                          IdSucursal
                      FROM dbo.tbUsuarios
                      WHERE IdUsuario = @IdUsuario;",
                    cn
                );


            cmd.Parameters.AddWithValue(
                "@IdUsuario",
                idUsuario
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
            Usuario usuario,
            string? claveInicial)
        {
            if (ExisteUsuario(
                usuario.UsuarioNombre,
                usuario.IdUsuario))
            {
                throw new Exception(
                    "Ya existe un usuario con ese nombre de acceso."
                );
            }


            using SqlConnection cn =
                new(_cn);


            using SqlCommand cmd =
                cn.CreateCommand();


            if (usuario.IdUsuario == 0)
            {
                if (string.IsNullOrWhiteSpace(
                    claveInicial))
                {
                    throw new Exception(
                        "Debe indicar la contraseña inicial."
                    );
                }


                cmd.CommandText =
                    @"INSERT INTO dbo.tbUsuarios
                      (
                          Usuario,
                          Clave,
                          NombreCompleto,
                          Rol,
                          Estado,
                          IdSucursal
                      )
                      VALUES
                      (
                          @Usuario,
                          @Clave,
                          @NombreCompleto,
                          @Rol,
                          @Estado,
                          @IdSucursal
                      );";


                cmd.Parameters.AddWithValue(
                    "@Clave",
                    claveInicial.Trim()
                );
            }
            else
            {
                cmd.CommandText =
                    @"UPDATE dbo.tbUsuarios
                      SET
                          Usuario = @Usuario,
                          NombreCompleto = @NombreCompleto,
                          Rol = @Rol,
                          Estado = @Estado,
                          IdSucursal = @IdSucursal
                      WHERE IdUsuario = @IdUsuario;";


                cmd.Parameters.AddWithValue(
                    "@IdUsuario",
                    usuario.IdUsuario
                );
            }


            AgregarParametrosUsuario(
                cmd,
                usuario
            );


            cn.Open();
            cmd.ExecuteNonQuery();
        }


        public void CambiarClave(
            int idUsuario,
            string nuevaClave)
        {
            using SqlConnection cn =
                new(_cn);


            using SqlCommand cmd =
                new(
                    @"UPDATE dbo.tbUsuarios
                      SET Clave = @Clave
                      WHERE IdUsuario = @IdUsuario;",
                    cn
                );


            cmd.Parameters.AddWithValue(
                "@IdUsuario",
                idUsuario
            );


            cmd.Parameters.AddWithValue(
                "@Clave",
                nuevaClave.Trim()
            );


            cn.Open();


            if (cmd.ExecuteNonQuery() == 0)
            {
                throw new Exception(
                    "El usuario seleccionado no existe."
                );
            }
        }


        public void CambiarEstado(
            int idUsuario,
            bool estado)
        {
            using SqlConnection cn =
                new(_cn);


            using SqlCommand cmd =
                new(
                    @"UPDATE dbo.tbUsuarios
                      SET Estado = @Estado
                      WHERE IdUsuario = @IdUsuario;",
                    cn
                );


            cmd.Parameters.AddWithValue(
                "@IdUsuario",
                idUsuario
            );


            cmd.Parameters.AddWithValue(
                "@Estado",
                estado
            );


            cn.Open();
            cmd.ExecuteNonQuery();
        }


        private bool ExisteUsuario(
            string usuario,
            int idUsuario)
        {
            using SqlConnection cn =
                new(_cn);


            using SqlCommand cmd =
                new(
                    @"SELECT COUNT(1)
                      FROM dbo.tbUsuarios
                      WHERE UPPER(LTRIM(RTRIM(Usuario))) =
                            UPPER(LTRIM(RTRIM(@Usuario)))
                        AND IdUsuario <> @IdUsuario;",
                    cn
                );


            cmd.Parameters.AddWithValue(
                "@Usuario",
                usuario
            );


            cmd.Parameters.AddWithValue(
                "@IdUsuario",
                idUsuario
            );


            cn.Open();


            return Convert.ToInt32(
                cmd.ExecuteScalar()
            ) > 0;
        }


        private static void AgregarParametrosUsuario(
            SqlCommand cmd,
            Usuario usuario)
        {
            cmd.Parameters.AddWithValue(
                "@Usuario",
                usuario.UsuarioNombre.Trim()
            );


            cmd.Parameters.AddWithValue(
                "@NombreCompleto",
                string.IsNullOrWhiteSpace(
                    usuario.NombreCompleto)
                    ? (object)DBNull.Value
                    : usuario.NombreCompleto.Trim()
            );


            cmd.Parameters.AddWithValue(
                "@Rol",
                string.IsNullOrWhiteSpace(
                    usuario.Rol)
                    ? (object)DBNull.Value
                    : usuario.Rol.Trim()
            );


            cmd.Parameters.AddWithValue(
                "@Estado",
                usuario.Estado
            );


            cmd.Parameters.AddWithValue(
                "@IdSucursal",
                usuario.IdSucursal.HasValue
                    ? (object)usuario.IdSucursal.Value
                    : DBNull.Value
            );
        }


        private static Usuario Mapear(
            SqlDataReader dr)
        {
            return new Usuario
            {
                IdUsuario =
                    Convert.ToInt32(
                        dr["IdUsuario"]
                    ),

                UsuarioNombre =
                    dr["Usuario"].ToString()
                    ?? string.Empty,

                Clave =
                    string.Empty,

                NombreCompleto =
                    dr["NombreCompleto"] == DBNull.Value
                        ? string.Empty
                        : dr["NombreCompleto"].ToString()
                          ?? string.Empty,

                Rol =
                    dr["Rol"] == DBNull.Value
                        ? string.Empty
                        : dr["Rol"].ToString()
                          ?? string.Empty,

                Estado =
                    Convert.ToBoolean(
                        dr["Estado"]
                    ),

                IdSucursal =
                    dr["IdSucursal"] == DBNull.Value
                        ? null
                        : Convert.ToInt32(
                            dr["IdSucursal"]
                        )
            };
        }


        private static string LeerTexto(
            SqlDataReader dr,
            params string[] columnas)
        {
            foreach (var columna in columnas)
            {
                if (TieneColumna(
                    dr,
                    columna))
                {
                    return dr[columna]?.ToString()
                           ?? string.Empty;
                }
            }


            return string.Empty;
        }


        private static bool TieneColumna(
            IDataRecord dr,
            string columna)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (string.Equals(
                    dr.GetName(i),
                    columna,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }


            return false;
        }
    }
}
