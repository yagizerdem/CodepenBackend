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
            migrationBuilder.DropForeignKey(
                name: "FK_Relations_AspNetUsers_FollowingdId",
                table: "Relations");

            migrationBuilder.RenameColumn(
                name: "FollowingdId",
                table: "Relations",
                newName: "FollowingId");

            migrationBuilder.RenameIndex(
                name: "IX_Relations_FollowingdId",
                table: "Relations",
                newName: "IX_Relations_FollowingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Relations_AspNetUsers_FollowingId",
                table: "Relations",
                column: "FollowingId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Relations_AspNetUsers_FollowingId",
                table: "Relations");

            migrationBuilder.RenameColumn(
                name: "FollowingId",
                table: "Relations",
                newName: "FollowingdId");

            migrationBuilder.RenameIndex(
                name: "IX_Relations_FollowingId",
                table: "Relations",
                newName: "IX_Relations_FollowingdId");

            migrationBuilder.AddForeignKey(
                name: "FK_Relations_AspNetUsers_FollowingdId",
                table: "Relations",
                column: "FollowingdId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
