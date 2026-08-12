using System.Web.Mvc;
using System.Web.Routing;

namespace ScrumMvp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                // La app abre en el Login: es el punto de entrada natural
                // (si no hay sesión, ninguna otra pantalla debería verse igual).
                defaults: new { controller = "Cuenta", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}