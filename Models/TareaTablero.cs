namespace ScrumMvp.Models
{
    // Fila combinada de tarea + historia + usuario responsable, lista para pintar
    // en el Tablero (HU-068) sin tener que hacer varias consultas por tarjeta.
    public class TareaTablero
    {
        public int Id { get; set; }
        public int HistoriaId { get; set; }
        public string HistoriaCodigo { get; set; }
        public string HistoriaTitulo { get; set; }

        public string Titulo { get; set; }
        public string Descripcion { get; set; }

        public int? ResponsableId { get; set; }
        public string ResponsableNombre { get; set; }

        public string Estado { get; set; }
        public decimal? HorasEstimadas { get; set; }
        public decimal? HorasRestantes { get; set; }
        public bool Bloqueada { get; set; }
    }
}
