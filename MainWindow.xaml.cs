using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using Toplanti.Data;
using Toplanti.Dialogs;
using Toplanti.Models;
using Toplanti.Services;

namespace Toplanti;

public partial class MainWindow : Window
{
    private ToplantiDbContext? _context;
    private Meeting? _currentMeeting;
    private Site? _selectedSite;

    public MainWindow()
    {
        try
        {
            InitializeComponent();
            LoadData();
            
            // Toplantı yönetimi sekmesi için başlangıç durumu
            if (cmbMeetingSelection != null)
            {
                txtMeetingStatus.Text = "(Toplanti seciniz)";
                txtMeetingStatus.Foreground = System.Windows.Media.Brushes.Red;
            }
        }
        catch (Exception ex)
        {
            NotificationService.ShowCriticalError("Uygulama baslatma hatasi", ex);
        }
    }

    private void LoadData()
    {
        try
        {
            _context = new ToplantiDbContext(GetDbContextOptions());
            
            // Load sites
            LoadSites();
        }
        catch (Exception ex)
        {
            NotificationService.ShowError("Veri yukleme hatasi", "Hata", ex);
        }
    }

    private void LoadSites()
    {
        if (_context == null) return;

        try
        {
            var sites = _context.Sites.OrderBy(s => s.Name).ToList();
            
            // "Seçiniz" seçeneğini ekle
            var siteList = new List<object> { "Seciniz" };
            siteList.AddRange(sites);
            
            cmbSite.ItemsSource = siteList;
            cmbSite.SelectedIndex = 0; // "Seçiniz" seçili olsun
            _selectedSite = null;
        }
        catch (Exception ex)
        {
            NotificationService.ShowError($"Site yukleme hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private void CmbSite_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (cmbSite.IsDropDownOpen)
        {
            var searchText = cmbSite.Text?.ToLower() ?? "";
            if (string.IsNullOrWhiteSpace(searchText))
            {
                LoadSites();
                return;
            }

            var sites = _context?.Sites.OrderBy(s => s.Name).ToList() ?? [];
            var filteredSites = sites.Where(s => s.Name.ToLower().Contains(searchText)).ToList();
            
            var siteList = new List<object> { "Seciniz" };
            siteList.AddRange(filteredSites);
            cmbSite.ItemsSource = siteList;
        }
    }

    private void CmbSite_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (cmbSite.SelectedItem is Site selectedSite)
            {
                _selectedSite = selectedSite;
                LoadDashboard();
            }
            else
            {
                _selectedSite = null;
                borderNoSite.Visibility = Visibility.Visible;
                spDashboard.Visibility = Visibility.Collapsed;
                tcDetails.Visibility = Visibility.Collapsed;
            }
        }
        catch (Exception ex)
        {
            NotificationService.ShowError("Site secim hatasi", "Hata", ex);
        }
    }

    private void LoadDashboard()
    {
        if (_selectedSite == null) return;

        try
        {
            borderNoSite.Visibility = Visibility.Collapsed;
            spDashboard.Visibility = Visibility.Visible;
            tcDetails.Visibility = Visibility.Collapsed;

            LoadDashboardStats();
            LoadRecentMeetings();
            LoadRecentDecisions();
        }
        catch (Exception ex)
        {
            NotificationService.ShowError("Dashboard yukleme hatasi", "Hata", ex);
        }
    }

    private void LoadDashboardStats()
    {
        if (_context == null || _selectedSite == null) return;

        try
        {
            var totalUnits = _context.Units.Count(u => u.SiteId == _selectedSite.Id && u.IsActive);
            txtTotalUnits.Text = totalUnits.ToString();

            var siteUnitIds = _context.Units
                .Where(u => u.SiteId == _selectedSite.Id && u.IsActive)
                .Select(u => u.Id)
                .ToList();

            var totalMeetings = siteUnitIds.Any()
                ? _context.Meetings
                    .Include(m => m.Attendances)
                    .Count(m => m.Attendances.Any(a => siteUnitIds.Contains(a.UnitId)) || !m.Attendances.Any())
                : _context.Meetings.Count();
            txtTotalMeetings.Text = totalMeetings.ToString();

            var meetingIds = siteUnitIds.Any()
                ? _context.MeetingAttendances
                    .Where(a => siteUnitIds.Contains(a.UnitId))
                    .Select(a => a.MeetingId)
                    .Distinct()
                    .ToList()
                : [];

            var totalDecisions = meetingIds.Any()
                ? _context.Decisions.Count(d => meetingIds.Contains(d.MeetingId))
                : 0;
            txtTotalDecisions.Text = totalDecisions.ToString();

            txtTotalLandShareDashboard.Text = _selectedSite.TotalLandShare.ToString("F2");
        }
        catch (Exception ex)
        {
            NotificationService.ShowError($"Istatistik yukleme hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private void LoadRecentMeetings()
    {
        if (_context == null || _selectedSite == null) return;

        try
        {
            var siteUnitIds = _context.Units
                .Where(u => u.SiteId == _selectedSite.Id && u.IsActive)
                .Select(u => u.Id)
                .ToList();

            var meetings = siteUnitIds.Any()
                ? _context.Meetings
                    .Include(m => m.Attendances)
                    .Where(m => m.Attendances.Any(a => siteUnitIds.Contains(a.UnitId)) || !m.Attendances.Any())
                    .OrderByDescending(m => m.MeetingDate)
                    .Take(5)
                    .ToList()
                : _context.Meetings
                    .OrderByDescending(m => m.MeetingDate)
                    .Take(5)
                    .ToList();

            dgRecentMeetings.ItemsSource = meetings;
        }
        catch (Exception ex)
        {
            NotificationService.ShowError($"Toplanti yukleme hatasi: {ex.Message}", "Hata", ex);
            dgRecentMeetings.ItemsSource = null;
        }
    }

    private void LoadRecentDecisions()
    {
        if (_context == null || _selectedSite == null) return;

        try
        {
            var siteUnitIds = _context.Units
                .Where(u => u.SiteId == _selectedSite.Id && u.IsActive)
                .Select(u => u.Id)
                .ToList();

            var meetingIds = siteUnitIds.Any()
                ? _context.MeetingAttendances
                    .Where(a => siteUnitIds.Contains(a.UnitId))
                    .Select(a => a.MeetingId)
                    .Distinct()
                    .ToList()
                : [];

            var decisions = meetingIds.Any()
                ? _context.Decisions
                    .Include(d => d.Votes)
                        .ThenInclude(v => v.Unit)
                    .Where(d => meetingIds.Contains(d.MeetingId))
                    .OrderByDescending(d => d.CreatedAt)
                    .Take(5)
                    .ToList()
                    .Select(d => new
                    {
                        d.Id,
                        d.Title,
                        d.Description,
                        d.IsApproved,
                        YesVotes = d.Votes.Count(v => v.VoteType == VoteType.Yes && siteUnitIds.Contains(v.UnitId)),
                        NoVotes = d.Votes.Count(v => v.VoteType == VoteType.No && siteUnitIds.Contains(v.UnitId)),
                        AbstainVotes = d.Votes.Count(v => v.VoteType == VoteType.Abstain && siteUnitIds.Contains(v.UnitId))
                    })
                    .ToList()
                : [];

            dgRecentDecisions.ItemsSource = decisions;
        }
        catch (Exception ex)
        {
            NotificationService.ShowError($"Karar yukleme hatasi: {ex.Message}", "Hata", ex);
            dgRecentDecisions.ItemsSource = null;
        }
    }

    private DbContextOptions<ToplantiDbContext> GetDbContextOptions()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ToplantiDbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ToplantiDb;Trusted_Connection=true;MultipleActiveResultSets=true");
        return optionsBuilder.Options;
    }

    private void LoadUnits()
    {
        if (_context == null || _selectedSite == null) return;
        
        dgUnits.ItemsSource = _context.Units
            .Include(u => u.UnitType)
            .Where(u => u.SiteId == _selectedSite.Id && u.IsActive)
            .OrderBy(u => u.Block)
            .ThenBy(u => u.Number)
            .ToList();
    }

    private void LoadMeetings()
    {
        if (_context == null || _selectedSite == null) return;
        
        // Meeting'lerde SiteId yoksa, siteye ait birimlerin katıldığı toplantıları bul
        var siteUnitIds = _context.Units
            .Where(u => u.SiteId == _selectedSite.Id && u.IsActive)
            .Select(u => u.Id)
            .ToList();

        var meetings = _context.Meetings
            .Include(m => m.Attendances)
            .Where(m => m.Attendances.Any(a => siteUnitIds.Contains(a.UnitId)) || 
                       !m.Attendances.Any()) // Henüz katılım olmayan toplantılar
            .OrderByDescending(m => m.MeetingDate)
            .ToList();

        dgMeetings.ItemsSource = meetings;
        
        // Oylama sekmesindeki toplantı listesini de güncelle
        if (cmbVotingMeeting != null)
        {
            cmbVotingMeeting.ItemsSource = meetings;
        }
        
        // Toplantı yönetimi sekmesindeki toplantı listesini de güncelle
        if (cmbMeetingSelection != null)
        {
            cmbMeetingSelection.ItemsSource = meetings;
        }
    }

    private void LoadDecisions()
    {
        if (_context == null) return;
        
        // Eğer oylama sekmesindeyse ve toplantı seçiliyse, sadece o toplantının kararlarını göster
        if (tabDecisions.IsSelected && _currentMeeting != null)
        {
            var votingDecisions = _context.Decisions
                .Include(d => d.Votes)
                    .ThenInclude(v => v.Unit)
                .Include(d => d.Meeting)
                .Where(d => d.MeetingId == _currentMeeting.Id)
                .OrderByDescending(d => d.CreatedAt)
                .ToList();

            dgDecisions.ItemsSource = votingDecisions;
            return;
        }

        // Aksi halde siteye ait birimlerin katıldığı toplantılardaki kararları bul
        if (_selectedSite == null) return;
        
        var siteUnitIds = _context.Units
            .Where(u => u.SiteId == _selectedSite.Id && u.IsActive)
            .Select(u => u.Id)
            .ToList();

        var meetingIds = _context.MeetingAttendances
            .Where(a => siteUnitIds.Contains(a.UnitId))
            .Select(a => a.MeetingId)
            .Distinct()
            .ToList();

        var decisions = _context.Decisions
            .Include(d => d.Votes)
                .ThenInclude(v => v.Unit)
            .Include(d => d.Meeting)
            .Where(d => meetingIds.Contains(d.MeetingId))
            .OrderByDescending(d => d.CreatedAt)
            .ToList()
            .Select(d => new
            {
                d.Id,
                d.Title,
                d.Description,
                d.IsApproved,
                YesVotes = d.Votes.Count(v => v.VoteType == VoteType.Yes && siteUnitIds.Contains(v.UnitId)),
                NoVotes = d.Votes.Count(v => v.VoteType == VoteType.No && siteUnitIds.Contains(v.UnitId)),
                AbstainVotes = d.Votes.Count(v => v.VoteType == VoteType.Abstain && siteUnitIds.Contains(v.UnitId))
            })
            .ToList();

        dgDecisions.ItemsSource = decisions;
    }


    private void BtnDeleteUnit_Click(object sender, RoutedEventArgs e)
    {
        if (_context == null) return;
        
        if (sender is Button btn && btn.Tag is int unitId)
        {
            var result = NotificationService.ShowConfirmation("Bu birimi silmek istediğinizden emin misiniz?", "Onay");
            if (result == MessageBoxResult.Yes)
            {
                var unit = _context.Units.Find(unitId);
                if (unit != null)
                {
                    unit.IsActive = false;
                    _context.SaveChanges();
                    LoadUnits();
                }
            }
        }
    }

    private void BtnBlockManagement_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_selectedSite == null)
            {
                NotificationService.ShowWarning("Lutfen once bir site secin.");
                return;
            }

            var window = new BlockUnitManagementWindow(_selectedSite);
            window.ShowDialog();
            
            // Seçili siteyi koru ve dashboard'u yenile
            LoadDashboard();
        }
        catch (Exception ex)
        {
            NotificationService.ShowError($"Blok ve daire yonetimi acilirken hata: {ex.Message}", "Hata", ex);
        }
    }

    // Dashboard Event Handlers
    private void CardUnits_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        tcDetails.Visibility = Visibility.Visible;
        spDashboard.Visibility = Visibility.Collapsed;
        tabUnits.IsSelected = true;
        LoadUnits();
    }

    private void CardMeetings_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        tcDetails.Visibility = Visibility.Visible;
        spDashboard.Visibility = Visibility.Collapsed;
        tabMeetings.IsSelected = true;
        LoadMeetings();
    }

    private void CardDecisions_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        tcDetails.Visibility = Visibility.Visible;
        spDashboard.Visibility = Visibility.Collapsed;
        tabDecisions.IsSelected = true;
        LoadDecisions();
    }

    private void BtnBackToDashboard_Click(object sender, RoutedEventArgs e)
    {
        tcDetails.Visibility = Visibility.Collapsed;
        spDashboard.Visibility = Visibility.Visible;
        LoadDashboard();
    }

    private void BtnMeetingsDetail_Click(object sender, RoutedEventArgs e)
    {
        tcDetails.Visibility = Visibility.Visible;
        spDashboard.Visibility = Visibility.Collapsed;
        tabMeetings.IsSelected = true;
        LoadMeetings();
    }

    private void BtnDecisionsDetail_Click(object sender, RoutedEventArgs e)
    {
        tcDetails.Visibility = Visibility.Visible;
        spDashboard.Visibility = Visibility.Collapsed;
        tabDecisions.IsSelected = true;
        LoadDecisions();
    }

    private void DgRecentMeetings_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (dgRecentMeetings.SelectedItem is Meeting meeting)
        {
            tcDetails.Visibility = Visibility.Visible;
            spDashboard.Visibility = Visibility.Collapsed;
            tabMeetings.IsSelected = true;
            LoadMeetings();
            
            // Seçili toplantıyı işaretle
            if (dgMeetings.ItemsSource is System.Collections.IEnumerable items)
            {
                foreach (var item in items)
                {
                    if (item is Meeting m && m.Id == meeting.Id)
                    {
                        dgMeetings.SelectedItem = item;
                        dgMeetings.ScrollIntoView(item);
                        break;
                    }
                }
            }
        }
    }

    private void DgRecentDecisions_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (dgRecentDecisions.SelectedItem != null)
        {
            tcDetails.Visibility = Visibility.Visible;
            spDashboard.Visibility = Visibility.Collapsed;
            tabDecisions.IsSelected = true;
            LoadDecisions();
        }
    }

    private void BtnCreateMeeting_Click(object sender, RoutedEventArgs e)
    {
        if (_context == null)
        {
            NotificationService.ShowError("Veritabani baglantisi bulunamadi.", "Hata");
            return;
        }

        if (_selectedSite == null)
        {
            NotificationService.ShowWarning("Lutfen once bir site secin.");
            return;
        }

        try
        {
            var dialog = new CreateMeetingDialog(_context, _selectedSite);
            if (dialog.ShowDialog() == true && dialog.CreatedMeeting != null)
            {
                var createdMeeting = dialog.CreatedMeeting;
                
                // Veritabanından güncel toplantıyı yükle
                _currentMeeting = _context.Meetings
                    .Include(m => m.Attendances)
                        .ThenInclude(a => a.Unit)
                            .ThenInclude(u => u!.UnitType)
                    .Include(m => m.Proxies)
                        .ThenInclude(p => p.GiverUnit)
                    .Include(m => m.Proxies)
                        .ThenInclude(p => p.ReceiverUnit)
                    .Include(m => m.AgendaItems)
                    .Include(m => m.Documents)
                    .FirstOrDefault(m => m.Id == createdMeeting.Id);

                LoadMeetings();
                UpdateMeetingInfo();
                EnableDisableMeetingControls();
                
                // Toplantı seçim ComboBox'ını güncelle ve seçim değişikliğini tetikle
                if (cmbMeetingSelection != null && _currentMeeting != null)
                {
                    cmbMeetingSelection.SelectedItem = _currentMeeting;
                    // Seçim değişikliğini manuel olarak tetikle
                    if (_currentMeeting.IsCompleted)
                    {
                        txtMeetingStatus.Text = "(Tamamlanmis)";
                        txtMeetingStatus.Foreground = System.Windows.Media.Brushes.Red;
                    }
                    else
                    {
                        txtMeetingStatus.Text = "";
                    }
                    UpdateMeetingInfo();
                    LoadDecisions();
                    LoadAgendaItems();
                    LoadDocuments();
                    LoadVotingItems();
                    EnableDisableMeetingControls();
                }
                
                // DataGrid'de seçili yap
                if (dgMeetings != null && _currentMeeting != null)
                {
                    dgMeetings.SelectedItem = _currentMeeting;
                    dgMeetings.ScrollIntoView(_currentMeeting);
                }
            }
        }
        catch (Exception ex)
        {
            NotificationService.ShowError($"Toplanti olusturma hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private void DgMeetings_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (dgMeetings.SelectedItem is Meeting meeting)
        {
            _currentMeeting = _context?.Meetings
                .Include(m => m.Attendances)
                    .ThenInclude(a => a.Unit)
                        .ThenInclude(u => u!.UnitType)
                .Include(m => m.Proxies)
                    .ThenInclude(p => p.GiverUnit)
                .Include(m => m.Proxies)
                    .ThenInclude(p => p.ReceiverUnit)
                .Include(m => m.AgendaItems)
                .Include(m => m.Documents)
                .FirstOrDefault(m => m.Id == meeting.Id);
            
            UpdateMeetingInfo();
            LoadDecisions();
            LoadAgendaItems();
            LoadDocuments();
            LoadVotingItems();
            EnableDisableMeetingControls();
            
            // Oylama sekmesindeki toplantı seçimini güncelle
            if (cmbVotingMeeting != null && _currentMeeting != null)
            {
                cmbVotingMeeting.SelectedItem = _currentMeeting;
            }
        }
    }

    private void DgDecisions_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // This can be used for future enhancements
    }

    private void UpdateMeetingInfo()
    {
        if (_currentMeeting == null || _context == null)
        {
            txtMeetingInfo.Text = "Lütfen bir toplantı seçin.";
            txtQuorumInfo.Text = "";
            return;
        }

        _context.Entry(_currentMeeting).Reload();
        _currentMeeting = _context.Meetings
            .Include(m => m.Attendances)
            .Include(m => m.Proxies)
            .FirstOrDefault(m => m.Id == _currentMeeting.Id);

        if (_currentMeeting == null) return;

        var attendedUnits = _context.MeetingAttendances
            .Where(a => a.MeetingId == _currentMeeting.Id)
            .Include(a => a.Unit)
            .Select(a => a.Unit!)
            .ToList();

        var attendedLandShare = attendedUnits.Sum(u => u.LandShare);
        var attendedCount = attendedUnits.Count;

        txtMeetingInfo.Text = $"Toplantı: {_currentMeeting.Title}\n" +
                             $"Tarih: {_currentMeeting.MeetingDate:dd.MM.yyyy HH:mm}\n" +
                             $"Toplam Birim: {_currentMeeting.TotalUnitCount}\n" +
                             $"Katılan Birim: {attendedCount}\n" +
                             $"Toplam Arsa Payı: {_currentMeeting.TotalSiteLandShare:F2}\n" +
                             $"Katılan Arsa Payı: {attendedLandShare:F2}";

        var quorumService = new QuorumService();
        var (achieved, message) = quorumService.CheckQuorum(
            _currentMeeting.TotalUnitCount,
            attendedCount,
            _currentMeeting.TotalSiteLandShare,
            attendedLandShare);

        txtQuorumInfo.Text = message;
        txtQuorumInfo.Foreground = achieved ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
    }

    private void BtnAddAttendance_Click(object sender, RoutedEventArgs e)
    {
        if (_currentMeeting == null || _context == null)
        {
            NotificationService.ShowWarning("Lutfen once bir toplanti secin.");
            return;
        }

        if (_selectedSite == null)
        {
            NotificationService.ShowWarning("Lutfen once bir site secin.");
            return;
        }

        var dialog = new AttendanceDialog(_context.Units.Where(u => u.SiteId == _selectedSite.Id && u.IsActive).ToList());
        if (dialog.ShowDialog() == true && dialog.SelectedUnits.Count > 0)
        {
            var existingUnitIds = _context.MeetingAttendances
                .Where(a => a.MeetingId == _currentMeeting.Id)
                .Select(a => a.UnitId)
                .ToList();

            var newAttendances = new List<MeetingAttendance>();
            var skippedUnits = new List<string>();

            foreach (var unit in dialog.SelectedUnits)
            {
                if (existingUnitIds.Contains(unit.Id))
                {
                    skippedUnits.Add(unit.Number);
                    continue;
                }

                newAttendances.Add(new MeetingAttendance
                {
                    MeetingId = _currentMeeting.Id,
                    UnitId = unit.Id,
                    IsProxy = false
                });
            }

            if (newAttendances.Count > 0)
            {
                _context.MeetingAttendances.AddRange(newAttendances);
                _context.SaveChanges();

                var message = $"{newAttendances.Count} adet katilim eklendi.";
                if (skippedUnits.Count > 0)
                {
                    message += $"\n{skippedUnits.Count} adet birim zaten katilimli: {string.Join(", ", skippedUnits)}";
                }
                NotificationService.ShowSuccess(message);
            }
            else if (skippedUnits.Count > 0)
            {
                NotificationService.ShowWarning($"Secilen tum birimler zaten toplantiya katilmis.\n{string.Join(", ", skippedUnits)}");
            }

            UpdateMeetingInfo();
        }
    }

    private void BtnAddProxy_Click(object sender, RoutedEventArgs e)
    {
        var validationService = new MeetingValidationService(_context!);
        
        var contextValidation = validationService.ValidateMeetingAndContext(_currentMeeting, _context);
        if (!contextValidation.IsValid)
        {
            NotificationService.ShowWarning(contextValidation.ErrorMessage!);
            return;
        }

        var completedValidation = validationService.ValidateMeetingNotCompleted(_currentMeeting!, "Vekalet eklenemez");
        if (!completedValidation.IsValid)
        {
            NotificationService.ShowWarning(completedValidation.ErrorMessage!);
            return;
        }

        var proxyService = new ProxyService();
        var currentProxies = _context.Proxies
            .Where(p => p.MeetingId == _currentMeeting.Id)
            .Include(p => p.GiverUnit)
            .ToList();
            
        var currentProxyCount = currentProxies.Count;
        var currentProxyLandShare = currentProxies
            .Where(p => p.GiverUnit != null)
            .Sum(p => p.GiverUnit!.LandShare);
        
        var dialog = new ProxyDialog(
            _context.Units.Where(u => u.IsActive).ToList(),
            _currentMeeting.TotalUnitCount,
            _currentMeeting.TotalSiteLandShare,
            currentProxyCount,
            currentProxyLandShare,
            currentProxies);
            
        if (dialog.ShowDialog() == true && dialog.GiverUnits.Any() && (dialog.ReceiverUnit != null || !string.IsNullOrWhiteSpace(dialog.ReceiverName)))
        {
            var addedCount = 0;
            var skippedCount = 0;
            
            foreach (var giverUnit in dialog.GiverUnits)
            {
                // Check if proxy already exists
                Proxy? existingProxy = null;
                if (dialog.ReceiverUnit != null)
                {
                    existingProxy = _context.Proxies
                        .FirstOrDefault(p => p.MeetingId == _currentMeeting.Id && 
                                            p.GiverUnitId == giverUnit.Id && 
                                            p.ReceiverUnitId == dialog.ReceiverUnit.Id);
                }
                else if (!string.IsNullOrWhiteSpace(dialog.ReceiverName))
                {
                    existingProxy = _context.Proxies
                        .FirstOrDefault(p => p.MeetingId == _currentMeeting.Id && 
                                            p.GiverUnitId == giverUnit.Id && 
                                            p.ReceiverUnitId == null &&
                                            p.ReceiverName == dialog.ReceiverName);
                }
                
                if (existingProxy != null)
                {
                    skippedCount++;
                    continue;
                }

                var proxy = new Proxy
                {
                    MeetingId = _currentMeeting.Id,
                    GiverUnitId = giverUnit.Id,
                    ReceiverUnitId = dialog.ReceiverUnit?.Id,
                    ReceiverName = dialog.ReceiverName,
                    ReceiverPhone = dialog.ReceiverPhone
                };

                _context.Proxies.Add(proxy);
                addedCount++;

                // Add proxy attendance for giver
                var proxyAttendance = _context.MeetingAttendances
                    .FirstOrDefault(a => a.MeetingId == _currentMeeting.Id && a.UnitId == giverUnit.Id);

                if (proxyAttendance == null)
                {
                    proxyAttendance = new MeetingAttendance
                    {
                        MeetingId = _currentMeeting.Id,
                        UnitId = giverUnit.Id,
                        IsProxy = true,
                        ProxyId = proxy.Id
                    };
                    _context.MeetingAttendances.Add(proxyAttendance);
                }
            }

            // Add attendance for receiver if it's a unit (not "Other")
            if (dialog.ReceiverUnit != null)
            {
                var receiverAttendance = _context.MeetingAttendances
                    .FirstOrDefault(a => a.MeetingId == _currentMeeting.Id && a.UnitId == dialog.ReceiverUnit.Id);

                if (receiverAttendance == null)
                {
                    receiverAttendance = new MeetingAttendance
                    {
                        MeetingId = _currentMeeting.Id,
                        UnitId = dialog.ReceiverUnit.Id,
                        IsProxy = false
                    };
                    _context.MeetingAttendances.Add(receiverAttendance);
                }
            }

            _context.SaveChanges();

            var message = $"{addedCount} vekalet eklendi.";
            if (skippedCount > 0)
            {
                message += $" {skippedCount} vekalet zaten mevcut oldugu icin atlandi.";
            }
            
            UpdateMeetingInfo();
            MessageBox.Show(message, "Basarili", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void BtnCheckQuorum_Click(object sender, RoutedEventArgs e)
    {
        if (_currentMeeting == null || _context == null)
        {
            NotificationService.ShowWarning("Lütfen önce bir toplantı seçin.");
            return;
        }

        var attendedUnits = _context.MeetingAttendances
            .Where(a => a.MeetingId == _currentMeeting.Id)
            .Include(a => a.Unit)
            .Select(a => a.Unit!)
            .ToList();

        var attendedLandShare = attendedUnits.Sum(u => u.LandShare);
        var attendedCount = attendedUnits.Count;

        var quorumService = new QuorumService();
        var (achieved, message) = quorumService.CheckQuorum(
            _currentMeeting.TotalUnitCount,
            attendedCount,
            _currentMeeting.TotalSiteLandShare,
            attendedLandShare);

        _currentMeeting.AttendedUnitCount = attendedCount;
        _currentMeeting.AttendedLandShare = attendedLandShare;
        _currentMeeting.QuorumAchieved = achieved;
        _context.SaveChanges();

        UpdateMeetingInfo();
        if (achieved)
            NotificationService.ShowInfo(message, "Yeter Sayı Sağlandı");
        else
            NotificationService.ShowWarning(message, "Yeter Sayı Sağlanamadı");
    }

    private void BtnGenerateMinutes_Click(object sender, RoutedEventArgs e)
    {
        if (_currentMeeting == null || _context == null)
        {
            NotificationService.ShowWarning("Lütfen önce bir toplantı seçin.");
            return;
        }

        // Reload meeting with all related data
        var meeting = _context.Meetings
            .Include(m => m.Decisions)
            .FirstOrDefault(m => m.Id == _currentMeeting.Id);

        if (meeting == null) return;

        var allUnits = _context.Units.Where(u => u.IsActive).ToList();
        var proxies = _context.Proxies
            .Where(p => p.MeetingId == meeting.Id)
            .Include(p => p.GiverUnit)
            .Include(p => p.ReceiverUnit)
            .ToList();

        var minutesService = new MeetingMinutesService();
        var minutes = minutesService.GenerateMeetingMinutes(meeting, allUnits, proxies);

        meeting.MeetingMinutes = minutes;
        _context.SaveChanges();

        var window = new MinutesWindow(minutes);
        window.ShowDialog();
    }


    private void BtnCreateDecision_Click(object sender, RoutedEventArgs e)
    {
        var validationService = new MeetingValidationService(_context!);
        
        var contextValidation = validationService.ValidateMeetingAndContext(_currentMeeting, _context);
        if (!contextValidation.IsValid)
        {
            NotificationService.ShowWarning(contextValidation.ErrorMessage!);
            return;
        }

        var completedValidation = validationService.ValidateMeetingNotCompleted(_currentMeeting!, "Yeni karar eklenemez");
        if (!completedValidation.IsValid)
        {
            NotificationService.ShowWarning(completedValidation.ErrorMessage!);
            return;
        }

        if (string.IsNullOrWhiteSpace(txtDecisionTitle.Text) ||
            string.IsNullOrWhiteSpace(txtDecisionDescription.Text))
        {
            NotificationService.ShowWarning("Lutfen tum alanlari doldurun.");
            return;
        }

        var decision = new Decision
        {
            MeetingId = _currentMeeting.Id,
            Title = txtDecisionTitle.Text.Trim(),
            Description = txtDecisionDescription.Text.Trim(),
            YesVotes = 0,
            NoVotes = 0,
            AbstainVotes = 0,
            IsApproved = false
        };

        _context.Decisions.Add(decision);
        _context.SaveChanges();

        LoadDecisions();

        txtDecisionTitle.Clear();
        txtDecisionDescription.Clear();

        NotificationService.ShowSuccess("Karar başarıyla oluşturuldu.", "Başarılı");
    }

    private void BtnVote_Click(object sender, RoutedEventArgs e)
    {
        if (_currentMeeting == null || _context == null) return;

        var validationService = new MeetingValidationService(_context);
        var completedValidation = validationService.ValidateMeetingNotCompleted(_currentMeeting, "Oy kullanilamaz");
        if (!completedValidation.IsValid)
        {
            NotificationService.ShowWarning(completedValidation.ErrorMessage!);
            return;
        }

        if (sender is Button btn && btn.Tag is int decisionId)
        {
            var decision = _context.Decisions
                .Include(d => d.Meeting)
                .FirstOrDefault(d => d.Id == decisionId);
            if (decision == null) return;
            
            if (decision.Meeting?.IsCompleted == true)
            {
                NotificationService.ShowWarning("Bu toplanti tamamlanmis. Oy kullanilamaz.");
                return;
            }

            var attendedUnits = _context.MeetingAttendances
                .Where(a => a.MeetingId == _currentMeeting.Id)
                .Include(a => a.Unit)
                .Select(a => a.Unit!)
                .ToList();

            var dialog = new VoteDialog(attendedUnits, decision);
            if (dialog.ShowDialog() == true)
            {
                // Remove existing votes for this decision
                var existingVotes = _context.Votes.Where(v => v.DecisionId == decisionId).ToList();
                _context.Votes.RemoveRange(existingVotes);

                // Add new votes
                foreach (var vote in dialog.Votes)
                {
                    vote.DecisionId = decisionId;
                    vote.MeetingId = _currentMeeting.Id;
                    _context.Votes.Add(vote);
                }

                // Calculate results
                var votingService = new VotingService();
                var units = _context.Units.Where(u => u.IsActive).ToList();
                var allVotes = _context.Votes.Where(v => v.DecisionId == decisionId).ToList();
                var (yesCount, noCount, abstainCount, yesLandShare, noLandShare, abstainLandShare) =
                    votingService.CalculateVotes(allVotes, units);

                decision.YesVotes = yesCount;
                decision.NoVotes = noCount;
                decision.AbstainVotes = abstainCount;
                decision.YesLandShare = yesLandShare;
                decision.NoLandShare = noLandShare;
                decision.AbstainLandShare = abstainLandShare;

                var attendedLandShare = attendedUnits.Sum(u => u.LandShare);
                decision.IsApproved = votingService.IsDecisionApproved(
                    yesCount, noCount, yesLandShare, noLandShare,
                    attendedUnits.Count, attendedLandShare);

                _context.SaveChanges();
                LoadDecisions();
            }
        }
    }

    private void BtnDecisionDetail_Click(object sender, RoutedEventArgs e)
    {
        if (_context == null) return;

        if (sender is Button btn && btn.Tag is int decisionId)
        {
            var decision = _context.Decisions
                .Include(d => d.Votes)
                    .ThenInclude(v => v.Unit)
                .FirstOrDefault(d => d.Id == decisionId);

            if (decision != null)
            {
                var window = new DecisionDetailWindow(decision);
                window.ShowDialog();
            }
        }
    }

    private void BtnAbout_Click(object sender, RoutedEventArgs e)
    {
        var aboutDialog = new AboutDialog();
        aboutDialog.Owner = this;
        aboutDialog.ShowDialog();
    }

    private void BtnHiddenDelete_Click(object sender, RoutedEventArgs e)
    {
        var passwordDialog = new PasswordDialog();
        if (passwordDialog.ShowDialog() == true && passwordDialog.IsPasswordCorrect)
        {
            var result = NotificationService.ShowConfirmation(
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
                "DIKKAT!");

            if (result != MessageBoxResult.Yes) return;

            try
            {
                if (_context == null)
                {
                    _context = new ToplantiDbContext(GetDbContextOptions());
                }

                // Delete in correct order to avoid foreign key violations
                _context.Votes.RemoveRange(_context.Votes);
                _context.Decisions.RemoveRange(_context.Decisions);
                _context.MeetingAttendances.RemoveRange(_context.MeetingAttendances);
                _context.Proxies.RemoveRange(_context.Proxies);
                _context.Units.RemoveRange(_context.Units);
                _context.Meetings.RemoveRange(_context.Meetings);
                _context.Sites.RemoveRange(_context.Sites);
                
                _context.SaveChanges();

                LoadUnits();
                LoadMeetings();

                NotificationService.ShowSuccess("Tum veriler basariyla silindi.");
            }
            catch (Exception ex)
            {
                NotificationService.ShowError($"Veri silme hatasi: {ex.Message}", "Hata", ex);
            }
        }
    }

    private void BtnAddAgenda_Click(object sender, RoutedEventArgs e)
    {
        if (_context == null || _currentMeeting == null)
        {
            NotificationService.ShowWarning("Lutfen once bir toplanti secin.");
            return;
        }

        if (string.IsNullOrWhiteSpace(txtAgendaTitle.Text))
        {
            NotificationService.ShowWarning("Lutfen gundem basligini girin.");
            return;
        }

        try
        {
            var orders = _context.AgendaItems
                .Where(a => a.MeetingId == _currentMeeting.Id)
                .Select(a => a.Order)
                .ToList();
            
            var maxOrder = orders.Any() ? orders.Max() : 0;

            var agendaItem = new AgendaItem
            {
                MeetingId = _currentMeeting.Id,
                Title = txtAgendaTitle.Text.Trim(),
                Description = null,
                Order = maxOrder + 1
            };

            _context.AgendaItems.Add(agendaItem);
            _context.SaveChanges();

            txtAgendaTitle.Clear();
            LoadAgendaItems();
        }
        catch (Exception ex)
        {
            NotificationService.ShowError($"Gundem ekleme hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private void BtnDeleteAgenda_Click(object sender, RoutedEventArgs e)
    {
        if (_context == null || _currentMeeting == null) return;

        if (sender is Button button && button.DataContext is AgendaItem agendaItem)
        {
            try
            {
                _context.AgendaItems.Remove(agendaItem);
                _context.SaveChanges();
                LoadAgendaItems();
            }
            catch (Exception ex)
            {
                NotificationService.ShowError($"Gundem silme hatasi: {ex.Message}", "Hata", ex);
            }
        }
    }

    private void LoadAgendaItems()
    {
        if (_context == null || _currentMeeting == null)
        {
            dgAgendaItems.ItemsSource = null;
            return;
        }

        var agendaItems = _context.AgendaItems
            .Where(a => a.MeetingId == _currentMeeting.Id)
            .OrderBy(a => a.Order)
            .ToList();

        dgAgendaItems.ItemsSource = agendaItems;
    }

    private void BtnAddDocument_Click(object sender, RoutedEventArgs e)
    {
        if (_context == null || _currentMeeting == null)
        {
            NotificationService.ShowWarning("Lutfen once bir toplanti secin.");
            return;
        }

        if (_currentMeeting.IsCompleted)
        {
            NotificationService.ShowWarning("Bu toplanti tamamlanmis. Yeni belge eklenemez.");
            return;
        }

        if (string.IsNullOrWhiteSpace(txtDocumentTitle.Text) || cmbDocumentType.SelectedItem == null)
        {
            NotificationService.ShowWarning("Lutfen belge basligi ve tipini secin.");
            return;
        }

        try
        {
            var documentType = (cmbDocumentType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Diger";

            var document = new Document
            {
                MeetingId = _currentMeeting.Id,
                Title = txtDocumentTitle.Text.Trim(),
                DocumentType = documentType,
                Content = null
            };

            _context.Documents.Add(document);
            _context.SaveChanges();

            txtDocumentTitle.Clear();
            cmbDocumentType.SelectedIndex = 0;
            LoadDocuments();
        }
        catch (Exception ex)
        {
            NotificationService.ShowError($"Belge ekleme hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private void BtnDeleteDocument_Click(object sender, RoutedEventArgs e)
    {
        if (_context == null || _currentMeeting == null) return;

        if (sender is Button button && button.DataContext is Document document)
        {
            try
            {
                _context.Documents.Remove(document);
                _context.SaveChanges();
                LoadDocuments();
            }
            catch (Exception ex)
            {
                NotificationService.ShowError($"Belge silme hatasi: {ex.Message}", "Hata", ex);
            }
        }
    }

    private void LoadDocuments()
    {
        if (_context == null || _currentMeeting == null)
        {
            dgDocuments.ItemsSource = null;
            return;
        }

        var documents = _context.Documents
            .Where(d => d.MeetingId == _currentMeeting.Id)
            .OrderBy(d => d.CreatedAt)
            .ToList();

        dgDocuments.ItemsSource = documents;
    }

    private void BtnAddVoting_Click(object sender, RoutedEventArgs e)
    {
        if (_context == null || _currentMeeting == null)
        {
            NotificationService.ShowWarning("Lutfen once bir toplanti secin.");
            return;
        }

        if (_currentMeeting.IsCompleted)
        {
            NotificationService.ShowWarning("Bu toplanti tamamlanmis. Yeni oylama eklenemez.");
            return;
        }

        if (string.IsNullOrWhiteSpace(txtVotingTitle.Text))
        {
            NotificationService.ShowWarning("Lutfen oylama basligini girin.");
            return;
        }

        try
        {
            var decision = new Decision
            {
                MeetingId = _currentMeeting.Id,
                Title = txtVotingTitle.Text.Trim(),
                Description = txtVotingDescription.Text?.Trim() ?? "",
                YesVotes = 0,
                NoVotes = 0,
                AbstainVotes = 0,
                YesLandShare = 0,
                NoLandShare = 0,
                AbstainLandShare = 0,
                IsApproved = false
            };

            _context.Decisions.Add(decision);
            _context.SaveChanges();

            txtVotingTitle.Clear();
            txtVotingDescription.Clear();
            LoadVotingItems();
            LoadDecisions();
        }
        catch (Exception ex)
        {
            NotificationService.ShowError($"Oylama ekleme hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private void BtnDeleteVoting_Click(object sender, RoutedEventArgs e)
    {
        if (_context == null || _currentMeeting == null) return;

        if (sender is Button button && button.DataContext is Decision decision)
        {
            try
            {
                _context.Decisions.Remove(decision);
                _context.SaveChanges();
                LoadVotingItems();
                LoadDecisions();
            }
            catch (Exception ex)
            {
                NotificationService.ShowError($"Oylama silme hatasi: {ex.Message}", "Hata", ex);
            }
        }
    }

    private void LoadVotingItems()
    {
        if (_context == null || _currentMeeting == null)
        {
            dgVotingItems.ItemsSource = null;
            return;
        }

        var decisions = _context.Decisions
            .Where(d => d.MeetingId == _currentMeeting.Id)
            .OrderBy(d => d.CreatedAt)
            .ToList();

        dgVotingItems.ItemsSource = decisions;
    }

    private void BtnCompleteMeeting_Click(object sender, RoutedEventArgs e)
    {
        if (_context == null || _currentMeeting == null)
        {
            NotificationService.ShowWarning("Lutfen once bir toplanti secin.");
            return;
        }

        var validationService = new MeetingValidationService(_context);
        
        var alreadyCompletedValidation = validationService.ValidateMeetingAlreadyCompleted(_currentMeeting);
        if (!alreadyCompletedValidation.IsValid)
        {
            NotificationService.ShowInfo(alreadyCompletedValidation.ErrorMessage!);
            return;
        }

        var decisionsValidation = validationService.ValidateMeetingHasDecisions(_currentMeeting);
        if (!decisionsValidation.IsValid)
        {
            NotificationService.ShowWarning(decisionsValidation.ErrorMessage!, "Karar Gerekli");
            return;
        }

        var result = NotificationService.ShowConfirmation(
            "Bu toplantiyi tamamlamak istediginizden emin misiniz?\n\n" +
            "Toplanti tamamlandiktan sonra:\n" +
            "- Yeni katilim eklenemez\n" +
            "- Yeni vekalet eklenemez\n" +
            "- Yeni karar/oylama eklenemez\n" +
            "- Mevcut oylamalara oy kullanilamaz\n" +
            "- Toplanti bilgileri duzenlenemez\n\n" +
            "Devam etmek istiyor musunuz?",
            "Toplantiyi Tamamla");

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                _currentMeeting.IsCompleted = true;
                _context.SaveChanges();

                EnableDisableMeetingControls();
                LoadMeetings();
                UpdateMeetingInfo();

                NotificationService.ShowSuccess("Toplanti basariyla tamamlandi.");
            }
            catch (Exception ex)
            {
                NotificationService.ShowError($"Toplanti tamamlama hatasi: {ex.Message}", "Hata", ex);
            }
        }
    }

    private void EnableDisableMeetingControls()
    {
        if (_currentMeeting == null)
        {
            btnAddAttendance.IsEnabled = false;
            btnAddProxy.IsEnabled = false;
            btnCheckQuorum.IsEnabled = false;
            btnCompleteMeeting.IsEnabled = false;
            btnGenerateMinutes.IsEnabled = false;
            txtAgendaTitle.IsEnabled = false;
            btnAddAgenda.IsEnabled = false;
            txtDocumentTitle.IsEnabled = false;
            cmbDocumentType.IsEnabled = false;
            btnAddDocument.IsEnabled = false;
            txtVotingTitle.IsEnabled = false;
            txtVotingDescription.IsEnabled = false;
            btnAddVoting.IsEnabled = false;
            return;
        }

        bool isCompleted = _currentMeeting.IsCompleted;

        btnAddAttendance.IsEnabled = !isCompleted;
        btnAddProxy.IsEnabled = !isCompleted;
        btnCheckQuorum.IsEnabled = !isCompleted;
        btnCompleteMeeting.IsEnabled = !isCompleted;
        btnGenerateMinutes.IsEnabled = isCompleted; // Sadece tamamlanmış toplantılar için tutanak oluşturulabilir
        txtAgendaTitle.IsEnabled = !isCompleted;
        btnAddAgenda.IsEnabled = !isCompleted;
        txtDocumentTitle.IsEnabled = !isCompleted;
        cmbDocumentType.IsEnabled = !isCompleted;
        btnAddDocument.IsEnabled = !isCompleted;
        txtVotingTitle.IsEnabled = !isCompleted;
        txtVotingDescription.IsEnabled = !isCompleted;
        btnAddVoting.IsEnabled = !isCompleted;
    }

    private void CmbVotingMeeting_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (cmbVotingMeeting.IsDropDownOpen && _context != null && _selectedSite != null)
        {
            var searchText = cmbVotingMeeting.Text?.ToLower() ?? "";
            if (string.IsNullOrWhiteSpace(searchText))
            {
                LoadMeetings();
                return;
            }

            // Site'ye ait birimlerin katıldığı toplantıları bul
            var siteUnitIds = _context.Units
                .Where(u => u.SiteId == _selectedSite.Id && u.IsActive)
                .Select(u => u.Id)
                .ToList();

            var meetings = _context.Meetings
                .Include(m => m.Attendances)
                .Where(m => m.Attendances.Any(a => siteUnitIds.Contains(a.UnitId)) || !m.Attendances.Any())
                .OrderByDescending(m => m.MeetingDate)
                .ToList();
            
            var filteredMeetings = meetings.Where(m => m.Title.ToLower().Contains(searchText)).ToList();
            cmbVotingMeeting.ItemsSource = filteredMeetings;
        }
    }

    private void CmbVotingMeeting_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cmbVotingMeeting.SelectedItem is Meeting selectedMeeting && _context != null)
        {
            _currentMeeting = _context.Meetings
                .Include(m => m.AgendaItems)
                .Include(m => m.Documents)
                .FirstOrDefault(m => m.Id == selectedMeeting.Id);

            if (_currentMeeting != null)
            {
                if (_currentMeeting.IsCompleted)
                {
                    txtVotingMeetingStatus.Text = "(Tamamlanmis)";
                    txtVotingMeetingStatus.Foreground = System.Windows.Media.Brushes.Red;
                    btnCreateDecision.IsEnabled = false;
                    txtDecisionTitle.IsEnabled = false;
                    txtDecisionDescription.IsEnabled = false;
                }
                else
                {
                    txtVotingMeetingStatus.Text = "";
                    btnCreateDecision.IsEnabled = true;
                    txtDecisionTitle.IsEnabled = true;
                    txtDecisionDescription.IsEnabled = true;
                }

                LoadDecisions();
            }
        }
        else
        {
            txtVotingMeetingStatus.Text = "(Toplanti seciniz)";
            txtVotingMeetingStatus.Foreground = System.Windows.Media.Brushes.Red;
            btnCreateDecision.IsEnabled = false;
            txtDecisionTitle.IsEnabled = false;
            txtDecisionDescription.IsEnabled = false;
            dgDecisions.ItemsSource = null;
        }
    }

    private void CmbMeetingSelection_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (cmbMeetingSelection.IsDropDownOpen && _context != null && _selectedSite != null)
        {
            var searchText = cmbMeetingSelection.Text?.ToLower() ?? "";
            if (string.IsNullOrWhiteSpace(searchText))
            {
                LoadMeetings();
                return;
            }

            // Site'ye ait birimlerin katıldığı toplantıları bul
            var siteUnitIds = _context.Units
                .Where(u => u.SiteId == _selectedSite.Id && u.IsActive)
                .Select(u => u.Id)
                .ToList();

            var meetings = _context.Meetings
                .Include(m => m.Attendances)
                .Where(m => m.Attendances.Any(a => siteUnitIds.Contains(a.UnitId)) || !m.Attendances.Any())
                .OrderByDescending(m => m.MeetingDate)
                .ToList();
            
            var filteredMeetings = meetings.Where(m => m.Title.ToLower().Contains(searchText)).ToList();
            cmbMeetingSelection.ItemsSource = filteredMeetings;
        }
    }

    private void CmbMeetingSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cmbMeetingSelection.SelectedItem is Meeting selectedMeeting && _context != null)
        {
            _currentMeeting = _context.Meetings
                .Include(m => m.Attendances)
                    .ThenInclude(a => a.Unit)
                        .ThenInclude(u => u!.UnitType)
                .Include(m => m.Proxies)
                    .ThenInclude(p => p.GiverUnit)
                .Include(m => m.Proxies)
                    .ThenInclude(p => p.ReceiverUnit)
                .Include(m => m.AgendaItems)
                .Include(m => m.Documents)
                .FirstOrDefault(m => m.Id == selectedMeeting.Id);

            if (_currentMeeting != null)
            {
                if (_currentMeeting.IsCompleted)
                {
                    txtMeetingStatus.Text = "(Tamamlanmis)";
                    txtMeetingStatus.Foreground = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    txtMeetingStatus.Text = "";
                }

                UpdateMeetingInfo();
                LoadDecisions();
                LoadAgendaItems();
                LoadDocuments();
                LoadVotingItems();
                EnableDisableMeetingControls();
            }
        }
        else
        {
            _currentMeeting = null;
            txtMeetingStatus.Text = "(Toplanti seciniz)";
            txtMeetingStatus.Foreground = System.Windows.Media.Brushes.Red;
            UpdateMeetingInfo();
            LoadAgendaItems();
            LoadDocuments();
            LoadVotingItems();
            EnableDisableMeetingControls();
        }
    }

    private void BtnMoveAgendaUp_Click(object sender, RoutedEventArgs e)
    {
        if (_context == null || _currentMeeting == null || _currentMeeting.IsCompleted) return;

        if (sender is Button button && button.DataContext is AgendaItem agendaItem)
        {
            try
            {
                var allItems = _context.AgendaItems
                    .Where(a => a.MeetingId == _currentMeeting.Id)
                    .OrderBy(a => a.Order)
                    .ToList();

                var currentIndex = allItems.FindIndex(a => a.Id == agendaItem.Id);
                if (currentIndex > 0)
                {
                    var previousItem = allItems[currentIndex - 1];
                    var tempOrder = agendaItem.Order;
                    agendaItem.Order = previousItem.Order;
                    previousItem.Order = tempOrder;

                    _context.SaveChanges();
                    LoadAgendaItems();
                }
            }
            catch (Exception ex)
            {
                NotificationService.ShowError($"Gundem sirasi degistirme hatasi: {ex.Message}", "Hata", ex);
            }
        }
    }

    private void BtnMoveAgendaDown_Click(object sender, RoutedEventArgs e)
    {
        if (_context == null || _currentMeeting == null || _currentMeeting.IsCompleted) return;

        if (sender is Button button && button.DataContext is AgendaItem agendaItem)
        {
            try
            {
                var allItems = _context.AgendaItems
                    .Where(a => a.MeetingId == _currentMeeting.Id)
                    .OrderBy(a => a.Order)
                    .ToList();

                var currentIndex = allItems.FindIndex(a => a.Id == agendaItem.Id);
                if (currentIndex < allItems.Count - 1)
                {
                    var nextItem = allItems[currentIndex + 1];
                    var tempOrder = agendaItem.Order;
                    agendaItem.Order = nextItem.Order;
                    nextItem.Order = tempOrder;

                    _context.SaveChanges();
                    LoadAgendaItems();
                }
            }
            catch (Exception ex)
            {
                NotificationService.ShowError($"Gundem sirasi degistirme hatasi: {ex.Message}", "Hata", ex);
            }
        }
    }
}

