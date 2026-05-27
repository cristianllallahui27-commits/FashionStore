using AutoMapper;
using FashionStore.Domain.DTOs;
using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FashionStore.Web.Controllers
{
    public class PrendasController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PrendasController(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // =====================================
        // INDEX
        // =====================================

        public async Task<IActionResult> Index()
        {
            var prendas =
                await _unitOfWork.Prendas.GetAllAsync();

            var prendasDTO =
                _mapper.Map<IEnumerable<PrendaDTO>>(prendas);

            return View(prendasDTO);
        }

        // =====================================
        // DETAILS
        // =====================================

        public async Task<IActionResult> Details(int id)
        {
            var prenda =
                await _unitOfWork.Prendas.GetByIdAsync(id);

            if (prenda == null)
                return NotFound();

            var prendaDTO =
                _mapper.Map<PrendaDTO>(prenda);

            return View(prendaDTO);
        }

        // =====================================
        // CREATE GET
        // =====================================

        public async Task<IActionResult> Create()
        {
            await CargarCategorias();

            return View();
        }

        // =====================================
        // CREATE POST
        // =====================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            PrendaDTO prendaDTO,
            IFormFile imagenFile)
        {
            if (!ModelState.IsValid)
            {
                await CargarCategorias();

                return View(prendaDTO);
            }

            var prenda =
                _mapper.Map<Prenda>(prendaDTO);

            // SUBIR IMAGEN

            if (imagenFile != null &&
                imagenFile.Length > 0)
            {
                var nombreArchivo =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(imagenFile.FileName);

                var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                // Ensure the folder exists to avoid DirectoryNotFoundException in tests and runtime
                Directory.CreateDirectory(imagesFolder);

                var ruta = Path.Combine(imagesFolder, nombreArchivo);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await imagenFile.CopyToAsync(stream);
                }

                prenda.ImagenUrl = nombreArchivo;
            }

            await _unitOfWork.Prendas.AddAsync(prenda);

            await _unitOfWork.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // DASHBOARD
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
        // EDIT GET
        // =====================================

        public async Task<IActionResult> Edit(int id)
        {
            var prenda =
                await _unitOfWork.Prendas.GetByIdAsync(id);

            if (prenda == null)
                return NotFound();

            var prendaDTO =
                _mapper.Map<PrendaDTO>(prenda);

            await CargarCategorias();

            return View(prendaDTO);
        }

        // =====================================
        // EDIT POST
        // =====================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            PrendaDTO prendaDTO)
        {
            if (id != prendaDTO.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await CargarCategorias();

                return View(prendaDTO);
            }

            var prenda =
                _mapper.Map<Prenda>(prendaDTO);

            _unitOfWork.Prendas.Update(prenda);

            await _unitOfWork.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // =====================================
        // DELETE GET
        // =====================================

        public async Task<IActionResult> Delete(int id)
        {
            var prenda =
                await _unitOfWork.Prendas.GetByIdAsync(id);

            if (prenda == null)
                return NotFound();

            var prendaDTO =
                _mapper.Map<PrendaDTO>(prenda);

            return View(prendaDTO);
        }

        // =====================================
        // DELETE POST
        // =====================================

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prenda =
                await _unitOfWork.Prendas.GetByIdAsync(id);

            if (prenda == null)
                return NotFound();

            _unitOfWork.Prendas.Delete(prenda);

            await _unitOfWork.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // =====================================
        // CARGAR CATEGORÍAS
        // =====================================

        private async Task CargarCategorias()
        {
            var categorias =
                await _unitOfWork.Categorias.GetAllAsync();

            ViewBag.Categorias = categorias
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre
                })
                .ToList();
        }
    }
}