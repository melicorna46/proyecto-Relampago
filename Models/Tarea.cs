namespace ScrumMvp.Models
{
    // Mapea la tabla 'tarea'. Las tarjetas del Tablero (HU-062, HU-063, HU-068, HU-069).
    public class Tarea
    {
        public int Id { get; set; }
        public int HistoriaId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int? ResponsableId { get; set; }
        public string Estado { get; set; } // pendiente | en_progreso | en_revision | pruebas | terminado
        public decimal? HorasEstimadas { get; set; }
        public decimal? HorasRestantes { get; set; }
        public bool Bloqueada { get; set; }
        public int Orden { get; set; }
    }
}
