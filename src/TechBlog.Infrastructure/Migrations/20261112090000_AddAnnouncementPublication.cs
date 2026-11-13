using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechBlog.Infrastructure.Migrations;

public partial class AddAnnouncementPublication : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "PublishedAt", table: "Announcements",
            type: "timestamp with time zone", nullable: true);
        migrationBuilder.Sql("""
            UPDATE "Announcements" SET "PublishedAt" = "DateCreated" WHERE "Active";
            """);
        migrationBuilder.AlterColumn<bool>(
            name: "Active", table: "Announcements", type: "boolean",
            nullable: false, defaultValue: false,
            oldClrType: typeof(bool), oldType: "boolean");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<bool>(
            name: "Active", table: "Announcements", type: "boolean",
            nullable: false, oldClrType: typeof(bool), oldType: "boolean", oldDefaultValue: false);
        migrationBuilder.DropColumn(name: "PublishedAt", table: "Announcements");
    }
}
