using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBooking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeNearbyAttractionsStandalone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NearbyAttractions_Hotels_HotelId",
                table: "NearbyAttractions");

            migrationBuilder.DropIndex(
                name: "IX_NearbyAttractions_HotelId",
                table: "NearbyAttractions");

            migrationBuilder.DropColumn(
                name: "DistanceMeters",
                table: "NearbyAttractions");

            migrationBuilder.DropColumn(
                name: "HotelId",
                table: "NearbyAttractions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DistanceMeters",
                table: "NearbyAttractions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HotelId",
                table: "NearbyAttractions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_NearbyAttractions_HotelId",
                table: "NearbyAttractions",
                column: "HotelId");

            migrationBuilder.AddForeignKey(
                name: "FK_NearbyAttractions_Hotels_HotelId",
                table: "NearbyAttractions",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
