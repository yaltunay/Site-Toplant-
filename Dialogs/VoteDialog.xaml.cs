using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Toplanti.Models;

namespace Toplanti.Dialogs;

public partial class VoteDialog : Window
{
    private readonly List<Unit> _units;
    private readonly Decision _decision;
    private readonly Dictionary<DataGridRow, ComboBox> _voteControls = [];

    public List<Vote> Votes { get; private set; } = [];

    public VoteDialog(List<Unit> units, Decision decision)
    {
        InitializeComponent();
        _units = units;
        _decision = decision;
        
        txtDecisionInfo.Text = $"Karar: {decision.Title}\nAçıklama: {decision.Description}";
        dgUnits.ItemsSource = units;
    }

    private void CmbVote_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is ComboBox cmb)
        {
            cmb.ItemsSource = new Dictionary<string, VoteType>
            {
                { "Evet", VoteType.Yes },
                { "Hayır", VoteType.No },
                { "Çekimser", VoteType.Abstain }
            }.ToList();
        }
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
        Votes = [];
        
        foreach (var unit in _units)
        {
            // Find the ComboBox for this unit
            var row = dgUnits.ItemContainerGenerator.ContainerFromItem(unit) as DataGridRow;
            if (row == null) continue;
            
            var cmb = FindVisualChild<ComboBox>(row);
            if (cmb?.SelectedValue is KeyValuePair<string, VoteType> selectedPair)
            {
                Votes.Add(new Vote
                {
                    UnitId = unit.Id,
                    VoteType = selectedPair.Value
                });
            }
        }

        if (Votes.Count == 0)
        {
            MessageBox.Show("Lütfen en az bir birim için oy seçin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        DialogResult = true;
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T result)
                return result;
            
            var childOfChild = FindVisualChild<T>(child);
            if (childOfChild != null)
                return childOfChild;
        }
        return null;
    }
}

