using Dapper;
using ScrumMvp.Models;

namespace ScrumMvp.Data
{
    public class SprintRepository
    {
        // El "Sprint vigente" es el más reciente que todavía no se cerró.
        // Simplifica el MVP: un solo Sprint activo por proyecto a la vez.
        public Sprint ObtenerVigentePorProyecto(int proyectoId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT id            AS Id,
                           proyecto_id   AS ProyectoId,
                           nombre        AS Nombre,
                           sprint_goal   AS SprintGoal,
                           fecha_inicio  AS FechaInicio,
                           fecha_fin     AS FechaFin,
                           estado        AS Estado
                    FROM sprint
                    WHERE proyecto_id = @proyectoId AND estado <> 'cerrado'
                    ORDER BY id DESC
                    LIMIT 1";

                return con.QueryFirstOrDefault<Sprint>(sql, new { proyectoId });
            }
        }

        public Sprint ObtenerPorId(int id)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT id            AS Id,
                           proyecto_id   AS ProyectoId,
                           nombre        AS Nombre,
                           sprint_goal   AS SprintGoal,
                           fecha_inicio  AS FechaInicio,
                           fecha_fin     AS FechaFin,
                           estado        AS Estado
                    FROM sprint
                    WHERE id = @id";

                return con.QueryFirstOrDefault<Sprint>(sql, new { id });
            }
        }

        // HU-049 + HU-052: crea el Sprint. Nace 'activo' directo: para este MVP
        // solo existe un Sprint vigente por proyecto a la vez (no hay HU de "iniciar Sprint" aparte).
        public int Crear(SprintViewModel m)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    INSERT INTO sprint (proyecto_id, nombre, sprint_goal, fecha_inicio, fecha_fin, estado)
                    VALUES (@ProyectoId, @Nombre, @SprintGoal, @FechaInicio, @FechaFin, 'activo');
                    SELECT LAST_INSERT_ID();";

                return con.ExecuteScalar<int>(sql, new
                {
                    m.ProyectoId,
                    m.Nombre,
                    m.SprintGoal,
                    m.FechaInicio,
                    m.FechaFin
                });
            }
        }

        // HU-054: cierre formal del Sprint.
        public void Cerrar(int id)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"UPDATE sprint SET estado = 'cerrado' WHERE id = @id";
                con.Execute(sql, new { id });
            }
        }
    }
}
