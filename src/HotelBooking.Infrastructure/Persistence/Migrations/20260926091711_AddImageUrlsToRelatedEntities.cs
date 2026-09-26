using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBooking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlsToRelatedEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoomTypeImageUrl",
                table: "RoomTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttractionImageUrl",
                table: "NearbyAttractions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HotelImageUrl",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityImageUrl",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AmenityImageUrl",
                table: "Amenities",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomTypeImageUrl",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "AttractionImageUrl",
                table: "NearbyAttractions");

            migrationBuilder.DropColumn(
                name: "HotelImageUrl",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "CityImageUrl",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "AmenityImageUrl",
                table: "Amenities");
        }
    }
}
