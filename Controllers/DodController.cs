using System.Web.Mvc;
using ScrumMvp.Data;
using ScrumMvp.Filters;
using ScrumMvp.Helpers;
using ScrumMvp.Models;

namespace ScrumMvp.Controllers
{
    [RequiereSesion]
    public class DodController : Controller
    {
        private readonly ProyectoRepository _proyectoRepo = new ProyectoRepository();
        private readonly EquipoRepository _equipoRepo = new EquipoRepository();
        private readonly HistoriaRepository _historiaRepo = new HistoriaRepository();
        private readonly DodRepository _dodRepo = new DodRepository();

        // ---- Permiso: cualquier miembro del equipo. La Definition of Done es del
        //      equipo completo (Scrum Guide), no de un único rol. ----
        private Proyecto ObtenerProyectoParaVer(int proyectoId)
        {
            var proyecto = _proyectoRepo.ObtenerPorId(proyectoId);
            if (proyecto == null) return null;

            var usuario = (UsuarioSesion)Session["UsuarioActual"];
            bool esCreador = proyecto.CreadoPor == usuario.Id;
            bool esMiembroDelEquipo = _equipoRepo.ObtenerRolDeUsuarioEnProyecto(proyectoId, usuario.Id) != null;

            return (esCreador || esMiembroDelEquipo) ? proyecto : null;
        }

        // GET /Dod/Index?proyectoId=X — HU-084: administrar los criterios de la DoD.
        public ActionResult Index(int proyectoId)
        {
            var proyecto = ObtenerProyectoParaVer(proyectoId);
            if (proyecto == null)
            {
                TempData["Error"] = "No tenés acceso a ese proyecto.";
                return RedirectToAction("Index", "Proyecto");
            }

            ViewBag.ProyectoId = proyectoId;
            ViewBag.NombreProyecto = proyecto.Nombre;

            var criterios = _dodRepo.ObtenerPorProyecto(proyectoId);
            return View(criterios);
        }

        // POST /Dod/Crear — HU-084.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(DodCriterioViewModel modelo)
        {
            var proyecto = ObtenerProyectoParaVer(modelo.ProyectoId);
            if (proyecto == null)
            {
                TempData["Error"] = "No tenés acceso a ese proyecto.";
                return RedirectToAction("Index", "Proyecto");
            }

            if (!string.IsNullOrWhiteSpace(modelo.Descripcion))
            {
                _dodRepo.Crear(modelo);
                TempData["Mensaje"] = "Criterio agregado a la Definition of Done.";
            }
            else
            {
                TempData["Error"] = "Escribí el criterio antes de agregarlo.";
            }

            return RedirectToAction("Index", new { proyectoId = modelo.ProyectoId });
        }

        // POST /Dod/Eliminar — HU-084.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id, int proyectoId)
        {
            var proyecto = ObtenerProyectoParaVer(proyectoId);
            if (proyecto != null)
            {
                _dodRepo.Eliminar(id);
                TempData["Mensaje"] = "Criterio eliminado.";
            }
            else
            {
                TempData["Error"] = "No tenés acceso a ese proyecto.";
            }

            return RedirectToAction("Index", new { proyectoId });
        }

        // GET /Dod/Checklist?historiaId=X — HU-085: checklist de una historia puntual.
        public ActionResult Checklist(int historiaId)
        {
            var historia = _historiaRepo.ObtenerPorId(historiaId);
            if (historia == null)
            {
                TempData["Error"] = "La historia no existe.";
                return RedirectToAction("Index", "Proyecto");
            }

            var proyecto = ObtenerProyectoParaVer(historia.ProyectoId);
            if (proyecto == null)
            {
                TempData["Error"] = "No tenés acceso a ese proyecto.";
                return RedirectToAction("Index", "Proyecto");
            }

            ViewBag.Historia = historia;
            ViewBag.NombreProyecto = proyecto.Nombre;
            ViewBag.PendientesCount = _dodRepo.ContarPendientes(historiaId, historia.ProyectoId);

            var checklist = _dodRepo.ObtenerChecklist(historiaId, historia.ProyectoId);
            return View(checklist);
        }

        // POST /Dod/MarcarCumplido — HU-085.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarcarCumplido(int historiaId, int dodCriterioId, bool cumplido)
        {
            var historia = _historiaRepo.ObtenerPorId(historiaId);
            var proyecto = historia != null ? ObtenerProyectoParaVer(historia.ProyectoId) : null;

            if (proyecto != null)
            {
                var usuario = (UsuarioSesion)Session["UsuarioActual"];
                _dodRepo.MarcarCumplido(historiaId, dodCriterioId, cumplido, usuario.Id);
            }
            else
            {
                TempData["Error"] = "No tenés acceso a ese proyecto.";
            }

            return RedirectToAction("Checklist", new { historiaId });
        }

        // POST /Dod/Terminar — HU-086: no deja pasar la historia a 'terminada' si falta
        // algún criterio de la Definition of Done sin cumplir.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Terminar(int historiaId)
        {
            var historia = _historiaRepo.ObtenerPorId(historiaId);
            if (historia == null)
            {
                TempData["Error"] = "La historia no existe.";
                return RedirectToAction("Index", "Proyecto");
            }

            var proyecto = ObtenerProyectoParaVer(historia.ProyectoId);
            if (proyecto == null)
            {
                TempData["Error"] = "No tenés acceso a ese proyecto.";
                return RedirectToAction("Index", "Proyecto");
            }

            int pendientes = _dodRepo.ContarPendientes(historiaId, historia.ProyectoId);
            if (pendientes > 0)
            {
                TempData["Error"] = "Todavía faltan " + pendientes + " criterio(s) de la Definition of Done sin cumplir.";
            }
            else
            {
                _historiaRepo.CambiarEstado(historiaId, EstadoHistoriaHelper.Terminada);
                TempData["Mensaje"] = "Historia marcada como terminada.";
            }

            return RedirectToAction("Checklist", new { historiaId });
        }
    }
}
