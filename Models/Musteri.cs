using System.ComponentModel;

namespace LoyaltyRewardsApp.Models
{
    public class Musteri
    {
        public int Id { get; set; } 

        [DisplayName("Name and Surname")]
        public string AdSoyad { get; set; } 
        [DisplayName("E-mail Address")]
        public string Email { get; set; }
        [DisplayName("Points")]
        public int ToplamPuan { get; set; } = 0; 
        [DisplayName("Password")]
        public string Sifre { get; set; } 
        [DisplayName("Rol")]
        public string Rol { get; set; } = "Uye"; 

    }
}
