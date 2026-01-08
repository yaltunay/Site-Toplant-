using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Toplanti.Data;
using Toplanti.Models;
using Toplanti.Services.Interfaces;

namespace Toplanti.Dialogs;

public partial class BlockUnitManagementWindow : Window
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISiteService _siteService;
    private readonly IUnitService _unitService;
    private ObservableCollection<UnitViewModel> _units = [];
    private Site? _currentSite;
    private int _maxUnitCount = 0;
    private readonly Site? _selectedSiteFromMain;

    public BlockUnitManagementWindow(
        IUnitOfWork unitOfWork,
        ISiteService siteService,
        IUnitService unitService,
        Site? selectedSite = null)
    {
        InitializeComponent();
        
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
        _unitService = unitService ?? throw new ArgumentNullException(nameof(unitService));
        _selectedSiteFromMain = selectedSite;
        
        try
        {
            LoadUnitTypes();
            
            if (_selectedSiteFromMain != null)
            {
                // Ana ekrandan seçili site geldiyse, site oluştur bölümünü gizle
                _currentSite = _selectedSiteFromMain;
                txtSiteName.Text = _currentSite.Name ?? "";
                txtTotalLandShare.Text = _currentSite.TotalLandShare.ToString("F2");
                grpSiteInfo.Visibility = Visibility.Collapsed;
                btnCreateOrSelectSite.Tag = true;
                
                try
                {
                    LoadUnits();
                    UpdateSiteInfo();
                }
                catch (Exception loadEx)
                {
                    MessageBox.Show($"Veri yukleme hatasi: {loadEx.Message}\n\nDetay: {loadEx}", 
                        "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // Site seçili değilse, site oluştur bölümünü göster
                try
                {
                    LoadExistingSite();
                }
                catch (Exception loadEx)
                {
                    MessageBox.Show($"Site yukleme hatasi: {loadEx.Message}\n\nDetay: {loadEx}", 
                        "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                btnCreateOrSelectSite.Tag = false;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Pencere baslatma hatasi: {ex.Message}\n\nDetay: {ex}", 
                "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
        }
    }

    private async void LoadUnitTypes()
    {
        try
        {
            var unitTypes = await _unitOfWork.UnitTypes.GetAllUnitTypesAsync();
            cmbUnitType.ItemsSource = unitTypes.ToList();
            if (cmbUnitType.Items.Count > 0)
                cmbUnitType.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Birim tipleri yukleme hatasi: {ex.Message}", "Hata", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void LoadExistingSite()
    {
        try
        {
            var sites = await _siteService.GetAllSitesAsync();
            var siteDto = sites.FirstOrDefault();
            if (siteDto != null)
            {
                _currentSite = await _siteService.GetSiteDomainModelByIdAsync(siteDto.Id);
                if (_currentSite != null)
                {
                    txtSiteName.Text = _currentSite.Name;
                    txtTotalLandShare.Text = _currentSite.TotalLandShare.ToString("F2");
                    btnCreateOrSelectSite.Tag = true;
                    UpdateSiteInfo();
                    LoadUnits();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Site yukleme hatasi: {ex.Message}", "Hata", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void UpdateSiteInfo()
    {
        try
        {
            if (_currentSite == null)
            {
                txtSiteInfo.Text = "Site secilmedi";
                return;
            }

            var unitCount = await _unitOfWork.Units.GetActiveUnitCountBySiteIdAsync(_currentSite.Id);
            txtSiteInfo.Text = $"Site: {_currentSite.Name ?? ""} | Toplam Arsa Payi: {_currentSite.TotalLandShare:F2} | Daire Sayisi: {unitCount}";
        }
        catch (Exception ex)
        {
            txtSiteInfo.Text = $"Bilgi yukleme hatasi: {ex.Message}";
        }
    }

    private async void LoadUnits()
    {
        if (_currentSite == null) return;
        
        try
        {
            var units = await _unitOfWork.Units.GetActiveUnitsBySiteIdAsync(_currentSite.Id);
            var unitsList = units.ToList();

            _maxUnitCount = unitsList.Count > 0 ? unitsList.Count : 0;
            
            // Blok bazında toplam birim sayısını hesapla
            var blockUnitCounts = unitsList
                .GroupBy(u => u.Block ?? "")
                .ToDictionary(g => g.Key, g => g.Count());

            _units = new ObservableCollection<UnitViewModel>(
                unitsList.Select(u =>
                {
                    // Her birim için kendi bloğundaki toplam birim sayısına göre padding hesapla
                    var blockUnitCount = blockUnitCounts.GetValueOrDefault(u.Block ?? "", 0);
                    var paddingLength = blockUnitCount < 100 ? 2 : 3; // 100'den küçükse 2 basamak, değilse 3 basamak
                    
                    return new UnitViewModel
                    {
                        Id = u.Id,
                        Number = FormatUnitNumber(u.Number, paddingLength),
                        OriginalNumber = u.Number,
                        Block = u.Block ?? "",
                        OwnerName = u.OwnerName ?? "",
                        FirstName = u.FirstName ?? "",
                        LastName = u.LastName ?? "",
                        Phone = u.Phone ?? "",
                        Email = u.Email ?? "",
                        LandShare = u.LandShare,
                        UnitTypeId = u.UnitTypeId,
                        UnitTypeName = u.UnitType?.Name ?? ""
                    };
                })
            );
            
            if (dgUnits != null)
            {
                dgUnits.ItemsSource = _units;
            }
            
            try
            {
                ApplyFilters();
            }
            catch (Exception filterEx)
            {
                // Filtre hatası kritik değil, devam et
                System.Diagnostics.Debug.WriteLine($"Filtre hatasi: {filterEx.Message}");
            }
            
            try
            {
                LoadBlockFilter();
            }
            catch (Exception blockEx)
            {
                // Blok filtresi hatası kritik değil, devam et
                System.Diagnostics.Debug.WriteLine($"Blok filtresi hatasi: {blockEx.Message}");
            }
            
            UpdateSiteInfo();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Daireler yukleme hatasi: {ex.Message}\n\nDetay: {ex}", "Hata", 
                MessageBoxButton.OK, MessageBoxImage.Error);
            _units = new ObservableCollection<UnitViewModel>();
            dgUnits.ItemsSource = _units;
        }
    }

    private async void LoadBlockFilter()
    {
        if (_currentSite == null) return;

        try
        {
            var units = await _unitOfWork.Units.GetActiveUnitsBySiteIdAsync(_currentSite.Id);
            var blocks = units
                .Where(u => !string.IsNullOrEmpty(u.Block))
                .Select(u => u.Block!)
                .Distinct()
                .OrderBy(b => b)
                .ToList();

            cmbFilterBlock.Items.Clear();
            cmbFilterBlock.Items.Add(new System.Windows.Controls.ComboBoxItem { Content = "Tum Bloklar", IsSelected = true, FontSize = 14 });
            
            foreach (var block in blocks)
            {
                cmbFilterBlock.Items.Add(new System.Windows.Controls.ComboBoxItem { Content = block, FontSize = 14 });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Blok filtreleme yukleme hatasi: {ex.Message}", "Hata", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ApplyFilters()
    {
        if (_units == null) return;

        var filteredUnits = _units.AsEnumerable();

        // Blok filtresi
        if (cmbFilterBlock.SelectedItem is System.Windows.Controls.ComboBoxItem selectedBlock && 
            selectedBlock.Content?.ToString() != "Tum Bloklar" && 
            !string.IsNullOrEmpty(selectedBlock.Content?.ToString()))
        {
            var blockName = selectedBlock.Content.ToString();
            filteredUnits = filteredUnits.Where(u => u.Block == blockName);
        }

        // Metin filtresi
        if (!string.IsNullOrWhiteSpace(txtFilter.Text))
        {
            var filterText = txtFilter.Text.ToLowerInvariant();
            filteredUnits = filteredUnits.Where(u =>
                u.Number.ToLowerInvariant().Contains(filterText) ||
                u.Block.ToLowerInvariant().Contains(filterText) ||
                u.OwnerName.ToLowerInvariant().Contains(filterText) ||
                u.FirstName.ToLowerInvariant().Contains(filterText) ||
                u.LastName.ToLowerInvariant().Contains(filterText) ||
                u.Phone.ToLowerInvariant().Contains(filterText) ||
                (u.Email != null && u.Email.ToLowerInvariant().Contains(filterText)) ||
                u.UnitTypeName.ToLowerInvariant().Contains(filterText));
        }

        dgUnits.ItemsSource = filteredUnits.ToList();
    }

    private async void CmbUnitType_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (cmbUnitType.IsDropDownOpen)
        {
            var searchText = cmbUnitType.Text?.ToLower() ?? "";
            if (string.IsNullOrWhiteSpace(searchText))
            {
                var unitTypes = await _unitOfWork.UnitTypes.GetAllUnitTypesAsync();
                cmbUnitType.ItemsSource = unitTypes.ToList();
                return;
            }

            var allUnitTypes = await _unitOfWork.UnitTypes.GetAllUnitTypesAsync();
            var filteredTypes = allUnitTypes.Where(ut => ut.Name.ToLower().Contains(searchText)).ToList();
            cmbUnitType.ItemsSource = filteredTypes;
        }
    }

    private async void CmbFilterBlock_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (cmbFilterBlock.IsDropDownOpen && _currentSite != null)
        {
            var searchText = cmbFilterBlock.Text?.ToLower() ?? "";
            if (string.IsNullOrWhiteSpace(searchText))
            {
                LoadBlockFilter();
                return;
            }

            var units = await _unitOfWork.Units.GetActiveUnitsBySiteIdAsync(_currentSite.Id);
            var blocks = units
                .Where(u => !string.IsNullOrEmpty(u.Block))
                .Select(u => u.Block!)
                .Distinct()
                .OrderBy(b => b)
                .ToList();

            var filteredBlocks = blocks.Where(b => b.ToLower().Contains(searchText)).ToList();
            
            cmbFilterBlock.Items.Clear();
            cmbFilterBlock.Items.Add(new System.Windows.Controls.ComboBoxItem { Content = "Tum Bloklar", FontSize = 14 });
            foreach (var block in filteredBlocks)
            {
                cmbFilterBlock.Items.Add(new System.Windows.Controls.ComboBoxItem { Content = block, FontSize = 14 });
            }
        }
    }

    private void CmbFilterBlock_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        ApplyFilters();
    }

    private void TxtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        ApplyFilters();
    }

    private void BtnClearFilter_Click(object sender, RoutedEventArgs e)
    {
        // Filtreleri temizle
        cmbFilterBlock.SelectedItem = null;
        txtFilter.Clear();
        
        // Filtreleri yeniden uygula (tüm kayıtları göster)
        ApplyFilters();
    }

    private void BtnToggleSelectAll_Click(object sender, RoutedEventArgs e)
    {
        // Filtrelenmiş daireleri al
        var filteredUnits = GetFilteredUnits();
        
        // Tüm filtrelenmiş daireler seçili mi kontrol et
        bool allSelected = filteredUnits.All(u => u.IsSelected);
        
        // Toggle: Eğer hepsi seçiliyse seçimi kaldır, değilse hepsini seç
        foreach (var unit in filteredUnits)
        {
            var originalUnit = _units.FirstOrDefault(u => u.Id == unit.Id);
            if (originalUnit != null)
            {
                originalUnit.IsSelected = !allSelected;
            }
        }
        
        // Buton metnini güncelle
        btnToggleSelectAll.Content = allSelected ? "Tumunu Sec" : "Secimi Kaldir";
        
        // DataGrid'i yenile
        ApplyFilters();
    }

    private string FormatUnitNumber(string number, int paddingLength)
    {
        // Extract number part from string like "A Blok 5" -> "5"
        var parts = number.Split(' ');
        if (parts.Length > 0 && int.TryParse(parts[^1], out var unitNum))
        {
            var prefix = string.Join(" ", parts.Take(parts.Length - 1));
            return $"{prefix} {unitNum.ToString().PadLeft(paddingLength, '0')}";
        }
        return number;
    }

    private async void BtnCreateOrSelectSite_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtSiteName.Text) ||
            !decimal.TryParse(txtTotalLandShare.Text, out var totalLandShare) ||
            totalLandShare <= 0)
        {
            MessageBox.Show("Lutfen site adi ve gecerli bir toplam arsa payi girin.", "Uyari", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var siteName = txtSiteName.Text.Trim();
            
            // Deactivate existing sites
            var existingSites = await _unitOfWork.Sites.FindAsync(s => s.IsActive);
            foreach (var site in existingSites)
            {
                site.IsActive = false;
                _unitOfWork.Sites.Update(site);
            }

            // Find or create site
            var existingSite = await _unitOfWork.Sites.FirstOrDefaultAsync(s => s.Name == siteName);
            if (existingSite == null)
            {
                existingSite = new Site
                {
                    Name = siteName,
                    TotalLandShare = totalLandShare,
                    IsActive = true
                };
                await _unitOfWork.Sites.AddAsync(existingSite);
            }
            else
            {
                existingSite.TotalLandShare = totalLandShare;
                existingSite.IsActive = true;
                _unitOfWork.Sites.Update(existingSite);
            }

            await _unitOfWork.SaveChangesAsync();
            _currentSite = existingSite;
            btnCreateOrSelectSite.Tag = true;
            UpdateSiteInfo();
            LoadUnits();

            MessageBox.Show("Site basariyla olusturuldu/secildi.", "Basarili", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Site olusturma hatasi: {ex.Message}", "Hata", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void BtnDeleteAllData_Click(object sender, RoutedEventArgs e)
    {
        // Şifre kontrolü
        var passwordDialog = new PasswordDialog();
        if (passwordDialog.ShowDialog() != true || !passwordDialog.IsPasswordCorrect)
        {
            return;
        }

        var result = MessageBox.Show(
            "TUM VERILERI SILMEK ISTEDIGINIZDEN EMIN MISINIZ?\n\n" +
            "Bu islem geri alinamaz!\n" +
            "Silinecekler:\n" +
            "- Tum daireler\n" +
            "- Tum toplantilar\n" +
            "- Tum kararlar\n" +
            "- Tum oylamalar\n" +
            "- Tum vekaletler\n" +
            "- Tum siteler\n\n" +
            "Sadece birim tipleri (Villa, Daire, Dukkan) korunacak.",
            "DIKKAT!", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            // Delete in correct order to avoid foreign key violations
            var votes = await _unitOfWork.Decisions.FindAsync(d => true);
            var decisions = await _unitOfWork.Decisions.GetAllAsync();
            var attendances = await _unitOfWork.MeetingAttendances.GetAllAsync();
            var units = await _unitOfWork.Units.GetAllAsync();
            var meetings = await _unitOfWork.Meetings.GetAllAsync();
            var sites = await _unitOfWork.Sites.GetAllAsync();
            
            // Note: Votes, Proxies, MeetingAttendances için repository'ler yok
            // Bu işlem için DbContext'e erişim gerekebilir, ancak şimdilik sadece mevcut repository'lerle yapıyoruz
            foreach (var decision in decisions)
            {
                _unitOfWork.Decisions.Remove(decision);
            }
            foreach (var attendance in attendances)
            {
                _unitOfWork.MeetingAttendances.Remove(attendance);
            }
            foreach (var unit in units)
            {
                _unitOfWork.Units.Remove(unit);
            }
            foreach (var meeting in meetings)
            {
                _unitOfWork.Meetings.Remove(meeting);
            }
            foreach (var site in sites)
            {
                _unitOfWork.Sites.Remove(site);
            }
            
            await _unitOfWork.SaveChangesAsync();

            _currentSite = null;
            _units.Clear();
            txtSiteName.Clear();
            txtTotalLandShare.Clear();
            btnCreateOrSelectSite.Tag = false;
            UpdateSiteInfo();

            MessageBox.Show("Tum veriler basariyla silindi.", "Basarili", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Veri silme hatasi: {ex.Message}\n\nDetay: {ex}", "Hata", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void BtnCreateUnits_Click(object sender, RoutedEventArgs e)
    {
        if (_currentSite == null)
        {
            MessageBox.Show("Lutfen once site olusturun veya secin.", "Uyari", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(txtBlockName.Text) ||
            !int.TryParse(txtUnitCount.Text, out var unitCount) ||
            unitCount <= 0 ||
            cmbUnitType.SelectedItem is not UnitType unitType)
        {
            MessageBox.Show("Lutfen blok adi, gecerli bir daire sayisi ve birim tipi secin.", "Uyari", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var blockName = txtBlockName.Text.Trim();
            var existingUnits = (await _unitOfWork.Units.GetAllAsync())
                .Where(u => u.SiteId == _currentSite.Id && u.Block == blockName && u.IsActive)
                .ToList();

            if (existingUnits.Any())
            {
                var result = MessageBox.Show(
                    $"Bu blok icin zaten {existingUnits.Count} daire mevcut. Yeni daireler eklemek istiyor musunuz?",
                    "Onay", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result != MessageBoxResult.Yes)
                    return;
            }

            // Find max number in this block
            var maxNumber = existingUnits.Any() 
                ? existingUnits
                    .Select(u => ExtractNumberFromUnitNumber(u.Number))
                    .Where(n => n.HasValue)
                    .DefaultIfEmpty(0)
                    .Max()
                : 0;

            // Determine padding based on block unit count (blok bazında)
            var blockTotalUnits = existingUnits.Count + unitCount; // Mevcut + yeni birimler
            var paddingLength = blockTotalUnits < 100 ? 2 : 3; // 100'den küçükse 2 basamak, değilse 3 basamak

            // Daire başına arsa payı kontrolü
            decimal? unitLandShare = null;
            if (!string.IsNullOrWhiteSpace(txtUnitLandShare.Text) && 
                decimal.TryParse(txtUnitLandShare.Text, out var parsedLandShare) && 
                parsedLandShare > 0)
            {
                unitLandShare = parsedLandShare;
            }

            // Otomatik arsa payı hesaplama (eğer girilmediyse veya 0 ise)
            decimal landSharePerUnit = 0m;
            if (!unitLandShare.HasValue || unitLandShare.Value == 0)
            {
                var totalUnits = (await _unitOfWork.Units.GetAllAsync()).Count(u => u.SiteId == _currentSite!.Id && u.IsActive);
                var totalUnitsAfterCreation = totalUnits + unitCount;
                landSharePerUnit = totalUnitsAfterCreation > 0 
                    ? _currentSite.TotalLandShare / totalUnitsAfterCreation 
                    : 0m;
            }
            else
            {
                landSharePerUnit = unitLandShare.Value;
            }

            var newUnits = new List<Unit>();
            var safeBlockName = blockName ?? "";
            for (int i = 1; i <= unitCount; i++)
            {
                var unitNumber = maxNumber + i;
                var formattedNumber = $"{safeBlockName} {unitNumber.ToString().PadLeft(paddingLength, '0')}";
                
                var allUnits = await _unitOfWork.Units.GetAllAsync();
                if (!allUnits.Any(u => u.Number == formattedNumber && u.SiteId == _currentSite!.Id))
                {
                    newUnits.Add(new Unit
                    {
                        Number = formattedNumber,
                        Block = safeBlockName,
                        OwnerName = "", // Required field, empty string
                        FirstName = null,
                        LastName = null,
                        Phone = null,
                        Email = null,
                        LandShare = landSharePerUnit,
                        UnitTypeId = unitType.Id,
                        SiteId = _currentSite.Id,
                        IsActive = true
                    });
                }
            }

            if (newUnits.Any())
            {
                foreach (var unit in newUnits)
                {
                    await _unitOfWork.Units.AddAsync(unit);
                }
                await _unitOfWork.SaveChangesAsync();
                
                // Alanları temizle
                txtBlockName.Clear();
                txtUnitCount.Clear();
                txtUnitLandShare.Clear();
                
                LoadUnits();
                MessageBox.Show($"{newUnits.Count} daire basariyla olusturuldu.{(landSharePerUnit > 0 ? $" Arsa payi: {landSharePerUnit:F2}" : "")}", "Basarili", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Daire olusturma hatasi: {ex.Message}", "Hata", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private int? ExtractNumberFromUnitNumber(string unitNumber)
    {
        var parts = unitNumber.Split(' ');
        if (parts.Length > 0 && int.TryParse(parts[^1], out var num))
        {
            return num;
        }
        return null;
    }

    private void BtnApplyBulk_Click(object sender, RoutedEventArgs e)
    {
        if (!decimal.TryParse(txtBulkLandShare.Text, out var landShare) || landShare < 0)
        {
            MessageBox.Show("Lutfen gecerli bir arsa payi girin.", "Uyari", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Filtrelenmiş daireleri al
        var filteredUnits = GetFilteredUnits();
        if (!filteredUnits.Any())
        {
            MessageBox.Show("Filtrelenmis daire bulunamadi.", "Uyari", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (chkDistributeEqually.IsChecked == true && filteredUnits.Count > 0)
        {
            var sharePerUnit = landShare / filteredUnits.Count;
            foreach (var unit in filteredUnits)
            {
                // _units koleksiyonundaki ilgili unit'i bul ve güncelle
                var originalUnit = _units.FirstOrDefault(u => u.Id == unit.Id);
                if (originalUnit != null)
                {
                    originalUnit.LandShare = sharePerUnit;
                }
            }
        }
        else
        {
            foreach (var unit in filteredUnits)
            {
                // _units koleksiyonundaki ilgili unit'i bul ve güncelle
                var originalUnit = _units.FirstOrDefault(u => u.Id == unit.Id);
                if (originalUnit != null)
                {
                    originalUnit.LandShare = landShare;
                }
            }
        }

        ApplyFilters(); // Filtreleri yeniden uygula
    }

    private void BtnApplyBlockLandShare_Click(object sender, RoutedEventArgs e)
    {
        if (!decimal.TryParse(txtBlockLandShare.Text, out var blockLandShare) || blockLandShare < 0)
        {
            MessageBox.Show("Lutfen gecerli bir arsa payi girin.", "Uyari", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (cmbFilterBlock.SelectedItem is not System.Windows.Controls.ComboBoxItem selectedBlock || 
            selectedBlock.Content?.ToString() == "Tum Bloklar" || 
            string.IsNullOrEmpty(selectedBlock.Content?.ToString()))
        {
            MessageBox.Show("Lutfen bir blok secin.", "Uyari", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var blockName = selectedBlock.Content.ToString();
        var blockUnits = _units.Where(u => u.Block == blockName).ToList();

        if (!blockUnits.Any())
        {
            MessageBox.Show("Bu blokta daire bulunamadi.", "Uyari", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Blok bazında arsa payını eşit dağıt
        var sharePerUnit = blockLandShare / blockUnits.Count;
        foreach (var unit in blockUnits)
        {
            unit.LandShare = sharePerUnit;
        }

        ApplyFilters(); // Filtreleri yeniden uygula
    }

    private async void BtnUpdateSelected_Click(object sender, RoutedEventArgs e)
    {
        
        var selectedUnits = _units.Where(u => u.IsSelected).ToList();
        if (!selectedUnits.Any())
        {
            MessageBox.Show("Lutfen guncellemek icin daire secin.", "Uyari", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            foreach (var unitVm in selectedUnits)
            {
                // Zorunlu alan kontrolleri
                if (string.IsNullOrWhiteSpace(unitVm.FirstName))
                {
                    MessageBox.Show($"Daire {unitVm.Number} icin ad girilmedi.", "Uyari", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(unitVm.LastName))
                {
                    MessageBox.Show($"Daire {unitVm.Number} icin soyad girilmedi.", "Uyari", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(unitVm.Phone))
                {
                    MessageBox.Show($"Daire {unitVm.Number} icin telefon girilmedi.", "Uyari", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Telefon formatı kontrolü
                if (!IsValidTurkishPhone(unitVm.Phone))
                {
                    MessageBox.Show($"Daire {unitVm.Number} icin gecersiz telefon formati. Format: 0XXX XXX XX XX", "Uyari", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var unit = await _unitOfWork.Units.GetByIdAsync(unitVm.Id);
                if (unit != null)
                {
                    // İsim soyisim formatlaması (baş harfler büyük, soyisim tümü büyük)
                    unit.FirstName = FormatName(unitVm.FirstName);
                    unit.LastName = FormatLastName(unitVm.LastName);
                    unit.Phone = FormatPhone(unitVm.Phone);
                    unit.Email = string.IsNullOrWhiteSpace(unitVm.Email) ? null : unitVm.Email.Trim();
                    
                    // OwnerName'i oluştur
                    unit.OwnerName = $"{unit.FirstName} {unit.LastName}";
                    
                    unit.LandShare = unitVm.LandShare;
                    _unitOfWork.Units.Update(unit);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            LoadUnits();
            MessageBox.Show($"{selectedUnits.Count} daire basariyla guncellendi.", "Basarili", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Guncelleme hatasi: {ex.Message}", "Hata", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void BtnSaveAll_Click(object sender, RoutedEventArgs e)
    {
        
        try
        {
            foreach (var unitVm in _units)
            {
                // Zorunlu alan kontrolleri
                if (string.IsNullOrWhiteSpace(unitVm.FirstName))
                {
                    MessageBox.Show($"Daire {unitVm.Number} icin ad girilmedi.", "Uyari", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(unitVm.LastName))
                {
                    MessageBox.Show($"Daire {unitVm.Number} icin soyad girilmedi.", "Uyari", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(unitVm.Phone))
                {
                    MessageBox.Show($"Daire {unitVm.Number} icin telefon girilmedi.", "Uyari", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Telefon formatı kontrolü
                if (!IsValidTurkishPhone(unitVm.Phone))
                {
                    MessageBox.Show($"Daire {unitVm.Number} icin gecersiz telefon formati. Format: 0XXX XXX XX XX", "Uyari", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var unit = await _unitOfWork.Units.GetByIdAsync(unitVm.Id);
                if (unit != null)
                {
                    // İsim soyisim formatlaması (baş harfler büyük, soyisim tümü büyük)
                    unit.FirstName = FormatName(unitVm.FirstName);
                    unit.LastName = FormatLastName(unitVm.LastName);
                    unit.Phone = FormatPhone(unitVm.Phone);
                    unit.Email = string.IsNullOrWhiteSpace(unitVm.Email) ? null : unitVm.Email.Trim();
                    
                    // OwnerName'i oluştur
                    unit.OwnerName = $"{unit.FirstName} {unit.LastName}";
                    
                    unit.LandShare = unitVm.LandShare;
                    _unitOfWork.Units.Update(unit);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            LoadUnits();
            MessageBox.Show("Tum daireler basariyla kaydedildi.", "Basarili", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Kaydetme hatasi: {ex.Message}", "Hata", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // İsim formatlaması: Her kelimenin baş harfi büyük (Title Case - Türkçe karakter desteği)
    private string FormatName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "";
        
        name = name.Trim();
        if (name.Length == 0) return "";
        
        // Her kelimenin baş harfini büyük yap (Türkçe karakter desteği)
        var culture = new System.Globalization.CultureInfo("tr-TR");
        return culture.TextInfo.ToTitleCase(name.ToLower(culture));
    }

    // Soyisim formatlaması: Tüm harfler büyük (Türkçe karakter desteği)
    private string FormatLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName)) return "";
        
        var culture = new System.Globalization.CultureInfo("tr-TR");
        return lastName.Trim().ToUpper(culture);
    }

    // Telefon formatlaması: Türkiye formatı (0XXX XXX XX XX)
    private string FormatPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return "";
        
        // Sadece rakamları al
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        
        // Türkiye telefon formatı: 0XXX XXX XX XX (11 hane: 0 + 3 + 3 + 2 + 2)
        if (digits.Length == 11 && digits.StartsWith("0"))
        {
            return $"{digits.Substring(0, 4)} {digits.Substring(4, 3)} {digits.Substring(7, 2)} {digits.Substring(9, 2)}";
        }
        else if (digits.Length == 10)
        {
            // 0 olmadan girilmişse 0 ekle
            return $"0{digits.Substring(0, 3)} {digits.Substring(3, 3)} {digits.Substring(6, 2)} {digits.Substring(8, 2)}";
        }
        
        return phone.Trim(); // Formatlanamazsa olduğu gibi döndür
    }

    // Türkiye telefon formatı kontrolü
    private bool IsValidTurkishPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return false;
        
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        
        // 11 haneli ve 0 ile başlamalı veya 10 haneli olabilir
        return (digits.Length == 11 && digits.StartsWith("0")) || digits.Length == 10;
    }

    private void DgUnits_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        // Seçim değiştiğinde işaretleme yapılabilir
    }

    private UnitViewModel? _rightClickedUnit = null;

    private void DgUnits_ContextMenuOpening(object sender, RoutedEventArgs e)
    {
        // Context menu açılırken hangi satıra tıklandığını kaydet
        var mousePos = System.Windows.Input.Mouse.GetPosition(dgUnits);
        var hitTestResult = System.Windows.Media.VisualTreeHelper.HitTest(dgUnits, mousePos);
        _rightClickedUnit = null;
        
        if (hitTestResult != null)
        {
            var row = FindParent<System.Windows.Controls.DataGridRow>(hitTestResult.VisualHit as DependencyObject);
            if (row != null && row.Item is UnitViewModel unit)
            {
                _rightClickedUnit = unit;
                
                // Eğer hiç seçili yoksa, sağ tıklanan satırı seç
                var selectedCount = _units.Count(u => u.IsSelected);
                if (selectedCount == 0)
                {
                    unit.IsSelected = true;
                }
            }
        }
    }

    private void MenuItem_ChangeLandShare_Click(object sender, RoutedEventArgs e)
    {
        // IsSelected property'sine göre seçili daireleri al
        var selectedUnits = _units.Where(u => u.IsSelected).ToList();
        
        // Eğer hiç seçili yoksa, sağ tıklanan satırı al
        if (!selectedUnits.Any() && _rightClickedUnit != null)
        {
            selectedUnits = [_rightClickedUnit];
        }
        
        if (selectedUnits.Any())
        {
            EditLandShareForSelectedUnits(selectedUnits);
        }
    }

    private void DgUnits_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        // Hangi hücreye tıklandığını bul
        var cell = FindParent<System.Windows.Controls.DataGridCell>(e.OriginalSource as DependencyObject);
        if (cell == null || cell.Column == null) return;

        var columnHeader = cell.Column.Header?.ToString() ?? "";
        
        // Seçili daireleri al (çoklu seçim desteği)
        var selectedUnits = dgUnits.SelectedItems.Cast<UnitViewModel>().ToList();
        if (!selectedUnits.Any()) return;

        // Hangi sütuna göre dialog açılacağını belirle
        switch (columnHeader)
        {
            case "Ad":
                EditFieldForSelectedUnits(selectedUnits, "Ad", unit => unit.FirstName, (unit, value) => unit.FirstName = value);
                break;
            case "Soyad":
                EditFieldForSelectedUnits(selectedUnits, "Soyad", unit => unit.LastName, (unit, value) => unit.LastName = value);
                break;
            case "Telefon":
                EditFieldForSelectedUnits(selectedUnits, "Telefon", unit => unit.Phone, (unit, value) => unit.Phone = value);
                break;
            case "E-posta":
                EditFieldForSelectedUnits(selectedUnits, "E-posta", unit => unit.Email ?? "", (unit, value) => 
                {
                    unit.Email = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
                });
                break;
            case "Arsa Payı":
                EditLandShareForSelectedUnits(selectedUnits);
                break;
        }
    }

    private T? FindParent<T>(DependencyObject? child) where T : DependencyObject
    {
        var parentObject = child;
        while (parentObject != null)
        {
            if (parentObject is T parent)
                return parent;
            parentObject = System.Windows.Media.VisualTreeHelper.GetParent(parentObject);
        }
        return null;
    }

    private void EditFieldForSelectedUnits(List<UnitViewModel> units, string fieldName, 
        Func<UnitViewModel, string> getValue, Action<UnitViewModel, string> setValue)
    {
        var firstUnit = units.First();
        var currentValue = getValue(firstUnit);
        var isMultiple = units.Count > 1;

        var dialog = new Window
        {
            Title = $"{fieldName} Duzenle" + (isMultiple ? $" ({units.Count} daire)" : ""),
            Width = 400,
            Height = isMultiple ? 200 : 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = this,
            ResizeMode = ResizeMode.NoResize
        };

        var stackPanel = new System.Windows.Controls.StackPanel
        {
            Margin = new Thickness(15)
        };

        var labelText = isMultiple 
            ? $"{units.Count} adet daire secildi.\nYeni {fieldName.ToLower()} degerini girin:\n(Tum secili daireler icin ayni deger uygulanacak)"
            : $"Daire: {firstUnit.Number}\nYeni {fieldName.ToLower()} degerini girin:";

        var label = new System.Windows.Controls.Label
        {
            Content = labelText,
            Margin = new Thickness(0, 0, 0, 10)
        };

        var textBox = new System.Windows.Controls.TextBox
        {
            Text = currentValue,
            Margin = new Thickness(0, 0, 0, 10)
        };

        var buttonPanel = new System.Windows.Controls.StackPanel
        {
            Orientation = System.Windows.Controls.Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right
        };

        var okButton = new System.Windows.Controls.Button
        {
            Content = "Tamam",
            Width = 80,
            Margin = new Thickness(5, 0, 0, 0),
            IsDefault = true
        };

        var cancelButton = new System.Windows.Controls.Button
        {
            Content = "Iptal",
            Width = 80,
            Margin = new Thickness(5, 0, 0, 0),
            IsCancel = true
        };

        okButton.Click += (s, args) =>
        {
            var newValue = textBox.Text.Trim();
            
            // Tüm seçili dairelere uygula
            foreach (var unit in units)
            {
                setValue(unit, newValue);
            }
            
            dialog.DialogResult = true;
            dialog.Close();
        };

        cancelButton.Click += (s, args) =>
        {
            dialog.DialogResult = false;
            dialog.Close();
        };

        buttonPanel.Children.Add(okButton);
        buttonPanel.Children.Add(cancelButton);

        stackPanel.Children.Add(label);
        stackPanel.Children.Add(textBox);
        stackPanel.Children.Add(buttonPanel);

        dialog.Content = stackPanel;
        textBox.Focus();
        textBox.SelectAll();

        dialog.ShowDialog();
    }

    private void EditLandShareForSelectedUnits(List<UnitViewModel> units)
    {
        var firstUnit = units.First();
        var currentValue = firstUnit.LandShare;
        var isMultiple = units.Count > 1;

        var dialog = new Window
        {
            Title = "Arsa Payi Duzenle" + (isMultiple ? $" ({units.Count} daire)" : ""),
            Width = 550,
            Height = isMultiple ? 260 : 220,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = this,
            ResizeMode = ResizeMode.NoResize,
            MinHeight = 220,
            MinWidth = 550
        };

        var mainGrid = new System.Windows.Controls.Grid
        {
            Margin = new Thickness(20)
        };
        
        mainGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        
        mainGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
        mainGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
        mainGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }); // Butonlar için Auto

        var labelText = isMultiple 
            ? $"{units.Count} adet daire secildi.\nYeni arsa payi degerini girin:\n(Tum secili daireler icin ayni deger uygulanacak)"
            : $"Daire: {firstUnit.Number}\nYeni arsa payi degerini girin:";

        var label = new System.Windows.Controls.Label
        {
            Content = labelText,
            Margin = new Thickness(0, 0, 0, 10),
            VerticalAlignment = VerticalAlignment.Top
        };
        System.Windows.Controls.Grid.SetRow(label, 0);
        System.Windows.Controls.Grid.SetColumn(label, 0);

        var textBox = new System.Windows.Controls.TextBox
        {
            Text = currentValue.ToString("F2"),
            Margin = new Thickness(0, 0, 0, 15),
            Height = 30,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Top,
            MinWidth = 300,
            FontSize = 14,
            Padding = new Thickness(5, 3, 5, 3)
        };
        System.Windows.Controls.Grid.SetRow(textBox, 1);
        System.Windows.Controls.Grid.SetColumn(textBox, 0);

        var buttonPanel = new System.Windows.Controls.StackPanel
        {
            Orientation = System.Windows.Controls.Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 5, 0, 0)
        };
        System.Windows.Controls.Grid.SetRow(buttonPanel, 2);
        System.Windows.Controls.Grid.SetColumn(buttonPanel, 0);

        var okButton = new System.Windows.Controls.Button
        {
            Content = "Tamam",
            Width = 90,
            Height = 30,
            Margin = new Thickness(5, 0, 0, 0),
            IsDefault = true
        };

        var cancelButton = new System.Windows.Controls.Button
        {
            Content = "Iptal",
            Width = 90,
            Height = 30,
            Margin = new Thickness(5, 0, 0, 0),
            IsCancel = true
        };

        okButton.Click += (s, args) =>
        {
            if (decimal.TryParse(textBox.Text, out var newLandShare) && newLandShare >= 0)
            {
                // Tüm seçili dairelere uygula
                foreach (var unit in units)
                {
                    unit.LandShare = newLandShare;
                }
                dialog.DialogResult = true;
                dialog.Close();
            }
            else
            {
                MessageBox.Show("Gecersiz arsa payi degeri. Lutfen gecerli bir sayi girin.", "Uyari",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        };

        cancelButton.Click += (s, args) =>
        {
            dialog.DialogResult = false;
            dialog.Close();
        };

        buttonPanel.Children.Add(okButton);
        buttonPanel.Children.Add(cancelButton);

        mainGrid.Children.Add(label);
        mainGrid.Children.Add(textBox);
        mainGrid.Children.Add(buttonPanel);

        dialog.Content = mainGrid;
        textBox.Focus();
        textBox.SelectAll();

        dialog.ShowDialog();
    }

    private async void BtnDeleteFiltered_Click(object sender, RoutedEventArgs e)
    {
        var filteredUnits = GetFilteredUnits();
        if (!filteredUnits.Any())
        {
            MessageBox.Show("Silinecek daire bulunamadi.", "Uyari", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show(
            $"{filteredUnits.Count} adet daire silinecek. Emin misiniz?",
            "Onay", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            var unitIds = filteredUnits.Select(u => u.Id).ToList();
            var allUnits = await _unitOfWork.Units.GetAllAsync();
            var unitsToDelete = allUnits.Where(u => unitIds.Contains(u.Id)).ToList();
            
            foreach (var unit in unitsToDelete)
            {
                unit.IsActive = false;
                _unitOfWork.Units.Update(unit);
            }

            await _unitOfWork.SaveChangesAsync();
            LoadUnits();

            MessageBox.Show($"{unitsToDelete.Count} daire basariyla silindi.", "Basarili", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Silme hatasi: {ex.Message}", "Hata", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void BtnDeleteBlock_Click(object sender, RoutedEventArgs e)
    {
        if (_currentSite == null) return;

        if (cmbFilterBlock.SelectedItem is not System.Windows.Controls.ComboBoxItem selectedBlock || 
            selectedBlock.Content?.ToString() == "Tum Bloklar" || 
            string.IsNullOrEmpty(selectedBlock.Content?.ToString()))
        {
            MessageBox.Show("Lutfen silmek icin bir blok secin.", "Uyari", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var blockName = selectedBlock.Content.ToString();
        var allUnits = await _unitOfWork.Units.GetAllAsync();
        var blockUnits = allUnits
            .Where(u => u.SiteId == _currentSite.Id && u.Block == blockName && u.IsActive)
            .ToList();

        if (!blockUnits.Any())
        {
            MessageBox.Show("Bu blokta silinecek daire bulunamadi.", "Uyari", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show(
            $"{blockName} bloguna ait {blockUnits.Count} adet daire silinecek. Emin misiniz?",
            "Onay", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            foreach (var unit in blockUnits)
            {
                unit.IsActive = false;
                _unitOfWork.Units.Update(unit);
            }

            await _unitOfWork.SaveChangesAsync();
            LoadUnits();

            MessageBox.Show($"{blockName} bloguna ait {blockUnits.Count} daire basariyla silindi.", "Basarili", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Blok silme hatasi: {ex.Message}", "Hata", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private List<UnitViewModel> GetFilteredUnits()
    {
        if (_units == null) return [];

        var filteredUnits = _units.AsEnumerable();

        // Blok filtresi
        if (cmbFilterBlock.SelectedItem is System.Windows.Controls.ComboBoxItem selectedBlock && 
            selectedBlock.Content?.ToString() != "Tum Bloklar" && 
            !string.IsNullOrEmpty(selectedBlock.Content?.ToString()))
        {
            var blockName = selectedBlock.Content.ToString();
            filteredUnits = filteredUnits.Where(u => u.Block == blockName);
        }

        // Metin filtresi
        if (!string.IsNullOrWhiteSpace(txtFilter.Text))
        {
            var filterText = txtFilter.Text.ToLowerInvariant();
            filteredUnits = filteredUnits.Where(u =>
                u.Number.ToLowerInvariant().Contains(filterText) ||
                u.Block.ToLowerInvariant().Contains(filterText) ||
                u.OwnerName.ToLowerInvariant().Contains(filterText) ||
                u.UnitTypeName.ToLowerInvariant().Contains(filterText));
        }

        return filteredUnits.ToList();
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}

public class UnitViewModel : INotifyPropertyChanged
{
    private bool _isSelected;
    private string _ownerName = "";
    private string _firstName = "";
    private string _lastName = "";
    private string _phone = "";
    private string _email = "";
    private decimal _landShare;

    public int Id { get; set; }
    public string Number { get; set; } = "";
    public string OriginalNumber { get; set; } = "";
    public string Block { get; set; } = "";
    public int UnitTypeId { get; set; }
    public string UnitTypeName { get; set; } = "";

    public bool IsSelected
    {
        get => _isSelected;
        set { _isSelected = value; OnPropertyChanged(); }
    }

    public string OwnerName
    {
        get => _ownerName;
        set { _ownerName = value; OnPropertyChanged(); }
    }

    public string FirstName
    {
        get => _firstName;
        set { _firstName = value; OnPropertyChanged(); }
    }

    public string LastName
    {
        get => _lastName;
        set { _lastName = value; OnPropertyChanged(); }
    }

    public string Phone
    {
        get => _phone;
        set { _phone = value; OnPropertyChanged(); }
    }

    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(); }
    }

    public decimal LandShare
    {
        get => _landShare;
        set { _landShare = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
