using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZarzadzaniePrzychodniaWeterynaryjna.Migrations
{
    /// <inheritdoc />
    public partial class DodaniePozycjiWizyty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pozycja_Wizyty",
                columns: table => new
                {
                    id_pozycji_wizyty = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_wizyty = table.Column<int>(type: "int", nullable: false),
                    id_katalogu = table.Column<int>(type: "int", nullable: false),
                    Ilosc = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    Cena_Zastosowana = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    VAT_Zastosowany = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pozycja_Wizyty", x => x.id_pozycji_wizyty);
                    table.CheckConstraint("CHK_PozycjaWizyty_Cena", "[Cena_Zastosowana] >= 0");
                    table.CheckConstraint("CHK_PozycjaWizyty_Ilosc", "[Ilosc] > 0");
                    table.ForeignKey(
                        name: "FK_Pozycja_Wizyty_Katalog_id_katalogu",
                        column: x => x.id_katalogu,
                        principalTable: "Katalog",
                        principalColumn: "id_pozycji",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pozycja_Wizyty_Wizyta_Medyczna_id_wizyty",
                        column: x => x.id_wizyty,
                        principalTable: "Wizyta_Medyczna",
                        principalColumn: "id_wizyty",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pozycja_Wizyty_id_katalogu",
                table: "Pozycja_Wizyty",
                column: "id_katalogu");

            migrationBuilder.CreateIndex(
                name: "IX_Pozycja_Wizyty_id_wizyty",
                table: "Pozycja_Wizyty",
                column: "id_wizyty");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pozycja_Wizyty");
        }
    }
}
