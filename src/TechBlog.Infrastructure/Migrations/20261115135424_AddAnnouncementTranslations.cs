using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechBlog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAnnouncementTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnnouncementTranslations",
                columns: table => new
                {
                    AnnouncementId = table.Column<int>(type: "integer", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Button = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementTranslations", x => new { x.AnnouncementId, x.Language });
                    table.CheckConstraint("CK_AnnouncementTranslations_Language", "\"Language\" = 'en'");
                    table.ForeignKey(
                        name: "FK_AnnouncementTranslations_Announcements_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "Announcements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnouncementTranslations");
        }
    }
}
