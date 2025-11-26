using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LoyaltyRewardsApp.Data; // Kendi proje isminizi yazın
using LoyaltyRewardsApp.Models;

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
            // Veritabanında bu mail ve şifreye sahip biri var mı?
            var kullanici = _context.Musteriler.FirstOrDefault(x => x.Email == email && x.Sifre == sifre);

            if (kullanici != null)
            {
                // Kullanıcı bulundu! Şimdi ona bir "Kimlik Kartı" (Claims) hazırlayalım.
                var talepler = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, kullanici.AdSoyad),
                    new Claim(ClaimTypes.Email, kullanici.Email),
                    new Claim(ClaimTypes.Role, kullanici.Rol), // Rolü burada yüklüyoruz!
                    new Claim("MusteriId", kullanici.Id.ToString()) // ID'sini de gizlice ekleyelim
                };

                var kimlik = new ClaimsIdentity(talepler, "CerezSistemi");
                var kural = new ClaimsPrincipal(kimlik);

                // --- ESKİ KODLARI SİLİN VE SADECE BUNU YAPIŞTIRIN ---

                var girisOzellikleri = new AuthenticationProperties
                {
                    IsPersistent = false, // Beni Hatırla KAPALI
                    ExpiresUtc = null     // Süre verme! (Süre vermezsen tarayıcı kapanınca ölür)
                };

                await HttpContext.SignInAsync("CerezSistemi", kural, girisOzellikleri);

                // ----------------------------------------------------

                // Eğer Admisse Admin paneline, değilse Anasayfaya gitsin
                if (kullanici.Rol == "Admin")
                {
                    return RedirectToAction("Index", "Home"); // Admin kampanyalara gitsin
                }

                return RedirectToAction("Index", "Home");
            }

            // Kullanıcı bulunamazsa
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
            // 1. Bu e-posta adresiyle daha önce kayıt olunmuş mu?
            var varMi = _context.Musteriler.Any(x => x.Email == yeniUye.Email);
            if (varMi)
            {
                ViewBag.Hata = "Bu e-posta adresi zaten kullanılıyor!";
                return View();
            }

            // 2. Yeni üyenin varsayılan ayarlarını yapalım
            yeniUye.Rol = "Uye"; // Güvenlik önlemi: Kimse kayıt olurken kendini Admin yapamasın
            yeniUye.ToplamPuan = 0; // Yeni üyenin puanı 0 başlar

            // 3. Veritabanına Ekle
            _context.Musteriler.Add(yeniUye);
            _context.SaveChanges();

            // 4. Kayıt başarılıysa Giriş sayfasına yönlendir
            TempData["Basarili"] = "Kaydınız oluşturuldu! Şimdi giriş yapabilirsiniz.";
            return RedirectToAction("Giris");
        }
    }
}