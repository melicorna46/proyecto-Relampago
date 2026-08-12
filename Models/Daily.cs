using System;

namespace ScrumMvp.Models
{
    // Mapea la tabla 'daily'. HU-074: Daily Scrum de una persona en un Sprint.
    public class Daily
    {
        public int Id { get; set; }
        public int SprintId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime Fecha { get; set; }
        public string QueAvance { get; set; }
        public string QueHare { get; set; }
        public bool TieneImpedimento { get; set; }
        public string ImpedimentoTexto { get; set; }
        public DateTime CreadoEn { get; set; }
    }
}
