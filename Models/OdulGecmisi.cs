using System.ComponentModel.DataAnnotations;

namespace LoyaltyRewardsApp.Models
{
    public class OdulGecmisi
    {
        public int Id { get; set; }

        public int MusteriId { get; set; } // Ödülü kim aldı?

        public string OdulAdi { get; set; } // Hangi ödülü aldı? (İsim olarak saklayalım, ödül silinse bile adı kalsın)

        public int HarcananPuan { get; set; } // Kaça aldı?

        public DateTime Tarih { get; set; } // Ne zaman aldı?
    }
}