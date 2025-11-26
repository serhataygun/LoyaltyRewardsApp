using Microsoft.AspNetCore.Mvc;
using LoyaltyRewardsApp.Data;   // Veritabaný ayarlarý klasörü
using LoyaltyRewardsApp.Models; // Tablo modelleri klasörü

namespace LoyaltyRewardsApp.Controllers
{
    public class HomeController : Controller
    {
        // Veritabaný köprümüzü tanýmlýyoruz
        private readonly UygulamaDbContext _context;

        // Bu kýsým, proje çalýþýnca veritabanýný buraya baðlar
        public HomeController(UygulamaDbContext context)
        {
            _context = context;
        }

        // 1. ANASAYFA (GET)
        public IActionResult Index()
        {
            // Cüzdandaki kimliðe bak: Giriþ yapanýn e-postasý nedir?
            var girisYapanEmail = User.Claims.FirstOrDefault(c => System.Security.Claims.ClaimTypes.Email == c.Type)?.Value;

            // O e-postaya sahip müþteriyi veritabanýndan bul
            var musteri = _context.Musteriler.FirstOrDefault(m => m.Email == girisYapanEmail);

            // Eðer giriþ yapmamýþsa veya müþteri bulunamazsa boþ bir ekran göster (veya giriþe yönlendir)
            if (musteri == null)
            {
                // Ýsterseniz burada direkt giriþ sayfasýna atabilirsiniz:
                // return RedirectToAction("Giris", "Hesap");

                // Þimdilik Misafir gösterelim:
                musteri = new Musteri { AdSoyad = "Misafir (Lütfen Giriþ Yapýn)", ToplamPuan = 0 };
            }

            ViewBag.Oduller = _context.Oduller.ToList();

            return View(musteri);
        }


        // Ödüller Sayfasýný Açan Fonksiyon
        public IActionResult Odullerimiz()
        {
            // 1. Giriþ yapan kullanýcýyý bul (Puan kontrolü için lazým)
            var girisYapanEmail = User.Claims.FirstOrDefault(c => System.Security.Claims.ClaimTypes.Email == c.Type)?.Value;
            var musteri = _context.Musteriler.FirstOrDefault(m => m.Email == girisYapanEmail);

            // 2. Ödülleri Çek
            ViewBag.Oduller = _context.Oduller.ToList();

            return View(musteri);
        }

        // 2. ÖDÜL KULLANMA (POST)
        [HttpPost]
        public IActionResult OdulKullan(int odulId)
        {
            // Yine giriþ yapaný buluyoruz
            var girisYapanEmail = User.Claims.FirstOrDefault(c => System.Security.Claims.ClaimTypes.Email == c.Type)?.Value;
            var musteri = _context.Musteriler.FirstOrDefault(m => m.Email == girisYapanEmail);

            var odul = _context.Oduller.Find(odulId);

            if (musteri == null || odul == null) return RedirectToAction("Index");

            if (musteri.ToplamPuan >= odul.GerekliPuan)
            {
                musteri.ToplamPuan -= odul.GerekliPuan;
                _context.SaveChanges();
                TempData["Mesaj"] = "Tebrikler! Ödülünüzü aldýnýz.";
            }
            else
            {
                TempData["Hata"] = "Puanýnýz yetersiz!";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // 3. PROMOSYON KODU GÝRÝÞÝ (POST)
        [HttpPost]
        public IActionResult PuanKazan(string girilenKod)
        {
            // 1. Giriþ yapan kullanýcýyý bul
            var girisYapanEmail = User.Claims.FirstOrDefault(c => System.Security.Claims.ClaimTypes.Email == c.Type)?.Value;

            if (girisYapanEmail == null)
            {
                TempData["Hata"] = "Kod kullanmak için önce giriþ yapmalýsýnýz!";
                return RedirectToAction("Index");
            }

            var musteri = _context.Musteriler.FirstOrDefault(m => m.Email == girisYapanEmail);

            // 2. Girilen kod veritabanýnda var mý? Ve daha önemlisi: KULLANILMAMIÞ MI?
            // (Büyük/küçük harf duyarlýlýðýný kaldýrmak için ikisini de ToUpper ile büyütüyoruz)
            var promosyon = _context.PromosyonKodlari
                                    .FirstOrDefault(p => p.Kod.ToUpper() == girilenKod.ToUpper() && p.KullanildiMi == false);

            if (promosyon != null)
            {
                // 3. Kod Geçerli! Puaný Yükle
                musteri.ToplamPuan += promosyon.PuanDegeri;

                // 4. Kodu "Kullanýldý" olarak iþaretle (Bir daha kullanýlamasýn)
                promosyon.KullanildiMi = true;

                // 5. Her iki tabloyu da güncelle (Müþteri ve PromosyonKodu)
                _context.SaveChanges();

                TempData["Mesaj"] = $"Tebrikler! {promosyon.PuanDegeri} puan kazandýnýz.";
            }
            else
            {
                TempData["Hata"] = "Girdiðiniz kod hatalý veya daha önce kullanýlmýþ.";
            }

            return RedirectToAction("Index");
        }
    }
}