namespace ScrumMvp.Helpers
{
    // Fuente única de verdad para el estado de una historia.
    // Los "Value" deben calzar EXACTO con el ENUM de la tabla 'historia' en MySQL.
    public static class EstadoHistoriaHelper
    {
        public const string Backlog = "backlog";
        public const string EnSprint = "en_sprint";
        public const string Terminada = "terminada";
        public const string Aceptada = "aceptada";

        public static string Etiqueta(string valor)
        {
            switch (valor)
            {
                case Backlog: return "Backlog";
                case EnSprint: return "En Sprint";
                case Terminada: return "Terminada";
                case Aceptada: return "Aceptada";
                default: return valor;
            }
        }

        public static string ClaseBadge(string valor)
        {
            switch (valor)
            {
                case Backlog: return "text-bg-light text-dark border";
                case EnSprint: return "text-bg-primary";
                case Terminada: return "text-bg-info";
                case Aceptada: return "text-bg-success";
                default: return "text-bg-light";
            }
        }
    }
}
