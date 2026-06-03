using System.Windows;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Services
{
    public class DialogService : IDialogService
    {
        public void ShowInformation(string message, string title = "Informacja")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowError(string message, string title = "Błąd")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public bool AskQuestion(string message, string title = "Potwierdzenie")
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }
    }
}