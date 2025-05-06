using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BackendTestTask.Database.Migrations
{
    public partial class DefaultMigr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Meteorites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Class = table.Column<string>(type: "text", nullable: false),
                    Mass = table.Column<double>(type: "double precision", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Fall = table.Column<string>(type: "text", nullable: false),
                    Reclat = table.Column<double>(type: "double precision", nullable: true),
                    Reclong = table.Column<double>(type: "double precision", nullable: true),
                    GeoType = table.Column<string>(type: "text", nullable: true),
                    GeoLongitude = table.Column<double>(type: "double precision", nullable: true),
                    GeoLatitude = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meteorites", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Meteorites_Class",
                table: "Meteorites",
                column: "Class");

            migrationBuilder.CreateIndex(
                name: "IX_Meteorites_Year",
                table: "Meteorites",
                column: "Year");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Meteorites");
        }
    }
}
