using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LoyaltyRewardsApp.Data;
using LoyaltyRewardsApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace LoyaltyRewardsApp.Controllers
{
    [Authorize(Roles = "Admin")] // Sadece Admin rolü olanlar bu dosyayı çalıştırabilir!
    public class OdullerController : Controller
    {
        private readonly UygulamaDbContext _context;

        public OdullerController(UygulamaDbContext context)
        {
            _context = context;
        }

        // GET: Oduller
        public async Task<IActionResult> Index()
        {
            return View(await _context.Oduller.ToListAsync());
        }

        // GET: Oduller/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var odul = await _context.Oduller
                .FirstOrDefaultAsync(m => m.Id == id);
            if (odul == null)
            {
                return NotFound();
            }

            return View(odul);
        }

        // GET: Oduller/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Oduller/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Baslik,Aciklama,GerekliPuan,ResimUrl")] Odul odul)
        {
            if (ModelState.IsValid)
            {
                _context.Add(odul);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(odul);
        }

        // GET: Oduller/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var odul = await _context.Oduller.FindAsync(id);
            if (odul == null)
            {
                return NotFound();
            }
            return View(odul);
        }

        // POST: Oduller/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Baslik,Aciklama,GerekliPuan,ResimUrl")] Odul odul)
        {
            if (id != odul.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(odul);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OdulExists(odul.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(odul);
        }

        // GET: Oduller/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var odul = await _context.Oduller
                .FirstOrDefaultAsync(m => m.Id == id);
            if (odul == null)
            {
                return NotFound();
            }

            return View(odul);
        }

        // POST: Oduller/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var odul = await _context.Oduller.FindAsync(id);
            if (odul != null)
            {
                _context.Oduller.Remove(odul);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OdulExists(int id)
        {
            return _context.Oduller.Any(e => e.Id == id);
        }
    }
}
