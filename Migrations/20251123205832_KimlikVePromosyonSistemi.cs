using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoyaltyRewardsApp.Migrations
{
    /// <inheritdoc />
    public partial class KimlikVePromosyonSistemi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Rol",
                table: "Musteriler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sifre",
                table: "Musteriler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PromosyonKodlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PuanDegeri = table.Column<int>(type: "int", nullable: false),
                    KullanildiMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromosyonKodlari", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromosyonKodlari");

            migrationBuilder.DropColumn(
                name: "Rol",
                table: "Musteriler");

            migrationBuilder.DropColumn(
                name: "Sifre",
                table: "Musteriler");
        }
    }
}
