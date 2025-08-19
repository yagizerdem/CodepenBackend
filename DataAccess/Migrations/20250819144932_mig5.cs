using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class mig5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OldPenVersionsEntity_PenEntity_PenId",
                table: "OldPenVersionsEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_PenEntity_AspNetUsers_AuthorId",
                table: "PenEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PenEntity",
                table: "PenEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OldPenVersionsEntity",
                table: "OldPenVersionsEntity");

            migrationBuilder.RenameTable(
                name: "PenEntity",
                newName: "Pens");

            migrationBuilder.RenameTable(
                name: "OldPenVersionsEntity",
                newName: "OldPenVersions");

            migrationBuilder.RenameIndex(
                name: "IX_PenEntity_AuthorId",
                table: "Pens",
                newName: "IX_Pens_AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_OldPenVersionsEntity_PenId",
                table: "OldPenVersions",
                newName: "IX_OldPenVersions_PenId");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Pens",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pens",
                table: "Pens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OldPenVersions",
                table: "OldPenVersions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OldPenVersions_Pens_PenId",
                table: "OldPenVersions",
                column: "PenId",
                principalTable: "Pens",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Pens_AspNetUsers_AuthorId",
                table: "Pens",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OldPenVersions_Pens_PenId",
                table: "OldPenVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_Pens_AspNetUsers_AuthorId",
                table: "Pens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pens",
                table: "Pens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OldPenVersions",
                table: "OldPenVersions");

            migrationBuilder.RenameTable(
                name: "Pens",
                newName: "PenEntity");

            migrationBuilder.RenameTable(
                name: "OldPenVersions",
                newName: "OldPenVersionsEntity");

            migrationBuilder.RenameIndex(
                name: "IX_Pens_AuthorId",
                table: "PenEntity",
                newName: "IX_PenEntity_AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_OldPenVersions_PenId",
                table: "OldPenVersionsEntity",
                newName: "IX_OldPenVersionsEntity_PenId");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "PenEntity",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PenEntity",
                table: "PenEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OldPenVersionsEntity",
                table: "OldPenVersionsEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OldPenVersionsEntity_PenEntity_PenId",
                table: "OldPenVersionsEntity",
                column: "PenId",
                principalTable: "PenEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PenEntity_AspNetUsers_AuthorId",
                table: "PenEntity",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
