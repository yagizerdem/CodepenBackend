using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PenLikeEntity_AspNetUsers_UserId",
                table: "PenLikeEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_PenLikeEntity_Pens_PenId",
                table: "PenLikeEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PenLikeEntity",
                table: "PenLikeEntity");

            migrationBuilder.RenameTable(
                name: "PenLikeEntity",
                newName: "PenLikes");

            migrationBuilder.RenameIndex(
                name: "IX_PenLikeEntity_UserId",
                table: "PenLikes",
                newName: "IX_PenLikes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PenLikeEntity_PenId",
                table: "PenLikes",
                newName: "IX_PenLikes_PenId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PenLikes",
                table: "PenLikes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PenLikes_AspNetUsers_UserId",
                table: "PenLikes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PenLikes_Pens_PenId",
                table: "PenLikes",
                column: "PenId",
                principalTable: "Pens",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PenLikes_AspNetUsers_UserId",
                table: "PenLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_PenLikes_Pens_PenId",
                table: "PenLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PenLikes",
                table: "PenLikes");

            migrationBuilder.RenameTable(
                name: "PenLikes",
                newName: "PenLikeEntity");

            migrationBuilder.RenameIndex(
                name: "IX_PenLikes_UserId",
                table: "PenLikeEntity",
                newName: "IX_PenLikeEntity_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PenLikes_PenId",
                table: "PenLikeEntity",
                newName: "IX_PenLikeEntity_PenId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PenLikeEntity",
                table: "PenLikeEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PenLikeEntity_AspNetUsers_UserId",
                table: "PenLikeEntity",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PenLikeEntity_Pens_PenId",
                table: "PenLikeEntity",
                column: "PenId",
                principalTable: "Pens",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
