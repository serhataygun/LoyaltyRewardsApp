using System.ComponentModel;

namespace LoyaltyRewardsApp.Models
{
    public class Odul
    {
        public int Id { get; set; }
        [DisplayName("Reward Title")]
        public string Baslik { get; set; } 
        [DisplayName("Description")]
        public string Aciklama { get; set; } 
        [DisplayName("Required Points")]
        public int GerekliPuan { get; set; } 
    }
}
