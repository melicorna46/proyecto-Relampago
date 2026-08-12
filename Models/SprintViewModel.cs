using System;
using System.ComponentModel.DataAnnotations;

namespace ScrumMvp.Models
{
    // HU-049 Crear Sprint + HU-052 Sprint Goal.
    public class SprintViewModel
    {
        public int ProyectoId { get; set; }

        [Required(ErrorMessage = "Ponele un nombre al Sprint.")]
        [Display(Name = "Nombre del Sprint")]
        [StringLength(120)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El Sprint Goal es obligatorio.")]
        [Display(Name = "Sprint Goal")]
        [StringLength(500)]
        public string SprintGoal { get; set; }

        [Required(ErrorMessage = "Indicá la fecha de inicio.")]
        [Display(Name = "Fecha de inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "Indicá la fecha de fin.")]
        [Display(Name = "Fecha de fin")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }
    }
}
