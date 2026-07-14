using AutoMapper;
using FashionStore.Domain.DTOs;
using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FashionStore.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class PrendasController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] _extensionesPermitidas = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long _maxImagenBytes = 5 * 1024 * 1024; // 5 MB

        public PrendasController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _env = env;
        }

        // =====================================
        // INDEX — todos los autenticados
        // =====================================

        public async Task<IActionResult> Index()
        {
            var prendas = await _unitOfWork.Prendas.GetAllAsync();
            var prendasDTO = _mapper.Map<IEnumerable<PrendaDTO>>(prendas);
            return View(prendasDTO);
        }

        // =====================================
        // DETAILS — todos los autenticados
        // =====================================

        public async Task<IActionResult> Details(int id)
        {
            var prenda = await _unitOfWork.Prendas.GetByIdAsync(id);
            if (prenda == null) return NotFound();
            var prendaDTO = _mapper.Map<PrendaDTO>(prenda);
            return View(prendaDTO);
        }

        // =====================================
        // DASHBOARD — todos los autenticados
        // =====================================

        public async Task<IActionResult> Dashboard()
        {
            var prendas = await _unitOfWork.Prendas.GetAllAsync();
            var categorias = await _unitOfWork.Categorias.GetAllAsync();

            var byCategory = prendas.GroupBy(p => p.CategoriaId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count(), Stock = g.Sum(x => x.Stock) })
                .ToList();

            var labels = new List<string>();
            var counts = new List<int>();
            var stocks = new List<int>();

            foreach (var item in byCategory)
            {
                var cat = categorias.FirstOrDefault(c => c.Id == item.CategoryId);
                labels.Add(cat?.Nombre ?? "Sin categoría");
                counts.Add(item.Count);
                stocks.Add(item.Stock);
            }

            ViewData["Labels"] = System.Text.Json.JsonSerializer.Serialize(labels);
            ViewData["Counts"] = System.Text.Json.JsonSerializer.Serialize(counts);
            ViewData["Stocks"] = System.Text.Json.JsonSerializer.Serialize(stocks);

            return View();
        }

        // =====================================
        // CREATE GET — solo Administrador
        // =====================================

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create()
        {
            await CargarCategorias();
            return View();
        }

        // =====================================
        // CREATE POST — solo Administrador
        // =====================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create(PrendaDTO prendaDTO, IFormFile? imagenFile)
        {
            if (!ModelState.IsValid)
            {
                await CargarCategorias();
                return View(prendaDTO);
            }

            var prenda = _mapper.Map<Prenda>(prendaDTO);

            if (imagenFile != null && imagenFile.Length > 0)
            {
                var resultado = await GuardarImagenAsync(imagenFile);
                if (resultado.Error != null)
                {
                    ModelState.AddModelError("imagenFile", resultado.Error);
                    await CargarCategorias();
                    return View(prendaDTO);
                }
                prenda.ImagenUrl = resultado.NombreArchivo;
            }

            await _unitOfWork.Prendas.AddAsync(prenda);
            await _unitOfWork.CommitAsync();

            TempData["Success"] = $"Prenda '{prenda.Nombre}' creada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // =====================================
        // EDIT GET — solo Administrador
        // =====================================

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id)
        {
            var prenda = await _unitOfWork.Prendas.GetByIdAsync(id);
            if (prenda == null) return NotFound();

            var prendaDTO = _mapper.Map<PrendaDTO>(prenda);
            await CargarCategorias();
            return View(prendaDTO);
        }

        // =====================================
        // EDIT POST — solo Administrador
        // =====================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, PrendaDTO prendaDTO, IFormFile? imagenFile = null)
        {
            if (id != prendaDTO.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                await CargarCategorias();
                return View(prendaDTO);
            }

            var prenda = _mapper.Map<Prenda>(prendaDTO);

            if (imagenFile != null && imagenFile.Length > 0)
            {
                var resultado = await GuardarImagenAsync(imagenFile);
                if (resultado.Error != null)
                {
                    ModelState.AddModelError("imagenFile", resultado.Error);
                    await CargarCategorias();
                    return View(prendaDTO);
                }
                prenda.ImagenUrl = resultado.NombreArchivo;
            }
            else
            {
                // Conservar imagen existente si no se sube nueva
                prenda.ImagenUrl = prendaDTO.ImagenUrl;
            }

            _unitOfWork.Prendas.Update(prenda);
            await _unitOfWork.CommitAsync();

            TempData["Success"] = $"Prenda '{prenda.Nombre}' actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // =====================================
        // DELETE GET — solo Administrador
        // =====================================

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var prenda = await _unitOfWork.Prendas.GetByIdAsync(id);
            if (prenda == null) return NotFound();

            var prendaDTO = _mapper.Map<PrendaDTO>(prenda);
            return View(prendaDTO);
        }

        // =====================================
        // DELETE POST — solo Administrador
        // =====================================

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prenda = await _unitOfWork.Prendas.GetByIdAsync(id);
            if (prenda == null) return NotFound();

            _unitOfWork.Prendas.Delete(prenda);
            await _unitOfWork.CommitAsync();

            TempData["Success"] = "Prenda eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // =====================================
        // HELPERS PRIVADOS
        // =====================================

        private async Task CargarCategorias()
        {
            var categorias = await _unitOfWork.Categorias.GetAllAsync();
            ViewBag.Categorias = categorias
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nombre })
                .ToList();
        }

        private async Task<(string? NombreArchivo, string? Error)> GuardarImagenAsync(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!_extensionesPermitidas.Contains(ext))
                return (null, $"Extensión no permitida. Use: {string.Join(", ", _extensionesPermitidas)}");

            if (file.Length > _maxImagenBytes)
                return (null, "La imagen supera el tamaño máximo de 5 MB.");

            var carpeta = Path.Combine(_env.WebRootPath, "uploads", "productos");
            Directory.CreateDirectory(carpeta);

            var nombreArchivo = Guid.NewGuid().ToString() + ext;
            var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            using var stream = new FileStream(rutaCompleta, FileMode.Create);
            await file.CopyToAsync(stream);

            return (nombreArchivo, null);
        }
    }
}
