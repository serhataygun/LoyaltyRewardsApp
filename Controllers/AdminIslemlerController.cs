using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoyaltyRewardsApp.Data;
using LoyaltyRewardsApp.Models;

namespace LoyaltyRewardsApp.Controllers
{
    [Authorize(Roles = "Admin")] // Sadece Admin girebilir
    public class AdminIslemlerController : Controller
    {
        private readonly UygulamaDbContext _context;

        public AdminIslemlerController(UygulamaDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Tüm geçmiş işlemleri tarihe göre (en yeni en üstte) çekelim
            // Burada küçük bir LINQ join işlemi yapacağız ki Müşterinin adını da görelim
            var islemler = (from islem in _context.GecmisIslemler
                            join musteri in _context.Musteriler on islem.MusteriId equals musteri.Id
                            select new IslemViewModel // Yeni bir sanal model kullanacağız
                            {
                                Id = islem.Id,
                                MusteriAdi = musteri.AdSoyad,
                                MusteriEmail = musteri.Email,
                                OdulAdi = islem.OdulAdi,
                                HarcananPuan = islem.HarcananPuan,
                                Tarih = islem.Tarih
                            })
                            .OrderByDescending(x => x.Tarih)
                            .ToList();

            return View(islemler);
        }
    }

    // Bu sayfa için özel, sadece veri taşımaya yarayan küçük bir model
    public class IslemViewModel
    {
        public int Id { get; set; }
        public string MusteriAdi { get; set; }
        public string MusteriEmail { get; set; }
        public string OdulAdi { get; set; }
        public int HarcananPuan { get; set; }
        public DateTime Tarih { get; set; }
    }
}