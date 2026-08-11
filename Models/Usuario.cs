namespace ScrumMvp.Models
{
    // Mapea la tabla 'usuario' de la base.
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Especialidad { get; set; }
        public string Telefono { get; set; }
        public bool Activo { get; set; }
    }
}
