using System.ComponentModel.DataAnnotations;

namespace ScrumMvp.Models
{
    // HU-007: al crear el equipo, quien lo crea también se suma como su primer miembro,
    // así que de una vez le pedimos cuál va a ser su rol ahí.
    public class CrearEquipoViewModel
    {
        public int ProyectoId { get; set; }

        [Required(ErrorMessage = "El nombre del equipo es obligatorio.")]
        [Display(Name = "Nombre del equipo")]
        [StringLength(160)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Elegí tu rol en este equipo.")]
        [Display(Name = "Tu rol en este equipo")]
        public string RolCreador { get; set; }
    }
}
