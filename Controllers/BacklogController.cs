using System.Web.Mvc;
using ScrumMvp.Data;
using ScrumMvp.Filters;
using ScrumMvp.Helpers;
using ScrumMvp.Models;

namespace ScrumMvp.Controllers
{
    [RequiereSesion]
    public class BacklogController : Controller
    {
        private readonly ProyectoRepository _proyectoRepo = new ProyectoRepository();
        private readonly HistoriaRepository _historiaRepo = new HistoriaRepository();
        private readonly EquipoRepository _equipoRepo = new EquipoRepository();

        // ---- Permiso para VER el backlog: quien creó el proyecto, o cualquier
        //      miembro del equipo (todos deberían poder consultarlo, no solo el PO). ----
        private Proyecto ObtenerProyectoParaVer(int proyectoId)
        {
            var proyecto = _proyectoRepo.ObtenerPorId(proyectoId);
            if (proyecto == null) return null;

            var usuario = (UsuarioSesion)Session["UsuarioActual"];
            bool esCreador = proyecto.CreadoPor == usuario.Id;
            bool esMiembroDelEquipo = _equipoRepo.ObtenerRolDeUsuarioEnProyecto(proyectoId, usuario.Id) != null;

            return (esCreador || esMiembroDelEquipo) ? proyecto : null;
        }

        // ---- Permiso para EDITAR el backlog: acá sí se respeta literalmente la HU
        //      ("Como Product Owner, quiero..."). Solo alguien con el rol Product Owner
        //      asignado en el equipo de este proyecto puede crear/editar/reordenar.
        //      Si el proyecto todavía no tiene equipo, tampoco hay PO: se bloquea. ----
        private Proyecto ObtenerProyectoParaEditarBacklog(int proyectoId, out string mensajeError)
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

            if (rol != RolesEquipo.ProductOwner)
            {
                mensajeError = "Solo el Product Owner del equipo puede modificar el Product Backlog.";
                return null;
            }

            return proyecto;
        }

        // GET /Backlog/Index?proyectoId=X
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

            var historias = _historiaRepo.ObtenerBacklogPorProyecto(proyectoId);

            ViewBag.ProyectoId = proyectoId;
            ViewBag.NombreProyecto = proyecto.Nombre;
            // La vista usa esto para mostrar u ocultar los botones de editar/reordenar/crear:
            // solo tienen sentido si quien mira es el Product Owner.
            ViewBag.EsProductOwner = miRol == RolesEquipo.ProductOwner;

            return View(historias);
        }

        // GET /Backlog/Crear?proyectoId=X
        public ActionResult Crear(int proyectoId)
        {
            string error;
            var proyecto = ObtenerProyectoParaEditarBacklog(proyectoId, out error);
            if (proyecto == null)
            {
                TempData["Error"] = error;
                return RedirectToAction("Index", new { proyectoId });
            }

            ViewBag.NombreProyecto = proyecto.Nombre;
            ViewBag.Prioridades = PrioridadHelper.ComoLista();
            ViewBag.StoryPointsOpciones = StoryPointsHelper.ComoLista();

            return View(new HistoriaViewModel { ProyectoId = proyectoId, Prioridad = PrioridadHelper.Media });
        }

        // POST /Backlog/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(HistoriaViewModel modelo)
        {
            string error;
            var proyecto = ObtenerProyectoParaEditarBacklog(modelo.ProyectoId, out error);
            if (proyecto == null)
            {
                TempData["Error"] = error;
                return RedirectToAction("Index", new { proyectoId = modelo.ProyectoId });
            }

            if (!ModelState.IsValid)
            {
                ViewBag.NombreProyecto = proyecto.Nombre;
                ViewBag.Prioridades = PrioridadHelper.ComoLista(modelo.Prioridad);
                ViewBag.StoryPointsOpciones = StoryPointsHelper.ComoLista(modelo.StoryPoints);
                return View(modelo);
            }

            _historiaRepo.Crear(modelo);

            TempData["Mensaje"] = "Historia agregada al Product Backlog.";
            return RedirectToAction("Index", new { proyectoId = modelo.ProyectoId });
        }

        // GET /Backlog/Editar?id=X
        public ActionResult Editar(int id)
        {
            var historia = _historiaRepo.ObtenerPorId(id);
            if (historia == null)
            {
                TempData["Error"] = "La historia no existe.";
                return RedirectToAction("Index", "Proyecto");
            }

            string error;
            var proyecto = ObtenerProyectoParaEditarBacklog(historia.ProyectoId, out error);
            if (proyecto == null)
            {
                TempData["Error"] = error;
                return RedirectToAction("Index", new { proyectoId = historia.ProyectoId });
            }

            var modelo = new HistoriaViewModel
            {
                Id = historia.Id,
                ProyectoId = historia.ProyectoId,
                Titulo = historia.Titulo,
                RolComo = historia.RolComo,
                NecesidadQuiero = historia.NecesidadQuiero,
                PropositoPara = historia.PropositoPara,
                CriteriosAceptacion = historia.CriteriosAceptacion,
                Prioridad = historia.Prioridad,
                StoryPoints = historia.StoryPoints
            };

            ViewBag.NombreProyecto = proyecto.Nombre;
            ViewBag.Codigo = historia.Codigo;
            ViewBag.Prioridades = PrioridadHelper.ComoLista(historia.Prioridad);
            ViewBag.StoryPointsOpciones = StoryPointsHelper.ComoLista(historia.StoryPoints);

            return View(modelo);
        }

        // POST /Backlog/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(HistoriaViewModel modelo)
        {
            string error;
            var proyecto = ObtenerProyectoParaEditarBacklog(modelo.ProyectoId, out error);
            if (proyecto == null)
            {
                TempData["Error"] = error;
                return RedirectToAction("Index", new { proyectoId = modelo.ProyectoId });
            }

            if (!ModelState.IsValid)
            {
                var historiaOriginal = _historiaRepo.ObtenerPorId(modelo.Id);
                ViewBag.NombreProyecto = proyecto.Nombre;
                ViewBag.Codigo = historiaOriginal != null ? historiaOriginal.Codigo : "";
                ViewBag.Prioridades = PrioridadHelper.ComoLista(modelo.Prioridad);
                ViewBag.StoryPointsOpciones = StoryPointsHelper.ComoLista(modelo.StoryPoints);
                return View(modelo);
            }

            _historiaRepo.Actualizar(modelo);

            TempData["Mensaje"] = "Historia actualizada.";
            return RedirectToAction("Index", new { proyectoId = modelo.ProyectoId });
        }

        // POST /Backlog/MoverArriba
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoverArriba(int id, int proyectoId)
        {
            string error;
            var proyecto = ObtenerProyectoParaEditarBacklog(proyectoId, out error);
            if (proyecto != null)
            {
                _historiaRepo.MoverArriba(id, proyectoId);
            }
            else
            {
                TempData["Error"] = error;
            }
            return RedirectToAction("Index", new { proyectoId });
        }

        // POST /Backlog/MoverAbajo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoverAbajo(int id, int proyectoId)
        {
            string error;
            var proyecto = ObtenerProyectoParaEditarBacklog(proyectoId, out error);
            if (proyecto != null)
            {
                _historiaRepo.MoverAbajo(id, proyectoId);
            }
            else
            {
                TempData["Error"] = error;
            }
            return RedirectToAction("Index", new { proyectoId });
        }
    }
}
