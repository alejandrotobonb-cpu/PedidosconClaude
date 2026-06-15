using Azure.Communication.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portal.Domain.Interfaces;
using Portal.Infrastructure.Persistence;
using Portal.Infrastructure.SagAdapter;
using Portal.Infrastructure.Services;

namespace Portal.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        // Base de datos: SQLite en Development, SQL Server en producción
        if (environment.IsDevelopment())
        {
            services.AddDbContext<PortalDbContext>(options =>
                options.UseSqlite(
                    configuration.GetConnectionString("PortalDb")
                    ?? "Data Source=portal_dev.db"));
        }
        else
        {
            services.AddDbContext<PortalDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("PortalDb")));
        }

        services.AddScoped<IPortalDbContext>(sp => sp.GetRequiredService<PortalDbContext>());

        // Email: no-op en dev si no hay connection string configurada
        var acsConnectionString = configuration["AzureCommunicationServices:ConnectionString"];
        var acsSender = configuration["AzureCommunicationServices:SenderAddress"];

        if (!string.IsNullOrWhiteSpace(acsConnectionString) &&
            !acsConnectionString.StartsWith("YOUR_"))
        {
            services.AddSingleton<IEmailService>(_ =>
                new EmailService(new EmailClient(acsConnectionString), acsSender!));
        }
        else
        {
            services.AddSingleton<IEmailService, NoOpEmailService>();
        }

        // SAG HTTP client
        var sagBaseUrl = configuration["SagApi:BaseUrl"];
        var sagToken = configuration["SagApi:Token"];
        if (!string.IsNullOrWhiteSpace(sagBaseUrl) && !sagBaseUrl.StartsWith("YOUR_"))
        {
            services.AddHttpClient("SAG", client =>
            {
                client.BaseAddress = new Uri(sagBaseUrl);
                if (!string.IsNullOrWhiteSpace(sagToken))
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sagToken}");
            });
        }
        else
        {
            services.AddHttpClient("SAG");
        }

        // SAG services — siempre registrados para que SagController resuelva ISagSyncService
        services.AddScoped<ISagClient, SagClient>();
        services.AddSingleton<SagSyncService>();
        services.AddSingleton<ISagSyncService>(sp => sp.GetRequiredService<SagSyncService>());

        // Background sync automático: solo con URL real configurada
        if (!string.IsNullOrWhiteSpace(sagBaseUrl) && !sagBaseUrl.StartsWith("YOUR_"))
        {
            services.AddHostedService(sp => sp.GetRequiredService<SagSyncService>());
        }

        return services;
    }
}
