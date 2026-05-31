using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZarzadzaniePrzychodniaWeterynaryjna.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Właściciel",
                columns: table => new
                {
                    id_właściciela = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Telefon = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Imię = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Nazwisko = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Nazwa_firmy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Data_rejestracji = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Właściciel", x => x.id_właściciela);
                    table.CheckConstraint("CHK_Wlasciciel_Dane", "([Imię] IS NOT NULL AND [Nazwisko] IS NOT NULL) OR [Nazwa_firmy] IS NOT NULL");
                });

            migrationBuilder.CreateTable(
                name: "Zwierzę",
                columns: table => new
                {
                    id_zwierzęcia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_właściciela = table.Column<int>(type: "int", nullable: false),
                    Imię = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Gatunek = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Rasa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Płeć = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Data_Urodzenia = table.Column<DateTime>(type: "datetime2", nullable: true),
                    waga = table.Column<decimal>(type: "decimal(5,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zwierzę", x => x.id_zwierzęcia);
                    table.CheckConstraint("CHK_Zwierze_Waga", "[waga] > 0");
                    table.ForeignKey(
                        name: "FK_Zwierzę_Właściciel_id_właściciela",
                        column: x => x.id_właściciela,
                        principalTable: "Właściciel",
                        principalColumn: "id_właściciela",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Właściciel_Email",
                table: "Właściciel",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Właściciel_Telefon",
                table: "Właściciel",
                column: "Telefon",
                unique: true,
                filter: "[Telefon] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Zwierzę_id_właściciela",
                table: "Zwierzę",
                column: "id_właściciela");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Zwierzę");

            migrationBuilder.DropTable(
                name: "Właściciel");
        }
    }
}
