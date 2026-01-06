using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Toplanti.Data;
using Toplanti.Infrastructure;

namespace Toplanti;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Her çalıştırmada hata dosyasını sil
        ClearErrorFile();
        
        // Global exception handlers
        this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        
        try
        {
            // Configure Dependency Injection
            var serviceCollection = new ServiceCollection();
            var connectionString = ApplicationConfiguration.GetConnectionString();
            serviceCollection.AddApplicationServices(connectionString);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            // Initialize database
            using var scope = _serviceProvider.CreateScope();
            var dbInitializer = new DatabaseInitializer(
                scope.ServiceProvider.GetRequiredService<ToplantiDbContext>());
            
            await dbInitializer.InitializeAsync();
            await dbInitializer.SeedAsync();

            // Create and show MainWindow with ViewModel
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            WriteErrorToFile(ex);
        }
    }


    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }

    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        WriteErrorToFile(e.Exception);
        e.Handled = true; // Prevent app from closing
        // Pop-up gösterilmiyor, sadece dosyaya yazılıyor
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            WriteErrorToFile(ex);
        }
    }

    private void ClearErrorFile()
    {
        try
        {
            var errorFilePath = ApplicationConfiguration.GetErrorLogFilePath();
            if (File.Exists(errorFilePath))
            {
                File.Delete(errorFilePath);
            }
        }
        catch
        {
            // If we can't delete file, ignore
        }
    }

    private void WriteErrorToFile(Exception ex)
    {
        try
        {
            var errorFilePath = ApplicationConfiguration.GetErrorLogFilePath();
            
            // Yeni hata metnini oluştur
            var newErrorText = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n" +
                          $"Hata Mesaji: {ex.Message}\n" +
                          $"Hata Tipi: {ex.GetType().FullName}\n" +
                          $"Stack Trace:\n{ex.StackTrace}\n";
            
            if (ex.InnerException != null)
            {
                newErrorText += $"\nIc Hata:\n{ex.InnerException.Message}\n" +
                           $"Ic Hata Stack Trace:\n{ex.InnerException.StackTrace}\n";
            }
            
            newErrorText += $"\n{new string('=', 80)}\n\n";
            
            // Dosya varsa içeriğini oku, yoksa boş string kullan
            string existingContent = "";
            if (File.Exists(errorFilePath))
            {
                existingContent = File.ReadAllText(errorFilePath);
            }
            
            // Yeni hatayı en üste ekle (en son hata en üstte)
            var allContent = newErrorText + existingContent;
            
            File.WriteAllText(errorFilePath, allContent);
        }
        catch
        {
            // If we can't write to file, ignore (don't crash while handling crash)
        }
    }
}

