using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Toplanti.Models;

namespace Toplanti.Dialogs;

public partial class AttendanceDialog : Window
{
    private ObservableCollection<UnitViewModel> _unitViewModels = [];
    public List<Unit> SelectedUnits { get; private set; } = [];

    public AttendanceDialog(List<Unit> units)
    {
        InitializeComponent();
        _unitViewModels = new ObservableCollection<UnitViewModel>(
            units.Select(u => new UnitViewModel
            {
                Unit = u,
                IsSelected = false
            })
        );
        dgUnits.ItemsSource = _unitViewModels;
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
        SelectedUnits = _unitViewModels
            .Where(vm => vm.IsSelected)
            .Select(vm => vm.Unit)
            .ToList();
            
        if (SelectedUnits.Count == 0)
        {
            MessageBox.Show("Lutfen en az bir birim secin.", "Uyari", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        DialogResult = true;
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void CheckBox_Click(object sender, RoutedEventArgs e)
    {
        // Checkbox tıklaması binding ile hallediliyor, event'i handle et ki DataGrid selection tetiklenmesin
        if (sender is CheckBox checkBox)
        {
            e.Handled = true;
        }
    }

    private void DgUnits_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        // Checkbox'a tıklanırsa işleme müdahale etme
        if (e.OriginalSource is System.Windows.Controls.Primitives.ToggleButton)
        {
            return;
        }

        // Satıra tıklandığında checkbox'ı toggle et ve satırı seç
        var dependencyObject = (System.Windows.DependencyObject)e.OriginalSource;
        var row = FindParent<DataGridRow>(dependencyObject);
        if (row != null && row.Item is UnitViewModel viewModel)
        {
            // Checkbox'ı toggle et
            viewModel.IsSelected = !viewModel.IsSelected;
            
            // Satırı seç veya seçimi kaldır
            if (viewModel.IsSelected)
            {
                dgUnits.SelectedItem = viewModel;
            }
            else
            {
                dgUnits.SelectedItem = null;
            }
            
            e.Handled = true;
        }
    }

    private static T? FindParent<T>(System.Windows.DependencyObject child) where T : System.Windows.DependencyObject
    {
        var parentObject = System.Windows.Media.VisualTreeHelper.GetParent(child);
        if (parentObject == null) return null;
        if (parentObject is T parent) return parent;
        return FindParent<T>(parentObject);
    }

    private void DgUnits_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // SelectionChanged event'i satır seçimini yönetmek için kullanılıyor
        // PreviewMouseDown'da zaten checkbox toggle edildiği için burada ek işlem yapmıyoruz
    }

    private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var viewModel in _unitViewModels)
        {
            viewModel.IsSelected = true;
        }
    }

    private void BtnDeselectAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var viewModel in _unitViewModels)
        {
            viewModel.IsSelected = false;
        }
    }

    private class UnitViewModel : INotifyPropertyChanged
    {
        public Unit Unit { get; set; } = null!;
        
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        // Unit özelliklerine erişim için proxy properties
        public string Number => Unit.Number;
        public string FirstName => Unit.FirstName ?? "";
        public string LastName => Unit.LastName ?? "";
        public string Phone => Unit.Phone ?? "";
        public decimal LandShare => Unit.LandShare;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

