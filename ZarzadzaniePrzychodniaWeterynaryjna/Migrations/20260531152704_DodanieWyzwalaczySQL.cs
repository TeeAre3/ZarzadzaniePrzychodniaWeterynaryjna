using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZarzadzaniePrzychodniaWeterynaryjna.Migrations
{
    /// <inheritdoc />
    public partial class DodanieWyzwalaczySQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TRIGGER trg_BlokadaKonfliktowCzasowych
                ON Harmonogram
                AFTER INSERT, UPDATE
                AS
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM Harmonogram h
                        JOIN inserted i ON h.id_rezerwacji != i.id_rezerwacji
                        WHERE (i.Planowana_Data_Rozpoczęcia < DATEADD(MINUTE, h.Szacowany_czas_trwania, h.Planowana_Data_Rozpoczęcia))
                        AND (DATEADD(MINUTE, i.Szacowany_czas_trwania, i.Planowana_Data_Rozpoczęcia) > h.Planowana_Data_Rozpoczęcia)
                    )
                    BEGIN
                        RAISERROR('Błąd: Wybrany termin nakłada się na inną zaplanowaną rezerwację w harmonogramie.', 16, 1);
                        ROLLBACK TRANSACTION;
                    END
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
