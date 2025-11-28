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
using Microsoft.AspNetCore.Identity;

namespace LoyaltyRewardsApp.Controllers
{
    [Authorize(Roles = "Admin")] // Sadece Admin rolü olanlar bu dosyayı çalıştırabilir!
    public class MusterilerController : Controller
    {
        private readonly UygulamaDbContext _context;

        public MusterilerController(UygulamaDbContext context)
        {
            _context = context;
        }

        // GET: Musteriler
        public async Task<IActionResult> Index()
        {
            return View(await _context.Musteriler.ToListAsync());
        }

        // GET: Musteriler/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var musteri = await _context.Musteriler
                .FirstOrDefaultAsync(m => m.Id == id);
            if (musteri == null)
            {
                return NotFound();
            }

            return View(musteri);
        }

        // GET: Musteriler/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Musteriler/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AdSoyad,Email,ToplamPuan,Sifre,Rol")] Musteri musteri)
        {
            if (ModelState.IsValid)
            {
                // --- BURAYI EKLE ---
                var hasher = new PasswordHasher<Musteri>();
                musteri.Sifre = hasher.HashPassword(musteri, musteri.Sifre);
                // -------------------

                _context.Add(musteri);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(musteri);
        }

        // GET: Musteriler/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var musteri = await _context.Musteriler.FindAsync(id);
            if (musteri == null)
            {
                return NotFound();
            }
            return View(musteri);
        }

        // POST: Musteriler/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AdSoyad,Email,ToplamPuan,Sifre,Rol")] Musteri musteri)
        {
            if (id != musteri.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(musteri);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MusteriExists(musteri.Id))
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
            return View(musteri);
        }

        // GET: Musteriler/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var musteri = await _context.Musteriler
                .FirstOrDefaultAsync(m => m.Id == id);
            if (musteri == null)
            {
                return NotFound();
            }

            return View(musteri);
        }

        // POST: Musteriler/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var musteri = await _context.Musteriler.FindAsync(id);
            if (musteri != null)
            {
                _context.Musteriler.Remove(musteri);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MusteriExists(int id)
        {
            return _context.Musteriler.Any(e => e.Id == id);
        }
    }
}
