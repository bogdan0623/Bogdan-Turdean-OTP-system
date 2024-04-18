using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OTPBackend.Migrations
{
    /// <inheritdoc />
    public partial class tfa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TfsSecret",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TfsSecret",
                table: "Users");
        }
    }
}
