using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Data.Migrations;

/// <inheritdoc />
public partial class AddServiceDataTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ServiceData",
            columns: table => new
            {
                Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Data = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ServiceData", x => x.Key);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ServiceData");
    }
}