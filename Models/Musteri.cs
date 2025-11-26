using System.ComponentModel;

namespace LoyaltyRewardsApp.Models
{
    public class Musteri
    {
        public int Id { get; set; } // Her müşterinin benzersiz numarası

        [DisplayName("Adı Soyadı")]
        public string AdSoyad { get; set; } // Müşterinin adı
        [DisplayName("E-Posta Adresi")]
        public string Email { get; set; } // Giriş için e-posta
        [DisplayName("Puanı")]
        public int ToplamPuan { get; set; } = 0; // Başlangıç puanı 0 olsun
        [DisplayName("Şifresi")]
        public string Sifre { get; set; } // Giriş şifresi
        [DisplayName("Rolü")]
        public string Rol { get; set; } = "Uye"; // Varsayılan olarak herkes 'Uye' olsun. Admin'i elle yapacağız.

    }
}
