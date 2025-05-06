using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hopmate.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionforPenalty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "penalty",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "penalty");
        }
    }
}
