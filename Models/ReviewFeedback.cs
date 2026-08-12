using System;

namespace ScrumMvp.Models
{
    // Mapea la tabla 'review_feedback'. HU-100: Registrar feedback de la Review.
    public class ReviewFeedback
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public string Autor { get; set; }
        public string Comentario { get; set; }
        public DateTime CreadoEn { get; set; }
    }
}
