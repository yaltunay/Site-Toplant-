using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Toplanti.Data;
using Toplanti.Services;
using Toplanti.Services.Interfaces;
using Toplanti.ViewModels;

namespace Toplanti.Infrastructure;

/// <summary>
/// Dependency Injection Container yapılandırması
/// Tüm servis kayıtlarını burada yönetir
/// </summary>
public static class DependencyInjectionContainer
{
    /// <summary>
    /// Tüm servisleri kaydeder
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration instance</param>
    /// <returns>Yapılandırılmış service collection</returns>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configuration servisini kaydet
        services.AddSingleton<ApplicationConfiguration>(sp =>
            new ApplicationConfiguration(configuration));

        // Connection string'i al
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' bulunamadı.");
    {
        // Database Context
        services.AddDbContext<ToplantiDbContext>(options =>
            options.UseSqlServer(connectionString),
            ServiceLifetime.Scoped);

        // Core Services
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IQuorumService, QuorumService>();
        services.AddScoped<IVotingService, VotingService>();
        services.AddScoped<IProxyService, ProxyService>();
        services.AddScoped<ISiteService, SiteService>();
        services.AddScoped<IUnitService, UnitService>();
        services.AddScoped<IDecisionService, DecisionService>();

        // MeetingService - diğer servislere bağımlı
        services.AddScoped<IMeetingService>(sp =>
        {
            var context = sp.GetRequiredService<ToplantiDbContext>();
            var quorumService = sp.GetRequiredService<IQuorumService>();
            var proxyService = sp.GetRequiredService<IProxyService>();
            var votingService = sp.GetRequiredService<IVotingService>();
            return new MeetingService(context, quorumService, proxyService, votingService);
        });

        // ValidationService - DbContext'e bağımlı
        services.AddScoped<IMeetingValidationService>(sp =>
        {
            var context = sp.GetRequiredService<ToplantiDbContext>();
            return new MeetingValidationService(context);
        });

        // ViewModels - Transient olarak kaydet (her kullanımda yeni instance)
        services.AddTransient<MainWindowViewModel>();

        // Views - Transient olarak kaydet
        services.AddTransient<MainWindow>(sp =>
        {
            var viewModel = sp.GetRequiredService<MainWindowViewModel>();
            return new MainWindow(viewModel);
        });

        return services;
    }
}

