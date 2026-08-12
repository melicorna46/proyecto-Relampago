using System.ComponentModel.DataAnnotations;

namespace ScrumMvp.Models
{
    // HU-084: Como equipo, quiero definir los criterios de la Definition of Done.
    public class DodCriterioViewModel
    {
        public int ProyectoId { get; set; }

        [Required(ErrorMessage = "Escribí el criterio.")]
        [Display(Name = "Criterio")]
        [StringLength(300)]
        public string Descripcion { get; set; }
    }
}
