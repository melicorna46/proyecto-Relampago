using Dapper;
using ScrumMvp.Models;

namespace ScrumMvp.Data
{
    // Todo el acceso a la tabla 'usuario' vive acá.
    // Así el controlador no tiene SQL desperdigado.
    public class UsuarioRepository
    {
        // HU-001: valida que el correo no exista antes de registrar.
        public bool ExisteEmail(string email)
        {
            using (var con = Db.GetConnection())
            {
                string sql = "SELECT COUNT(1) FROM usuario WHERE email = @email";
                int total = con.ExecuteScalar<int>(sql, new { email });
                return total > 0;
            }
        }

        // HU-001: inserta el usuario nuevo. password_hash ya viene encriptado
        // (el hash lo genera el controlador con BCrypt, nunca se guarda en texto plano).
        public void Registrar(string nombre, string email, string passwordHash, string especialidad)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    INSERT INTO usuario (nombre, email, password_hash, especialidad, activo)
                    VALUES (@nombre, @email, @passwordHash, @especialidad, 1)";

                con.Execute(sql, new { nombre, email, passwordHash, especialidad });
            }
        }

        // Se usa en HU-002 (Login) más adelante: trae el usuario por email.
        public Usuario ObtenerPorEmail(string email)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT id                 AS Id,
                           nombre              AS Nombre,
                           email               AS Email,
                           password_hash       AS PasswordHash,
                           especialidad        AS Especialidad,
                           telefono            AS Telefono,
                           activo              AS Activo
                    FROM usuario
                    WHERE email = @email";

                return con.QueryFirstOrDefault<Usuario>(sql, new { email });
            }
        }
    }
}
