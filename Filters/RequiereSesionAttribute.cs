using System.Web.Mvc;

namespace ScrumMvp.Filters
{
    // Se pone sobre cualquier controlador/acción que solo debe verse
    // con sesión iniciada. Si no hay usuario en Session, manda al Login.
    public class RequiereSesionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["UsuarioActual"] == null)
            {
                filterContext.Result = new RedirectResult("~/Cuenta/Login");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
