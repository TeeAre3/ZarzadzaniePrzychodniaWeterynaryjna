using System.Windows;
using ZarzadzaniePrzychodniaWeterynaryjna.ViewModels;

namespace ZarzadzaniePrzychodniaWeterynaryjna
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}