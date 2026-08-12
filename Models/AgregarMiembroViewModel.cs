using System.ComponentModel.DataAnnotations;

namespace ScrumMvp.Models
{
    // HU-008/009: invitar a alguien ya registrado y asignarle su rol en el equipo.
    public class AgregarMiembroViewModel
    {
        public int EquipoId { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingresá un correo válido.")]
        [Display(Name = "Correo del integrante")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Elegí un rol.")]
        [Display(Name = "Rol en el equipo")]
        public string Rol { get; set; }
    }
}
