using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Upico.Migrations
{
    public partial class AddChattingTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageHubs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReceiverId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageHubs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageHubs_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MessageHubs_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MessageHubId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_MessageHubs_MessageHubId",
                        column: x => x.MessageHubId,
                        principalTable: "MessageHubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageHubs_ReceiverId",
                table: "MessageHubs",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageHubs_SenderId_ReceiverId",
                table: "MessageHubs",
                columns: new[] { "SenderId", "ReceiverId" },
                unique: true,
                filter: "[SenderId] IS NOT NULL AND [ReceiverId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MessageHubId",
                table: "Messages",
                column: "MessageHubId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "MessageHubs");
        }
    }
}
