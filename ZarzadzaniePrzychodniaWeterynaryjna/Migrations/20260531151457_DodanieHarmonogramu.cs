using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZarzadzaniePrzychodniaWeterynaryjna.Migrations
{
    /// <inheritdoc />
    public partial class DodanieHarmonogramu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Harmonogram",
                columns: table => new
                {
                    id_rezerwacji = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_zwierzęcia = table.Column<int>(type: "int", nullable: false),
                    Planowana_Data_Rozpoczęcia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Szacowany_czas_trwania = table.Column<int>(type: "int", nullable: false),
                    Status_Wizyty = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Rzeczywisty_Czas_Rozpoczęcia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Rzeczywisty_Czas_Zakończenia = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Harmonogram", x => x.id_rezerwacji);
                    table.CheckConstraint("CHK_Czas_Rzeczywisty", "[Rzeczywisty_Czas_Rozpoczęcia] <= CURRENT_TIMESTAMP");
                    table.CheckConstraint("CHK_Czas_Zakon", "[Rzeczywisty_Czas_Zakończenia] >= [Rzeczywisty_Czas_Rozpoczęcia]");
                    table.ForeignKey(
                        name: "FK_Harmonogram_Zwierzę_id_zwierzęcia",
                        column: x => x.id_zwierzęcia,
                        principalTable: "Zwierzę",
                        principalColumn: "id_zwierzęcia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Harmonogram_id_zwierzęcia",
                table: "Harmonogram",
                column: "id_zwierzęcia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Harmonogram");
        }
    }
}
