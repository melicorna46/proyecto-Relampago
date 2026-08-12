using System.Linq;
using System.Web.Mvc;
using ScrumMvp.Data;
using ScrumMvp.Filters;
using ScrumMvp.Helpers;
using ScrumMvp.Models;

namespace ScrumMvp.Controllers
{
    [RequiereSesion]
    public class ReviewController : Controller
    {
        private readonly ProyectoRepository _proyectoRepo = new ProyectoRepository();
        private readonly SprintRepository _sprintRepo = new SprintRepository();
        private readonly EquipoRepository _equipoRepo = new EquipoRepository();
        private readonly HistoriaRepository _historiaRepo = new HistoriaRepository();
        private readonly ReviewRepository _reviewRepo = new ReviewRepository();

        // ---- Permiso: cualquier miembro del equipo. La Review es un evento de todo
        //      el equipo más los stakeholders, no de un único rol. ----
        private Proyecto ObtenerProyectoParaVer(int proyectoId)
        {
            var proyecto = _proyectoRepo.ObtenerPorId(proyectoId);
            if (proyecto == null) return null;

            var usuario = (UsuarioSesion)Session["UsuarioActual"];
            bool esCreador = proyecto.CreadoPor == usuario.Id;
            bool esMiembroDelEquipo = _equipoRepo.ObtenerRolDeUsuarioEnProyecto(proyectoId, usuario.Id) != null;

            return (esCreador || esMiembroDelEquipo) ? proyecto : null;
        }

        // GET /Review/Index?sprintId=X — HU-098: muestra el incremento (historias
        // terminadas/aceptadas del Sprint).
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

            var review = _reviewRepo.ObtenerPorSprint(sprintId);
            if (review == null)
            {
                _reviewRepo.Crear(sprintId);
                review = _reviewRepo.ObtenerPorSprint(sprintId);
            }

            var incremento = _historiaRepo.ObtenerPorSprint(sprintId)
                .Where(h => h.Estado == EstadoHistoriaHelper.Terminada || h.Estado == EstadoHistoriaHelper.Aceptada)
                .ToList();

            ViewBag.Sprint = sprint;
            ViewBag.NombreProyecto = proyecto.Nombre;
            ViewBag.Review = review;
            ViewBag.Feedback = _reviewRepo.ObtenerFeedback(review.Id);

            return View(incremento);
        }

        // POST /Review/GuardarResultado — HU-098.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GuardarResultado(int reviewId, string resultado, int sprintId)
        {
            var sprint = _sprintRepo.ObtenerPorId(sprintId);
            var proyecto = sprint != null ? ObtenerProyectoParaVer(sprint.ProyectoId) : null;

            if (proyecto != null)
            {
                _reviewRepo.ActualizarResultado(reviewId, resultado);
                TempData["Mensaje"] = "Resultado de la Review guardado.";
            }
            else
            {
                TempData["Error"] = "No tenés acceso a ese proyecto.";
            }

            return RedirectToAction("Index", new { sprintId });
        }

        // POST /Review/AgregarFeedback — HU-100.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarFeedback(int reviewId, string comentario, int sprintId)
        {
            var sprint = _sprintRepo.ObtenerPorId(sprintId);
            var proyecto = sprint != null ? ObtenerProyectoParaVer(sprint.ProyectoId) : null;

            if (proyecto != null && !string.IsNullOrWhiteSpace(comentario))
            {
                var usuario = (UsuarioSesion)Session["UsuarioActual"];
                _reviewRepo.AgregarFeedback(reviewId, usuario.Nombre, comentario);
                TempData["Mensaje"] = "Feedback registrado.";
            }
            else if (proyecto == null)
            {
                TempData["Error"] = "No tenés acceso a ese proyecto.";
            }

            return RedirectToAction("Index", new { sprintId });
        }

        // POST /Review/Aceptar — HU-099: valida (acepta) una historia terminada desde la Review.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Aceptar(int historiaId, int sprintId)
        {
            var sprint = _sprintRepo.ObtenerPorId(sprintId);
            var proyecto = sprint != null ? ObtenerProyectoParaVer(sprint.ProyectoId) : null;

            if (proyecto != null)
            {
                var historia = _historiaRepo.ObtenerPorId(historiaId);
                if (historia != null && historia.Estado == EstadoHistoriaHelper.Terminada)
                {
                    _historiaRepo.CambiarEstado(historiaId, EstadoHistoriaHelper.Aceptada);
                    TempData["Mensaje"] = "Historia aceptada.";
                }
                else
                {
                    TempData["Error"] = "Solo se pueden aceptar historias que ya están Terminadas.";
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
