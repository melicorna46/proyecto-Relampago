using System.Collections.Generic;
using Dapper;
using ScrumMvp.Models;

namespace ScrumMvp.Data
{
    public class TareaRepository
    {
        // HU-068: todas las tarjetas del tablero de un Sprint, con el código/título
        // de su historia y el nombre del responsable ya resueltos (para no hacer
        // una consulta aparte por cada tarjeta al pintar la vista).
        public List<TareaTablero> ObtenerPorSprint(int sprintId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT t.id              AS Id,
                           t.historia_id      AS HistoriaId,
                           h.codigo           AS HistoriaCodigo,
                           h.titulo           AS HistoriaTitulo,
                           t.titulo           AS Titulo,
                           t.descripcion      AS Descripcion,
                           t.responsable_id   AS ResponsableId,
                           u.nombre           AS ResponsableNombre,
                           t.estado           AS Estado,
                           t.horas_estimadas  AS HorasEstimadas,
                           t.horas_restantes  AS HorasRestantes,
                           t.bloqueada        AS Bloqueada
                    FROM tarea t
                    INNER JOIN historia h ON h.id = t.historia_id
                    LEFT JOIN usuario u ON u.id = t.responsable_id
                    WHERE h.sprint_id = @sprintId
                    ORDER BY t.orden, t.id";

                return new List<TareaTablero>(con.Query<TareaTablero>(sql, new { sprintId }));
            }
        }

        // HU-062: crea la tarea técnica dentro de una historia del Sprint. Nace 'pendiente'
        // y sin responsable: HU-063 exige que el Developer la tome por sí mismo, no que
        // nazca ya asignada.
        public void Crear(TareaViewModel m)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    INSERT INTO tarea (historia_id, titulo, descripcion, horas_estimadas, horas_restantes, estado)
                    VALUES (@HistoriaId, @Titulo, @Descripcion, @HorasEstimadas, @HorasEstimadas, 'pendiente')";

                con.Execute(sql, new { m.HistoriaId, m.Titulo, m.Descripcion, m.HorasEstimadas });
            }
        }

        // HU-063: el Developer se auto-asigna la tarea. El 'AND responsable_id IS NULL'
        // evita pisar a alguien que ya la haya tomado (la rúbrica penaliza que el Scrum
        // Master reparta tareas; acá cada quien toma la suya).
        public void AsignarmeComoResponsable(int tareaId, int usuarioId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    UPDATE tarea
                    SET responsable_id = @usuarioId
                    WHERE id = @tareaId AND responsable_id IS NULL";

                con.Execute(sql, new { tareaId, usuarioId });
            }
        }

        // HU-069: mover la tarjeta a otra columna del tablero.
        public void CambiarEstado(int tareaId, string nuevoEstado)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"UPDATE tarea SET estado = @nuevoEstado WHERE id = @tareaId";
                con.Execute(sql, new { tareaId, nuevoEstado });
            }
        }
    }
}
