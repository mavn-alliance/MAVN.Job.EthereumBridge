using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Job.EthereumBridge.MsSqlRepositories.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ethereum_bridge");

            migrationBuilder.CreateTable(
                name: "blocks_data",
                schema: "ethereum_bridge",
                columns: table => new
                {
                    key = table.Column<string>(nullable: false),
                    value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blocks_data", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "nonces",
                schema: "ethereum_bridge",
                columns: table => new
                {
                    master_wallet_address = table.Column<string>(nullable: false),
                    nonce = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nonces", x => x.master_wallet_address);
                });

            migrationBuilder.CreateTable(
                name: "operations",
                schema: "ethereum_bridge",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    transaction_hash = table.Column<string>(nullable: true),
                    transaction_data = table.Column<string>(nullable: true),
                    last_updated = table.Column<DateTime>(nullable: false),
                    operation_status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operations", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_operations_operation_status_last_updated",
                schema: "ethereum_bridge",
                table: "operations",
                columns: new[] { "operation_status", "last_updated" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blocks_data",
                schema: "ethereum_bridge");

            migrationBuilder.DropTable(
                name: "nonces",
                schema: "ethereum_bridge");

            migrationBuilder.DropTable(
                name: "operations",
                schema: "ethereum_bridge");
        }
    }
}
