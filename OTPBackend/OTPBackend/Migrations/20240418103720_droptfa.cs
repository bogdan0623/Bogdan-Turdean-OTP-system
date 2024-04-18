using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OTPBackend.Migrations
{
    /// <inheritdoc />
    public partial class droptfa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TfaSecret",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TfaSecret",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
