using System.Web.Mvc;
using ScrumMvp.Data;
using ScrumMvp.Filters;
using ScrumMvp.Helpers;
using ScrumMvp.Models;

namespace ScrumMvp.Controllers
{
    [RequiereSesion]
    public class EquipoController : Controller
    {
        private readonly ProyectoRepository _proyectoRepo = new ProyectoRepository();
        private readonly EquipoRepository _equipoRepo = new EquipoRepository();
        private readonly UsuarioRepository _usuarioRepo = new UsuarioRepository();

        // ============ HU-007: Crear equipo Scrum ============

        // GET /Equipo/Crear?proyectoId=X
        public ActionResult Crear(int proyectoId)
        {
            var proyecto = _proyectoRepo.ObtenerPorId(proyectoId);

            // El proyecto no existe: no hay nada que hacer.
            if (proyecto == null)
            {
                TempData["Error"] = "El proyecto no existe.";
                return RedirectToAction("Index", "Proyecto");
            }

            // Regla de permisos: solo quien creó el proyecto arma su equipo.
            var usuario = (UsuarioSesion)Session["UsuarioActual"];
            if (proyecto.CreadoPor != usuario.Id)
            {
                TempData["Error"] = "No tenés permiso para administrar este proyecto.";
                return RedirectToAction("Index", "Proyecto");
            }

            // Regla: un proyecto tiene un solo equipo. Si ya existe, no se crea otro.
            var equipoExistente = _equipoRepo.ObtenerPorProyecto(proyectoId);
            if (equipoExistente != null)
            {
                return RedirectToAction("Index", new { equipoId = equipoExistente.Id });
            }

            ViewBag.NombreProyecto = proyecto.Nombre;
            ViewBag.Roles = RolesEquipo.ComoLista();

            return View(new CrearEquipoViewModel { ProyectoId = proyectoId });
        }

        // POST /Equipo/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(CrearEquipoViewModel modelo)
        {
            var proyecto = _proyectoRepo.ObtenerPorId(modelo.ProyectoId);
            var usuario = (UsuarioSesion)Session["UsuarioActual"];

            if (proyecto == null || proyecto.CreadoPor != usuario.Id)
            {
                TempData["Error"] = "No tenés permiso para administrar este proyecto.";
                return RedirectToAction("Index", "Proyecto");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.NombreProyecto = proyecto.Nombre;
                ViewBag.Roles = RolesEquipo.ComoLista(modelo.RolCreador);
                return View(modelo);
            }

            // Doble chequeo por si alguien manda el POST directo sin pasar por el GET.
            var equipoExistente = _equipoRepo.ObtenerPorProyecto(modelo.ProyectoId);
            if (equipoExistente != null)
            {
                return RedirectToAction("Index", new { equipoId = equipoExistente.Id });
            }

            int equipoId = _equipoRepo.CrearConPrimerMiembro(
                modelo.ProyectoId, modelo.Nombre, usuario.Id, modelo.RolCreador);

            TempData["Mensaje"] = "Equipo creado. Ahora podés invitar al resto de tus compañeros.";
            return RedirectToAction("Index", new { equipoId });
        }

        // ============ HU-010 (de paso): consultar equipo, y HU-009: agregar miembros ============

        // GET /Equipo/Index?equipoId=X
        public ActionResult Index(int equipoId)
        {
            var equipo = _equipoRepo.ObtenerPorId(equipoId);
            if (equipo == null)
            {
                TempData["Error"] = "El equipo no existe.";
                return RedirectToAction("Index", "Proyecto");
            }

            var proyecto = _proyectoRepo.ObtenerPorId(equipo.ProyectoId);
            var miembros = _equipoRepo.ObtenerMiembros(equipoId);

            ViewBag.Equipo = equipo;
            ViewBag.NombreProyecto = proyecto != null ? proyecto.Nombre : "";
            ViewBag.Roles = RolesEquipo.ComoLista();

            return View(miembros);
        }

        // POST /Equipo/AgregarMiembro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarMiembro(AgregarMiembroViewModel modelo)
        {
            var equipo = _equipoRepo.ObtenerPorId(modelo.EquipoId);
            if (equipo == null)
            {
                TempData["Error"] = "El equipo no existe.";
                return RedirectToAction("Index", "Proyecto");
            }

            var proyecto = _proyectoRepo.ObtenerPorId(equipo.ProyectoId);
            var usuarioSesion = (UsuarioSesion)Session["UsuarioActual"];

            // Mismo control de permisos que al crear el equipo.
            if (proyecto == null || proyecto.CreadoPor != usuarioSesion.Id)
            {
                TempData["Error"] = "No tenés permiso para administrar este equipo.";
                return RedirectToAction("Index", "Proyecto");
            }

            // Función local: vuelve a armar la pantalla del equipo mostrando el error.
            ActionResult VolverConError(string mensaje)
            {
                ModelState.AddModelError("", mensaje);
                ViewBag.Equipo = equipo;
                ViewBag.NombreProyecto = proyecto.Nombre;
                ViewBag.Roles = RolesEquipo.ComoLista();
                ViewBag.ModeloAgregar = modelo;
                return View("Index", _equipoRepo.ObtenerMiembros(modelo.EquipoId));
            }

            if (!ModelState.IsValid)
            {
                return VolverConError("Revisá los datos del formulario.");
            }

            // 1. El correo tiene que pertenecer a alguien ya registrado.
            var usuarioInvitado = _usuarioRepo.ObtenerPorEmail(modelo.Email);
            if (usuarioInvitado == null)
            {
                return VolverConError("Ese correo no está registrado. Pedile que cree su cuenta primero.");
            }

            // 2. No se puede agregar dos veces a la misma persona.
            if (_equipoRepo.EsMiembro(modelo.EquipoId, usuarioInvitado.Id))
            {
                return VolverConError("Esa persona ya forma parte del equipo.");
            }

            // 3. Product Owner y Scrum Master son roles únicos en el equipo.
            bool esRolUnico = modelo.Rol == RolesEquipo.ProductOwner || modelo.Rol == RolesEquipo.ScrumMaster;
            if (esRolUnico && _equipoRepo.YaExisteRolEnEquipo(modelo.EquipoId, modelo.Rol))
            {
                return VolverConError($"Ya hay un {RolesEquipo.Etiqueta(modelo.Rol)} asignado en este equipo.");
            }

            // Todo válido: se agrega.
            _equipoRepo.AgregarMiembro(modelo.EquipoId, usuarioInvitado.Id, modelo.Rol);

            TempData["Mensaje"] = $"{usuarioInvitado.Nombre} se sumó al equipo como {RolesEquipo.Etiqueta(modelo.Rol)}.";
            return RedirectToAction("Index", new { equipoId = modelo.EquipoId });
        }
    }
}
