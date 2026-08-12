using System.Collections.Generic;
using System.Web.Mvc;

namespace ScrumMvp.Helpers
{
    // Fuente única de verdad para el estado de una tarea en el tablero.
    // Los "Value" deben calzar EXACTO con el ENUM de la tabla 'tarea' en MySQL.
    public static class EstadoTareaHelper
    {
        public const string Pendiente = "pendiente";
        public const string EnProgreso = "en_progreso";
        public const string EnRevision = "en_revision";
        public const string Pruebas = "pruebas";
        public const string Terminado = "terminado";

        // Orden de las columnas del tablero (HU-068).
        public static List<string> Columnas()
        {
            return new List<string> { Pendiente, EnProgreso, EnRevision, Pruebas, Terminado };
        }

        public static List<SelectListItem> ComoLista(string seleccionado = null)
        {
            var lista = new List<SelectListItem>();
            foreach (var valor in Columnas())
            {
                lista.Add(new SelectListItem { Value = valor, Text = Etiqueta(valor), Selected = valor == seleccionado });
            }
            return lista;
        }

        public static string Etiqueta(string valor)
        {
            switch (valor)
            {
                case Pendiente: return "Pendiente";
                case EnProgreso: return "En progreso";
                case EnRevision: return "En revisión";
                case Pruebas: return "Pruebas";
                case Terminado: return "Terminado";
                default: return valor;
            }
        }

        public static string ClaseBadge(string valor)
        {
            switch (valor)
            {
                case Pendiente: return "text-bg-secondary";
                case EnProgreso: return "text-bg-primary";
                case EnRevision: return "text-bg-warning";
                case Pruebas: return "text-bg-info";
                case Terminado: return "text-bg-success";
                default: return "text-bg-light";
            }
        }
    }
}
