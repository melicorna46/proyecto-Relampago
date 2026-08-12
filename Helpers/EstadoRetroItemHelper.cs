using System.Collections.Generic;
using System.Web.Mvc;

namespace ScrumMvp.Helpers
{
    // Fuente única de verdad para el estado de un retro_item (aplica a las acciones, HU-106).
    // Los "Value" deben calzar EXACTO con el ENUM de la tabla 'retro_item' en MySQL.
    public static class EstadoRetroItemHelper
    {
        public const string Pendiente = "pendiente";
        public const string EnProgreso = "en_progreso";
        public const string Hecha = "hecha";

        public static List<SelectListItem> ComoLista(string seleccionado = null)
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = Pendiente, Text = "Pendiente", Selected = seleccionado == Pendiente },
                new SelectListItem { Value = EnProgreso, Text = "En progreso", Selected = seleccionado == EnProgreso },
                new SelectListItem { Value = Hecha, Text = "Hecha", Selected = seleccionado == Hecha }
            };
        }

        public static string Etiqueta(string valor)
        {
            switch (valor)
            {
                case Pendiente: return "Pendiente";
                case EnProgreso: return "En progreso";
                case Hecha: return "Hecha";
                default: return valor;
            }
        }

        public static string ClaseBadge(string valor)
        {
            switch (valor)
            {
                case Pendiente: return "text-bg-secondary";
                case EnProgreso: return "text-bg-primary";
                case Hecha: return "text-bg-success";
                default: return "text-bg-light";
            }
        }
    }
}
