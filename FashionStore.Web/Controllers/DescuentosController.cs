using FashionStore.Domain.Entities;
using FashionStore.Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class DescuentosController : Controller
    {
        private readonly FashionStoreDbContext _context;

        public DescuentosController(FashionStoreDbContext context)
        {
            _context = context;
        }

        // GET: Descuentos
        public async Task<IActionResult> Index()
        {
            return View(await _context.DescuentosAutorizados.ToListAsync());
        }

        // GET: Descuentos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Descuentos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Tipo,Valor,Activo")] DescuentoAutorizado descuentoAutorizado)
        {
            if (ModelState.IsValid)
            {
                descuentoAutorizado.FechaCreacion = DateTime.UtcNow;
                _context.Add(descuentoAutorizado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(descuentoAutorizado);
        }

        // GET: Descuentos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var descuentoAutorizado = await _context.DescuentosAutorizados.FindAsync(id);
            if (descuentoAutorizado == null) return NotFound();

            return View(descuentoAutorizado);
        }

        // POST: Descuentos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Tipo,Valor,Activo,FechaCreacion")] DescuentoAutorizado descuentoAutorizado)
        {
            if (id != descuentoAutorizado.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(descuentoAutorizado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DescuentoAutorizadoExists(descuentoAutorizado.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(descuentoAutorizado);
        }

        // GET: Descuentos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var descuentoAutorizado = await _context.DescuentosAutorizados
                .FirstOrDefaultAsync(m => m.Id == id);
            if (descuentoAutorizado == null) return NotFound();

            return View(descuentoAutorizado);
        }

        // POST: Descuentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var descuentoAutorizado = await _context.DescuentosAutorizados.FindAsync(id);
            if (descuentoAutorizado != null)
            {
                _context.DescuentosAutorizados.Remove(descuentoAutorizado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DescuentoAutorizadoExists(int id)
        {
            return _context.DescuentosAutorizados.Any(e => e.Id == id);
        }
    }
}
