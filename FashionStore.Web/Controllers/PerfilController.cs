using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FashionStore.Web.Controllers
{
    [Authorize]
    public class PerfilController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;

        public PerfilController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        // GET: Perfil/Index
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "Identity" });

            var roles = await _userManager.GetRolesAsync(user);
            var rolActual = roles.Contains("Administrador") ? "Administrador" : "Vendedor";

            ViewBag.Email = user.Email;
            ViewBag.UserName = user.UserName;
            ViewBag.Rol = rolActual;
            ViewBag.EsAdministrador = rolActual == "Administrador";

            // Si es vendedor, cargar datos del vendedor
            Vendedor? vendedor = null;
            if (rolActual == "Vendedor")
            {
                var todos = await _unitOfWork.Vendedores.FindAsync(v =>
                    v.Correo == user.Email || v.Correo == user.UserName);
                vendedor = todos.FirstOrDefault();
            }

            ViewBag.Vendedor = vendedor;

            return View();
        }

        // POST: Perfil/CambiarPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPassword(
            string PasswordActual, string PasswordNuevo, string PasswordConfirmar)
        {
            if (PasswordNuevo != PasswordConfirmar)
            {
                TempData["Error"] = "La nueva contraseña y la confirmación no coinciden.";
                return RedirectToAction(nameof(Index));
            }

            if (PasswordNuevo.Length < 6)
            {
                TempData["Error"] = "La contraseña debe tener al menos 6 caracteres.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var result = await _userManager.ChangePasswordAsync(user, PasswordActual, PasswordNuevo);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Contraseña actualizada correctamente.";
            }
            else
            {
                TempData["Error"] = string.Join(" ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
