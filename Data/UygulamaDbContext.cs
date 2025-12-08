using Microsoft.EntityFrameworkCore; 
using LoyaltyRewardsApp.Models; 

namespace LoyaltyRewardsApp.Data
{
    public class UygulamaDbContext : DbContext
    {
        public UygulamaDbContext(DbContextOptions<UygulamaDbContext> options) : base(options)
        {
        }

        public DbSet<Musteri> Musteriler { get; set; }
        public DbSet<Odul> Oduller { get; set; }
        public DbSet<PromosyonKodu> PromosyonKodlari { get; set; }
        public DbSet<OdulGecmisi> GecmisIslemler { get; set; }
    }
}