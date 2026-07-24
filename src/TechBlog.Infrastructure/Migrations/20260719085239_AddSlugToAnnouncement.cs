using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechBlog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugToAnnouncement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Announcements",
                type: "text",
                nullable: false,
                defaultValue: "");

            // Generate slugs from existing titles before adding the unique index
            migrationBuilder.Sql(@"
                UPDATE ""Announcements""
                SET ""Slug"" = regexp_replace(
                    regexp_replace(
                        regexp_replace(
                            lower(trim(
                                regexp_replace(
                                    regexp_replace(""Title"", '[^a-zA-Z0-9\s-]', '', 'g'),
                                '\s+', ' ', 'g')
                            )),
                        '\s', '-', 'g'),
                    '-{2,}', '-', 'g'),
                '^-|-$', '', 'g')
                WHERE ""Slug"" = '' OR ""Slug"" IS NULL;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_Slug",
                table: "Announcements",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Announcements_Slug",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Announcements");
        }
    }
}
