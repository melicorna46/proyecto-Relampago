using System.Collections.Generic;
using Dapper;
using ScrumMvp.Helpers;
using ScrumMvp.Models;

namespace ScrumMvp.Data
{
    public class ImpedimentoRepository
    {
        // HU-078/081: todos los impedimentos del Sprint, con nombres ya resueltos.
        public List<ImpedimentoConUsuario> ObtenerPorSprint(int sprintId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT i.id                AS Id,
                           i.reportado_por      AS ReportadoPor,
                           ur.nombre            AS ReportadoPorNombre,
                           i.responsable_id     AS ResponsableId,
                           us.nombre            AS ResponsableNombre,
                           i.descripcion        AS Descripcion,
                           i.prioridad          AS Prioridad,
                           i.estado             AS Estado,
                           i.fecha_apertura     AS FechaApertura,
                           i.fecha_resolucion   AS FechaResolucion
                    FROM impedimento i
                    INNER JOIN usuario ur ON ur.id = i.reportado_por
                    LEFT JOIN usuario us ON us.id = i.responsable_id
                    WHERE i.sprint_id = @sprintId
                    ORDER BY FIELD(i.estado, 'abierto', 'en_gestion', 'resuelto'), i.fecha_apertura DESC";

                return new List<ImpedimentoConUsuario>(con.Query<ImpedimentoConUsuario>(sql, new { sprintId }));
            }
        }

        public Impedimento ObtenerPorId(int id)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT id                 AS Id,
                           sprint_id           AS SprintId,
                           reportado_por       AS ReportadoPor,
                           responsable_id      AS ResponsableId,
                           descripcion         AS Descripcion,
                           prioridad           AS Prioridad,
                           estado              AS Estado,
                           fecha_apertura      AS FechaApertura,
                           fecha_resolucion    AS FechaResolucion
                    FROM impedimento
                    WHERE id = @id";

                return con.QueryFirstOrDefault<Impedimento>(sql, new { id });
            }
        }

        // HU-078: nace 'abierto' y sin responsable — quien lo reporta no queda
        // forzado a resolverlo (HU-081 lo toma quien puede ayudar).
        public void Crear(ImpedimentoViewModel m, int reportadoPor)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    INSERT INTO impedimento (sprint_id, reportado_por, descripcion, prioridad, estado, fecha_apertura)
                    VALUES (@SprintId, @reportadoPor, @Descripcion, @Prioridad, 'abierto', CURDATE())";

                con.Execute(sql, new { m.SprintId, reportadoPor, m.Descripcion, m.Prioridad });
            }
        }

        // El equipo se autoasigna quién va a gestionar el impedimento. El 'AND responsable_id
        // IS NULL' evita pisar a alguien que ya lo tomó — nadie lo reparte, cada quien lo toma.
        public void AsignarmeComoResponsable(int impedimentoId, int usuarioId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    UPDATE impedimento
                    SET responsable_id = @usuarioId
                    WHERE id = @impedimentoId AND responsable_id IS NULL";

                con.Execute(sql, new { impedimentoId, usuarioId });
            }
        }

        // HU-081: avanza el impedimento al siguiente estado (abierto -> en_gestion -> resuelto).
        // Al llegar a 'resuelto' registra la fecha de resolución.
        public void AvanzarEstado(int impedimentoId, string nuevoEstado)
        {
            using (var con = Db.GetConnection())
            {
                string sql = nuevoEstado == EstadoImpedimentoHelper.Resuelto
                    ? @"UPDATE impedimento SET estado = @nuevoEstado, fecha_resolucion = CURDATE() WHERE id = @impedimentoId"
                    : @"UPDATE impedimento SET estado = @nuevoEstado WHERE id = @impedimentoId";

                con.Execute(sql, new { impedimentoId, nuevoEstado });
            }
        }
    }
}
