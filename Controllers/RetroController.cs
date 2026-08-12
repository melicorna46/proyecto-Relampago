using System.Web.Mvc;
using ScrumMvp.Data;
using ScrumMvp.Filters;
using ScrumMvp.Models;

namespace ScrumMvp.Controllers
{
    [RequiereSesion]
    public class RetroController : Controller
    {
        private readonly ProyectoRepository _proyectoRepo = new ProyectoRepository();
        private readonly SprintRepository _sprintRepo = new SprintRepository();
        private readonly EquipoRepository _equipoRepo = new EquipoRepository();
        private readonly RetroRepository _retroRepo = new RetroRepository();

        // ---- Permiso: cualquier miembro del equipo. La Retro es de todo el equipo. ----
        private Proyecto ObtenerProyectoParaVer(int proyectoId)
        {
            var proyecto = _proyectoRepo.ObtenerPorId(proyectoId);
            if (proyecto == null) return null;

            var usuario = (UsuarioSesion)Session["UsuarioActual"];
            bool esCreador = proyecto.CreadoPor == usuario.Id;
            bool esMiembroDelEquipo = _equipoRepo.ObtenerRolDeUsuarioEnProyecto(proyectoId, usuario.Id) != null;

            return (esCreador || esMiembroDelEquipo) ? proyecto : null;
        }

        // GET /Retro/Index?sprintId=X — HU-103: abre (o retoma) la Retrospectiva del Sprint.
        public ActionResult Index(int sprintId)
        {
            var sprint = _sprintRepo.ObtenerPorId(sprintId);
            if (sprint == null)
            {
                TempData["Error"] = "El Sprint no existe.";
                return RedirectToAction("Index", "Proyecto");
            }

            var proyecto = ObtenerProyectoParaVer(sprint.ProyectoId);
            if (proyecto == null)
            {
                TempData["Error"] = "No tenés acceso a ese proyecto.";
                return RedirectToAction("Index", "Proyecto");
            }

            var retro = _retroRepo.ObtenerPorSprint(sprintId);
            if (retro == null)
            {
                _retroRepo.Crear(sprintId);
                retro = _retroRepo.ObtenerPorSprint(sprintId);
            }

            ViewBag.Sprint = sprint;
            ViewBag.NombreProyecto = proyecto.Nombre;
            ViewBag.Retro = retro;

            var items = _retroRepo.ObtenerItems(retro.Id);
            return View(items);
        }

        // POST /Retro/CrearProblema — HU-105.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearProblema(int retrospectivaId, string descripcion, int sprintId)
        {
            AgregarItem(retrospectivaId, "problema", descripcion, sprintId);
            return RedirectToAction("Index", new { sprintId });
        }

        // POST /Retro/CrearAccion — HU-106.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearAccion(int retrospectivaId, string descripcion, int sprintId)
        {
            AgregarItem(retrospectivaId, "accion", descripcion, sprintId);
            return RedirectToAction("Index", new { sprintId });
        }

        private void AgregarItem(int retrospectivaId, string tipo, string descripcion, int sprintId)
        {
            var sprint = _sprintRepo.ObtenerPorId(sprintId);
            var proyecto = sprint != null ? ObtenerProyectoParaVer(sprint.ProyectoId) : null;

            if (proyecto != null && !string.IsNullOrWhiteSpace(descripcion))
            {
                _retroRepo.AgregarItem(retrospectivaId, tipo, descripcion);
                TempData["Mensaje"] = "Registrado.";
            }
            else if (proyecto == null)
            {
                TempData["Error"] = "No tenés acceso a ese proyecto.";
            }
        }

        // POST /Retro/Tomar — HU-106: alguien del equipo se autoasigna la acción de mejora.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Tomar(int itemId, int sprintId)
        {
            var sprint = _sprintRepo.ObtenerPorId(sprintId);
            var proyecto = sprint != null ? ObtenerProyectoParaVer(sprint.ProyectoId) : null;

            if (proyecto != null)
            {
                var usuario = (UsuarioSesion)Session["UsuarioActual"];
                _retroRepo.AsignarmeComoResponsable(itemId, usuario.Id);
            }
            else
            {
                TempData["Error"] = "No tenés acceso a ese proyecto.";
            }

            return RedirectToAction("Index", new { sprintId });
        }

        // POST /Retro/CambiarEstado — HU-106: avanza pendiente -> en_progreso -> hecha.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarEstado(int itemId, string nuevoEstado, int sprintId)
        {
            var sprint = _sprintRepo.ObtenerPorId(sprintId);
            var proyecto = sprint != null ? ObtenerProyectoParaVer(sprint.ProyectoId) : null;

            if (proyecto != null)
            {
                _retroRepo.CambiarEstado(itemId, nuevoEstado);
            }
            else
            {
                TempData["Error"] = "No tenés acceso a ese proyecto.";
            }

            return RedirectToAction("Index", new { sprintId });
        }
    }
}
