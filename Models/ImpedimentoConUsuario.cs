using System;

namespace ScrumMvp.Models
{
    // Fila combinada de impedimento + usuario (reportó / responsable), para listar (HU-078/081).
    public class ImpedimentoConUsuario
    {
        public int Id { get; set; }
        public int ReportadoPor { get; set; }
        public string ReportadoPorNombre { get; set; }
        public int? ResponsableId { get; set; }
        public string ResponsableNombre { get; set; }
        public string Descripcion { get; set; }
        public string Prioridad { get; set; }
        public string Estado { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaResolucion { get; set; }
    }
}
