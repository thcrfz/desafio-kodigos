using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OsApi.Migrations
{
    /// <inheritdoc />
    public partial class AddChecklistItemOptionalToFoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChecklistItemId",
                table: "OSFotos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OSFotos_ChecklistItemId",
                table: "OSFotos",
                column: "ChecklistItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_OSFotos_ChecklistItems_ChecklistItemId",
                table: "OSFotos",
                column: "ChecklistItemId",
                principalTable: "ChecklistItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OSFotos_ChecklistItems_ChecklistItemId",
                table: "OSFotos");

            migrationBuilder.DropIndex(
                name: "IX_OSFotos_ChecklistItemId",
                table: "OSFotos");

            migrationBuilder.DropColumn(
                name: "ChecklistItemId",
                table: "OSFotos");
        }
    }
}
