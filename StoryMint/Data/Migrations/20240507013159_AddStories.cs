using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoryMint.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    AgeGroup = table.Column<string>(type: "TEXT", nullable: false),
                    Genre = table.Column<int>(type: "INTEGER", nullable: false),
                    Tone = table.Column<int>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: false),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedById = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stories_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stories_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<long>(type: "INTEGER", nullable: false),
                    Voice = table.Column<string>(type: "TEXT", nullable: false),
                    Dimension = table.Column<string>(type: "TEXT", nullable: false),
                    StoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Deleted = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: false),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedById = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Media_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Media_AspNetUsers_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Media_Stories_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Stories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Media_CreatedById",
                table: "Media",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Media_LastModifiedById",
                table: "Media",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Media_StoryId",
                table: "Media",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Stories_CreatedById",
                table: "Stories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Stories_LastModifiedById",
                table: "Stories",
                column: "LastModifiedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Media");

            migrationBuilder.DropTable(
                name: "Stories");
        }
    }
}
