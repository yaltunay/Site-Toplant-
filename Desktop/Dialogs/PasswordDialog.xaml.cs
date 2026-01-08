using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Toplanti.Dialogs;

public partial class PasswordDialog : Window
{
    // MD5 hash of "AnkaraKonakları"
    private const string ExpectedHash = "96b17ca741ef1a13734ceaba0c0e5061";

    public bool IsPasswordCorrect { get; private set; }

    public PasswordDialog()
    {
        InitializeComponent();
        txtPassword.Focus();
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
        CheckPassword();
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            CheckPassword();
        }
    }

    private void CheckPassword()
    {
        var password = txtPassword.Password;
        var hash = ComputeMD5Hash(password);

        if (hash.Equals(ExpectedHash, StringComparison.OrdinalIgnoreCase))
        {
            IsPasswordCorrect = true;
            DialogResult = true;
            Close();
        }
        else
        {
            MessageBox.Show("Hatali sifre!", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            txtPassword.Clear();
            txtPassword.Focus();
        }
    }

    private static string ComputeMD5Hash(string input)
    {
        using var md5 = MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}

