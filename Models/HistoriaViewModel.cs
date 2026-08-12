using System.ComponentModel.DataAnnotations;

namespace ScrumMvp.Models
{
    // HU-019 Crear elemento, HU-020 Editar, HU-029 Historia (Como/quiero/para),
    // HU-030 Criterios de aceptación, HU-031 Prioridad, HU-043 Story Points.
    // Todas viven en la misma tabla, así que comparten un solo formulario.
    public class HistoriaViewModel
    {
        public int Id { get; set; } // 0 = historia nueva
        public int ProyectoId { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [Display(Name = "Título")]
        [StringLength(200)]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Indicá el rol (ej: comprador, productor...).")]
        [Display(Name = "Como (rol)")]
        [StringLength(200)]
        public string RolComo { get; set; }

        [Required(ErrorMessage = "Indicá qué necesita ese rol.")]
        [Display(Name = "Quiero")]
        [StringLength(300)]
        public string NecesidadQuiero { get; set; }

        [Display(Name = "Para")]
        [StringLength(300)]
        public string PropositoPara { get; set; }

        [Display(Name = "Criterios de aceptación")]
        public string CriteriosAceptacion { get; set; }

        [Required(ErrorMessage = "Elegí una prioridad.")]
        [Display(Name = "Prioridad")]
        public string Prioridad { get; set; }

        [Display(Name = "Story Points")]
        public decimal? StoryPoints { get; set; }
    }
}
