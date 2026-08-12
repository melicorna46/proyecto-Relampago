namespace ScrumMvp.Models
{
    // Mapea la tabla 'dod_criterio'. HU-084: Definition of Done del proyecto.
    public class DodCriterio
    {
        public int Id { get; set; }
        public int ProyectoId { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
    }
}
