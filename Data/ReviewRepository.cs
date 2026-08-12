using System.Collections.Generic;
using Dapper;
using ScrumMvp.Models;

namespace ScrumMvp.Data
{
    public class ReviewRepository
    {
        public SprintReview ObtenerPorSprint(int sprintId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT id         AS Id,
                           sprint_id  AS SprintId,
                           fecha      AS Fecha,
                           resultado  AS Resultado
                    FROM sprint_review
                    WHERE sprint_id = @sprintId
                    ORDER BY id DESC
                    LIMIT 1";

                return con.QueryFirstOrDefault<SprintReview>(sql, new { sprintId });
            }
        }

        // HU-098: abre la Review del Sprint la primera vez que alguien entra a la pantalla.
        public int Crear(int sprintId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    INSERT INTO sprint_review (sprint_id, fecha)
                    VALUES (@sprintId, CURDATE());
                    SELECT LAST_INSERT_ID();";

                return con.ExecuteScalar<int>(sql, new { sprintId });
            }
        }

        // HU-098: guarda el resumen del incremento mostrado en la Review.
        public void ActualizarResultado(int id, string resultado)
        {
            using (var con = Db.GetConnection())
            {
                string sql = "UPDATE sprint_review SET resultado = @resultado WHERE id = @id";
                con.Execute(sql, new { id, resultado });
            }
        }

        // HU-100.
        public List<ReviewFeedback> ObtenerFeedback(int reviewId)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    SELECT id          AS Id,
                           review_id   AS ReviewId,
                           autor       AS Autor,
                           comentario  AS Comentario,
                           creado_en   AS CreadoEn
                    FROM review_feedback
                    WHERE review_id = @reviewId
                    ORDER BY creado_en";

                return new List<ReviewFeedback>(con.Query<ReviewFeedback>(sql, new { reviewId }));
            }
        }

        public void AgregarFeedback(int reviewId, string autor, string comentario)
        {
            using (var con = Db.GetConnection())
            {
                string sql = @"
                    INSERT INTO review_feedback (review_id, autor, comentario, creado_en)
                    VALUES (@reviewId, @autor, @comentario, NOW())";

                con.Execute(sql, new { reviewId, autor, comentario });
            }
        }
    }
}
