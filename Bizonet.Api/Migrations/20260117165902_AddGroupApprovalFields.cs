using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bizonet.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupApprovalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BizonetApproved",
                table: "Groups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "GroupLink",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BizonetApproved",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "GroupLink",
                table: "Groups");
        }
    }
}
