using System.Windows;
using Toplanti.Models;
using Toplanti.Services.Interfaces;

namespace Toplanti.Dialogs;

public partial class CreateMeetingDialog : Window
{
    private readonly IMeetingService _meetingService;
    private readonly Site? _selectedSite;
    public Meeting? CreatedMeeting { get; private set; }

    public CreateMeetingDialog(IMeetingService meetingService, Site selectedSite)
    {
        InitializeComponent();
        _meetingService = meetingService ?? throw new ArgumentNullException(nameof(meetingService));
        _selectedSite = selectedSite;
        
        dpMeetingDate.SelectedDate = DateTime.Now;
        txtMeetingTitle.Focus();
    }

    private async void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedSite == null)
        {
            MessageBox.Show("Site bilgisi bulunamadi.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (string.IsNullOrWhiteSpace(txtMeetingTitle.Text))
        {
            MessageBox.Show("Lutfen toplanti basligini girin.", "Uyari", MessageBoxButton.OK, MessageBoxImage.Warning);
            txtMeetingTitle.Focus();
            return;
        }

        if (dpMeetingDate.SelectedDate == null)
        {
            MessageBox.Show("Lutfen toplanti tarihini secin.", "Uyari", MessageBoxButton.OK, MessageBoxImage.Warning);
            dpMeetingDate.Focus();
            return;
        }

        try
        {
            var meeting = await _meetingService.CreateMeetingAsync(
            {
                Title = txtMeetingTitle.Text.Trim(),
                MeetingDate = dpMeetingDate.SelectedDate.Value,
                TotalSiteLandShare = totalLandShare,
                TotalUnitCount = totalUnits,
                AttendedUnitCount = 0,
                AttendedLandShare = 0,
                QuorumAchieved = false,
                IsCompleted = false
            };

            _context.Meetings.Add(meeting);
            _context.SaveChanges();

            CreatedMeeting = meeting;
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Toplanti olusturma hatasi: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}

