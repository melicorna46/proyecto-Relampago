using System.ComponentModel.DataAnnotations;

namespace ScrumMvp.Models
{
    // HU-062: Como Developer, quiero dividir una historia en tareas técnicas.
    public class TareaViewModel
    {
        public int HistoriaId { get; set; }

        [Required(ErrorMessage = "Ponele un título a la tarea.")]
        [Display(Name = "Título")]
        [StringLength(200)]
        public string Titulo { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Horas estimadas")]
        public decimal? HorasEstimadas { get; set; }
    }
}
