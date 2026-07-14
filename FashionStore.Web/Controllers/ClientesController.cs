using AutoMapper;
using FashionStore.Domain.DTOs;
using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FashionStore.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ClientesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;

        public ClientesController(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;

            _mapper = mapper;
        }

        // ============================
        // LISTAR
        // ============================

        public async Task<IActionResult> Index()
        {
            var clientes =
                await _unitOfWork.Clientes.GetAllAsync();

            var clientesDTO =
                _mapper.Map<IEnumerable<ClienteDTO>>(clientes);

            return View(clientesDTO);
        }

        // ============================
        // CREATE GET
        // ============================

        public IActionResult Create() => View();

        // ============================
        // CREATE POST
        // ============================

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(
            ClienteDTO clienteDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(clienteDTO);
            }

            var cliente =
                _mapper.Map<Cliente>(clienteDTO);

            await _unitOfWork.Clientes.AddAsync(cliente);

            await _unitOfWork.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // DASHBOARD
        public async Task<IActionResult> Dashboard()
        {
            var clientes = await _unitOfWork.Clientes.GetAllAsync();

            var total = clientes.Count();
            var recent = clientes.OrderByDescending(c => c.Id).Take(10).ToList();

            ViewData["TotalClientes"] = total;
            ViewData["RecentClients"] = System.Text.Json.JsonSerializer.Serialize(recent.Select(c => new { c.NombreCompleto, c.DNI }));

            return View();
        }

        // ============================
        // DETAILS
        // ============================

        public async Task<IActionResult> Details(int id)
        {
            var cliente =
                await _unitOfWork.Clientes.GetByIdAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            var clienteDTO =
                _mapper.Map<ClienteDTO>(cliente);

            return View(clienteDTO);
        }

        // ============================
        // EDIT GET
        // ============================

        public async Task<IActionResult> Edit(int id)
        {
            var cliente =
                await _unitOfWork.Clientes.GetByIdAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            var clienteDTO =
                _mapper.Map<ClienteDTO>(cliente);

            return View(clienteDTO);
        }

        // ============================
        // EDIT POST
        // ============================

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(
            int id,
            ClienteDTO clienteDTO)
        {
            if (id != clienteDTO.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(clienteDTO);
            }

            var cliente =
                _mapper.Map<Cliente>(clienteDTO);

            _unitOfWork.Clientes.Update(cliente);

            await _unitOfWork.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // ============================
        // DELETE GET
        // ============================

        public async Task<IActionResult> Delete(int id)
        {
            var cliente =
                await _unitOfWork.Clientes.GetByIdAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            var clienteDTO =
                _mapper.Map<ClienteDTO>(cliente);

            return View(clienteDTO);
        }

        // ============================
        // DELETE POST
        // ============================

        [HttpPost, ActionName("Delete")]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente =
                await _unitOfWork.Clientes.GetByIdAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            _unitOfWork.Clientes.Delete(cliente);

            await _unitOfWork.CommitAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
