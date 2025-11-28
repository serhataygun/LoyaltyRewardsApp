using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoyaltyRewardsApp.Migrations
{
    /// <inheritdoc />
    public partial class GecmisTablosu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GecmisIslemler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MusteriId = table.Column<int>(type: "int", nullable: false),
                    OdulAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HarcananPuan = table.Column<int>(type: "int", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GecmisIslemler", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GecmisIslemler");
        }
    }
}
