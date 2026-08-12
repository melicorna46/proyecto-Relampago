using System.Web.Mvc;
using ScrumMvp.Data;
using ScrumMvp.Filters;
using ScrumMvp.Helpers;
using ScrumMvp.Models;

namespace ScrumMvp.Controllers
{
    [RequiereSesion]
    public class ImpedimentoController : Controller
    {
        private readonly ProyectoRepository _proyectoRepo = new ProyectoRepository();
        private readonly SprintRepository _sprintRepo = new SprintRepository();
        private readonly EquipoRepository _equipoRepo = new EquipoRepository();
        private readonly ImpedimentoRepository _impedimentoRepo = new ImpedimentoRepository();

        // ---- Permiso: cualquier miembro del equipo (o quien creó el proyecto).
        //      Reportar y gestionar impedimentos es tarea de todo el equipo, no de un rol. ----
        private Proyecto ObtenerProyectoParaVer(int proyectoId)
        {
            var proyecto = _proyectoRepo.ObtenerPorId(proyectoId);
            if (proyecto == null) return null;

            var usuario = (UsuarioSesion)Session["UsuarioActual"];
            bool esCreador = proyecto.CreadoPor == usuario.Id;
            bool esMiembroDelEquipo = _equipoRepo.ObtenerRolDeUsuarioEnProyecto(proyectoId, usuario.Id) != null;

            return (esCreador || esMiembroDelEquipo) ? proyecto : null;
        }

        // GET /Impedimento/Index?sprintId=X — HU-078 + HU-081.
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

            var impedimentos = _impedimentoRepo.ObtenerPorSprint(sprintId);
            return View(impedimentos);
        }

        // GET /Impedimento/Crear?sprintId=X
        public ActionResult Crear(int sprintId)
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
            ViewBag.Prioridades = PrioridadHelper.ComoLista();

            return View(new ImpedimentoViewModel { SprintId = sprintId, Prioridad = PrioridadHelper.Media });
        }

        // POST /Impedimento/Crear — HU-078.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(ImpedimentoViewModel modelo)
        {
            var sprint = _sprintRepo.ObtenerPorId(modelo.SprintId);
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

            if (!ModelState.IsValid)
            {
                ViewBag.Sprint = sprint;
                ViewBag.NombreProyecto = proyecto.Nombre;
                ViewBag.Prioridades = PrioridadHelper.ComoLista(modelo.Prioridad);
                return View(modelo);
            }

            var usuario = (UsuarioSesion)Session["UsuarioActual"];
            _impedimentoRepo.Crear(modelo, usuario.Id);

            TempData["Mensaje"] = "Impedimento registrado.";
            return RedirectToAction("Index", new { sprintId = modelo.SprintId });
        }

        // POST /Impedimento/Tomar — se autoasigna quien va a gestionarlo.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Tomar(int impedimentoId, int sprintId)
        {
            var sprint = _sprintRepo.ObtenerPorId(sprintId);
            var proyecto = sprint != null ? ObtenerProyectoParaVer(sprint.ProyectoId) : null;

            if (proyecto != null)
            {
                var usuario = (UsuarioSesion)Session["UsuarioActual"];
                _impedimentoRepo.AsignarmeComoResponsable(impedimentoId, usuario.Id);
            }
            else
            {
                TempData["Error"] = "No tenés acceso a ese proyecto.";
            }

            return RedirectToAction("Index", new { sprintId });
        }

        // POST /Impedimento/AvanzarEstado — HU-081: abierto -> en_gestion -> resuelto.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AvanzarEstado(int impedimentoId, int sprintId)
        {
            var sprint = _sprintRepo.ObtenerPorId(sprintId);
            var proyecto = sprint != null ? ObtenerProyectoParaVer(sprint.ProyectoId) : null;

            if (proyecto != null)
            {
                var impedimento = _impedimentoRepo.ObtenerPorId(impedimentoId);
                string siguiente = impedimento != null ? EstadoImpedimentoHelper.SiguienteEstado(impedimento.Estado) : null;

                if (siguiente != null)
                {
                    _impedimentoRepo.AvanzarEstado(impedimentoId, siguiente);
                }
            }
            else
            {
                TempData["Error"] = "No tenés acceso a ese proyecto.";
            }

            return RedirectToAction("Index", new { sprintId });
        }
    }
}
