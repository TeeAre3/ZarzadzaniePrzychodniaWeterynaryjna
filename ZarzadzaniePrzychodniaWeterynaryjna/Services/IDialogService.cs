namespace ZarzadzaniePrzychodniaWeterynaryjna.Services
{
    public interface IDialogService
    {
        void ShowInformation(string message, string title = "Informacja");
        void ShowError(string message, string title = "Błąd");
        bool AskQuestion(string message, string title = "Potwierdzenie");
    }
}