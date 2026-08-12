using System;

namespace ScrumMvp.Models
{
    // Fila combinada de dod_criterio + historia_dod, para el checklist de una historia (HU-085).
    public class DodChecklistItem
    {
        public int DodCriterioId { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
        public bool Cumplido { get; set; }
        public int? VerificadoPor { get; set; }
        public string VerificadoPorNombre { get; set; }
        public DateTime? VerificadoEn { get; set; }
    }
}
