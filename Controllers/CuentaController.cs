using System.Web.Mvc;
using BCrypt.Net;
using ScrumMvp.Data;
using ScrumMvp.Models;

namespace ScrumMvp.Controllers
{
    public class CuentaController : Controller
    {
        private readonly UsuarioRepository _repo = new UsuarioRepository();

        // GET /Cuenta/Registro  ->  muestra el formulario vacío
        public ActionResult Registro()
        {
            return View(new RegistroViewModel());
        }

        // POST /Cuenta/Registro  ->  valida, encripta la contraseña y guarda
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(RegistroViewModel modelo)
        {
            // 1. Validaciones del formulario (las de RegistroViewModel: requeridos, formato, largo)
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            // 2. Regla de negocio: el correo debe ser único
            if (_repo.ExisteEmail(modelo.Email))
            {
                ModelState.AddModelError("Email", "Ya existe una cuenta registrada con ese correo.");
                return View(modelo);
            }

            // 3. Nunca se guarda la contraseña en texto plano: se guarda su hash.
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(modelo.Password);

            // 4. Guardar en la base
            _repo.Registrar(modelo.Nombre, modelo.Email, passwordHash, modelo.Especialidad);

            // 5. Confirmación al usuario y redirección al login
            TempData["Mensaje"] = "Cuenta creada correctamente. Ya podés iniciar sesión.";
            return RedirectToAction("Login");
        }

        // Placeholder: se completa en HU-002 (próximo paso)
        public ActionResult Login()
        {
            return View();
        }
    }
}
