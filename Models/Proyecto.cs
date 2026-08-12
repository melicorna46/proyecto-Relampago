namespace ScrumMvp.Models
{
    public class Proyecto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string ProductGoal { get; set; }
        public string Vision { get; set; }
        public string Descripcion { get; set; }
        public int? CreadoPor { get; set; }
    }
}
