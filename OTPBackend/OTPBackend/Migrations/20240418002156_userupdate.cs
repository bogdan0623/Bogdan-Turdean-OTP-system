using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OTPBackend.Migrations
{
    /// <inheritdoc />
    public partial class userupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TfsSecret",
                table: "Users",
                newName: "TfaSecret");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TfaSecret",
                table: "Users",
                newName: "TfsSecret");
        }
    }
}
