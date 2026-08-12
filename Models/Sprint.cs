using System;

namespace ScrumMvp.Models
{
    // Mapea la tabla 'sprint'. HU-049 Crear Sprint, HU-052 Sprint Goal, HU-054 Cerrar.
    public class Sprint
    {
        public int Id { get; set; }
        public int ProyectoId { get; set; }
        public string Nombre { get; set; }
        public string SprintGoal { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Estado { get; set; } // planificado | activo | cerrado
    }
}
