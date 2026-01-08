using System.Windows;
using Toplanti.Models;

namespace Toplanti.Dialogs;

public partial class DecisionDetailWindow : Window
{
    public DecisionDetailWindow(Decision decision)
    {
        InitializeComponent();
        
        txtTitle.Text = decision.Title;
        txtDescription.Text = decision.Description;
        
        dgVotes.ItemsSource = decision.Votes;
        
        txtResults.Text = $"Evet: {decision.YesVotes} birim ({decision.YesLandShare:F2} arsa payı)\n" +
                         $"Hayır: {decision.NoVotes} birim ({decision.NoLandShare:F2} arsa payı)\n" +
                         $"Çekimser: {decision.AbstainVotes} birim ({decision.AbstainLandShare:F2} arsa payı)";
        
        txtStatus.Text = $"Karar Durumu: {(decision.IsApproved ? "KABUL EDİLDİ" : "REDDEDİLDİ")}";
        txtStatus.Foreground = decision.IsApproved ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
    }
}

