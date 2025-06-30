using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ibuy.Migrations
{
    /// <inheritdoc />
    public partial class AddSoldAtToItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SoldAt",
                table: "Items",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoldAt",
                table: "Items");
        }
    }
}
