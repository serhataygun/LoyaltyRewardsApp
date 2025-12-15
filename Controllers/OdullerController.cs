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
    [Authorize(Roles = "Admin")]

    // "Admin" rolü olmayan hiç kimse bu sayfaya giremez.
    public class OdullerController : Controller
    {
        private readonly UygulamaDbContext _context;

        public OdullerController(UygulamaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Oduller.ToListAsync());
        }

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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Odul odul)
        {
            if (ModelState.IsValid)
            {
                _context.Add(odul);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(odul);
        }

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
