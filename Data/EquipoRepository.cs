using System.Collections.Generic;
using Dapper;
using ScrumMvp.Models;

namespace ScrumMvp.Data
{
    public class EquipoRepository
    {
        public Equipo ObtenerPorProyecto(int proyectoId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT id          AS Id,
                           proyecto_id AS ProyectoId,
                           nombre      AS Nombre
                    FROM equipo
                    WHERE proyecto_id = @proyectoId
                    LIMIT 1";

                return con.QueryFirstOrDefault<Equipo>(sql, new { proyectoId });
            }
        }

        public Equipo ObtenerPorId(int equipoId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT id          AS Id,
                           proyecto_id AS ProyectoId,
                           nombre      AS Nombre
                    FROM equipo
                    WHERE id = @equipoId";

                return con.QueryFirstOrDefault<Equipo>(sql, new { equipoId });
            }
        }

        public int CrearConPrimerMiembro(int proyectoId, string nombreEquipo, int usuarioCreadorId, string rolCreador)
        {
            using (var con = Db.GetConnection())
            {
                con.Open();
                using (var tx = con.BeginTransaction())
                {
                    try
                    {
                        string sqlEquipo = @"
                            INSERT INTO equipo (proyecto_id, nombre)
                            VALUES (@proyectoId, @nombreEquipo);
                            SELECT LAST_INSERT_ID();";

                        int equipoId = con.ExecuteScalar<int>(
                            sqlEquipo, new { proyectoId, nombreEquipo }, transaction: tx);

                        string sqlMiembro = @"
                            INSERT INTO equipo_miembro (equipo_id, usuario_id, rol)
                            VALUES (@equipoId, @usuarioCreadorId, @rolCreador);";

                        con.Execute(sqlMiembro,
                            new { equipoId, usuarioCreadorId, rolCreador }, transaction: tx);

                        tx.Commit();
                        return equipoId;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<MiembroEquipo> ObtenerMiembros(int equipoId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT u.id     AS UsuarioId,
                           u.nombre AS Nombre,
                           u.email  AS Email,
                           em.rol   AS Rol
                    FROM equipo_miembro em
                    INNER JOIN usuario u ON u.id = em.usuario_id
                    WHERE em.equipo_id = @equipoId
                    ORDER BY FIELD(em.rol, 'product_owner', 'scrum_master', 'developer'), u.nombre";

                return new List<MiembroEquipo>(con.Query<MiembroEquipo>(sql, new { equipoId }));
            }
        }

        public bool YaExisteRolEnEquipo(int equipoId, string rol)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"SELECT COUNT(1) FROM equipo_miembro WHERE equipo_id = @equipoId AND rol = @rol";
                int total = con.ExecuteScalar<int>(sql, new { equipoId, rol });
                return total > 0;
            }
        }

        public bool EsMiembro(int equipoId, int usuarioId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"SELECT COUNT(1) FROM equipo_miembro WHERE equipo_id = @equipoId AND usuario_id = @usuarioId";
                int total = con.ExecuteScalar<int>(sql, new { equipoId, usuarioId });
                return total > 0;
            }
        }

        public void AgregarMiembro(int equipoId, int usuarioId, string rol)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"INSERT INTO equipo_miembro (equipo_id, usuario_id, rol) VALUES (@equipoId, @usuarioId, @rol)";
                con.Execute(sql, new { equipoId, usuarioId, rol });
            }
        }

        // NUEVO: el rol que tiene un usuario específico dentro del equipo de un proyecto.
        // Devuelve null si el proyecto no tiene equipo todavía, o si el usuario no es miembro.
        // Esto es lo que permite validar de verdad "Como Product Owner, quiero..." en vez de
        // asumir que quien creó el proyecto puede hacer cualquier cosa.
        public string ObtenerRolDeUsuarioEnProyecto(int proyectoId, int usuarioId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT em.rol
                    FROM equipo e
                    INNER JOIN equipo_miembro em ON em.equipo_id = e.id
                    WHERE e.proyecto_id = @proyectoId AND em.usuario_id = @usuarioId
                    LIMIT 1";

                return con.QueryFirstOrDefault<string>(sql, new { proyectoId, usuarioId });
            }
        }
    }
}
