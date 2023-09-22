using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaikoroTravelBackend.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.CreateTable(
            name: "Instructions",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                StrId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                State = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_Instructions", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "Logs",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                LogLevel = table.Column<int>(type: "int", nullable: false),
                Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                User = table.Column<int>(type: "int", nullable: false),
                TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Writer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                WriterLine = table.Column<int>(type: "int", nullable: false),
                AdditionalObject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IsPosted = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_Logs", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "Lotteries",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                StrId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Value = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_Lotteries", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "LotterySyncs",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                StrId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                User = table.Column<int>(type: "int", nullable: false),
                Number = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_LotterySyncs", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "Routes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                StrId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                State = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_Routes", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "UserTokenInfos",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                User = table.Column<int>(type: "int", nullable: false),
                Token = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_UserTokenInfos", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropTable(
            name: "Instructions");

        _ = migrationBuilder.DropTable(
            name: "Logs");

        _ = migrationBuilder.DropTable(
            name: "Lotteries");

        _ = migrationBuilder.DropTable(
            name: "LotterySyncs");

        _ = migrationBuilder.DropTable(
            name: "Routes");

        _ = migrationBuilder.DropTable(
            name: "UserTokenInfos");
    }
}
