using System.Collections.Generic;
using System.Web.Mvc;
using ScrumMvp.Data;
using ScrumMvp.Filters;
using ScrumMvp.Models;

namespace ScrumMvp.Controllers
{
    [RequiereSesion]
    public class ProyectoController : Controller
    {
        private readonly ProyectoRepository _repo = new ProyectoRepository();
        private readonly EquipoRepository _equipoRepo = new EquipoRepository();

        // GET /Proyecto  ->  lista los proyectos del usuario logueado,
        // con el Id de su equipo si ya tiene uno (para armar el botón correcto).
        public ActionResult Index()
        {
            var usuario = (UsuarioSesion)Session["UsuarioActual"];
            var proyectos = _repo.ObtenerPorCreador(usuario.Id);

            var lista = new List<ProyectoConEquipoViewModel>();
            foreach (var p in proyectos)
            {
                var equipo = _equipoRepo.ObtenerPorProyecto(p.Id);
                lista.Add(new ProyectoConEquipoViewModel
                {
                    Proyecto = p,
                    EquipoId = equipo != null ? (int?)equipo.Id : null
                });
            }

            return View(lista);
        }

        public ActionResult Crear()
        {
            return View(new ProyectoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(ProyectoViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = (UsuarioSesion)Session["UsuarioActual"];
            int nuevoId = _repo.Crear(modelo.Nombre, modelo.ProductGoal, modelo.Vision, modelo.Descripcion, usuario.Id);

            TempData["Mensaje"] = "Proyecto creado correctamente. Ahora armá tu equipo Scrum.";
            return RedirectToAction("Index");
        }
    }
}
