using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class mig4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PenComments_AspNetUsers_UserId",
                table: "PenComments");

            migrationBuilder.DropForeignKey(
                name: "FK_PenComments_Pens_PenId",
                table: "PenComments");

            migrationBuilder.AddForeignKey(
                name: "FK_PenComments_AspNetUsers_UserId",
                table: "PenComments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PenComments_Pens_PenId",
                table: "PenComments",
                column: "PenId",
                principalTable: "Pens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PenComments_AspNetUsers_UserId",
                table: "PenComments");

            migrationBuilder.DropForeignKey(
                name: "FK_PenComments_Pens_PenId",
                table: "PenComments");

            migrationBuilder.AddForeignKey(
                name: "FK_PenComments_AspNetUsers_UserId",
                table: "PenComments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PenComments_Pens_PenId",
                table: "PenComments",
                column: "PenId",
                principalTable: "Pens",
                principalColumn: "Id");
        }
    }
}
