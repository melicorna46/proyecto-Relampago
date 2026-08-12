using System.ComponentModel.DataAnnotations;

namespace ScrumMvp.Models
{
    // HU-013 Crear producto, HU-014 Visión, HU-015 Product Goal, HU-016 Descripción.
    // Las cuatro viven en la misma tabla 'proyecto', así que se cargan en un solo formulario.
    public class ProyectoViewModel
    {
        [Required(ErrorMessage = "El nombre del proyecto es obligatorio.")]
        [Display(Name = "Nombre del proyecto")]
        [StringLength(160)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El Product Goal es obligatorio.")]
        [Display(Name = "Product Goal")]
        [StringLength(500, ErrorMessage = "Máximo 500 caracteres.")]
        public string ProductGoal { get; set; }

        [Display(Name = "Visión del producto")]
        [StringLength(500, ErrorMessage = "Máximo 500 caracteres.")]
        public string Vision { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
    }
}
