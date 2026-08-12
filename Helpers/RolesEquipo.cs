using System.Collections.Generic;
using System.Web.Mvc;

namespace ScrumMvp.Helpers
{
    // Fuente única de verdad para los roles de equipo_miembro.
    // Los "Value" deben coincidir EXACTO con el ENUM de la tabla equipo_miembro en MySQL.
    public static class RolesEquipo
    {
        public const string ProductOwner = "product_owner";
        public const string ScrumMaster = "scrum_master";
        public const string Developer = "developer";

        public static List<SelectListItem> ComoLista(string seleccionado = null)
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = ProductOwner, Text = "Product Owner", Selected = seleccionado == ProductOwner },
                new SelectListItem { Value = ScrumMaster,  Text = "Scrum Master",  Selected = seleccionado == ScrumMaster },
                new SelectListItem { Value = Developer,    Text = "Developer",     Selected = seleccionado == Developer }
            };
        }

        // Texto lindo para mostrar en pantalla a partir del valor guardado en la BD.
        public static string Etiqueta(string valor)
        {
            switch (valor)
            {
                case ProductOwner: return "Product Owner";
                case ScrumMaster: return "Scrum Master";
                case Developer: return "Developer";
                default: return valor;
            }
        }

        // Clase de color (Bootstrap) para el badge de cada rol.
        public static string ClaseBadge(string valor)
        {
            switch (valor)
            {
                case ProductOwner: return "text-bg-primary";
                case ScrumMaster: return "text-bg-success";
                case Developer: return "text-bg-secondary";
                default: return "text-bg-light";
            }
        }
    }
}
