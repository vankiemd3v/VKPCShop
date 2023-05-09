using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VKPCShop.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixdb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParentCaregoryId",
                table: "Categories",
                newName: "ParentCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParentCategoryId",
                table: "Categories",
                newName: "ParentCaregoryId");
        }
    }
}
