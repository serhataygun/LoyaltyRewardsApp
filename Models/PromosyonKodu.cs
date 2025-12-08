using System.ComponentModel;

namespace LoyaltyRewardsApp.Models
{
    public class PromosyonKodu
    {
        public int Id { get; set; }

        [DisplayName("Code")]
        public string Kod { get; set; }

        [DisplayName("Point Value")]
        public int PuanDegeri { get; set; }

        [DisplayName("Used?")]
        public bool KullanildiMi { get; set; } = false;
    }
}