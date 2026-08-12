using System;

namespace ScrumMvp.Models
{
    // Mapea la tabla 'sprint_review'. HU-098: Mostrar el incremento en la Review.
    public class SprintReview
    {
        public int Id { get; set; }
        public int SprintId { get; set; }
        public DateTime Fecha { get; set; }
        public string Resultado { get; set; }
    }
}
