using System.Collections.Generic;
using Dapper;
using ScrumMvp.Models;

namespace ScrumMvp.Data
{
    public class DailyRepository
    {
        // HU-074: si esta persona ya cargó su Daily de hoy en este Sprint, lo trae
        // (así el formulario se abre en modo edición en vez de duplicar el registro:
        // la tabla tiene UNIQUE(sprint_id, usuario_id, fecha)).
        public Daily ObtenerDeHoy(int sprintId, int usuarioId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT id                 AS Id,
                           sprint_id           AS SprintId,
                           usuario_id          AS UsuarioId,
                           fecha               AS Fecha,
                           que_avance          AS QueAvance,
                           que_hare            AS QueHare,
                           tiene_impedimento   AS TieneImpedimento,
                           impedimento_texto   AS ImpedimentoTexto,
                           creado_en           AS CreadoEn
                    FROM daily
                    WHERE sprint_id = @sprintId AND usuario_id = @usuarioId AND fecha = CURDATE()";

                return con.QueryFirstOrDefault<Daily>(sql, new { sprintId, usuarioId });
            }
        }

        // HU-074: el Daily de hoy de todo el equipo, para mostrarlos juntos en la reunión.
        public List<DailyConUsuario> ObtenerDeHoyPorSprint(int sprintId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT d.id                 AS Id,
                           d.usuario_id          AS UsuarioId,
                           u.nombre              AS UsuarioNombre,
                           d.fecha               AS Fecha,
                           d.que_avance          AS QueAvance,
                           d.que_hare            AS QueHare,
                           d.tiene_impedimento   AS TieneImpedimento,
                           d.impedimento_texto   AS ImpedimentoTexto
                    FROM daily d
                    INNER JOIN usuario u ON u.id = d.usuario_id
                    WHERE d.sprint_id = @sprintId AND d.fecha = CURDATE()
                    ORDER BY u.nombre";

                return new List<DailyConUsuario>(con.Query<DailyConUsuario>(sql, new { sprintId }));
            }
        }

        public void Crear(DailyViewModel m, int usuarioId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    INSERT INTO daily (sprint_id, usuario_id, fecha, que_avance, que_hare, tiene_impedimento, impedimento_texto)
                    VALUES (@SprintId, @usuarioId, CURDATE(), @QueAvance, @QueHare, @TieneImpedimento, @ImpedimentoTexto)";

                con.Execute(sql, new { m.SprintId, usuarioId, m.QueAvance, m.QueHare, m.TieneImpedimento, m.ImpedimentoTexto });
            }
        }

        // HU-074: corrige el Daily de hoy en vez de duplicarlo.
        public void Actualizar(int id, DailyViewModel m)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    UPDATE daily SET
                        que_avance = @QueAvance,
                        que_hare = @QueHare,
                        tiene_impedimento = @TieneImpedimento,
                        impedimento_texto = @ImpedimentoTexto
                    WHERE id = @id";

                con.Execute(sql, new { id, m.QueAvance, m.QueHare, m.TieneImpedimento, m.ImpedimentoTexto });
            }
        }
    }
}
