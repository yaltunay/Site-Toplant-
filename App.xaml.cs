using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Toplanti.Data;

namespace Toplanti;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Her çalıştırmada hata dosyasını sil
        ClearErrorFile();
        
        // Global exception handlers
        this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        
        try
        {
            // Initialize database
            var optionsBuilder = new DbContextOptionsBuilder<ToplantiDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ToplantiDb;Trusted_Connection=true;MultipleActiveResultSets=true");
            
            using var context = new ToplantiDbContext(optionsBuilder.Options);
            
            // Ensure database exists, then apply migrations
            if (context.Database.CanConnect())
            {
                // Database exists, apply migrations
                context.Database.Migrate();
            }
            else
            {
                // Database doesn't exist, create it and apply migrations
                context.Database.Migrate();
            }
            
            // Seed initial data if needed - UnitTypes must be created first
            if (!context.UnitTypes.Any())
            {
                var unitTypes = new List<Models.UnitType>
                {
                    new Models.UnitType { Name = "Villa", LandShareMultiplier = 1.5m, Description = "Villa tipi birim" },
                    new Models.UnitType { Name = "Daire", LandShareMultiplier = 1.0m, Description = "Daire tipi birim" },
                    new Models.UnitType { Name = "Dükkan", LandShareMultiplier = 1.2m, Description = "Dükkan tipi birim" }
                };
                
                context.UnitTypes.AddRange(unitTypes);
                context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            WriteErrorToFile(ex);
        }
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
            var errorFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hata.txt");
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
            var errorFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hata.txt");
            
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

