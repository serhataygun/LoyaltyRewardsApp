using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LoyaltyRewardsApp.Data;
using LoyaltyRewardsApp.Models;
using Microsoft.AspNetCore.Identity;

namespace LoyaltyRewardsApp.Controllers
{
    public class HesapController : Controller
    {
        private readonly UygulamaDbContext _context;

        public HesapController(UygulamaDbContext context)
        {
            _context = context;
        }
        public IActionResult Giris()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Giris(string email, string sifre)
        {
            var kullanici = _context.Musteriler.FirstOrDefault(x => x.Email == email);

            if (kullanici != null)
            {
                var hasher = new PasswordHasher<Musteri>();

                var sonuc = hasher.VerifyHashedPassword(kullanici, kullanici.Sifre, sifre);

                if (sonuc == PasswordVerificationResult.Success)
                {
                    var talepler = new List<Claim>
            {
                new Claim(ClaimTypes.Name, kullanici.AdSoyad),
                new Claim(ClaimTypes.Email, kullanici.Email),
                new Claim(ClaimTypes.Role, kullanici.Rol),
                new Claim("MusteriId", kullanici.Id.ToString())
            };

                    var kimlik = new ClaimsIdentity(talepler, "CerezSistemi");
                    var kural = new ClaimsPrincipal(kimlik);

                    var girisOzellikleri = new AuthenticationProperties
                    {
                        IsPersistent = false,
                        ExpiresUtc = null
                    };

                    await HttpContext.SignInAsync("CerezSistemi", kural, girisOzellikleri);

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Hata = "Invalid email or password!";
            return View();
        }

        public async Task<IActionResult> Cikis()
        {
            await HttpContext.SignOutAsync("CerezSistemi"); 
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Kayit()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Kayit(Musteri yeniUye)
        {
            var varMi = _context.Musteriler.Any(x => x.Email == yeniUye.Email);
            if (varMi)
            {
                ViewBag.Hata = "This email address is already in use!";
                return View();
            }

            var hasher = new PasswordHasher<Musteri>();

            yeniUye.Sifre = hasher.HashPassword(yeniUye, yeniUye.Sifre);

            yeniUye.Rol = "Uye";
            yeniUye.ToplamPuan = 0;

            _context.Musteriler.Add(yeniUye);
            _context.SaveChanges();

            TempData["Basarili"] = "Your registration has been created! You can now log in.";
            return RedirectToAction("Giris");
        }

        public IActionResult Profil()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "MusteriId");

            if (idClaim == null) return RedirectToAction("Giris");

            int id = int.Parse(idClaim.Value);
            var kullanici = _context.Musteriler.Find(id);

            if (kullanici == null)
            {
                return RedirectToAction("Cikis");
            }

            return View(kullanici);
        }

        [HttpPost]
        public async Task<IActionResult> Profil(Musteri guncelVeri)
        {
            var dbKullanici = _context.Musteriler.Find(guncelVeri.Id);

            if (dbKullanici != null)
            {
                dbKullanici.AdSoyad = guncelVeri.AdSoyad;
                dbKullanici.Email = guncelVeri.Email;

                if (!string.IsNullOrEmpty(guncelVeri.Sifre))
                {
                    var hasher = new PasswordHasher<Musteri>();
                    dbKullanici.Sifre = hasher.HashPassword(dbKullanici, guncelVeri.Sifre);
                }

                _context.SaveChanges();

                var talepler = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, dbKullanici.AdSoyad), 
                    new Claim(ClaimTypes.Email, dbKullanici.Email),   
                    new Claim(ClaimTypes.Role, dbKullanici.Rol),
                    new Claim("MusteriId", dbKullanici.Id.ToString()) 
                };

                var kimlik = new ClaimsIdentity(talepler, "CerezSistemi");
                var kural = new ClaimsPrincipal(kimlik);

                var girisOzellikleri = new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = null
                };

                await HttpContext.SignInAsync("CerezSistemi", kural, girisOzellikleri);

                TempData["Basarili"] = "Your information has been successfully updated.";
            }

            return RedirectToAction("Profil");
        }
    }
}