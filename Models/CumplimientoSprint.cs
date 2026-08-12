namespace ScrumMvp.Models
{
    // HU-116: % de Story Points completados (terminada/aceptada) sobre el total del Sprint.
    public class CumplimientoSprint
    {
        public decimal TotalPuntos { get; set; }
        public decimal PuntosCompletados { get; set; }

        public decimal Porcentaje
        {
            get { return TotalPuntos > 0 ? System.Math.Round(PuntosCompletados / TotalPuntos * 100, 1) : 0; }
        }
    }
}
