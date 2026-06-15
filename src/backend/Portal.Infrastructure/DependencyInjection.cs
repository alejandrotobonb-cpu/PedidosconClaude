using Azure.Communication.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portal.Domain.Interfaces;
using Portal.Infrastructure.Persistence;
using Portal.Infrastructure.SagAdapter;
using Portal.Infrastructure.Services;

namespace Portal.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PortalDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("PortalDb")));

        services.AddScoped<IPortalDbContext>(sp => sp.GetRequiredService<PortalDbContext>());

        services.AddSingleton<IEmailService>(_ =>
        {
            var connectionString = configuration["AzureCommunicationServices:ConnectionString"]!;
            var senderAddress = configuration["AzureCommunicationServices:SenderAddress"]!;
            return new EmailService(new EmailClient(connectionString), senderAddress);
        });

        services.AddHttpClient("SAG", client =>
        {
            client.BaseAddress = new Uri(configuration["SagApi:BaseUrl"]!);
            client.DefaultRequestHeaders.Add("Authorization",
                $"Bearer {configuration["SagApi:Token"]}");
        });

        services.AddHostedService<SagSyncService>();

        return services;
    }
}
