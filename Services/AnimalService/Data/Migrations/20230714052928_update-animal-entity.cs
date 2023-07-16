using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AnimalService.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAnimalEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Animals",
                table: "Animals");

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "PublicId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "PublicId",
                keyValue: 2);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Animals",
                table: "Animals",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "Age", "Breed", "Color", "CoverImageUrl", "CreatedAt", "Description", "Name", "PublicId", "Sex", "Status", "Type", "UpdatedAt", "Weight" },
                values: new object[,]
                {
                    { new Guid("4b22475e-dc6d-40be-866f-32f6aae242a9"), 2, "Double doodle", "White", "https://placedog.net/500", new DateTime(2023, 7, 14, 5, 29, 28, 661, DateTimeKind.Utc).AddTicks(1420), "lorem ipsum", "Dee Dee", 1, "Female", 0, "Dog", new DateTime(2023, 7, 14, 5, 29, 28, 661, DateTimeKind.Utc).AddTicks(1420), 10 },
                    { new Guid("62aa9d4a-40ea-46ab-9186-789618d5e014"), 5, "Bengal cat", "Beige", "https://placekitten.com/200/200", new DateTime(2023, 7, 14, 5, 29, 28, 661, DateTimeKind.Utc).AddTicks(1450), "lorem ipsum", "Buttercup", 2, "Male", 0, "Cat", new DateTime(2023, 7, 14, 5, 29, 28, 661, DateTimeKind.Utc).AddTicks(1450), 5 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Animals",
                table: "Animals");

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("4b22475e-dc6d-40be-866f-32f6aae242a9"));

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: new Guid("62aa9d4a-40ea-46ab-9186-789618d5e014"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Animals",
                table: "Animals",
                column: "PublicId");

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "PublicId", "Age", "Breed", "Color", "CoverImageUrl", "CreatedAt", "Description", "Id", "Name", "Sex", "Status", "Type", "UpdatedAt", "Weight" },
                values: new object[,]
                {
                    { 1, 2, "Double doodle", "White", "https://placedog.net/500", new DateTime(2023, 7, 13, 21, 29, 35, 435, DateTimeKind.Utc).AddTicks(3290), "lorem ipsum", new Guid("db2afe99-0d4a-4504-a358-00017457d34e"), "Dee Dee", "Female", 0, "Dog", new DateTime(2023, 7, 13, 21, 29, 35, 435, DateTimeKind.Utc).AddTicks(3290), 10 },
                    { 2, 5, "Bengal cat", "Beige", "https://placekitten.com/200/200", new DateTime(2023, 7, 13, 21, 29, 35, 435, DateTimeKind.Utc).AddTicks(3320), "lorem ipsum", new Guid("98d42f39-103c-45bb-b911-8bdb72f4f6a8"), "Buttercup", "Male", 0, "Cat", new DateTime(2023, 7, 13, 21, 29, 35, 435, DateTimeKind.Utc).AddTicks(3320), 5 }
                });
        }
    }
}
