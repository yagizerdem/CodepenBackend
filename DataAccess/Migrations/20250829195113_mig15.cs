using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class mig15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrivateChatMessageEntityId",
                table: "MediaWrapper",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaWrapper_PrivateChatMessageEntityId",
                table: "MediaWrapper",
                column: "PrivateChatMessageEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaWrapper_PrivateChatMessages_PrivateChatMessageEntityId",
                table: "MediaWrapper",
                column: "PrivateChatMessageEntityId",
                principalTable: "PrivateChatMessages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaWrapper_PrivateChatMessages_PrivateChatMessageEntityId",
                table: "MediaWrapper");

            migrationBuilder.DropIndex(
                name: "IX_MediaWrapper_PrivateChatMessageEntityId",
                table: "MediaWrapper");

            migrationBuilder.DropColumn(
                name: "PrivateChatMessageEntityId",
                table: "MediaWrapper");
        }
    }
}
