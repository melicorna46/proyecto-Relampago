using System.Web.Mvc;
using ScrumMvp.Data;
using ScrumMvp.Filters;
using ScrumMvp.Models;

namespace ScrumMvp.Controllers
{
    [RequiereSesion]
    public class TableroController : Controller
    {
        private readonly ProyectoRepository _proyectoRepo = new ProyectoRepository();
        private readonly SprintRepository _sprintRepo = new SprintRepository();
        private readonly HistoriaRepository _historiaRepo = new HistoriaRepository();
        private readonly TareaRepository _tareaRepo = new TareaRepository();
        private readonly EquipoRepository _equipoRepo = new EquipoRepository();

        // ---- Permiso: cualquier miembro del equipo (o quien creó el proyecto).
        //      El tablero es colaborativo: todo el equipo puede crear tareas,
        //      tomarlas y moverlas (así lo pide HU-062/063/069). ----
        private Proyecto ObtenerProyectoParaVer(int proyectoId)
        {
            var proyecto = _proyectoRepo.ObtenerPorId(proyectoId);
            if (proyecto == null) return null;

            var usuario = (UsuarioSesion)Session["UsuarioActual"];
            bool esCreador = proyecto.CreadoPor == usuario.Id;
            bool esMiembroDelEquipo = _equipoRepo.ObtenerRolDeUsuarioEnProyecto(proyectoId, usuario.Id) != null;

            return (esCreador || esMiembroDelEquipo) ? proyecto : null;
        }

        // GET /Tablero/Index?sprintId=X — HU-068: tablero visual del Sprint.
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

            ViewBag.Sprint = sprint;
            ViewBag.NombreProyecto = proyecto.Nombre;
            ViewBag.HistoriasDelSprint = _historiaRepo.ObtenerPorSprint(sprintId);

            var tareas = _tareaRepo.ObtenerPorSprint(sprintId);
            return View(tareas);
        }

        // GET /Tablero/Crear?historiaId=X&sprintId=Y — HU-062.
        public ActionResult Crear(int historiaId, int sprintId)
        {
            var historia = _historiaRepo.ObtenerPorId(historiaId);
            if (historia == null)
            {
                TempData["Error"] = "La historia no existe.";
                return RedirectToAction("Index", new { sprintId });
            }

            var proyecto = ObtenerProyectoParaVer(historia.ProyectoId);
            if (proyecto == null)
            {
                TempData["Error"] = "No tenés acceso a ese proyecto.";
                return RedirectToAction("Index", "Proyecto");
            }

            ViewBag.Historia = historia;
            ViewBag.SprintId = sprintId;

            return View(new TareaViewModel { HistoriaId = historiaId });
        }

        // POST /Tablero/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(TareaViewModel modelo, int sprintId)
        {
            var historia = _historiaRepo.ObtenerPorId(modelo.HistoriaId);
            if (historia == null)
            {
                TempData["Error"] = "La historia no existe.";
                return RedirectToAction("Index", new { sprintId });
            }

            var proyecto = ObtenerProyectoParaVer(historia.ProyectoId);
            if (proyecto == null)
            {
                TempData["Error"] = "No tenés acceso a ese proyecto.";
                return RedirectToAction("Index", "Proyecto");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Historia = historia;
                ViewBag.SprintId = sprintId;
                return View(modelo);
            }

            _tareaRepo.Crear(modelo);

            TempData["Mensaje"] = "Tarea creada.";
            return RedirectToAction("Index", new { sprintId });
        }

        // POST /Tablero/Tomar — HU-063: el Developer se auto-asigna la tarea.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Tomar(int tareaId, int sprintId)
        {
            var sprint = _sprintRepo.ObtenerPorId(sprintId);
            var proyecto = sprint != null ? ObtenerProyectoParaVer(sprint.ProyectoId) : null;

            if (proyecto != null)
            {
                var usuario = (UsuarioSesion)Session["UsuarioActual"];
                _tareaRepo.AsignarmeComoResponsable(tareaId, usuario.Id);
            }
            else
            {
                TempData["Error"] = "No tenés acceso a ese proyecto.";
            }

            return RedirectToAction("Index", new { sprintId });
        }

        // POST /Tablero/CambiarEstado — HU-069: mover la tarjeta entre columnas.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarEstado(int tareaId, string nuevoEstado, int sprintId)
        {
            var sprint = _sprintRepo.ObtenerPorId(sprintId);
            var proyecto = sprint != null ? ObtenerProyectoParaVer(sprint.ProyectoId) : null;

            if (proyecto != null)
            {
                _tareaRepo.CambiarEstado(tareaId, nuevoEstado);
            }
            else
            {
                TempData["Error"] = "No tenés acceso a ese proyecto.";
            }

            return RedirectToAction("Index", new { sprintId });
        }
    }
}
