using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Toplanti.Data;
using Toplanti.Dialogs;
using Toplanti.Models;
using Toplanti.Services.Interfaces;
using Toplanti.ViewModels.Commands;

namespace Toplanti.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IAgendaItemRepository _agendaItemRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IDecisionRepository _decisionRepository;
    private readonly INotificationService _notificationService;
    private readonly ISiteService _siteService;
    private readonly IMeetingService _meetingService;
    private readonly IUnitService _unitService;
    private readonly IDecisionService _decisionService;
    private readonly IMeetingValidationService _validationService;
    private readonly IQuorumService _quorumService;
    private readonly IVotingService _votingService;
    private readonly IProxyService _proxyService;

    // Properties
    private ObservableCollection<object> _sites = new();
    private Site? _selectedSite;
    private Meeting? _currentMeeting;
    private bool _isDashboardVisible = true;
    private bool _isDetailsVisible = false;
    private bool _isNoSiteVisible = true;

    // Dashboard Properties
    private string _totalUnits = "0";
    private string _totalMeetings = "0";
    private string _totalDecisions = "0";
    private string _totalLandShare = "0.00";
    private ObservableCollection<Meeting> _recentMeetings = new();
    private ObservableCollection<object> _recentDecisions = new();

    // Units Tab
    private ObservableCollection<Unit> _units = new();

    // Meetings Tab
    private ObservableCollection<Meeting> _meetings = new();
    private ObservableCollection<Meeting> _meetingSelectionList = new();
    private Meeting? _selectedMeeting;
    private string _meetingStatus = "(Toplanti seciniz)";
    private string _meetingInfo = "Lütfen bir toplantı seçin.";
    private string _quorumInfo = "";
    private ObservableCollection<AgendaItem> _agendaItems = new();
    private ObservableCollection<Document> _documents = new();
    private ObservableCollection<Decision> _votingItems = new();
    private string _agendaTitle = "";
    private string _documentTitle = "";
    private string _documentType = "Genel Kurul Icraat Raporu";
    private string _votingTitle = "";
    private string _votingDescription = "";

    // Decisions Tab
    private ObservableCollection<object> _decisions = new();
    private ObservableCollection<Meeting> _votingMeetingList = new();
    private Meeting? _selectedVotingMeeting;
    private string _votingMeetingStatus = "(Toplanti seciniz)";
    private string _decisionTitle = "";
    private string _decisionDescription = "";
    
    // Selection properties for DataGrids
    private Meeting? _selectedRecentMeeting;
    private object? _selectedRecentDecision;
    private Unit? _selectedUnit;
    private AgendaItem? _selectedAgendaItem;
    private Document? _selectedDocument;
    private Decision? _selectedVotingItem;
    private Decision? _selectedDecision;
    
    // Site search text
    private string _siteSearchText = "";
    private string _meetingSearchText = "";
    private string _votingMeetingSearchText = "";

    public MainWindowViewModel(
        ToplantiDbContext context,
        IAgendaItemRepository agendaItemRepository,
        IDocumentRepository documentRepository,
        IDecisionRepository decisionRepository,
        INotificationService notificationService,
        ISiteService siteService,
        IMeetingService meetingService,
        IUnitService unitService,
        IDecisionService decisionService,
        IMeetingValidationService validationService,
        IQuorumService quorumService,
        IVotingService votingService,
        IProxyService proxyService)
    {
        _agendaItemRepository = agendaItemRepository ?? throw new ArgumentNullException(nameof(agendaItemRepository));
        _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
        _decisionRepository = decisionRepository ?? throw new ArgumentNullException(nameof(decisionRepository));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
        _meetingService = meetingService ?? throw new ArgumentNullException(nameof(meetingService));
        _unitService = unitService ?? throw new ArgumentNullException(nameof(unitService));
        _decisionService = decisionService ?? throw new ArgumentNullException(nameof(decisionService));
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        _quorumService = quorumService ?? throw new ArgumentNullException(nameof(quorumService));
        _votingService = votingService ?? throw new ArgumentNullException(nameof(votingService));
        _proxyService = proxyService ?? throw new ArgumentNullException(nameof(proxyService));

        InitializeCommands();
        LoadSitesAsync();
    }

    #region Properties

    public ObservableCollection<object> Sites
    {
        get => _sites;
        set => SetProperty(ref _sites, value);
    }

    public object? SelectedSite
    {
        get => _selectedSite;
        set
        {
            // Ignore if "Seciniz" string is selected or null
            if (value == null || value is string)
            {
                if (_selectedSite != null)
                {
                    _selectedSite = null;
                    OnPropertyChanged(nameof(SelectedSite));
                    OnSiteSelectionChanged();
                }
                return;
            }

            if (value is Site site && SetProperty(ref _selectedSite, site))
            {
                OnSiteSelectionChanged();
            }
        }
    }

    public Meeting? CurrentMeeting
    {
        get => _currentMeeting;
        set
        {
            if (SetProperty(ref _currentMeeting, value))
            {
                OnCurrentMeetingChanged();
            }
        }
    }

    public bool IsDashboardVisible
    {
        get => _isDashboardVisible;
        set => SetProperty(ref _isDashboardVisible, value);
    }

    public bool IsDetailsVisible
    {
        get => _isDetailsVisible;
        set => SetProperty(ref _isDetailsVisible, value);
    }

    public bool IsNoSiteVisible
    {
        get => _isNoSiteVisible;
        set => SetProperty(ref _isNoSiteVisible, value);
    }

    // Dashboard Properties
    public string TotalUnits
    {
        get => _totalUnits;
        set => SetProperty(ref _totalUnits, value);
    }

    public string TotalMeetings
    {
        get => _totalMeetings;
        set => SetProperty(ref _totalMeetings, value);
    }

    public string TotalDecisions
    {
        get => _totalDecisions;
        set => SetProperty(ref _totalDecisions, value);
    }

    public string TotalLandShare
    {
        get => _totalLandShare;
        set => SetProperty(ref _totalLandShare, value);
    }

    public ObservableCollection<Meeting> RecentMeetings
    {
        get => _recentMeetings;
        set => SetProperty(ref _recentMeetings, value);
    }

    public ObservableCollection<object> RecentDecisions
    {
        get => _recentDecisions;
        set => SetProperty(ref _recentDecisions, value);
    }

    // Units Tab
    public ObservableCollection<Unit> Units
    {
        get => _units;
        set => SetProperty(ref _units, value);
    }

    // Meetings Tab
    public ObservableCollection<Meeting> Meetings
    {
        get => _meetings;
        set => SetProperty(ref _meetings, value);
    }

    public ObservableCollection<Meeting> MeetingSelectionList
    {
        get => _meetingSelectionList;
        set => SetProperty(ref _meetingSelectionList, value);
    }

    public Meeting? SelectedMeeting
    {
        get => _selectedMeeting;
        set
        {
            // Ignore if null or not a Meeting
            if (value == null || value is not Meeting meeting)
            {
                if (_selectedMeeting != null)
                {
                    _selectedMeeting = null;
                    CurrentMeeting = null;
                }
                return;
            }

            if (SetProperty(ref _selectedMeeting, meeting))
            {
                CurrentMeeting = meeting;
            }
        }
    }

    public string MeetingStatus
    {
        get => _meetingStatus;
        set => SetProperty(ref _meetingStatus, value);
    }

    public string MeetingInfo
    {
        get => _meetingInfo;
        set => SetProperty(ref _meetingInfo, value);
    }

    public string QuorumInfo
    {
        get => _quorumInfo;
        set => SetProperty(ref _quorumInfo, value);
    }

    public ObservableCollection<AgendaItem> AgendaItems
    {
        get => _agendaItems;
        set => SetProperty(ref _agendaItems, value);
    }

    public ObservableCollection<Document> Documents
    {
        get => _documents;
        set => SetProperty(ref _documents, value);
    }

    public ObservableCollection<Decision> VotingItems
    {
        get => _votingItems;
        set => SetProperty(ref _votingItems, value);
    }

    public string AgendaTitle
    {
        get => _agendaTitle;
        set => SetProperty(ref _agendaTitle, value);
    }

    public string DocumentTitle
    {
        get => _documentTitle;
        set => SetProperty(ref _documentTitle, value);
    }

    public string DocumentType
    {
        get => _documentType;
        set => SetProperty(ref _documentType, value);
    }

    public string VotingTitle
    {
        get => _votingTitle;
        set => SetProperty(ref _votingTitle, value);
    }

    public string VotingDescription
    {
        get => _votingDescription;
        set => SetProperty(ref _votingDescription, value);
    }

    // Decisions Tab
    public ObservableCollection<object> Decisions
    {
        get => _decisions;
        set => SetProperty(ref _decisions, value);
    }

    public ObservableCollection<Meeting> VotingMeetingList
    {
        get => _votingMeetingList;
        set => SetProperty(ref _votingMeetingList, value);
    }

    public Meeting? SelectedVotingMeeting
    {
        get => _selectedVotingMeeting;
        set
        {
            // Ignore if null or not a Meeting
            if (value == null || value is not Meeting meeting)
            {
                if (_selectedVotingMeeting != null)
                {
                    _selectedVotingMeeting = null;
                    CurrentMeeting = null;
                }
                return;
            }

            if (SetProperty(ref _selectedVotingMeeting, meeting))
            {
                CurrentMeeting = meeting;
                LoadDecisionsAsync();
            }
        }
    }

    public string VotingMeetingStatus
    {
        get => _votingMeetingStatus;
        set => SetProperty(ref _votingMeetingStatus, value);
    }

    public string DecisionTitle
    {
        get => _decisionTitle;
        set => SetProperty(ref _decisionTitle, value);
    }

    public string DecisionDescription
    {
        get => _decisionDescription;
        set => SetProperty(ref _decisionDescription, value);
    }

    // Selection Properties
    public Meeting? SelectedRecentMeeting
    {
        get => _selectedRecentMeeting;
        set
        {
            if (SetProperty(ref _selectedRecentMeeting, value) && value != null)
            {
                ShowMeetings();
            }
        }
    }

    public object? SelectedRecentDecision
    {
        get => _selectedRecentDecision;
        set
        {
            if (SetProperty(ref _selectedRecentDecision, value) && value != null)
            {
                ShowDecisions();
            }
        }
    }

    public Unit? SelectedUnit
    {
        get => _selectedUnit;
        set => SetProperty(ref _selectedUnit, value);
    }

    public AgendaItem? SelectedAgendaItem
    {
        get => _selectedAgendaItem;
        set => SetProperty(ref _selectedAgendaItem, value);
    }

    public Document? SelectedDocument
    {
        get => _selectedDocument;
        set => SetProperty(ref _selectedDocument, value);
    }

    public Decision? SelectedVotingItem
    {
        get => _selectedVotingItem;
        set => SetProperty(ref _selectedVotingItem, value);
    }

    public Decision? SelectedDecision
    {
        get => _selectedDecision;
        set => SetProperty(ref _selectedDecision, value);
    }

    public string SiteSearchText
    {
        get => _siteSearchText;
        set
        {
            if (SetProperty(ref _siteSearchText, value))
            {
                FilterSites();
            }
        }
    }

    public string MeetingSearchText
    {
        get => _meetingSearchText;
        set
        {
            if (SetProperty(ref _meetingSearchText, value))
            {
                FilterMeetings();
            }
        }
    }

    public string VotingMeetingSearchText
    {
        get => _votingMeetingSearchText;
        set
        {
            if (SetProperty(ref _votingMeetingSearchText, value))
            {
                FilterVotingMeetings();
            }
        }
    }

    #endregion

    #region Commands

    public ICommand CreateMeetingCommand { get; private set; } = null!;
    public ICommand AddAttendanceCommand { get; private set; } = null!;
    public ICommand AddProxyCommand { get; private set; } = null!;
    public ICommand CheckQuorumCommand { get; private set; } = null!;
    public ICommand CompleteMeetingCommand { get; private set; } = null!;
    public ICommand GenerateMinutesCommand { get; private set; } = null!;
    public ICommand CreateDecisionCommand { get; private set; } = null!;
    public ICommand AddAgendaCommand { get; private set; } = null!;
    public ICommand AddDocumentCommand { get; private set; } = null!;
    public ICommand AddVotingCommand { get; private set; } = null!;
    public ICommand BackToDashboardCommand { get; private set; } = null!;
    public ICommand ShowUnitsCommand { get; private set; } = null!;
    public ICommand ShowMeetingsCommand { get; private set; } = null!;
    public ICommand ShowDecisionsCommand { get; private set; } = null!;
    public ICommand BlockManagementCommand { get; private set; } = null!;
    public ICommand DeleteUnitCommand { get; private set; } = null!;
    public ICommand VoteCommand { get; private set; } = null!;
    public ICommand DecisionDetailCommand { get; private set; } = null!;
    public ICommand AboutCommand { get; private set; } = null!;
    public ICommand MoveAgendaUpCommand { get; private set; } = null!;
    public ICommand MoveAgendaDownCommand { get; private set; } = null!;
    public ICommand DeleteAgendaCommand { get; private set; } = null!;
    public ICommand DeleteDocumentCommand { get; private set; } = null!;
    public ICommand DeleteVotingCommand { get; private set; } = null!;
    public ICommand HiddenDeleteCommand { get; private set; } = null!;

    private void InitializeCommands()
    {
        CreateMeetingCommand = new RelayCommand(async _ => await CreateMeetingAsync());
        AddAttendanceCommand = new RelayCommand(async _ => await AddAttendanceAsync());
        AddProxyCommand = new RelayCommand(async _ => await AddProxyAsync());
        CheckQuorumCommand = new RelayCommand(async _ => await CheckQuorumAsync());
        CompleteMeetingCommand = new RelayCommand(async _ => await CompleteMeetingAsync());
        GenerateMinutesCommand = new RelayCommand(async _ => await GenerateMinutesAsync());
        CreateDecisionCommand = new RelayCommand(async _ => await CreateDecisionAsync());
        AddAgendaCommand = new RelayCommand(async _ => await AddAgendaAsync());
        AddDocumentCommand = new RelayCommand(async _ => await AddDocumentAsync());
        AddVotingCommand = new RelayCommand(async _ => await AddVotingAsync());
        BackToDashboardCommand = new RelayCommand(_ => ShowDashboard());
        ShowUnitsCommand = new RelayCommand(_ => ShowUnits());
        ShowMeetingsCommand = new RelayCommand(_ => ShowMeetings());
        ShowDecisionsCommand = new RelayCommand(_ => ShowDecisions());
        BlockManagementCommand = new RelayCommand(_ => OpenBlockManagement());
        DeleteUnitCommand = new RelayCommand<int>(async id => await DeleteUnitAsync(id));
        VoteCommand = new RelayCommand<int>(async id => await VoteAsync(id));
        DecisionDetailCommand = new RelayCommand<int>(id => ShowDecisionDetail(id));
        AboutCommand = new RelayCommand(_ => ShowAbout());
        MoveAgendaUpCommand = new RelayCommand<AgendaItem>(async item => await MoveAgendaUpAsync(item));
        MoveAgendaDownCommand = new RelayCommand<AgendaItem>(async item => await MoveAgendaDownAsync(item));
        DeleteAgendaCommand = new RelayCommand<AgendaItem>(async item => await DeleteAgendaAsync(item));
        DeleteDocumentCommand = new RelayCommand<Document>(async doc => await DeleteDocumentAsync(doc));
        DeleteVotingCommand = new RelayCommand<Decision>(async decision => await DeleteVotingAsync(decision));
        HiddenDeleteCommand = new RelayCommand(_ => HandleHiddenDelete());
    }

    #endregion

    #region Private Methods

    private async void LoadSitesAsync()
    {
        try
        {
            var sites = await _siteService.GetAllSitesAsync();
            var siteList = new List<object> { "Seciniz" };
            siteList.AddRange(sites);
            Sites = new ObservableCollection<object>(siteList);
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Site yukleme hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private async void OnSiteSelectionChanged()
    {
        if (SelectedSite == null)
        {
            IsNoSiteVisible = true;
            IsDashboardVisible = false;
            IsDetailsVisible = false;
            return;
        }

        IsNoSiteVisible = false;
        IsDashboardVisible = true;
        IsDetailsVisible = false;

        await LoadDashboardAsync();
    }

    private async Task LoadDashboardAsync()
    {
        if (SelectedSite == null) return;

        try
        {
            var stats = await _meetingService.GetDashboardStatsAsync(SelectedSite.Id);
            TotalUnits = stats.TotalUnits.ToString();
            TotalMeetings = stats.TotalMeetings.ToString();
            TotalDecisions = stats.TotalDecisions.ToString();
            TotalLandShare = stats.TotalLandShare.ToString("F2");

            var meetings = await _meetingService.GetMeetingsBySiteIdAsync(SelectedSite.Id);
            RecentMeetings = new ObservableCollection<Meeting>(meetings.Take(5));

            // Recent decisions loading logic would go here
            // This is simplified for now
        }
        catch (Exception ex)
        {
            _notificationService.ShowError("Dashboard yukleme hatasi", "Hata", ex);
        }
    }

    private async void OnCurrentMeetingChanged()
    {
        if (CurrentMeeting == null)
        {
            MeetingInfo = "Lütfen bir toplantı seçin.";
            QuorumInfo = "";
            return;
        }

        await LoadMeetingDetailsAsync();
    }

    private async Task LoadMeetingDetailsAsync()
    {
        if (CurrentMeeting == null) return;

        try
        {
            var meeting = await _meetingService.GetMeetingByIdAsync(CurrentMeeting.Id);
            if (meeting == null) return;

            CurrentMeeting = meeting;

            MeetingStatus = meeting.IsCompleted ? "(Tamamlanmis)" : "";
            UpdateMeetingInfo();
            await LoadAgendaItemsAsync();
            await LoadDocumentsAsync();
            await LoadVotingItemsAsync();
            await LoadDecisionsAsync();
        }
        catch (Exception ex)
        {
            _notificationService.ShowError("Toplanti detay yukleme hatasi", "Hata", ex);
        }
    }

    private void UpdateMeetingInfo()
    {
        if (CurrentMeeting == null) return;

        var attendedUnits = CurrentMeeting.Attendances
            .Where(a => a.Unit != null)
            .Select(a => a.Unit!)
            .ToList();

        var attendedLandShare = attendedUnits.Sum(u => u.LandShare);
        var attendedCount = attendedUnits.Count;

        MeetingInfo = $"Toplantı: {CurrentMeeting.Title}\n" +
                     $"Tarih: {CurrentMeeting.MeetingDate:dd.MM.yyyy HH:mm}\n" +
                     $"Toplam Birim: {CurrentMeeting.TotalUnitCount}\n" +
                     $"Katılan Birim: {attendedCount}\n" +
                     $"Toplam Arsa Payı: {CurrentMeeting.TotalSiteLandShare:F2}\n" +
                     $"Katılan Arsa Payı: {attendedLandShare:F2}";

        var (achieved, message) = _quorumService.CheckQuorum(
            CurrentMeeting.TotalUnitCount,
            attendedCount,
            CurrentMeeting.TotalSiteLandShare,
            attendedLandShare);

        QuorumInfo = message;
    }

    private async Task LoadAgendaItemsAsync()
    {
        if (CurrentMeeting == null) return;

        try
        {
            var items = await _agendaItemRepository.GetAgendaItemsByMeetingIdAsync(CurrentMeeting.Id);
            AgendaItems = new ObservableCollection<AgendaItem>(items);
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Gundem yukleme hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private async Task LoadDocumentsAsync()
    {
        if (CurrentMeeting == null) return;

        try
        {
            var documents = await _documentRepository.GetDocumentsByMeetingIdAsync(CurrentMeeting.Id);
            Documents = new ObservableCollection<Document>(documents);
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Belge yukleme hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private async Task LoadVotingItemsAsync()
    {
        if (CurrentMeeting == null) return;

        try
        {
            var decisions = await _decisionRepository.GetDecisionsByMeetingIdAsync(CurrentMeeting.Id);
            VotingItems = new ObservableCollection<Decision>(decisions);
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Oylama yukleme hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private async Task LoadDecisionsAsync()
    {
        if (CurrentMeeting == null) return;

        try
        {
            var decisions = await _decisionService.GetDecisionsByMeetingIdAsync(CurrentMeeting.Id);
            Decisions = new ObservableCollection<object>(decisions);
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Karar yukleme hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private async Task LoadMeetingsAsync()
    {
        if (SelectedSite == null) return;

        try
        {
            var meetings = await _meetingService.GetMeetingsBySiteIdAsync(SelectedSite.Id);
            Meetings = new ObservableCollection<Meeting>(meetings);
            MeetingSelectionList = new ObservableCollection<Meeting>(meetings);
            VotingMeetingList = new ObservableCollection<Meeting>(meetings);
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Toplanti yukleme hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private async Task LoadUnitsAsync()
    {
        if (SelectedSite == null) return;

        try
        {
            var units = await _unitService.GetUnitsBySiteIdAsync(SelectedSite.Id);
            Units = new ObservableCollection<Unit>(units);
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Birim yukleme hatasi: {ex.Message}", "Hata", ex);
        }
    }

    #endregion

    #region Command Implementations

    private async Task CreateMeetingAsync()
    {
        if (SelectedSite == null)
        {
            _notificationService.ShowWarning("Lutfen once bir site secin.");
            return;
        }

        try
        {
            var dialog = new CreateMeetingDialog(_meetingService, SelectedSite);
            if (dialog.ShowDialog() == true && dialog.CreatedMeeting != null)
            {
                await LoadMeetingsAsync();
                CurrentMeeting = dialog.CreatedMeeting;
                SelectedMeeting = CurrentMeeting;
            }
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Toplanti olusturma hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private async Task AddAttendanceAsync()
    {
        // Implementation will be added
        await Task.CompletedTask;
    }

    private async Task AddProxyAsync()
    {
        // Implementation will be added
        await Task.CompletedTask;
    }

    private async Task CheckQuorumAsync()
    {
        if (CurrentMeeting == null)
        {
            _notificationService.ShowWarning("Lütfen önce bir toplantı seçin.");
            return;
        }

        try
        {
            var result = await _meetingService.CheckQuorumAsync(CurrentMeeting.Id);
            UpdateMeetingInfo();

            if (result.Achieved)
                _notificationService.ShowInfo(result.Message, "Yeter Sayı Sağlandı");
            else
                _notificationService.ShowWarning(result.Message, "Yeter Sayı Sağlanamadı");
        }
        catch (Exception ex)
        {
            _notificationService.ShowError("Yeter sayi kontrolu hatasi", "Hata", ex);
        }
    }

    private async Task CompleteMeetingAsync()
    {
        if (CurrentMeeting == null)
        {
            _notificationService.ShowWarning("Lutfen once bir toplanti secin.");
            return;
        }

        var validation = _validationService.ValidateMeetingHasDecisions(CurrentMeeting);
        if (!validation.IsValid)
        {
            _notificationService.ShowWarning(validation.ErrorMessage!, "Karar Gerekli");
            return;
        }

        var result = _notificationService.ShowConfirmation(
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
                await _meetingService.CompleteMeetingAsync(CurrentMeeting.Id);
                await LoadMeetingsAsync();
                UpdateMeetingInfo();
                _notificationService.ShowSuccess("Toplanti basariyla tamamlandi.");
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Toplanti tamamlama hatasi: {ex.Message}", "Hata", ex);
            }
        }
    }

    private async Task GenerateMinutesAsync()
    {
        if (CurrentMeeting == null)
        {
            _notificationService.ShowWarning("Lütfen önce bir toplantı seçin.");
            return;
        }

        try
        {
            var minutes = await _meetingService.GenerateMeetingMinutesAsync(CurrentMeeting.Id);
            var window = new MinutesWindow(minutes);
            window.ShowDialog();
        }
        catch (Exception ex)
        {
            _notificationService.ShowError("Tutanak olusturma hatasi", "Hata", ex);
        }
    }

    private async Task CreateDecisionAsync()
    {
        if (CurrentMeeting == null)
        {
            _notificationService.ShowWarning("Lutfen once bir toplanti secin.");
            return;
        }

        var validation = _validationService.ValidateMeetingAndContext(CurrentMeeting);
        if (!validation.IsValid)
        {
            _notificationService.ShowWarning(validation.ErrorMessage!);
            return;
        }

        var completedValidation = _validationService.ValidateMeetingNotCompleted(CurrentMeeting, "Yeni karar eklenemez");
        if (!completedValidation.IsValid)
        {
            _notificationService.ShowWarning(completedValidation.ErrorMessage!);
            return;
        }

        if (string.IsNullOrWhiteSpace(DecisionTitle) || string.IsNullOrWhiteSpace(DecisionDescription))
        {
            _notificationService.ShowWarning("Lutfen tum alanlari doldurun.");
            return;
        }

        try
        {
            await _decisionService.CreateDecisionAsync(CurrentMeeting.Id, DecisionTitle, DecisionDescription);
            DecisionTitle = "";
            DecisionDescription = "";
            await LoadDecisionsAsync();
            _notificationService.ShowSuccess("Karar başarıyla oluşturuldu.", "Başarılı");
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Karar olusturma hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private async Task AddAgendaAsync()
    {
        if (CurrentMeeting == null || string.IsNullOrWhiteSpace(AgendaTitle))
        {
            _notificationService.ShowWarning("Lutfen gundem basligini girin.");
            return;
        }

        try
        {
            var maxOrder = AgendaItems.Any() ? AgendaItems.Max(a => a.Order) : 0;
            var agendaItem = new AgendaItem
            {
                MeetingId = CurrentMeeting.Id,
                Title = AgendaTitle.Trim(),
                Description = null,
                Order = maxOrder + 1
            };

            await _agendaItemRepository.AddAsync(agendaItem);
            await _context.SaveChangesAsync();

            AgendaTitle = "";
            await LoadAgendaItemsAsync();
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Gundem ekleme hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private async Task AddDocumentAsync()
    {
        if (CurrentMeeting == null || string.IsNullOrWhiteSpace(DocumentTitle))
        {
            _notificationService.ShowWarning("Lutfen belge basligini girin.");
            return;
        }

        try
        {
            var document = new Document
            {
                MeetingId = CurrentMeeting.Id,
                Title = DocumentTitle.Trim(),
                DocumentType = DocumentType ?? "Genel Kurul Icraat Raporu",
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Documents.AddAsync(document);
            await _unitOfWork.SaveChangesAsync();

            DocumentTitle = "";
            await LoadDocumentsAsync();
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Belge ekleme hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private async Task AddVotingAsync()
    {
        if (CurrentMeeting == null || string.IsNullOrWhiteSpace(VotingTitle))
        {
            _notificationService.ShowWarning("Lutfen oylama basligini girin.");
            return;
        }

        try
        {
            await _decisionService.CreateDecisionAsync(CurrentMeeting.Id, VotingTitle, VotingDescription ?? "");
            VotingTitle = "";
            VotingDescription = "";
            await LoadVotingItemsAsync();
            await LoadDecisionsAsync();
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Oylama ekleme hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private void ShowDashboard()
    {
        IsDashboardVisible = true;
        IsDetailsVisible = false;
        LoadDashboardAsync();
    }

    private void ShowUnits()
    {
        IsDashboardVisible = false;
        IsDetailsVisible = true;
        LoadUnitsAsync();
    }

    private void ShowMeetings()
    {
        IsDashboardVisible = false;
        IsDetailsVisible = true;
        LoadMeetingsAsync();
    }

    private void ShowDecisions()
    {
        IsDashboardVisible = false;
        IsDetailsVisible = true;
        LoadDecisionsAsync();
    }

    private void OpenBlockManagement()
    {
        if (SelectedSite == null)
        {
            _notificationService.ShowWarning("Lutfen once bir site secin.");
            return;
        }

        try
        {
            var window = new BlockUnitManagementWindow(SelectedSite);
            window.ShowDialog();
            LoadDashboardAsync();
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Blok ve daire yonetimi acilirken hata: {ex.Message}", "Hata", ex);
        }
    }

    private async Task DeleteUnitAsync(int unitId)
    {
        var result = _notificationService.ShowConfirmation("Bu birimi silmek istediğinizden emin misiniz?", "Onay");
        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _unitService.DeleteUnitAsync(unitId);
                await LoadUnitsAsync();
            }
            catch (Exception ex)
            {
                _notificationService.ShowError($"Birim silme hatasi: {ex.Message}", "Hata", ex);
            }
        }
    }

    private async Task VoteAsync(int decisionId)
    {
        // Implementation will be added
        await Task.CompletedTask;
    }

    private void ShowDecisionDetail(int decisionId)
    {
        try
        {
            var decision = await _decisionRepository.GetByIdAsync(decisionId);
            
            if (decision != null)
            {
                // Votes'ları yükle
                var decisionsWithVotes = await _decisionRepository.GetDecisionsWithVotesByMeetingIdAsync(decision.MeetingId);
                var decisionWithVotes = decisionsWithVotes.FirstOrDefault(d => d.Id == decisionId);
                
                if (decisionWithVotes != null)
                {
                    var window = new DecisionDetailWindow(decisionWithVotes);
                    window.ShowDialog();
                }
            }
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Karar detay gosterim hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private void ShowAbout()
    {
        var aboutDialog = new AboutDialog();
        aboutDialog.ShowDialog();
    }

    private async Task MoveAgendaUpAsync(AgendaItem? item)
    {
        if (item == null || CurrentMeeting == null) return;

        try
        {
            var currentOrder = item.Order;
            if (currentOrder <= 1) return;

            var previousItem = AgendaItems.FirstOrDefault(a => a.Order == currentOrder - 1);
            if (previousItem == null) return;

            item.Order = currentOrder - 1;
            previousItem.Order = currentOrder;

            _agendaItemRepository.Update(item);
            _agendaItemRepository.Update(previousItem);
            await _context.SaveChangesAsync();
            await LoadAgendaItemsAsync();
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Gundem tasima hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private async Task MoveAgendaDownAsync(AgendaItem? item)
    {
        if (item == null || CurrentMeeting == null) return;

        try
        {
            var currentOrder = item.Order;
            var maxOrder = AgendaItems.Max(a => a.Order);
            if (currentOrder >= maxOrder) return;

            var nextItem = AgendaItems.FirstOrDefault(a => a.Order == currentOrder + 1);
            if (nextItem == null) return;

            item.Order = currentOrder + 1;
            nextItem.Order = currentOrder;

            _unitOfWork.AgendaItems.Update(item);
            _unitOfWork.AgendaItems.Update(nextItem);
            await _unitOfWork.SaveChangesAsync();
            await LoadAgendaItemsAsync();
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Gundem tasima hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private async Task DeleteAgendaAsync(AgendaItem? item)
    {
        if (item == null || CurrentMeeting == null) return;

        var result = _notificationService.ShowConfirmation("Bu gundem maddesini silmek istediğinizden emin misiniz?", "Onay");
        if (result != MessageBoxResult.Yes) return;

        try
        {
            _unitOfWork.AgendaItems.Remove(item);
            await _unitOfWork.SaveChangesAsync();
            await LoadAgendaItemsAsync();
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Gundem silme hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private async Task DeleteDocumentAsync(Document? doc)
    {
        if (doc == null || CurrentMeeting == null) return;

        var result = _notificationService.ShowConfirmation("Bu belgeyi silmek istediğinizden emin misiniz?", "Onay");
        if (result != MessageBoxResult.Yes) return;

        try
        {
            _unitOfWork.Documents.Remove(doc);
            await _unitOfWork.SaveChangesAsync();
            await LoadDocumentsAsync();
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Belge silme hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private async Task DeleteVotingAsync(Decision? decision)
    {
        if (decision == null || CurrentMeeting == null) return;

        var result = _notificationService.ShowConfirmation("Bu oylama maddesini silmek istediğinizden emin misiniz?", "Onay");
        if (result != MessageBoxResult.Yes) return;

        try
        {
            _unitOfWork.Decisions.Remove(decision);
            await _unitOfWork.SaveChangesAsync();
            await LoadVotingItemsAsync();
            await LoadDecisionsAsync();
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Oylama silme hatasi: {ex.Message}", "Hata", ex);
        }
    }

    private void HandleHiddenDelete()
    {
        // Hidden delete functionality - can be implemented if needed
        // This is a special button for administrative purposes
    }

    private void FilterSites()
    {
        // Site filtering logic can be implemented here if needed
        // For now, we'll rely on ComboBox's built-in filtering
    }

    private void FilterMeetings()
    {
        // Meeting filtering logic can be implemented here if needed
        // For now, we'll rely on ComboBox's built-in filtering
    }

    private void FilterVotingMeetings()
    {
        // Voting meeting filtering logic can be implemented here if needed
        // For now, we'll rely on ComboBox's built-in filtering
    }

    #endregion
}

