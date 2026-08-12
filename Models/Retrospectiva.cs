using System;

namespace ScrumMvp.Models
{
    // Mapea la tabla 'retrospectiva'. HU-103: Crear retrospectiva.
    public class Retrospectiva
    {
        public int Id { get; set; }
        public int SprintId { get; set; }
        public DateTime Fecha { get; set; }
    }
}
