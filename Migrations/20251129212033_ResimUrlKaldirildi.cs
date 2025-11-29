using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoyaltyRewardsApp.Migrations
{
    /// <inheritdoc />
    public partial class ResimUrlKaldirildi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResimUrl",
                table: "Oduller");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResimUrl",
                table: "Oduller",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
