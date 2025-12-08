using LoyaltyRewardsApp.Data;
using LoyaltyRewardsApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyRewardsApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PromosyonKodlariController : Controller
    {
        private readonly UygulamaDbContext _context;

        public PromosyonKodlariController(UygulamaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.PromosyonKodlari.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promosyonKodu = await _context.PromosyonKodlari
                .FirstOrDefaultAsync(m => m.Id == id);
            if (promosyonKodu == null)
            {
                return NotFound();
            }

            return View(promosyonKodu);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Kod,PuanDegeri,KullanildiMi")] PromosyonKodu promosyonKodu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(promosyonKodu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(promosyonKodu);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promosyonKodu = await _context.PromosyonKodlari.FindAsync(id);
            if (promosyonKodu == null)
            {
                return NotFound();
            }
            return View(promosyonKodu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Kod,PuanDegeri,KullanildiMi")] PromosyonKodu promosyonKodu)
        {
            if (id != promosyonKodu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(promosyonKodu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PromosyonKoduExists(promosyonKodu.Id))
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
            return View(promosyonKodu);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promosyonKodu = await _context.PromosyonKodlari
                .FirstOrDefaultAsync(m => m.Id == id);
            if (promosyonKodu == null)
            {
                return NotFound();
            }

            return View(promosyonKodu);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var promosyonKodu = await _context.PromosyonKodlari.FindAsync(id);
            if (promosyonKodu != null)
            {
                _context.PromosyonKodlari.Remove(promosyonKodu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PromosyonKoduExists(int id)
        {
            return _context.PromosyonKodlari.Any(e => e.Id == id);
        }
    }
}
