using System;

namespace ScrumMvp.Models
{
    // Mapea la tabla 'burndown'. HU-112: un punto por día del Sprint.
    public class Burndown
    {
        public int Id { get; set; }
        public int SprintId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal PuntosRestantes { get; set; }
        public decimal HorasRestantes { get; set; }
    }
}
