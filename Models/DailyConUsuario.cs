using System;

namespace ScrumMvp.Models
{
    // Fila combinada de daily + usuario, para listar el Daily de todo el equipo (HU-074).
    public class DailyConUsuario
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; }
        public DateTime Fecha { get; set; }
        public string QueAvance { get; set; }
        public string QueHare { get; set; }
        public bool TieneImpedimento { get; set; }
        public string ImpedimentoTexto { get; set; }
    }
}
