using Microsoft.EntityFrameworkCore; // Bu satırın altı çiziliyse ampule tıklayıp ekleyin
using LoyaltyRewardsApp.Models; // Kendi proje isminizle değiştirin

namespace LoyaltyRewardsApp.Data
{
    public class UygulamaDbContext : DbContext
    {
        public UygulamaDbContext(DbContextOptions<UygulamaDbContext> options) : base(options)
        {
        }

        // Veritabanında oluşacak tabloları buraya yazıyoruz
        public DbSet<Musteri> Musteriler { get; set; }
        public DbSet<Odul> Oduller { get; set; }
        public DbSet<PromosyonKodu> PromosyonKodlari { get; set; }
    }
}