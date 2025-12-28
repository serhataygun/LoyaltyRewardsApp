using Microsoft.AspNetCore.Mvc;
using LoyaltyRewardsApp.Data;   
using LoyaltyRewardsApp.Models; 

namespace LoyaltyRewardsApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly UygulamaDbContext _context;

        public HomeController(UygulamaDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var girisYapanEmail = User.Claims.FirstOrDefault(c => System.Security.Claims.ClaimTypes.Email == c.Type)?.Value;

            var musteri = _context.Musteriler.FirstOrDefault(m => m.Email == girisYapanEmail);

            if (musteri == null)
            {
                musteri = new Musteri { AdSoyad = "Guest (Please Log In)", ToplamPuan = 0 };
            }

            ViewBag.Oduller = _context.Oduller.OrderBy(x => x.GerekliPuan).ToList();  //puana göre sýralama

            return View(musteri);
        }

        public IActionResult Odullerimiz()
        {
            var girisYapanEmail = User.Claims.FirstOrDefault(c => System.Security.Claims.ClaimTypes.Email == c.Type)?.Value;
            var musteri = _context.Musteriler.FirstOrDefault(m => m.Email == girisYapanEmail);

            ViewBag.Oduller = _context.Oduller.OrderBy(x => x.GerekliPuan).ToList();

            return View(musteri);
        }

        [HttpPost]
        public IActionResult OdulKullan(int odulId)
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "MusteriId");
            if (idClaim == null) return RedirectToAction("Giris", "Hesap");

            int id = int.Parse(idClaim.Value);
            var musteri = _context.Musteriler.Find(id);
            var odul = _context.Oduller.Find(odulId);

            if (musteri == null || odul == null) return RedirectToAction("Index");

            // Kullanýcýnýn puaný, ödülün puanýndan büyük veya eþitse iþlem yapýlýr.

            if (musteri.ToplamPuan >= odul.GerekliPuan)
            {
                musteri.ToplamPuan -= odul.GerekliPuan;

                var yeniKayit = new OdulGecmisi
                {
                    MusteriId = musteri.Id,
                    OdulAdi = odul.Baslik,
                    HarcananPuan = odul.GerekliPuan,
                    Tarih = DateTime.Now
                };
                _context.GecmisIslemler.Add(yeniKayit);

                //SQL kodu yazmadan C# ile veritabanýna kayýt

                _context.SaveChanges();
                TempData["Mesaj"] = "Congratulations, you've received your reward! You can view your rewards on the history page.";
            }
            else   //Puan yetmezse iþlem iptal edilir.
            {
                TempData["Hata"] = "Your score is insufficient!";
            }

            return RedirectToAction("Index"); 
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PuanKazan(string girilenKod)
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "MusteriId");
            if (idClaim == null) return RedirectToAction("Giris", "Hesap");

            int id = int.Parse(idClaim.Value);
            var musteri = _context.Musteriler.Find(id);

            if (musteri == null) return RedirectToAction("Giris", "Hesap");

            var aktifKodlar = _context.PromosyonKodlari.Where(p => p.KullanildiMi == false).ToList();
            var promosyon = aktifKodlar.FirstOrDefault(p =>
                string.Equals(p.Kod.Trim(), girilenKod.Trim(), StringComparison.OrdinalIgnoreCase));

            if (promosyon != null)
            {
                musteri.ToplamPuan += promosyon.PuanDegeri;
                //promosyon used döner
                promosyon.KullanildiMi = true;
                _context.SaveChanges();
                TempData["Mesaj"] = $"Congratulations! You earned {promosyon.PuanDegeri} points.";
            }
            else
            {
                TempData["Hata"] = "The code is incorrect or has been used.";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Gecmisim()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "MusteriId");

            if (idClaim == null) return RedirectToAction("Giris", "Hesap");

            int id = int.Parse(idClaim.Value);

            var gecmisListe = _context.GecmisIslemler
                                      .Where(x => x.MusteriId == id)
                                      .OrderByDescending(x => x.Tarih)  
                                      .ToList();

            return View(gecmisListe);
        }
    }
}