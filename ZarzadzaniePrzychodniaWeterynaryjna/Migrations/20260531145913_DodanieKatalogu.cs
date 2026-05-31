using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZarzadzaniePrzychodniaWeterynaryjna.Migrations
{
    /// <inheritdoc />
    public partial class DodanieKatalogu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Katalog",
                columns: table => new
                {
                    id_pozycji = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nazwa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Typ = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cena_Ewidencyjna = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    VAT = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Jednostka_Miary = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    czas_trwania_w_min = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Katalog", x => x.id_pozycji);
                    table.CheckConstraint("CHK_Katalog_Cena", "[Cena_Ewidencyjna] >= 0");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Katalog");
        }
    }
}
