using Microsoft.EntityFrameworkCore.Migrations;
using Mirra_Orchestrator.Repository;

#nullable disable

namespace Mirra_Orchestrator.Repository.Migrations
{
    /// <inheritdoc />
    public class AddInstagramPlatformSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccessToken",
                table: "customer_platforms_configurations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalAccountId",
                table: "customer_platforms_configurations",
                type: "nvarchar(64)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "customer_platforms_configurations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "customer_platforms_configurations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "customer_platforms_configurations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: false);

            migrationBuilder.InsertData(
                table: "platforms",
                columns: new[] { "Id", "Name", "Prompt", "SummaryPrompt", "SystemPrompt", "CreatedAt" },
                values: new object[]
                {
                    2,
                    InstagramPlatformPrompts.Name,
                    InstagramPlatformPrompts.Prompt,
                    InstagramPlatformPrompts.SummaryPrompt,
                    InstagramPlatformPrompts.SystemPrompt,
                    new DateTime(2026, 4, 26, 12, 0, 0, DateTimeKind.Utc)
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "platforms",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "AccessToken",
                table: "customer_platforms_configurations");

            migrationBuilder.DropColumn(
                name: "ExternalAccountId",
                table: "customer_platforms_configurations");

            // Url/Username/Password are left nullable; reverting to NOT NULL may fail if Instagram rows exist.
        }
    }
}
