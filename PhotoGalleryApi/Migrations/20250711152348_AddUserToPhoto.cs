using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoGalleryApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Photos",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Photos");
        }
    }
}
