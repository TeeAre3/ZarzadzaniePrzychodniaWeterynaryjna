using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Views
{
    public partial class ClientsPatientsView : UserControl
    {
        private readonly Dictionary<string, string> _originalValues = [];
        private readonly HashSet<string> _dirtyCells = [];
        private static readonly Regex WeightRegex = MyRegex();

        public ClientsPatientsView()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<ClientChangedMessage>(this, (r, m) => ClearTracking());
            WeakReferenceMessenger.Default.Register<PatientChangedMessage>(this, (r, m) => ClearTracking());
        }

        private void ClearTracking()
        {
            _originalValues.Clear();
            _dirtyCells.Clear();
        }

        private void Weight_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = WeightRegex.IsMatch(e.Text);
        }

        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            if (_dirtyCells.Count > 0)
            {
                MessageBox.Show("Masz niezapisane zmiany w tabeli!\nZapisz je lub cofnij przed sortowaniem kolumn.", "Blokada sortowania", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Handled = true; 
            }
        }

        private void DataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            if (e.EditingElement is TextBox textBox)
            {
                string key = $"{e.Row.Item.GetHashCode()}_{e.Column.Header}";
                if (!_originalValues.ContainsKey(key))
                {
                    _originalValues[key] = textBox.Text;
                }
            }
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var cell = GetParent<DataGridCell>(e.EditingElement);
                if (cell != null && e.EditingElement is TextBox textBox)
                {
                    string key = $"{e.Row.Item.GetHashCode()}_{e.Column.Header}";

                    if (_originalValues.ContainsKey(key))
                    {
                        if (_originalValues[key] != textBox.Text)
                        {
                            cell.Background = new SolidColorBrush(Color.FromRgb(255, 243, 205));
                            cell.Foreground = Brushes.Black;

                            _dirtyCells.Add(key);
                        }
                        else
                        {
                            cell.ClearValue(Control.BackgroundProperty);
                            cell.ClearValue(Control.ForegroundProperty);

                            _dirtyCells.Remove(key);
                        }
                    }
                }
            }
        }
        private T? GetParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            if (parentObject is T parent) return parent;
            return GetParent<T>(parentObject);
        }

        [GeneratedRegex("[^0-9,.]+", RegexOptions.Compiled)]
        private static partial Regex MyRegex();
    }
}