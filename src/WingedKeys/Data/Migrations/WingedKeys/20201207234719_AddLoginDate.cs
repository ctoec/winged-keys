using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WingedKeys.Data.Migrations.WingedKeys
{
    public partial class AddLoginDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>("LastLogin", "AspNetUsers", nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("LastLogin", "AspNetUsers");
        }
    }
}
