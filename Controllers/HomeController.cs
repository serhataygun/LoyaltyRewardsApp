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
            // ID ile kullanýcýyý bul (Garanti Yöntem)
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "MusteriId");
            if (idClaim == null) return RedirectToAction("Giris", "Hesap");

            int id = int.Parse(idClaim.Value);
            var musteri = _context.Musteriler.Find(id);
            var odul = _context.Oduller.Find(odulId);

            if (musteri == null || odul == null) return RedirectToAction("Index");

            if (musteri.ToplamPuan >= odul.GerekliPuan)
            {
                // Puaný düþ
                musteri.ToplamPuan -= odul.GerekliPuan;

                // Geçmiþe kaydet
                var yeniKayit = new OdulGecmisi
                {
                    MusteriId = musteri.Id,
                    OdulAdi = odul.Baslik,
                    HarcananPuan = odul.GerekliPuan,
                    Tarih = DateTime.Now
                };
                _context.GecmisIslemler.Add(yeniKayit);

                _context.SaveChanges();
                TempData["Mesaj"] = "Tebrikler, ödülünüzü aldýnýz! Geçmiþ sayfasýndan ödüllerinizi görüntüleyebilirsiniz.";
            }
            else
            {
                TempData["Hata"] = "Puanýnýz yetersiz!";
            }

            return RedirectToAction("Index"); // Veya return RedirectToAction("Odullerimiz");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // 3. PROMOSYON KODU GÝRÝÞÝ (POST)
        [HttpPost]
        public IActionResult PuanKazan(string girilenKod)
        {
            // ID ile kullanýcýyý bul
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "MusteriId");
            if (idClaim == null) return RedirectToAction("Giris", "Hesap");

            int id = int.Parse(idClaim.Value);
            var musteri = _context.Musteriler.Find(id);

            if (musteri == null) return RedirectToAction("Giris", "Hesap");

            // Kod Kontrolü (Ram üzerinde güvenli karþýlaþtýrma)
            var aktifKodlar = _context.PromosyonKodlari.Where(p => p.KullanildiMi == false).ToList();
            var promosyon = aktifKodlar.FirstOrDefault(p =>
                string.Equals(p.Kod.Trim(), girilenKod.Trim(), StringComparison.OrdinalIgnoreCase));

            if (promosyon != null)
            {
                musteri.ToplamPuan += promosyon.PuanDegeri;
                promosyon.KullanildiMi = true;
                _context.SaveChanges();
                TempData["Mesaj"] = $"Tebrikler! {promosyon.PuanDegeri} puan kazandýnýz.";
            }
            else
            {
                TempData["Hata"] = "Kod hatalý veya kullanýlmýþ.";
            }

            return RedirectToAction("Index");
        }

        // GEÇMÝÞ ÖDÜLLERÝM SAYFASI
        // GEÇMÝÞ ÖDÜLLERÝM SAYFASI
        public IActionResult Gecmisim()
        {
            // ESKÝ YÖNTEM (Mail ile buluyordu - HATA VERÝYOR)
            // var email = User.Claims.FirstOrDefault(c => System.Security.Claims.ClaimTypes.Email == c.Type)?.Value;
            // var musteri = _context.Musteriler.FirstOrDefault(m => m.Email == email);

            // YENÝ YÖNTEM (ID ile buluyor - GARANTÝ)
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "MusteriId");

            // Eðer ID yoksa (Giriþ yapmamýþsa)
            if (idClaim == null) return RedirectToAction("Giris", "Hesap");

            int id = int.Parse(idClaim.Value);

            // Sadece geçmiþi çekeceðimiz için müþteriyi bulmaya gerek yok, 
            // direkt ID üzerinden geçmiþ tablosuna sorgu atabiliriz.
            var gecmisListe = _context.GecmisIslemler
                                      .Where(x => x.MusteriId == id)
                                      .OrderByDescending(x => x.Tarih)
                                      .ToList();

            return View(gecmisListe);
        }
    }
}