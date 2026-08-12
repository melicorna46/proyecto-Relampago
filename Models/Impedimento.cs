using System;

namespace ScrumMvp.Models
{
    // Mapea la tabla 'impedimento'. HU-078 Registrar, HU-081 Estado.
    public class Impedimento
    {
        public int Id { get; set; }
        public int SprintId { get; set; }
        public int ReportadoPor { get; set; }
        public int? ResponsableId { get; set; }
        public string Descripcion { get; set; }
        public string Prioridad { get; set; } // alta | media | baja
        public string Estado { get; set; } // abierto | en_gestion | resuelto
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaResolucion { get; set; }
    }
}
