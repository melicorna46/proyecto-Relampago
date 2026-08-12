using System.Collections.Generic;
using System.Web.Mvc;
using ScrumMvp.Data;
using ScrumMvp.Models;

namespace ScrumMvp.Controllers
{
    public class CuentaController : Controller
    {
        private readonly UsuarioRepository _repo = new UsuarioRepository();

        private static readonly List<string> RolesDisponibles = new List<string>
        {
            "Product Owner",
            "Scrum Master",
            "Developer",
            "Tester / QA",
            "Stakeholder"
        };

        // ============ HU-001: Registro ============

        public ActionResult Registro()
        {
            ViewBag.Roles = new SelectList(RolesDisponibles);
            return View(new RegistroViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(RegistroViewModel modelo)
        {
            ViewBag.Roles = new SelectList(RolesDisponibles, modelo.Especialidad);

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            if (_repo.ExisteEmail(modelo.Email))
            {
                ModelState.AddModelError("Email", "Ya existe una cuenta registrada con ese correo.");
                return View(modelo);
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(modelo.Password);
            _repo.Registrar(modelo.Nombre, modelo.Email, passwordHash, modelo.Especialidad);

            TempData["Mensaje"] = "Cuenta creada correctamente. Ya podés iniciar sesión.";
            return RedirectToAction("Login");
        }

        // ============ HU-002: Login ============

        public ActionResult Login()
        {
            if (Session["UsuarioActual"] != null)
            {
                return RedirectToAction("Index", "Proyecto");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            Usuario usuario = _repo.ObtenerPorEmail(modelo.Email);

            bool credencialesValidas = usuario != null
                && BCrypt.Net.BCrypt.Verify(modelo.Password, usuario.PasswordHash);

            if (!credencialesValidas)
            {
                ModelState.AddModelError("", "Correo o contraseña incorrectos.");
                return View(modelo);
            }

            if (!usuario.Activo)
            {
                ModelState.AddModelError("", "Esta cuenta está desactivada. Contactá al administrador.");
                return View(modelo);
            }

            Session["UsuarioActual"] = new UsuarioSesion
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email
            };

            // Antes redirigía al Backlog; ahora entra primero a Proyectos
            // (tiene que existir un proyecto antes de que el backlog tenga sentido).
            return RedirectToAction("Index", "Proyecto");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }
    }
}
