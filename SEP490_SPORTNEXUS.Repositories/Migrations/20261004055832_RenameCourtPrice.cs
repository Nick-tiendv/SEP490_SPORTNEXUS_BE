using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP490_SPORTNEXUS_BE.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class RenameCourtPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PricePerHour",
                table: "Courts",
                newName: "DefaultPrice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DefaultPrice",
                table: "Courts",
                newName: "PricePerHour");
        }
    }
}
