using System.Collections.Generic;
using Dapper;
using ScrumMvp.Models;

namespace ScrumMvp.Data
{
    public class RetroRepository
    {
        public Retrospectiva ObtenerPorSprint(int sprintId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT id         AS Id,
                           sprint_id  AS SprintId,
                           fecha      AS Fecha
                    FROM retrospectiva
                    WHERE sprint_id = @sprintId
                    ORDER BY id DESC
                    LIMIT 1";

                return con.QueryFirstOrDefault<Retrospectiva>(sql, new { sprintId });
            }
        }

        // HU-103: abre la Retrospectiva del Sprint la primera vez que alguien entra a la pantalla.
        public int Crear(int sprintId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    INSERT INTO retrospectiva (sprint_id, fecha)
                    VALUES (@sprintId, CURDATE());
                    SELECT LAST_INSERT_ID();";

                return con.ExecuteScalar<int>(sql, new { sprintId });
            }
        }

        // HU-105 (problema) + HU-106 (acción), con el nombre del responsable ya resuelto.
        public List<RetroItemConUsuario> ObtenerItems(int retrospectivaId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT ri.id               AS Id,
                           ri.retrospectiva_id AS RetrospectivaId,
                           ri.tipo             AS Tipo,
                           ri.descripcion      AS Descripcion,
                           ri.responsable_id   AS ResponsableId,
                           u.nombre            AS ResponsableNombre,
                           ri.estado           AS Estado
                    FROM retro_item ri
                    LEFT JOIN usuario u ON u.id = ri.responsable_id
                    WHERE ri.retrospectiva_id = @retrospectivaId
                    ORDER BY ri.id";

                return new List<RetroItemConUsuario>(con.Query<RetroItemConUsuario>(sql, new { retrospectivaId }));
            }
        }

        // HU-105/106: agrega un ítem ('problema' o 'accion'). Nace 'pendiente' y sin
        // responsable — las acciones se autoasignan (HU-106), nadie se las reparte.
        public void AgregarItem(int retrospectivaId, string tipo, string descripcion)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    INSERT INTO retro_item (retrospectiva_id, tipo, descripcion, estado)
                    VALUES (@retrospectivaId, @tipo, @descripcion, 'pendiente')";

                con.Execute(sql, new { retrospectivaId, tipo, descripcion });
            }
        }

        // HU-106: alguien del equipo se autoasigna la acción de mejora.
        public void AsignarmeComoResponsable(int itemId, int usuarioId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    UPDATE retro_item
                    SET responsable_id = @usuarioId
                    WHERE id = @itemId AND responsable_id IS NULL";

                con.Execute(sql, new { itemId, usuarioId });
            }
        }

        // HU-106: avanza el estado de la acción (pendiente -> en_progreso -> hecha).
        public void CambiarEstado(int itemId, string nuevoEstado)
        {
            using (var con = Db.GetConnection())
            {
                string sql = "UPDATE retro_item SET estado = @nuevoEstado WHERE id = @itemId";
                con.Execute(sql, new { itemId, nuevoEstado });
            }
        }
    }
}
