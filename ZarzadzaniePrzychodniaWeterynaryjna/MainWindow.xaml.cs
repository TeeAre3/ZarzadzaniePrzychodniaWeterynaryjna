using System.Windows;
using ZarzadzaniePrzychodniaWeterynaryjna.ViewModels;

namespace ZarzadzaniePrzychodniaWeterynaryjna
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}