using System.Collections.Generic;

namespace ScrumMvp.Helpers
{
    // Fuente única de verdad para el estado de un impedimento.
    // Los "Value" deben calzar EXACTO con el ENUM de la tabla 'impedimento' en MySQL.
    // HU-081: la transición es siempre hacia adelante: abierto -> en_gestion -> resuelto.
    public static class EstadoImpedimentoHelper
    {
        public const string Abierto = "abierto";
        public const string EnGestion = "en_gestion";
        public const string Resuelto = "resuelto";

        public static string Etiqueta(string valor)
        {
            switch (valor)
            {
                case Abierto: return "Abierto";
                case EnGestion: return "En gestión";
                case Resuelto: return "Resuelto";
                default: return valor;
            }
        }

        public static string ClaseBadge(string valor)
        {
            switch (valor)
            {
                case Abierto: return "text-bg-danger";
                case EnGestion: return "text-bg-warning";
                case Resuelto: return "text-bg-success";
                default: return "text-bg-light";
            }
        }

        // HU-081: próximo estado válido desde el actual (null si ya está resuelto).
        public static string SiguienteEstado(string actual)
        {
            switch (actual)
            {
                case Abierto: return EnGestion;
                case EnGestion: return Resuelto;
                default: return null;
            }
        }

        public static List<string> Columnas()
        {
            return new List<string> { Abierto, EnGestion, Resuelto };
        }
    }
}
