using System.Web.Mvc;
using ScrumMvp.Data;
using ScrumMvp.Filters;
using ScrumMvp.Models;

namespace ScrumMvp.Controllers
{
    [RequiereSesion]
    public class DailyController : Controller
    {
        private readonly ProyectoRepository _proyectoRepo = new ProyectoRepository();
        private readonly SprintRepository _sprintRepo = new SprintRepository();
        private readonly EquipoRepository _equipoRepo = new EquipoRepository();
        private readonly DailyRepository _dailyRepo = new DailyRepository();

        // ---- Permiso: cualquier miembro del equipo (o quien creó el proyecto).
        //      El Daily es de todo el equipo, no de un rol en particular. ----
        private Proyecto ObtenerProyectoParaVer(int proyectoId)
        {
            var proyecto = _proyectoRepo.ObtenerPorId(proyectoId);
            if (proyecto == null) return null;

            var usuario = (UsuarioSesion)Session["UsuarioActual"];
            bool esCreador = proyecto.CreadoPor == usuario.Id;
            bool esMiembroDelEquipo = _equipoRepo.ObtenerRolDeUsuarioEnProyecto(proyectoId, usuario.Id) != null;

            return (esCreador || esMiembroDelEquipo) ? proyecto : null;
        }

        // GET /Daily/Index?sprintId=X — HU-074: Daily de hoy de todo el equipo.
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

            var usuario = (UsuarioSesion)Session["UsuarioActual"];

            ViewBag.Sprint = sprint;
            ViewBag.NombreProyecto = proyecto.Nombre;
            ViewBag.MiDailyDeHoy = _dailyRepo.ObtenerDeHoy(sprintId, usuario.Id);

            var dailiesDeHoy = _dailyRepo.ObtenerDeHoyPorSprint(sprintId);
            return View(dailiesDeHoy);
        }

        // GET /Daily/Registrar?sprintId=X
        public ActionResult Registrar(int sprintId)
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

            var usuario = (UsuarioSesion)Session["UsuarioActual"];
            var existente = _dailyRepo.ObtenerDeHoy(sprintId, usuario.Id);

            ViewBag.Sprint = sprint;
            ViewBag.NombreProyecto = proyecto.Nombre;

            var modelo = existente == null
                ? new DailyViewModel { SprintId = sprintId }
                : new DailyViewModel
                {
                    SprintId = sprintId,
                    QueAvance = existente.QueAvance,
                    QueHare = existente.QueHare,
                    TieneImpedimento = existente.TieneImpedimento,
                    ImpedimentoTexto = existente.ImpedimentoTexto
                };

            return View(modelo);
        }

        // POST /Daily/Registrar — un Daily por persona por día: si ya existe, lo actualiza.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registrar(DailyViewModel modelo)
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

            if (modelo.TieneImpedimento && string.IsNullOrWhiteSpace(modelo.ImpedimentoTexto))
            {
                ModelState.AddModelError("ImpedimentoTexto", "Contá cuál es el impedimento.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Sprint = sprint;
                ViewBag.NombreProyecto = proyecto.Nombre;
                return View(modelo);
            }

            var usuario = (UsuarioSesion)Session["UsuarioActual"];
            var existente = _dailyRepo.ObtenerDeHoy(modelo.SprintId, usuario.Id);

            if (existente == null)
            {
                _dailyRepo.Crear(modelo, usuario.Id);
            }
            else
            {
                _dailyRepo.Actualizar(existente.Id, modelo);
            }

            TempData["Mensaje"] = "Daily registrado.";
            return RedirectToAction("Index", new { sprintId = modelo.SprintId });
        }
    }
}
