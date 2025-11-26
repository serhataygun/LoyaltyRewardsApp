using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Giriþ Sistemi Servisi (Bileklik Sistemi)
builder.Services.AddAuthentication("CerezSistemi")
    .AddCookie("CerezSistemi", options =>
    {
        options.LoginPath = "/Hesap/Giris";
        options.AccessDeniedPath = "/Hesap/Giris";

        // --- ÝÞTE SÝHÝRLÝ DOKUNUÞ ---
        // Guid.NewGuid() her seferinde rastgele upuzun bir kod üretir.
        // Proje her stop-start olduðunda isim deðiþir, eski bilet yanar.
        options.Cookie.Name = "Sadakat_" + Guid.NewGuid();
        // ----------------------------

        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

// Veritabaný servisini ekliyoruz
builder.Services.AddDbContext<LoyaltyRewardsApp.Data.UygulamaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LoyaltyApp")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // 2. Kimlik Kontrolü (Bilekliðe bakma)
app.UseAuthorization();  // 3. Yetki Kontrolü (VIP bölümüne girebilir mi?)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
