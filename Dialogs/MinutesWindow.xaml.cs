using System.Windows;
using System.Windows.Input;

namespace Toplanti.Dialogs;

public partial class MinutesWindow : Window
{
    public MinutesWindow(string minutes)
    {
        InitializeComponent();
        txtMinutes.Text = minutes;
    }

    private void BtnCopy_Click(object sender, RoutedEventArgs e)
    {
        Clipboard.SetText(txtMinutes.Text);
        MessageBox.Show("Tutanak panoya kopyalandı.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}

