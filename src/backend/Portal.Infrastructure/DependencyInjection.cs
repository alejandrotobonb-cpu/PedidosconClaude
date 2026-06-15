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

        // SAG sync: solo si hay URL configurada
        var sagBaseUrl = configuration["SagApi:BaseUrl"];
        if (!string.IsNullOrWhiteSpace(sagBaseUrl) && !sagBaseUrl.StartsWith("YOUR_"))
        {
            services.AddHttpClient("SAG", client =>
            {
                client.BaseAddress = new Uri(sagBaseUrl);
                client.DefaultRequestHeaders.Add("Authorization",
                    $"Bearer {configuration["SagApi:Token"]}");
            });
            services.AddHostedService<SagSyncService>();
        }
        else
        {
            services.AddHttpClient("SAG");
        }

        return services;
    }
}
