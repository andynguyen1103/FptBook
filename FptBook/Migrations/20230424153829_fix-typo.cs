using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FptBook.Migrations
{
    /// <inheritdoc />
    public partial class fixtypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tittle",
                table: "Books",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Sumary",
                table: "Books",
                newName: "Summary");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Books",
                newName: "Tittle");

            migrationBuilder.RenameColumn(
                name: "Summary",
                table: "Books",
                newName: "Sumary");
        }
    }
}
