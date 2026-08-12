using System.ComponentModel.DataAnnotations;

namespace ScrumMvp.Models
{
    // HU-078: Como Developer, quiero registrar un impedimento que me está bloqueando.
    public class ImpedimentoViewModel
    {
        public int SprintId { get; set; }

        [Required(ErrorMessage = "Contá qué te está bloqueando.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Elegí una prioridad.")]
        [Display(Name = "Prioridad")]
        public string Prioridad { get; set; }
    }
}
