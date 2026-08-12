namespace ScrumMvp.Models
{
    // Combina el Proyecto con el Id de su equipo (si ya tiene uno),
    // para que la tarjeta de "Mis proyectos" sepa a dónde llevar el botón.
    public class ProyectoConEquipoViewModel
    {
        public Proyecto Proyecto { get; set; }
        public int? EquipoId { get; set; }
    }
}
