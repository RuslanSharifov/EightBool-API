using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eight.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Tablse_TableId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Tablse_Venues_VenueId",
                table: "Tablse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tablse",
                table: "Tablse");

            migrationBuilder.RenameTable(
                name: "Tablse",
                newName: "BilliardTables");

            migrationBuilder.RenameIndex(
                name: "IX_Tablse_VenueId",
                table: "BilliardTables",
                newName: "IX_BilliardTables_VenueId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BilliardTables",
                table: "BilliardTables",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BilliardTables_Venues_VenueId",
                table: "BilliardTables",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_BilliardTables_TableId",
                table: "Sessions",
                column: "TableId",
                principalTable: "BilliardTables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BilliardTables_Venues_VenueId",
                table: "BilliardTables");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_BilliardTables_TableId",
                table: "Sessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BilliardTables",
                table: "BilliardTables");

            migrationBuilder.RenameTable(
                name: "BilliardTables",
                newName: "Tablse");

            migrationBuilder.RenameIndex(
                name: "IX_BilliardTables_VenueId",
                table: "Tablse",
                newName: "IX_Tablse_VenueId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tablse",
                table: "Tablse",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Tablse_TableId",
                table: "Sessions",
                column: "TableId",
                principalTable: "Tablse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tablse_Venues_VenueId",
                table: "Tablse",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
