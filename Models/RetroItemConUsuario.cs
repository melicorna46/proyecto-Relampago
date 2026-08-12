namespace ScrumMvp.Models
{
    // Fila combinada de retro_item + usuario responsable. HU-105 (problemas) y HU-106 (acciones).
    public class RetroItemConUsuario
    {
        public int Id { get; set; }
        public int RetrospectivaId { get; set; }
        public string Tipo { get; set; } // positivo | problema | accion
        public string Descripcion { get; set; }
        public int? ResponsableId { get; set; }
        public string ResponsableNombre { get; set; }
        public string Estado { get; set; } // pendiente | en_progreso | hecha
    }
}
