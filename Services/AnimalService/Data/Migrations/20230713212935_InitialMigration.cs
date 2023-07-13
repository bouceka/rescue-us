using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AnimalService.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Animals",
                columns: table => new
                {
                    PublicId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Breed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weight = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoverImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animals", x => x.PublicId);
                });

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "PublicId", "Age", "Breed", "Color", "CoverImageUrl", "CreatedAt", "Description", "Id", "Name", "Sex", "Status", "Type", "UpdatedAt", "Weight" },
                values: new object[,]
                {
                    { 1, 2, "Double doodle", "White", "https://placedog.net/500", new DateTime(2023, 7, 13, 21, 29, 35, 435, DateTimeKind.Utc).AddTicks(3290), "lorem ipsum", new Guid("db2afe99-0d4a-4504-a358-00017457d34e"), "Dee Dee", "Female", 0, "Dog", new DateTime(2023, 7, 13, 21, 29, 35, 435, DateTimeKind.Utc).AddTicks(3290), 10 },
                    { 2, 5, "Bengal cat", "Beige", "https://placekitten.com/200/200", new DateTime(2023, 7, 13, 21, 29, 35, 435, DateTimeKind.Utc).AddTicks(3320), "lorem ipsum", new Guid("98d42f39-103c-45bb-b911-8bdb72f4f6a8"), "Buttercup", "Male", 0, "Cat", new DateTime(2023, 7, 13, 21, 29, 35, 435, DateTimeKind.Utc).AddTicks(3320), 5 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Animals");
        }
    }
}
