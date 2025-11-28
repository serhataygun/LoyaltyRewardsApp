using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LoyaltyRewardsApp.Data; // Kendi proje isminizi yazın
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

        // 1. Giriş Sayfasını Göster (GET)
        public IActionResult Giris()
        {
            return View();
        }

        // 2. Giriş Yap Butonuna Basılınca (POST)
        [HttpPost]
        public async Task<IActionResult> Giris(string email, string sifre)
        {
            // 1. Önce SADECE E-Posta ile kullanıcıyı bul (Şifreye henüz bakma)
            var kullanici = _context.Musteriler.FirstOrDefault(x => x.Email == email);

            if (kullanici != null)
            {
                // 2. Şifreyi Doğrula
                var hasher = new PasswordHasher<Musteri>();

                // Bu fonksiyon: Veritabanındaki Hash, Girilen Düz Şifre'yi karşılaştırır.
                var sonuc = hasher.VerifyHashedPassword(kullanici, kullanici.Sifre, sifre);

                // 3. Sonuç Başarılıysa İçeri Al
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

                    // Çerez ayarlarını (Guid.NewGuid olayını) burada kullanabilirsin yine
                    var girisOzellikleri = new AuthenticationProperties
                    {
                        IsPersistent = false,
                        ExpiresUtc = null
                    };

                    await HttpContext.SignInAsync("CerezSistemi", kural, girisOzellikleri);

                    return RedirectToAction("Index", "Home");
                }
            }

            // Kullanıcı bulunamazsa veya şifre yanlışsa
            ViewBag.Hata = "E-posta veya şifre hatalı!";
            return View();
        }

        // 3. Çıkış Yap (Logout)
        public async Task<IActionResult> Cikis()
        {
            await HttpContext.SignOutAsync("CerezSistemi"); // Bilekliği sök at
            return RedirectToAction("Index", "Home");
        }

        // 4. KAYIT SAYFASINI GÖSTER (GET)
        public IActionResult Kayit()
        {
            return View();
        }

        // 5. KAYIT OL BUTONUNA BASINCA (POST)
        [HttpPost]
        public IActionResult Kayit(Musteri yeniUye)
        {
            var varMi = _context.Musteriler.Any(x => x.Email == yeniUye.Email);
            if (varMi)
            {
                ViewBag.Hata = "Bu e-posta adresi zaten kullanılıyor!";
                return View();
            }

            // --- ŞİFRE HASHLEME BAŞLANGICI ---
            // 1. Hashleyici aracını oluştur
            var hasher = new PasswordHasher<Musteri>();

            // 2. Şifreyi hashle ve eski düz şifrenin üzerine yaz
            // (HashPassword metodu içine; kullanıcı nesnesini ve düz şifreyi alır)
            yeniUye.Sifre = hasher.HashPassword(yeniUye, yeniUye.Sifre);
            // ---------------------------------

            yeniUye.Rol = "Uye";
            yeniUye.ToplamPuan = 0;

            _context.Musteriler.Add(yeniUye);
            _context.SaveChanges();

            TempData["Basarili"] = "Kaydınız oluşturuldu! Şimdi giriş yapabilirsiniz.";
            return RedirectToAction("Giris");
        }

        // 6. PROFİL SAYFASINI GÖSTER (GET)
        public IActionResult Profil()
        {
            // Giriş yapanın e-postasını bul
            var email = User.Claims.FirstOrDefault(c => System.Security.Claims.ClaimTypes.Email == c.Type)?.Value;

            // Veritabanından o kişiyi getir
            var kullanici = _context.Musteriler.FirstOrDefault(x => x.Email == email);

            return View(kullanici);
        }

        // 7. PROFİL GÜNCELLEME (POST)
        [HttpPost]
        public IActionResult Profil(Musteri guncelVeri)
        {
            // Veritabanındaki asıl kaydı buluyoruz (ID üzerinden)
            var dbKullanici = _context.Musteriler.Find(guncelVeri.Id);

            if (dbKullanici != null)
            {
                // 1. Ad ve Email güncelle
                dbKullanici.AdSoyad = guncelVeri.AdSoyad;
                dbKullanici.Email = guncelVeri.Email;

                // 2. Şifre alanı boş bırakılmadıysa, yeni şifreyi hashleyip kaydet
                if (!string.IsNullOrEmpty(guncelVeri.Sifre))
                {
                    var hasher = new PasswordHasher<Musteri>();
                    dbKullanici.Sifre = hasher.HashPassword(dbKullanici, guncelVeri.Sifre);
                }
                // (Eğer şifre kutusu boş bırakıldıysa, eski şifreye dokunmuyoruz)

                _context.SaveChanges();
                TempData["Basarili"] = "Bilgileriniz başarıyla güncellendi.";
            }

            return RedirectToAction("Profil"); // Sayfayı yenile
        }
    }
}