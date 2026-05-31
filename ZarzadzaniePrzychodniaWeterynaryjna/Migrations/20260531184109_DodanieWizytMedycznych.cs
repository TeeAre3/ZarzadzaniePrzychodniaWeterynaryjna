using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZarzadzaniePrzychodniaWeterynaryjna.Migrations
{
    /// <inheritdoc />
    public partial class DodanieWizytMedycznych : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Wizyta_Medyczna",
                columns: table => new
                {
                    id_wizyty = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_rezerwacji = table.Column<int>(type: "int", nullable: false),
                    Data_Wizyty = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Opis_Wywiadu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rozpoznanie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Zalecenia = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wizyta_Medyczna", x => x.id_wizyty);
                    table.ForeignKey(
                        name: "FK_Wizyta_Medyczna_Harmonogram_id_rezerwacji",
                        column: x => x.id_rezerwacji,
                        principalTable: "Harmonogram",
                        principalColumn: "id_rezerwacji",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wizyta_Medyczna_id_rezerwacji",
                table: "Wizyta_Medyczna",
                column: "id_rezerwacji");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Wizyta_Medyczna");
        }
    }
}
