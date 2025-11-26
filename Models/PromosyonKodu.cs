using System.ComponentModel;

namespace LoyaltyRewardsApp.Models
{
    public class PromosyonKodu
    {
        public int Id { get; set; }

        // Kodun kendisi (Örn: "YAZ2025")
        [DisplayName("Kod")]
        public string Kod { get; set; }

        // Bu kod kaç puan kazandıracak?
        [DisplayName("Puan Değeri")]
        public int PuanDegeri { get; set; }

        // Kod kullanıldı mı? (Varsayılan: Hayır/False)
        [DisplayName("Kullanıldı Mı?")]
        public bool KullanildiMi { get; set; } = false;
    }
}