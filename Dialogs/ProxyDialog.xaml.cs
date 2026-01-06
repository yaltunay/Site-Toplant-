using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Toplanti.Models;
using Toplanti.Services;

namespace Toplanti.Dialogs;

public partial class ProxyDialog : Window
{
    private ObservableCollection<UnitViewModel> _giverViewModels = [];
    private readonly int _totalUnitCount;
    private readonly decimal _totalLandShare;
    private readonly int _currentProxyCount;
    private readonly decimal _currentProxyLandShare;
    private readonly ProxyService _proxyService = new();
    
    public List<Unit> GiverUnits { get; private set; } = [];
    public Unit? ReceiverUnit { get; private set; }
    public string? ReceiverName { get; private set; }
    public string? ReceiverPhone { get; private set; }

    private readonly List<Proxy> _existingProxies;
    
    public ProxyDialog(List<Unit> units, int totalUnitCount, decimal totalLandShare, int currentProxyCount, decimal currentProxyLandShare, List<Proxy> existingProxies)
    {
        InitializeComponent();
        _totalUnitCount = totalUnitCount;
        _totalLandShare = totalLandShare;
        _currentProxyCount = currentProxyCount;
        _currentProxyLandShare = currentProxyLandShare;
        _existingProxies = existingProxies ?? [];
        
        _giverViewModels = new ObservableCollection<UnitViewModel>(
            units.Select(u => new UnitViewModel
            {
                Unit = u,
                IsSelected = false
            })
        );
        dgGiverUnits.ItemsSource = _giverViewModels;
        
        // Populate receiver dropdown with units and "Other" option
        var receiverItems = new List<ReceiverItem>();
        receiverItems.AddRange(units.Select(u => new ReceiverItem { Unit = u, IsOther = false }));
        receiverItems.Add(new ReceiverItem { Unit = null, IsOther = true });
        cmbReceiver.ItemsSource = receiverItems;
        
        UpdateProxyLimitInfo();
    }

    private void UpdateProxyLimitInfo()
    {
        var selectedUnits = _giverViewModels
            .Where(vm => vm.IsSelected)
            .Select(vm => vm.Unit)
            .ToList();
            
        var selectedCount = selectedUnits.Count;
        var selectedLandShare = selectedUnits.Sum(u => u.LandShare);
        
        var newTotalProxyCount = _currentProxyCount + selectedCount;
        var newTotalProxyLandShare = _currentProxyLandShare + selectedLandShare;
        
        var maxProxies = _proxyService.CalculateMaxProxyCount(_totalUnitCount);
        var maxProxyLandShare = _proxyService.CalculateMaxProxyLandShare(_totalLandShare);
        
        var countExceeded = newTotalProxyCount > maxProxies;
        var landShareExceeded = newTotalProxyLandShare > maxProxyLandShare;
        
        if (countExceeded || landShareExceeded)
        {
            var message = "UYARI: Yasal limit asildi!\n";
            if (countExceeded)
            {
                message += $"Sayi: {newTotalProxyCount}/{maxProxies} (KMK 31)\n";
            }
            if (landShareExceeded)
            {
                message += $"Arsa Payi: {newTotalProxyLandShare:F2}/{maxProxyLandShare:F2} (KMK 31)";
            }
            txtProxyLimitInfo.Text = message;
            txtProxyLimitInfo.Foreground = System.Windows.Media.Brushes.Red;
        }
        else if (selectedCount > 0)
        {
            txtProxyLimitInfo.Text = $"Sayi: {newTotalProxyCount}/{maxProxies}, Arsa Payi: {newTotalProxyLandShare:F2}/{maxProxyLandShare:F2} (KMK 31)";
            txtProxyLimitInfo.Foreground = System.Windows.Media.Brushes.Green;
        }
        else
        {
            txtProxyLimitInfo.Text = $"Mevcut: Sayi {_currentProxyCount}/{maxProxies}, Arsa Payi {_currentProxyLandShare:F2}/{maxProxyLandShare:F2} (KMK 31)";
            txtProxyLimitInfo.Foreground = System.Windows.Media.Brushes.Black;
        }
    }

    private void CheckBox_Click(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox)
        {
            e.Handled = true;
            UpdateProxyLimitInfo();
        }
    }

    private void DgGiverUnits_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.OriginalSource is System.Windows.Controls.Primitives.ToggleButton)
        {
            return;
        }

        var dependencyObject = (System.Windows.DependencyObject)e.OriginalSource;
        var row = FindParent<DataGridRow>(dependencyObject);
        if (row != null && row.Item is UnitViewModel viewModel)
        {
            viewModel.IsSelected = !viewModel.IsSelected;
            
            if (viewModel.IsSelected)
            {
                dgGiverUnits.SelectedItem = viewModel;
            }
            else
            {
                dgGiverUnits.SelectedItem = null;
            }
            
            e.Handled = true;
            UpdateProxyLimitInfo();
        }
    }

    private static T? FindParent<T>(System.Windows.DependencyObject child) where T : System.Windows.DependencyObject
    {
        var parentObject = System.Windows.Media.VisualTreeHelper.GetParent(child);
        if (parentObject == null) return null;
        if (parentObject is T parent) return parent;
        return FindParent<T>(parentObject);
    }

    private void DgGiverUnits_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // SelectionChanged event'i satır seçimini yönetmek için kullanılıyor
    }

    private void BtnSelectAllGivers_Click(object sender, RoutedEventArgs e)
    {
        foreach (var viewModel in _giverViewModels)
        {
            viewModel.IsSelected = true;
        }
        UpdateProxyLimitInfo();
    }

    private void BtnDeselectAllGivers_Click(object sender, RoutedEventArgs e)
    {
        foreach (var viewModel in _giverViewModels)
        {
            viewModel.IsSelected = false;
        }
        UpdateProxyLimitInfo();
    }

    private void CmbReceiver_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (cmbReceiver.IsDropDownOpen)
        {
            var searchText = cmbReceiver.Text?.ToLower() ?? "";
            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Reset to all items
                var allUnits = _giverViewModels.Select(vm => vm.Unit).ToList();
                var allReceiverItems = new List<ReceiverItem>();
                allReceiverItems.AddRange(allUnits.Select(u => new ReceiverItem { Unit = u, IsOther = false }));
                allReceiverItems.Add(new ReceiverItem { Unit = null, IsOther = true });
                cmbReceiver.ItemsSource = allReceiverItems;
                return;
            }

            var filteredUnitsList = _giverViewModels.Select(vm => vm.Unit)
                .Where(u => 
                    u.Number.ToLower().Contains(searchText) ||
                    (u.FirstName != null && u.FirstName.ToLower().Contains(searchText)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(searchText)) ||
                    $"{u.FirstName} {u.LastName}".Trim().ToLower().Contains(searchText)
                ).ToList();
            
            var filteredReceiverItems = new List<ReceiverItem>();
            filteredReceiverItems.AddRange(filteredUnitsList.Select(u => new ReceiverItem { Unit = u, IsOther = false }));
            if ("diğer".Contains(searchText) || "diger".Contains(searchText) || "other".Contains(searchText))
            {
                filteredReceiverItems.Add(new ReceiverItem { Unit = null, IsOther = true });
            }
            cmbReceiver.ItemsSource = filteredReceiverItems;
        }
    }

    private void CmbReceiver_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cmbReceiver.SelectedItem is ReceiverItem selectedItem)
        {
            if (selectedItem.IsOther)
            {
                spOtherName.Visibility = Visibility.Visible;
                txtOtherName.Text = "";
                txtOtherPhone.Text = "";
            }
            else
            {
                spOtherName.Visibility = Visibility.Collapsed;
                txtOtherName.Text = "";
                txtOtherPhone.Text = "";
            }
        }
    }
    
    private bool _isFormattingPhone = false;
    
    private void TxtOtherPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        // Sadece rakam girişine izin ver
        if (!PhoneNumberService.IsNumericInput(e.Text))
        {
            e.Handled = true;
            return;
        }
        
        // Maksimum uzunluk kontrolü
        var textBox = sender as TextBox;
        if (textBox != null && !PhoneNumberService.CanAddMoreCharacters(textBox.Text, e.Text))
        {
            e.Handled = true;
        }
    }
    
    private void TxtOtherPhone_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isFormattingPhone) return;
        
        var textBox = sender as TextBox;
        if (textBox == null) return;
        
        var currentText = textBox.Text;
        var selectionStart = textBox.SelectionStart;
        var formatted = PhoneNumberService.FormatPhoneNumber(currentText);
        
        if (formatted != currentText)
        {
            _isFormattingPhone = true;
            
            // Cursor pozisyonunu hesapla: mevcut pozisyondaki rakam sayısını bul
            var digitsBeforeCursor = PhoneNumberService.CleanPhoneNumber(
                currentText.Substring(0, Math.Min(selectionStart, currentText.Length))
            ).Length;
            
            textBox.Text = formatted;
            
            // Formatlanmış metinde aynı rakam sayısına karşılık gelen pozisyonu bul
            var newPosition = FindPositionInFormattedText(formatted, digitsBeforeCursor);
            textBox.SelectionStart = newPosition;
            
            _isFormattingPhone = false;
        }
    }
    
    private int FindPositionInFormattedText(string formattedText, int digitCount)
    {
        if (digitCount <= 0) return 0;
        if (digitCount >= PhoneNumberService.CleanPhoneNumber(formattedText).Length)
            return formattedText.Length;
        
        var digitIndex = 0;
        for (int i = 0; i < formattedText.Length; i++)
        {
            if (char.IsDigit(formattedText[i]))
            {
                digitIndex++;
                if (digitIndex >= digitCount)
                    return i + 1;
            }
        }
        
        return formattedText.Length;
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
        GiverUnits = _giverViewModels
            .Where(vm => vm.IsSelected)
            .Select(vm => vm.Unit)
            .ToList();
            
        if (GiverUnits.Count == 0)
        {
            MessageBox.Show("Lutfen en az bir vekalet veren birim secin.", "Uyari", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (cmbReceiver.SelectedItem is not ReceiverItem selectedReceiver)
        {
            MessageBox.Show("Lutfen vekalet alan secin.", "Uyari", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (selectedReceiver.IsOther)
        {
            // "Diğer" seçildi, isim ve telefon kontrolü yap
            if (string.IsNullOrWhiteSpace(txtOtherName.Text))
            {
                MessageBox.Show("Lutfen vekalet alan kisinin ad soyadini girin.", "Uyari", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(txtOtherPhone.Text))
            {
                MessageBox.Show("Lutfen vekalet alan kisinin telefon numarasini girin.", "Uyari", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (!PhoneNumberService.IsValidPhoneNumber(txtOtherPhone.Text))
            {
                MessageBox.Show($"Lutfen gecerli bir telefon numarasi girin.\nFormat: {PhoneNumberService.GetFormatDescription()}", "Uyari", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            ReceiverUnit = null;
            ReceiverName = txtOtherName.Text.Trim();
            ReceiverPhone = txtOtherPhone.Text.Trim();
        }
        else
        {
            // Birim seçildi
            ReceiverUnit = selectedReceiver.Unit;
            ReceiverName = null;
            ReceiverPhone = null;

            // Vekalet veren birimlerden biri vekalet alan birimle aynı olamaz
            if (ReceiverUnit != null && GiverUnits.Any(g => g.Id == ReceiverUnit.Id))
            {
                MessageBox.Show("Vekalet veren ve alan birim ayni olamaz.", "Uyari", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        // Aynı receiver'a verilen mevcut vekaletleri kontrol et
        int existingReceiverProxyCount = 0;
        decimal existingReceiverProxyLandShare = 0;
        
        if (ReceiverUnit != null)
        {
            // Aynı birime verilen mevcut vekaletler
            var receiverProxies = _existingProxies
                .Where(p => p.ReceiverUnitId == ReceiverUnit.Id)
                .ToList();
            existingReceiverProxyCount = receiverProxies.Count;
            existingReceiverProxyLandShare = receiverProxies
                .Where(p => p.GiverUnit != null)
                .Sum(p => p.GiverUnit!.LandShare);
        }
        else if (!string.IsNullOrWhiteSpace(ReceiverName))
        {
            // Aynı isme verilen mevcut vekaletler (telefon da eşleşmeli)
            var receiverProxies = _existingProxies
                .Where(p => p.ReceiverUnitId == null && 
                           p.ReceiverName == ReceiverName &&
                           (string.IsNullOrEmpty(p.ReceiverPhone) || p.ReceiverPhone == ReceiverPhone))
                .ToList();
            existingReceiverProxyCount = receiverProxies.Count;
            existingReceiverProxyLandShare = receiverProxies
                .Where(p => p.GiverUnit != null)
                .Sum(p => p.GiverUnit!.LandShare);
        }
        
        // Yasal limit kontrolü (hem sayı hem arsa payı) - mevcut receiver vekaletleriyle birlikte
        var selectedCount = GiverUnits.Count;
        var selectedLandShare = GiverUnits.Sum(u => u.LandShare);
        
        // Toplam: mevcut tüm vekaletler + aynı receiver'a verilen mevcut vekaletler + yeni seçilenler
        var totalCountForReceiver = _currentProxyCount + existingReceiverProxyCount + selectedCount;
        var totalLandShareForReceiver = _currentProxyLandShare + existingReceiverProxyLandShare + selectedLandShare;
        
        var maxProxies = _proxyService.CalculateMaxProxyCount(_totalUnitCount);
        var maxProxyLandShare = _proxyService.CalculateMaxProxyLandShare(_totalLandShare);
        
        var countExceeded = totalCountForReceiver > maxProxies;
        var landShareExceeded = totalLandShareForReceiver > maxProxyLandShare;
        
        if (countExceeded || landShareExceeded)
        {
            var message = "Yasal limit asildi!\n\n";
            if (countExceeded)
            {
                message += $"Sayi limiti: {totalCountForReceiver}/{maxProxies} (KMK 31)\n";
            }
            if (landShareExceeded)
            {
                message += $"Arsa payi limiti: {totalLandShareForReceiver:F2}/{maxProxyLandShare:F2} (KMK 31)\n";
            }
            if (existingReceiverProxyCount > 0)
            {
                message += $"\nBu kisiye zaten {existingReceiverProxyCount} vekalet verilmis.";
            }
            message += "\n\nVekalet verilemez!";
            
            MessageBox.Show(message, "Yasal Limit Asildi", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        DialogResult = true;
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    private class ReceiverItem
    {
        public Unit? Unit { get; set; }
        public bool IsOther { get; set; }
        public string DisplayName
        {
            get
            {
                if (IsOther)
                    return "Diğer";
                if (Unit != null)
                {
                    var name = $"{Unit.Number}";
                    if (!string.IsNullOrWhiteSpace(Unit.FirstName) || !string.IsNullOrWhiteSpace(Unit.LastName))
                    {
                        name += $" - {Unit.FirstName} {Unit.LastName}".Trim();
                    }
                    return name;
                }
                return "";
            }
        }
    }
}


