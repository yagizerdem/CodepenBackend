using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class mig6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfilePictureId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProfilePictureId",
                table: "AspNetUsers",
                column: "ProfilePictureId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_MediaWrapper_ProfilePictureId",
                table: "AspNetUsers",
                column: "ProfilePictureId",
                principalTable: "MediaWrapper",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_MediaWrapper_ProfilePictureId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProfilePictureId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfilePictureId",
                table: "AspNetUsers");
        }
    }
}
