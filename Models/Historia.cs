namespace ScrumMvp.Models
{
    // Mapea la tabla 'historia'. Un elemento del Product Backlog.
    public class Historia
    {
        public int Id { get; set; }
        public int ProyectoId { get; set; }
        public int? SprintId { get; set; }   // null = todavía está en el Product Backlog

        public string Codigo { get; set; }
        public string Titulo { get; set; }

        public string RolComo { get; set; }
        public string NecesidadQuiero { get; set; }
        public string PropositoPara { get; set; }

        public string CriteriosAceptacion { get; set; }
        public string Prioridad { get; set; }
        public int Orden { get; set; }
        public decimal? StoryPoints { get; set; }
        public string Estado { get; set; }
    }
}
