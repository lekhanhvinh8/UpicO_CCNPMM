using Microsoft.EntityFrameworkCore.Migrations;

namespace Upico.Migrations
{
    public partial class AddIsWithdrawToMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWithDraw",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWithDraw",
                table: "Messages");
        }
    }
}
