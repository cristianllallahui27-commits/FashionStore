using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;
using FashionStore.Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class VendedoresController : Controller
    {
        private readonly FashionStoreDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public VendedoresController(
            FashionStoreDbContext context,
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Vendedores
        public async Task<IActionResult> Index()
        {
            var vendedores = await _unitOfWork.Vendedores.GetAllAsync();
            return View(vendedores.OrderBy(v => v.Nombres).ToList());
        }

        // GET: Vendedores/Create
        public IActionResult Create()
        {
            return View();
        }

        // GET: Vendedores/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var vendedor = await _unitOfWork.Vendedores.GetByIdAsync(id);
            if (vendedor == null) return NotFound();
            return View(vendedor);
        }

        // POST: Vendedores/Create — crea ApplicationUser + Vendedor + asigna rol Vendedor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string Nombres, string Apellidos, string DNI,
            string Email, string Password, string? Telefono, string? Correo)
        {
            if (string.IsNullOrWhiteSpace(Nombres) || string.IsNullOrWhiteSpace(Apellidos)
                || string.IsNullOrWhiteSpace(DNI) || string.IsNullOrWhiteSpace(Email)
                || string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError("", "Todos los campos obligatorios deben completarse.");
                return View();
            }

            // Verificar duplicado de DNI
            var existeDNI = await _unitOfWork.Vendedores.FindAsync(v => v.DNI == DNI);
            if (existeDNI.Any())
            {
                ModelState.AddModelError("DNI", "Ya existe un vendedor con ese DNI.");
                return View();
            }

            // Crear ApplicationUser
            var user = new ApplicationUser
            {
                UserName = Email,
                Email = Email,
                EmailConfirmed = true,
                Activo = true
            };

            var resultUser = await _userManager.CreateAsync(user, Password);
            if (!resultUser.Succeeded)
            {
                foreach (var err in resultUser.Errors)
                    ModelState.AddModelError("", err.Description);
                return View();
            }

            // Asignar rol Vendedor
            if (!await _roleManager.RoleExistsAsync("Vendedor"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Vendedor"));
            }
            await _userManager.AddToRoleAsync(user, "Vendedor");

            // Crear entidad Vendedor
            var vendedor = new Vendedor
            {
                Nombres = Nombres,
                Apellidos = Apellidos,
                DNI = DNI,
                Telefono = Telefono,
                Correo = Correo ?? Email,
                // UsuarioId será seteado cuando sea necesario cambiar contraseña
                Estado = true,
                UltimaPasswordAdmin = Password  // guardamos para visibilidad del Admin
            };

            await _unitOfWork.Vendedores.AddAsync(vendedor);
            await _unitOfWork.CommitAsync();

            TempData["Success"] = $"Vendedor '{Nombres} {Apellidos}' creado correctamente. Puede iniciar sesión con: {Email}";
            return RedirectToAction(nameof(Index));
        }

        // GET: Vendedores/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var vendedor = await _unitOfWork.Vendedores.GetByIdAsync(id);
            if (vendedor == null) return NotFound();
            return View(vendedor);
        }

        // POST: Vendedores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vendedor vendedor)
        {
            if (id != vendedor.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(vendedor);

            // Verificar duplicado DNI (excluyendo el actual)
            var existeDNI = await _unitOfWork.Vendedores.FindAsync(v => v.DNI == vendedor.DNI && v.Id != id);
            if (existeDNI.Any())
            {
                ModelState.AddModelError("DNI", "Ya existe otro vendedor con ese DNI.");
                return View(vendedor);
            }

            _unitOfWork.Vendedores.Update(vendedor);
            await _unitOfWork.CommitAsync();

            TempData["Success"] = $"Vendedor '{vendedor.Nombres} {vendedor.Apellidos}' actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Vendedores/ToggleEstado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            var vendedor = await _unitOfWork.Vendedores.GetByIdAsync(id);
            if (vendedor == null) return NotFound();

            vendedor.Estado = !vendedor.Estado;
            _unitOfWork.Vendedores.Update(vendedor);
            await _unitOfWork.CommitAsync();

            if (!string.IsNullOrWhiteSpace(vendedor.Correo))
            {
                var user = await _userManager.FindByEmailAsync(vendedor.Correo)
                    ?? await _userManager.FindByNameAsync(vendedor.Correo);

                if (user != null)
                {
                    user.Activo = vendedor.Estado;
                    await _userManager.UpdateAsync(user);
                }
            }

            var estado = vendedor.Estado ? "activado" : "desactivado";
            TempData["Success"] = $"Vendedor '{vendedor.Nombres} {vendedor.Apellidos}' {estado} correctamente.";
            return RedirectToAction(nameof(Index));
        }
        // POST: Vendedores/CambiarPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CambiarPassword(int id, string NuevaPassword)
        {
            var vendedor = await _unitOfWork.Vendedores.GetByIdAsync(id);
            if (vendedor == null) return NotFound();

            if (string.IsNullOrWhiteSpace(NuevaPassword) || NuevaPassword.Length < 6)
            {
                TempData["Error"] = "La nueva contraseña debe tener al menos 6 caracteres.";
                return RedirectToAction(nameof(Edit), new { id });
            }

            // Intentar encontrar el usuario por correo del vendedor
            ApplicationUser? user = null;
            if (!string.IsNullOrEmpty(vendedor.Correo))
            {
                user = await _userManager.FindByEmailAsync(vendedor.Correo);
                if (user == null)
                    user = await _userManager.FindByNameAsync(vendedor.Correo);
            }

            if (user == null)
            {
                TempData["Error"] = $"No se encontró una cuenta de acceso para el correo '{vendedor.Correo}'. Verifica que el vendedor tenga un usuario de Identity creado.";
                return RedirectToAction(nameof(Edit), new { id });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, NuevaPassword);

            if (result.Succeeded)
            {
                // Guardar contraseña para visibilidad del administrador
                vendedor.UltimaPasswordAdmin = NuevaPassword;
                _unitOfWork.Vendedores.Update(vendedor);
                await _unitOfWork.CommitAsync();

                TempData["Success"] = $"✓ Contraseña actualizada correctamente para {vendedor.Nombres} {vendedor.Apellidos}.";
            }
            else
            {
                var errores = string.Join(" | ", result.Errors.Select(e => e.Description));
                TempData["Error"] = $"Error al actualizar: {errores}";
            }

            return RedirectToAction(nameof(Edit), new { id });
        }
    }
}
