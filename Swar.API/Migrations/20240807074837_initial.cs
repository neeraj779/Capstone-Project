using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Swar.API.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    HashedPassword = table.Column<byte[]>(type: "bytea", nullable: false),
                    PasswordHashKey = table.Column<byte[]>(type: "bytea", nullable: false),
                    UserStatus = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "LikedSongs",
                columns: table => new
                {
                    LikeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SongId = table.Column<string>(type: "text", nullable: false),
                    LikedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikedSongs", x => x.LikeId);
                    table.ForeignKey(
                        name: "FK_LikedSongs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayHistories",
                columns: table => new
                {
                    HistoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SongId = table.Column<string>(type: "text", nullable: false),
                    PlayedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayHistories", x => x.HistoryId);
                    table.ForeignKey(
                        name: "FK_PlayHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Playlists",
                columns: table => new
                {
                    PlaylistId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlaylistName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsPrivate = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlists", x => x.PlaylistId);
                    table.ForeignKey(
                        name: "FK_Playlists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistSongs",
                columns: table => new
                {
                    PlaylistId = table.Column<int>(type: "integer", nullable: false),
                    SongId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistSongs", x => new { x.PlaylistId, x.SongId });
                    table.ForeignKey(
                        name: "FK_PlaylistSongs_Playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlists",
                        principalColumn: "PlaylistId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Gender", "HashedPassword", "Name", "PasswordHashKey", "RegistrationDate", "Role", "UserStatus" },
                values: new object[] { 100, "admin@gmail.com", "Male", new byte[] { 255, 237, 216, 186, 55, 119, 53, 21, 24, 101, 208, 93, 160, 173, 28, 92, 104, 86, 100, 161, 40, 15, 43, 19, 174, 177, 229, 107, 154, 235, 181, 193, 58, 49, 152, 115, 11, 135, 40, 131, 10, 205, 223, 195, 82, 155, 8, 121, 175, 146, 253, 243, 192, 129, 58, 171, 199, 171, 51, 124, 16, 198, 235, 155 }, "admin", new byte[] { 104, 13, 235, 222, 119, 45, 76, 170, 192, 255, 83, 106, 166, 215, 13, 249, 253, 190, 26, 142, 96, 178, 243, 128, 97, 239, 44, 197, 176, 178, 230, 83, 37, 86, 98, 168, 69, 7, 46, 41, 19, 115, 20, 25, 50, 50, 172, 254, 125, 159, 155, 119, 249, 177, 103, 18, 70, 58, 235, 202, 95, 169, 224, 63, 80, 178, 24, 2, 24, 107, 158, 132, 170, 224, 124, 58, 37, 152, 15, 168, 105, 186, 76, 224, 57, 86, 84, 122, 93, 38, 178, 110, 162, 201, 96, 170, 225, 237, 28, 85, 131, 59, 198, 141, 146, 94, 111, 116, 242, 85, 55, 246, 68, 5, 240, 191, 197, 107, 118, 227, 234, 105, 43, 237, 35, 116, 115, 99 }, new DateTime(2024, 8, 7, 7, 48, 37, 231, DateTimeKind.Utc).AddTicks(2065), 0, 0 });

            migrationBuilder.CreateIndex(
                name: "IX_LikedSongs_UserId_SongId",
                table: "LikedSongs",
                columns: new[] { "UserId", "SongId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayHistories_UserId",
                table: "PlayHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_UserId",
                table: "Playlists",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LikedSongs");

            migrationBuilder.DropTable(
                name: "PlayHistories");

            migrationBuilder.DropTable(
                name: "PlaylistSongs");

            migrationBuilder.DropTable(
                name: "Playlists");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
