using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrochureAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    StrGUID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StrCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StrFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StrFilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StrQRCode = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.StrGUID);
                });

            migrationBuilder.CreateTable(
                name: "CompanyProfiles",
                columns: table => new
                {
                    StrGUID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StrCompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StrDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StrAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StrPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StrEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StrWebsite = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StrLogoPath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProfiles", x => x.StrGUID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    StrGUID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StrName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StrEmailId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StrPassword = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BolIsAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.StrGUID);
                });

            migrationBuilder.CreateTable(
                name: "ClientForms",
                columns: table => new
                {
                    StrGUID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StrName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StrEmailId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StrPhoneNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StrCategoryGUID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientForms", x => x.StrGUID);
                    table.ForeignKey(
                        name: "FK_ClientForms_Categories_StrCategoryGUID",
                        column: x => x.StrCategoryGUID,
                        principalTable: "Categories",
                        principalColumn: "StrGUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_StrCategory",
                table: "Categories",
                column: "StrCategory",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientForms_StrCategoryGUID",
                table: "ClientForms",
                column: "StrCategoryGUID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StrEmailId",
                table: "Users",
                column: "StrEmailId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_StrName",
                table: "Users",
                column: "StrName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientForms");

            migrationBuilder.DropTable(
                name: "CompanyProfiles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
