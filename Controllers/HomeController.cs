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
            // 1. Giriþ yapan kiþiyi bul
            var girisYapanEmail = User.Claims.FirstOrDefault(c => System.Security.Claims.ClaimTypes.Email == c.Type)?.Value;

            // Eðer email yoksa (Giriþ düþmüþse) anasayfaya at
            if (string.IsNullOrEmpty(girisYapanEmail)) return RedirectToAction("Index");

            var musteri = _context.Musteriler.FirstOrDefault(m => m.Email == girisYapanEmail);
            var odul = _context.Oduller.Find(odulId);

            // Güvenlik: Müþteri veya ödül yoksa iptal
            if (musteri == null || odul == null) return RedirectToAction("Index");

            // 2. Yeterli Puan Var mý?
            if (musteri.ToplamPuan >= odul.GerekliPuan)
            {
                // A) Puaný Düþ
                musteri.ToplamPuan -= odul.GerekliPuan;

                // B) GEÇMÝÞE KAYDET (Ýþte burasý eksik olabilir)
                var yeniKayit = new OdulGecmisi
                {
                    MusteriId = musteri.Id,
                    OdulAdi = odul.Baslik,
                    HarcananPuan = odul.GerekliPuan,
                    Tarih = DateTime.Now // Þu anki saat
                };

                // Bu satýr çok önemli: "Bu kaydý veritabanýna ekle" diyoruz
                _context.GecmisIslemler.Add(yeniKayit);

                // C) Tüm deðiþiklikleri (Puan düþüþü + Geçmiþ kaydý) veritabanýna iþle
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

            // 1. Önce veritabanýndaki "Kullanýlmamýþ" tüm kodlarý çekelim.
            // .ToList() dediðimiz an veriler veritabanýndan gelir ve RAM'e yüklenir.
            var aktifKodlar = _context.PromosyonKodlari.Where(p => p.KullanildiMi == false).ToList();

            // 2. Þimdi RAM'deki bu liste üzerinde o özel karþýlaþtýrmayý yapalým.
            // Artýk veritabanýndan çýktýðýmýz için C#'ýn tüm özelliklerini (ToUpperInvariant) kullanabiliriz.
            var promosyon = aktifKodlar.FirstOrDefault(p =>
                p.Kod.Trim().ToUpperInvariant() == girilenKod.Trim().ToUpperInvariant());

            // Buradan sonrasý ayný kalacak (if promosyon != null ...)

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

        // GEÇMÝÞ ÖDÜLLERÝM SAYFASI
        public IActionResult Gecmisim()
        {
            // Giriþ yapan kullanýcýyý bul
            var email = User.Claims.FirstOrDefault(c => System.Security.Claims.ClaimTypes.Email == c.Type)?.Value;
            var musteri = _context.Musteriler.FirstOrDefault(m => m.Email == email);

            if (musteri == null) return RedirectToAction("Giris", "Hesap");

            // Bu müþteriye ait geçmiþ kayýtlarý bul ve tarihe göre (En yeni en üstte) sýrala
            var gecmisListe = _context.GecmisIslemler
                                      .Where(x => x.MusteriId == musteri.Id)
                                      .OrderByDescending(x => x.Tarih)
                                      .ToList();

            return View(gecmisListe);
        }
    }
}