using System.Collections.Generic;
using Dapper;
using ScrumMvp.Models;

namespace ScrumMvp.Data
{
    public class HistoriaRepository
    {
        // El Product Backlog son los elementos que TODAVÍA no entraron a un Sprint
        // (sprint_id IS NULL). Una vez que Persona 2 arme la Planificación de Sprint,
        // los que se seleccionen van a dejar de aparecer acá.
        public List<Historia> ObtenerBacklogPorProyecto(int proyectoId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT id                    AS Id,
                           proyecto_id            AS ProyectoId,
                           sprint_id              AS SprintId,
                           codigo                 AS Codigo,
                           titulo                 AS Titulo,
                           rol_como                AS RolComo,
                           necesidad_quiero       AS NecesidadQuiero,
                           proposito_para         AS PropositoPara,
                           criterios_aceptacion   AS CriteriosAceptacion,
                           prioridad              AS Prioridad,
                           orden                  AS Orden,
                           story_points           AS StoryPoints,
                           estado                 AS Estado
                    FROM historia
                    WHERE proyecto_id = @proyectoId AND sprint_id IS NULL
                    ORDER BY orden";

                return new List<Historia>(con.Query<Historia>(sql, new { proyectoId }));
            }
        }

        public Historia ObtenerPorId(int id)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT id                    AS Id,
                           proyecto_id            AS ProyectoId,
                           sprint_id              AS SprintId,
                           codigo                 AS Codigo,
                           titulo                 AS Titulo,
                           rol_como                AS RolComo,
                           necesidad_quiero       AS NecesidadQuiero,
                           proposito_para         AS PropositoPara,
                           criterios_aceptacion   AS CriteriosAceptacion,
                           prioridad              AS Prioridad,
                           orden                  AS Orden,
                           story_points           AS StoryPoints,
                           estado                 AS Estado
                    FROM historia
                    WHERE id = @id";

                return con.QueryFirstOrDefault<Historia>(sql, new { id });
            }
        }

        // HU-019 + HU-029 + HU-030 + HU-031 + HU-043: crea la historia.
        // El código (US-01, US-02...) y el orden (al final de la lista) se calculan solos.
        public void Crear(HistoriaViewModel m)
        {
            using (var con = Db.GetConnection())
            {
                string sqlSiguiente = @"
                    SELECT COUNT(1) FROM historia WHERE proyecto_id = @ProyectoId";
                int totalActuales = con.ExecuteScalar<int>(sqlSiguiente, new { m.ProyectoId });

                string codigo = "US-" + (totalActuales + 1).ToString("00");
                int nuevoOrden = totalActuales + 1;

                string sql = @"
                    INSERT INTO historia
                        (proyecto_id, codigo, titulo, rol_como, necesidad_quiero, proposito_para,
                         criterios_aceptacion, prioridad, orden, story_points, estado)
                    VALUES
                        (@ProyectoId, @codigo, @Titulo, @RolComo, @NecesidadQuiero, @PropositoPara,
                         @CriteriosAceptacion, @Prioridad, @nuevoOrden, @StoryPoints, 'backlog')";

                con.Execute(sql, new
                {
                    m.ProyectoId,
                    codigo,
                    m.Titulo,
                    m.RolComo,
                    m.NecesidadQuiero,
                    m.PropositoPara,
                    m.CriteriosAceptacion,
                    m.Prioridad,
                    nuevoOrden,
                    m.StoryPoints
                });
            }
        }

        // HU-020: edita el contenido. No toca código, orden ni estado.
        public void Actualizar(HistoriaViewModel m)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    UPDATE historia SET
                        titulo = @Titulo,
                        rol_como = @RolComo,
                        necesidad_quiero = @NecesidadQuiero,
                        proposito_para = @PropositoPara,
                        criterios_aceptacion = @CriteriosAceptacion,
                        prioridad = @Prioridad,
                        story_points = @StoryPoints
                    WHERE id = @Id";

                con.Execute(sql, new
                {
                    m.Id,
                    m.Titulo,
                    m.RolComo,
                    m.NecesidadQuiero,
                    m.PropositoPara,
                    m.CriteriosAceptacion,
                    m.Prioridad,
                    m.StoryPoints
                });
            }
        }

        // HU-022: mueve una historia un puesto hacia arriba (menor 'orden'),
        // intercambiando su posición con la vecina inmediata. Es una transacción:
        // las dos filas cambian de orden juntas, o ninguna cambia.
        public void MoverArriba(int id, int proyectoId)
        {
            Mover(id, proyectoId, subir: true);
        }

        public void MoverAbajo(int id, int proyectoId)
        {
            Mover(id, proyectoId, subir: false);
        }

        private void Mover(int id, int proyectoId, bool subir)
        {
            using (var con = Db.GetConnection())
            {
                con.Open();
                using (var tx = con.BeginTransaction())
                {
                    try
                    {
                        // Orden actual de la historia que se quiere mover.
                        int ordenActual = con.ExecuteScalar<int>(
                            "SELECT orden FROM historia WHERE id = @id",
                            new { id }, transaction: tx);

                        // Busca la vecina: la de orden inmediatamente menor (subir)
                        // o inmediatamente mayor (bajar), dentro del mismo proyecto y backlog.
                        string sqlVecino = subir
                            ? @"SELECT id, orden FROM historia
                                WHERE proyecto_id = @proyectoId AND sprint_id IS NULL AND orden < @ordenActual
                                ORDER BY orden DESC LIMIT 1"
                            : @"SELECT id, orden FROM historia
                                WHERE proyecto_id = @proyectoId AND sprint_id IS NULL AND orden > @ordenActual
                                ORDER BY orden ASC LIMIT 1";

                        var vecino = con.QueryFirstOrDefault(
                            sqlVecino, new { proyectoId, ordenActual }, transaction: tx);

                        // Si no hay vecino (ya está primera/última), no hay nada que mover.
                        if (vecino == null)
                        {
                            tx.Rollback();
                            return;
                        }

                        int vecinoId = (int)vecino.id;
                        int vecinoOrden = (int)vecino.orden;

                        con.Execute("UPDATE historia SET orden = @vecinoOrden WHERE id = @id",
                            new { vecinoOrden, id }, transaction: tx);

                        con.Execute("UPDATE historia SET orden = @ordenActual WHERE id = @vecinoId",
                            new { ordenActual, vecinoId }, transaction: tx);

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
