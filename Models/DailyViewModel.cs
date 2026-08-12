using System.ComponentModel.DataAnnotations;

namespace ScrumMvp.Models
{
    // HU-074: Como Developer, quiero registrar mi Daily Scrum.
    public class DailyViewModel
    {
        public int SprintId { get; set; }

        [Required(ErrorMessage = "Contá qué avanzaste desde el último Daily.")]
        [Display(Name = "¿Qué avancé?")]
        public string QueAvance { get; set; }

        [Required(ErrorMessage = "Contá qué vas a hacer hoy.")]
        [Display(Name = "¿Qué haré?")]
        public string QueHare { get; set; }

        [Display(Name = "Tengo un impedimento")]
        public bool TieneImpedimento { get; set; }

        [Display(Name = "Descripción del impedimento")]
        public string ImpedimentoTexto { get; set; }
    }
}
