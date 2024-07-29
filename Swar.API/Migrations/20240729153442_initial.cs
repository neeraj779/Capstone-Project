using System;
using Microsoft.EntityFrameworkCore.Migrations;

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
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HashedPassword = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordHashKey = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    UserStatus = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "LikedSongs",
                columns: table => new
                {
                    LikeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SongId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LikedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
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
                    HistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SongId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
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
                    PlaylistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlaylistName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
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
                name: "UserUploadedSongs",
                columns: table => new
                {
                    SongId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SongName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Artist = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Album = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: false),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserUploadedSongs", x => x.SongId);
                    table.ForeignKey(
                        name: "FK_UserUploadedSongs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistSongs",
                columns: table => new
                {
                    PlaylistId = table.Column<int>(type: "int", nullable: false),
                    SongId = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                values: new object[] { 100, "admin@gmail.com", "Male", new byte[] { 105, 206, 164, 53, 222, 226, 133, 230, 244, 70, 164, 31, 104, 161, 46, 242, 108, 10, 234, 51, 129, 76, 145, 233, 217, 157, 211, 80, 199, 191, 253, 166, 141, 123, 224, 50, 227, 253, 192, 207, 107, 108, 133, 207, 20, 150, 79, 108, 138, 47, 95, 20, 241, 214, 220, 59, 22, 154, 175, 178, 41, 102, 241, 240 }, "admin", new byte[] { 254, 131, 17, 21, 216, 237, 51, 76, 205, 207, 186, 238, 14, 173, 45, 28, 6, 169, 14, 51, 199, 32, 227, 66, 84, 179, 22, 78, 71, 81, 155, 207, 158, 136, 149, 224, 255, 125, 68, 189, 166, 29, 200, 213, 26, 64, 241, 17, 185, 137, 92, 135, 154, 194, 249, 131, 214, 7, 212, 93, 190, 181, 85, 112, 184, 169, 47, 47, 33, 194, 105, 18, 132, 16, 93, 214, 112, 134, 154, 205, 139, 172, 153, 5, 6, 243, 239, 236, 96, 217, 135, 94, 205, 135, 131, 145, 31, 190, 221, 74, 9, 209, 255, 139, 95, 114, 178, 141, 234, 246, 3, 72, 52, 112, 119, 227, 21, 76, 159, 122, 35, 53, 110, 25, 162, 225, 201, 63 }, new DateTime(2024, 7, 29, 15, 34, 42, 215, DateTimeKind.Utc).AddTicks(6070), 0, 0 });

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

            migrationBuilder.CreateIndex(
                name: "IX_UserUploadedSongs_UserId",
                table: "UserUploadedSongs",
                column: "UserId");
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
                name: "UserUploadedSongs");

            migrationBuilder.DropTable(
                name: "Playlists");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
