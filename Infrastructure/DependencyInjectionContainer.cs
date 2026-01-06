using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Toplanti.Data;
using Toplanti.Data.Repositories;
using Toplanti.Infrastructure.Validation;
using Toplanti.Models;
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

        // Database Context
        services.AddDbContext<ToplantiDbContext>(options =>
            options.UseSqlServer(connectionString),
            ServiceLifetime.Scoped);

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories (UnitOfWork içinde kullanılmak üzere, ama DI'da da kayıtlı olmalılar)
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IMeetingRepository, MeetingRepository>();
        services.AddScoped<IUnitRepository, UnitRepository>();
        services.AddScoped<ISiteRepository, SiteRepository>();
        services.AddScoped<IDecisionRepository, DecisionRepository>();
        services.AddScoped<IAgendaItemRepository, AgendaItemRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IMeetingAttendanceRepository, MeetingAttendanceRepository>();
        services.AddScoped<IUnitTypeRepository, UnitTypeRepository>();

        // FluentValidation Validators
        services.AddScoped<IValidator<Meeting>, MeetingValidator>();
        services.AddScoped<IValidator<Unit>, UnitValidator>();
        services.AddScoped<IValidator<Decision>, DecisionValidator>();
        services.AddScoped<IValidator<Site>, SiteValidator>();

        // Validation Service
        services.AddScoped<ValidationService>();

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
            var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
            var context = sp.GetRequiredService<ToplantiDbContext>();
            var quorumService = sp.GetRequiredService<IQuorumService>();
            var proxyService = sp.GetRequiredService<IProxyService>();
            var votingService = sp.GetRequiredService<IVotingService>();
            var validationService = sp.GetRequiredService<ValidationService>();
            return new MeetingService(unitOfWork, context, quorumService, proxyService, votingService, validationService);
        });

        // ValidationService - DbContext'e bağımlı
        services.AddScoped<IMeetingValidationService>(sp =>
        {
            var context = sp.GetRequiredService<ToplantiDbContext>();
            return new MeetingValidationService(context);
        });

        // ViewModels - Transient olarak kaydet (her kullanımda yeni instance)
        services.AddTransient<MainWindowViewModel>(sp =>
        {
            var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
            var notificationService = sp.GetRequiredService<INotificationService>();
            var siteService = sp.GetRequiredService<ISiteService>();
            var meetingService = sp.GetRequiredService<IMeetingService>();
            var unitService = sp.GetRequiredService<IUnitService>();
            var decisionService = sp.GetRequiredService<IDecisionService>();
            var validationService = sp.GetRequiredService<IMeetingValidationService>();
            var quorumService = sp.GetRequiredService<IQuorumService>();
            var votingService = sp.GetRequiredService<IVotingService>();
            var proxyService = sp.GetRequiredService<IProxyService>();
            
            return new MainWindowViewModel(
                unitOfWork,
                notificationService,
                siteService,
                meetingService,
                unitService,
                decisionService,
                validationService,
                quorumService,
                votingService,
                proxyService);
        });

        // Views - Transient olarak kaydet
        services.AddTransient<MainWindow>(sp =>
        {
            var viewModel = sp.GetRequiredService<MainWindowViewModel>();
            return new MainWindow(viewModel);
        });

        return services;
    }
}

