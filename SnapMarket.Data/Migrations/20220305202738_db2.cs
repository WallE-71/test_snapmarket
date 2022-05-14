using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SnapMarket.Data.Migrations
{
    public partial class db2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFile",
                table: "FileStores");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ImageFile",
                table: "FileStores",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
