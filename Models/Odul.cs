using System.ComponentModel;

namespace LoyaltyRewardsApp.Models
{
    public class Odul
    {
        public int Id { get; set; }
        [DisplayName("Ödül Başlığı")]
        public string Baslik { get; set; } // Örneğin: "Bedava Kahve"
        [DisplayName("Açıklama")]
        public string Aciklama { get; set; } // "Tüm kahve çeşitlerinde geçerli"
        [DisplayName("Gerekli Puan")]
        public int GerekliPuan { get; set; } // Bu ödülü almak için kaç puan lazım?
    }
}
