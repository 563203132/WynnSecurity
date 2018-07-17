using Microsoft.EntityFrameworkCore.Migrations;

namespace WynnSecurity.DataAccess.Migrations
{
    public partial class RenameCustomerEmailToEmailOnCustomerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomerEmail",
                table: "Customer",
                newName: "Email");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Customer",
                newName: "CustomerEmail");
        }
    }
}
