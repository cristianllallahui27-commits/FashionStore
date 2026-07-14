using AutoMapper;
using FashionStore.Domain.DTOs;
using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;
using FashionStore.Web.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FashionStore.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CategoriasController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;

        public CategoriasController(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;

            _mapper = mapper;
        }

        // =========================================
        // LISTAR
        // =========================================

        public async Task<IActionResult> Index()
        {
            var categorias =
                await _unitOfWork.Categorias.GetAllAsync();

            var categoriasDTO =
                _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

            return View(categoriasDTO);
        }

        // =========================================
        // DETAILS
        // =========================================

        public async Task<IActionResult> Details(int id)
        {
            var categoria =
                await _unitOfWork.Categorias.GetByIdAsync(id);

            if (categoria == null)
                return NotFound();

            var categoriaDTO =
                _mapper.Map<CategoriaDTO>(categoria);

            return View(categoriaDTO);
        }

        // =========================================
        // CREATE GET
        // =========================================

        public IActionResult Create() => View();

        // =========================================
        // CREATE POST
        // =========================================

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(
            CategoriaDTO categoriaDTO)
        {
            if (!ModelState.IsValid)
                return View(categoriaDTO);

            var categoria =
                _mapper.Map<Categoria>(categoriaDTO);

            await _unitOfWork.Categorias.AddAsync(categoria);

            await _unitOfWork.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================================
        // DASHBOARD MODERNO
        // =========================================

        public async Task<IActionResult> Dashboard()
        {
            // CATEGORÍAS

            var categorias =
                await _unitOfWork.Categorias.GetAllAsync();

            // PRENDAS

            var prendas =
                await _unitOfWork.Prendas.GetAllAsync();

            // AGRUPAR PRENDAS POR CATEGORÍA

            var byCategory = prendas
                .GroupBy(p => p.CategoriaId)
                .Select(g => new
                {
                    CategoriaId = g.Key,

                    Cantidad = g.Count()
                })
                .ToList();

            // LISTAS PARA CHART JS

            var labels = new List<string>();

            var data = new List<int>();

            // OBTENER NOMBRE CATEGORÍA

            foreach (var item in byCategory)
            {
                var categoria =
                    await _unitOfWork.Categorias
                    .GetByIdAsync(item.CategoriaId);

                labels.Add(
                    categoria?.Nombre ?? "Sin categoría");

                data.Add(item.Cantidad);
            }

            // VIEWMODEL

            var model = new DashboardViewModel
            {
                TotalCategorias = categorias.Count(),

                TotalPrendas = prendas.Count(),

                SalesChartLabelsJson =
                    System.Text.Json.JsonSerializer.Serialize(labels),

                SalesChartDataJson =
                    System.Text.Json.JsonSerializer.Serialize(data),

                PrendasByCategoryLabelsJson =
                    System.Text.Json.JsonSerializer.Serialize(labels),

                PrendasByCategoryDataJson =
                    System.Text.Json.JsonSerializer.Serialize(data)
            };

            return View(model);
        }

        // =========================================
        // EDIT GET
        // =========================================

        public async Task<IActionResult> Edit(int id)
        {
            var categoria =
                await _unitOfWork.Categorias.GetByIdAsync(id);

            if (categoria == null)
                return NotFound();

            var categoriaDTO =
                _mapper.Map<CategoriaDTO>(categoria);

            return View(categoriaDTO);
        }

        // =========================================
        // EDIT POST
        // =========================================

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(
            int id,
            CategoriaDTO categoriaDTO)
        {
            if (id != categoriaDTO.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(categoriaDTO);

            var categoria =
                _mapper.Map<Categoria>(categoriaDTO);

            _unitOfWork.Categorias.Update(categoria);

            await _unitOfWork.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================================
        // DELETE GET
        // =========================================

        public async Task<IActionResult> Delete(int id)
        {
            var categoria =
                await _unitOfWork.Categorias.GetByIdAsync(id);

            if (categoria == null)
                return NotFound();

            var categoriaDTO =
                _mapper.Map<CategoriaDTO>(categoria);

            return View(categoriaDTO);
        }

        // =========================================
        // DELETE POST
        // =========================================

        [HttpPost,
        ActionName("Delete")]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoria =
                await _unitOfWork.Categorias.GetByIdAsync(id);

            if (categoria == null)
                return NotFound();

            _unitOfWork.Categorias.Delete(categoria);

            await _unitOfWork.CommitAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}