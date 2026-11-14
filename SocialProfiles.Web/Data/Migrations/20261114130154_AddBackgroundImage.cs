using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialProfiles.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddBackgroundImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackgroundImagePath",
                table: "Profiles",
                type: "TEXT",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundImagePath",
                table: "Profiles");
        }
    }
}
