using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ScrumMvp.Data;
using ScrumMvp.Filters;
using ScrumMvp.Helpers;
using ScrumMvp.Models;

namespace ScrumMvp.Controllers
{
    [RequiereSesion]
    public class SprintController : Controller
    {
        private readonly ProyectoRepository _proyectoRepo = new ProyectoRepository();
        private readonly SprintRepository _sprintRepo = new SprintRepository();
        private readonly HistoriaRepository _historiaRepo = new HistoriaRepository();
        private readonly EquipoRepository _equipoRepo = new EquipoRepository();

        // ---- Permiso para VER: cualquier miembro del equipo (o quien creó el proyecto). ----
        private Proyecto ObtenerProyectoParaVer(int proyectoId)
        {
            var proyecto = _proyectoRepo.ObtenerPorId(proyectoId);
            if (proyecto == null) return null;

            var usuario = (UsuarioSesion)Session["UsuarioActual"];
            bool esCreador = proyecto.CreadoPor == usuario.Id;
            bool esMiembroDelEquipo = _equipoRepo.ObtenerRolDeUsuarioEnProyecto(proyectoId, usuario.Id) != null;

            return (esCreador || esMiembroDelEquipo) ? proyecto : null;
        }

        // ---- Permiso para CREAR/CERRAR Sprint: solo el Scrum Master del equipo
        //      (HU-049 y HU-054 dicen literalmente "Como Scrum Master, quiero..."). ----
        private Proyecto ObtenerProyectoParaGestionarSprint(int proyectoId, out string mensajeError)
        {
            mensajeError = null;
            var proyecto = _proyectoRepo.ObtenerPorId(proyectoId);
            if (proyecto == null)
            {
                mensajeError = "El proyecto no existe.";
                return null;
            }

            var usuario = (UsuarioSesion)Session["UsuarioActual"];
            string rol = _equipoRepo.ObtenerRolDeUsuarioEnProyecto(proyectoId, usuario.Id);

            if (rol == null)
            {
                mensajeError = "Primero armá el equipo de este proyecto y asignate un rol.";
                return null;
            }

            if (rol != RolesEquipo.ScrumMaster)
            {
                mensajeError = "Solo el Scrum Master del equipo puede gestionar el Sprint.";
                return null;
            }

            return proyecto;
        }

        // GET /Sprint/Index?proyectoId=X
        public ActionResult Index(int proyectoId)
        {
            var proyecto = ObtenerProyectoParaVer(proyectoId);
            if (proyecto == null)
            {
                TempData["Error"] = "No tenés acceso a ese proyecto.";
                return RedirectToAction("Index", "Proyecto");
            }

            var usuario = (UsuarioSesion)Session["UsuarioActual"];
            string miRol = _equipoRepo.ObtenerRolDeUsuarioEnProyecto(proyectoId, usuario.Id);

            ViewBag.ProyectoId = proyectoId;
            ViewBag.NombreProyecto = proyecto.Nombre;
            ViewBag.EsScrumMaster = miRol == RolesEquipo.ScrumMaster;

            var sprint = _sprintRepo.ObtenerVigentePorProyecto(proyectoId);
            if (sprint == null)
            {
                return View("SinSprint");
            }

            ViewBag.Sprint = sprint;
            var historiasDelSprint = _historiaRepo.ObtenerPorSprint(sprint.Id);

            return View(historiasDelSprint);
        }

        // GET /Sprint/Crear?proyectoId=X
        public ActionResult Crear(int proyectoId)
        {
            string error;
            var proyecto = ObtenerProyectoParaGestionarSprint(proyectoId, out error);
            if (proyecto == null)
            {
                TempData["Error"] = error;
                return RedirectToAction("Index", new { proyectoId });
            }

            // Regla: un solo Sprint vigente (no cerrado) por proyecto a la vez.
            var sprintVigente = _sprintRepo.ObtenerVigentePorProyecto(proyectoId);
            if (sprintVigente != null)
            {
                TempData["Error"] = "Ya hay un Sprint en curso. Cerralo antes de crear uno nuevo.";
                return RedirectToAction("Index", new { proyectoId });
            }

            ViewBag.NombreProyecto = proyecto.Nombre;
            return View(new SprintViewModel
            {
                ProyectoId = proyectoId,
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today.AddDays(10)
            });
        }

        // POST /Sprint/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(SprintViewModel modelo)
        {
            string error;
            var proyecto = ObtenerProyectoParaGestionarSprint(modelo.ProyectoId, out error);
            if (proyecto == null)
            {
                TempData["Error"] = error;
                return RedirectToAction("Index", new { proyectoId = modelo.ProyectoId });
            }

            if (modelo.FechaFin < modelo.FechaInicio)
            {
                ModelState.AddModelError("", "La fecha de fin no puede ser anterior a la de inicio.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.NombreProyecto = proyecto.Nombre;
                return View(modelo);
            }

            int sprintId = _sprintRepo.Crear(modelo);

            TempData["Mensaje"] = "Sprint creado. Ahora seleccioná las historias del Product Backlog para armar el Sprint Backlog.";
            return RedirectToAction("Planificar", new { sprintId });
        }

        // GET /Sprint/Planificar?sprintId=X — HU-058: elegir historias del Product Backlog.
        public ActionResult Planificar(int sprintId)
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

            var backlogDisponible = _historiaRepo.ObtenerBacklogPorProyecto(sprint.ProyectoId);

            ViewBag.Sprint = sprint;
            ViewBag.NombreProyecto = proyecto.Nombre;

            return View(backlogDisponible);
        }

        // POST /Sprint/Planificar — HU-058 + HU-061: asigna las historias marcadas al Sprint Backlog.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Planificar(int sprintId, List<int> historiaIds)
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

            if (historiaIds != null && historiaIds.Any())
            {
                _historiaRepo.AsignarASprint(historiaIds, sprintId);
                TempData["Mensaje"] = "Sprint Backlog actualizado.";
            }
            else
            {
                TempData["Error"] = "No seleccionaste ninguna historia.";
            }

            return RedirectToAction("Index", new { proyectoId = sprint.ProyectoId });
        }

        // POST /Sprint/Cerrar — HU-054.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cerrar(int sprintId, int proyectoId)
        {
            string error;
            var proyecto = ObtenerProyectoParaGestionarSprint(proyectoId, out error);
            if (proyecto != null)
            {
                _sprintRepo.Cerrar(sprintId);
                TempData["Mensaje"] = "Sprint cerrado.";
            }
            else
            {
                TempData["Error"] = error;
            }
            return RedirectToAction("Index", new { proyectoId });
        }
    }
}
