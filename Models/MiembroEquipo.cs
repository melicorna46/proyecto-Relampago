namespace ScrumMvp.Models
{
    // Fila combinada de equipo_miembro + usuario, para mostrar en la lista del equipo.
    public class MiembroEquipo
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; } // product_owner | scrum_master | developer
    }
}
