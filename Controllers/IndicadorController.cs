using System.Web.Mvc;
using ScrumMvp.Data;
using ScrumMvp.Filters;
using ScrumMvp.Models;

namespace ScrumMvp.Controllers
{
    [RequiereSesion]
    public class IndicadorController : Controller
    {
        private readonly ProyectoRepository _proyectoRepo = new ProyectoRepository();
        private readonly SprintRepository _sprintRepo = new SprintRepository();
        private readonly EquipoRepository _equipoRepo = new EquipoRepository();
        private readonly IndicadorRepository _indicadorRepo = new IndicadorRepository();

        // ---- Permiso: cualquier miembro del equipo. ----
        private Proyecto ObtenerProyectoParaVer(int proyectoId)
        {
            var proyecto = _proyectoRepo.ObtenerPorId(proyectoId);
            if (proyecto == null) return null;

            var usuario = (UsuarioSesion)Session["UsuarioActual"];
            bool esCreador = proyecto.CreadoPor == usuario.Id;
            bool esMiembroDelEquipo = _equipoRepo.ObtenerRolDeUsuarioEnProyecto(proyectoId, usuario.Id) != null;

            return (esCreador || esMiembroDelEquipo) ? proyecto : null;
        }

        // GET /Indicador/Index?sprintId=X — HU-112 (Burndown) + HU-116 (% de cumplimiento).
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

            // Registra el punto de hoy con los datos reales de historia/tarea antes de graficar.
            _indicadorRepo.RegistrarSnapshotDeHoy(sprintId);

            ViewBag.Sprint = sprint;
            ViewBag.NombreProyecto = proyecto.Nombre;
            ViewBag.Cumplimiento = _indicadorRepo.ObtenerCumplimiento(sprintId);

            var serie = _indicadorRepo.ObtenerSerie(sprintId);
            return View(serie);
        }
    }
}
