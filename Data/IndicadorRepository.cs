using System.Collections.Generic;
using Dapper;
using ScrumMvp.Models;

namespace ScrumMvp.Data
{
    public class IndicadorRepository
    {
        // HU-112: registra (o corrige) el punto de hoy del Burndown, sumando los Story
        // Points de las historias todavía no cerradas y las horas restantes de las tareas
        // todavía abiertas. UNIQUE(sprint_id, fecha): un punto por día, se recalcula si
        // ya existía. No hay tarea programada en este MVP, así que el punto de cada día
        // se registra la primera vez que alguien visita el indicador ese día.
        public void RegistrarSnapshotDeHoy(int sprintId)
        {
            using (var con = Db.GetConnection())
            {
                string sqlPuntos = @"
                    SELECT COALESCE(SUM(story_points), 0)
                    FROM historia
                    WHERE sprint_id = @sprintId AND estado NOT IN ('terminada', 'aceptada')";
                decimal puntosRestantes = con.ExecuteScalar<decimal>(sqlPuntos, new { sprintId });

                string sqlHoras = @"
                    SELECT COALESCE(SUM(t.horas_restantes), 0)
                    FROM tarea t
                    INNER JOIN historia h ON h.id = t.historia_id
                    WHERE h.sprint_id = @sprintId AND t.estado <> 'terminado'";
                decimal horasRestantes = con.ExecuteScalar<decimal>(sqlHoras, new { sprintId });

                string sqlUpsert = @"
                    INSERT INTO burndown (sprint_id, fecha, puntos_restantes, horas_restantes)
                    VALUES (@sprintId, CURDATE(), @puntosRestantes, @horasRestantes)
                    ON DUPLICATE KEY UPDATE
                        puntos_restantes = @puntosRestantes,
                        horas_restantes = @horasRestantes";

                con.Execute(sqlUpsert, new { sprintId, puntosRestantes, horasRestantes });
            }
        }

        public List<Burndown> ObtenerSerie(int sprintId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT id                 AS Id,
                           sprint_id           AS SprintId,
                           fecha               AS Fecha,
                           puntos_restantes    AS PuntosRestantes,
                           horas_restantes     AS HorasRestantes
                    FROM burndown
                    WHERE sprint_id = @sprintId
                    ORDER BY fecha";

                return new List<Burndown>(con.Query<Burndown>(sql, new { sprintId }));
            }
        }

        // HU-116.
        public CumplimientoSprint ObtenerCumplimiento(int sprintId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT COALESCE(SUM(story_points), 0) AS TotalPuntos,
                           COALESCE(SUM(CASE WHEN estado IN ('terminada', 'aceptada') THEN story_points ELSE 0 END), 0) AS PuntosCompletados
                    FROM historia
                    WHERE sprint_id = @sprintId";

                return con.QueryFirstOrDefault<CumplimientoSprint>(sql, new { sprintId });
            }
        }
    }
}
