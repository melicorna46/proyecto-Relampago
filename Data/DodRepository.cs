using System.Collections.Generic;
using Dapper;
using ScrumMvp.Models;

namespace ScrumMvp.Data
{
    public class DodRepository
    {
        // HU-084: los criterios de la Definition of Done de un proyecto.
        public List<DodCriterio> ObtenerPorProyecto(int proyectoId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT id           AS Id,
                           proyecto_id  AS ProyectoId,
                           descripcion  AS Descripcion,
                           orden        AS Orden
                    FROM dod_criterio
                    WHERE proyecto_id = @proyectoId
                    ORDER BY orden";

                return new List<DodCriterio>(con.Query<DodCriterio>(sql, new { proyectoId }));
            }
        }

        public DodCriterio ObtenerPorId(int id)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT id           AS Id,
                           proyecto_id  AS ProyectoId,
                           descripcion  AS Descripcion,
                           orden        AS Orden
                    FROM dod_criterio
                    WHERE id = @id";

                return con.QueryFirstOrDefault<DodCriterio>(sql, new { id });
            }
        }

        // HU-084: agrega un criterio al final de la lista.
        public void Crear(DodCriterioViewModel m)
        {
            using (var con = Db.GetConnection())
            {
                string sqlSiguiente = "SELECT COUNT(1) FROM dod_criterio WHERE proyecto_id = @ProyectoId";
                int total = con.ExecuteScalar<int>(sqlSiguiente, new { m.ProyectoId });
                int nuevoOrden = total + 1;

                string sql = @"
                    INSERT INTO dod_criterio (proyecto_id, descripcion, orden)
                    VALUES (@ProyectoId, @Descripcion, @nuevoOrden)";

                con.Execute(sql, new { m.ProyectoId, m.Descripcion, nuevoOrden });
            }
        }

        public void Eliminar(int id)
        {
            using (var con = Db.GetConnection())
            {
                con.Execute("DELETE FROM historia_dod WHERE dod_criterio_id = @id", new { id });
                con.Execute("DELETE FROM dod_criterio WHERE id = @id", new { id });
            }
        }

        // HU-085: el checklist de una historia — todos los criterios del proyecto,
        // con su estado de cumplimiento en ESA historia (aunque todavía no se haya tocado).
        public List<DodChecklistItem> ObtenerChecklist(int historiaId, int proyectoId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT dc.id                AS DodCriterioId,
                           dc.descripcion        AS Descripcion,
                           dc.orden              AS Orden,
                           COALESCE(hd.cumplido, 0) AS Cumplido,
                           hd.verificado_por     AS VerificadoPor,
                           u.nombre              AS VerificadoPorNombre,
                           hd.verificado_en      AS VerificadoEn
                    FROM dod_criterio dc
                    LEFT JOIN historia_dod hd ON hd.dod_criterio_id = dc.id AND hd.historia_id = @historiaId
                    LEFT JOIN usuario u ON u.id = hd.verificado_por
                    WHERE dc.proyecto_id = @proyectoId
                    ORDER BY dc.orden";

                return new List<DodChecklistItem>(con.Query<DodChecklistItem>(sql, new { historiaId, proyectoId }));
            }
        }

        // HU-085: marca (o desmarca) un criterio para una historia puntual.
        // UNIQUE(historia_id, dod_criterio_id): si ya existe la fila, la actualiza en vez de duplicarla.
        public void MarcarCumplido(int historiaId, int dodCriterioId, bool cumplido, int verificadoPor)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    INSERT INTO historia_dod (historia_id, dod_criterio_id, cumplido, verificado_por, verificado_en)
                    VALUES (@historiaId, @dodCriterioId, @cumplido, @verificadoPor, NOW())
                    ON DUPLICATE KEY UPDATE
                        cumplido = @cumplido,
                        verificado_por = @verificadoPor,
                        verificado_en = NOW()";

                con.Execute(sql, new { historiaId, dodCriterioId, cumplido, verificadoPor });
            }
        }

        // HU-086: cuántos criterios de la Definition of Done todavía le faltan a esta historia.
        // Si el proyecto no tiene ningún criterio cargado, no hay nada que bloquear (da 0).
        public int ContarPendientes(int historiaId, int proyectoId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT COUNT(1)
                    FROM dod_criterio dc
                    LEFT JOIN historia_dod hd ON hd.dod_criterio_id = dc.id AND hd.historia_id = @historiaId
                    WHERE dc.proyecto_id = @proyectoId AND COALESCE(hd.cumplido, 0) = 0";

                return con.ExecuteScalar<int>(sql, new { historiaId, proyectoId });
            }
        }
    }
}
