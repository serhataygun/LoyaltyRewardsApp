using System.ComponentModel.DataAnnotations;

namespace LoyaltyRewardsApp.Models
{
    public class OdulGecmisi
    {
        public int Id { get; set; }

        public int MusteriId { get; set; } 

        public string OdulAdi { get; set; } 

        public int HarcananPuan { get; set; } 

        public DateTime Tarih { get; set; } 
    }
}