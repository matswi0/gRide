using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gRide.Migrations
{
    public partial class AppUserRegisterMethodField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChosenRegisterMethod",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChosenRegisterMethod",
                table: "AspNetUsers");
        }
    }
}
