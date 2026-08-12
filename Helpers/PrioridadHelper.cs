using System.Collections.Generic;
using System.Web.Mvc;

namespace ScrumMvp.Helpers
{
    // Fuente única de verdad para la Prioridad (debe calzar con el ENUM de 'historia' en MySQL)
    // y para las opciones de Story Points (escala Fibonacci, la más usada en Planning Poker).
    public static class PrioridadHelper
    {
        public const string Alta = "alta";
        public const string Media = "media";
        public const string Baja = "baja";

        public static List<SelectListItem> ComoLista(string seleccionado = null)
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = Alta,  Text = "Alta",  Selected = seleccionado == Alta },
                new SelectListItem { Value = Media, Text = "Media", Selected = seleccionado == Media },
                new SelectListItem { Value = Baja,  Text = "Baja",  Selected = seleccionado == Baja }
            };
        }

        public static string ClaseBadge(string valor)
        {
            switch (valor)
            {
                case Alta: return "text-bg-danger";
                case Media: return "text-bg-warning";
                case Baja: return "text-bg-success";
                default: return "text-bg-light";
            }
        }

        public static string Etiqueta(string valor)
        {
            switch (valor)
            {
                case Alta: return "Alta";
                case Media: return "Media";
                case Baja: return "Baja";
                default: return valor;
            }
        }
    }

    // Opciones de Story Points: escala Fibonacci (la estándar en Scrum para estimar).
    public static class StoryPointsHelper
    {
        public static List<SelectListItem> ComoLista(decimal? seleccionado = null)
        {
            var valores = new[] { "1", "2", "3", "5", "8", "13", "21" };
            var lista = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Sin estimar" }
            };

            foreach (var v in valores)
            {
                lista.Add(new SelectListItem
                {
                    Value = v,
                    Text = v,
                    Selected = seleccionado.HasValue && seleccionado.Value.ToString() == v
                });
            }

            return lista;
        }
    }
}
