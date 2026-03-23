using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JwtAuthentication.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BirthDay = table.Column<DateOnly>(type: "Date", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UserName = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Manager" },
                    { 3, "Staff" },
                    { 4, "User" },
                    { 5, "Guest" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Address", "BirthDay", "FullName", "Password", "UserName" },
                values: new object[,]
                {
                    { 1, "Hà Nội", new DateOnly(1995, 5, 12), "Nguyễn Văn An", "123456", "nguyenvanan" },
                    { 2, "TP Hồ Chí Minh", new DateOnly(1997, 8, 20), "Trần Thị Bình", "123456", "tranthibinh" },
                    { 3, "Đà Nẵng", new DateOnly(1993, 3, 15), "Lê Văn Cường", "123456", "levancuong" },
                    { 4, "Cần Thơ", new DateOnly(1999, 11, 2), "Phạm Thị Dung", "123456", "phamthidung" },
                    { 5, "Hải Phòng", new DateOnly(1990, 1, 25), "Hoàng Văn Em", "123456", "hoangvanem" },
                    { 6, "Nam Định", new DateOnly(1996, 6, 30), "Vũ Thị Hoa", "123456", "vuthihoa" },
                    { 7, "Bình Dương", new DateOnly(1994, 9, 18), "Đặng Văn Khoa", "123456", "dangvankhoa" },
                    { 8, "Nghệ An", new DateOnly(2000, 12, 5), "Bùi Thị Lan", "123456", "buithilan" },
                    { 9, "Khánh Hòa", new DateOnly(1992, 4, 10), "Phan Văn Minh", "123456", "phanvanminh" },
                    { 10, "Quảng Ninh", new DateOnly(1998, 7, 22), "Đỗ Thị Ngọc", "123456", "dothingoc" }
                });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 2, 2 },
                    { 3, 2 },
                    { 3, 3 },
                    { 4, 4 },
                    { 5, 5 },
                    { 1, 6 },
                    { 2, 6 },
                    { 2, 7 },
                    { 3, 7 },
                    { 4, 7 },
                    { 4, 8 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
