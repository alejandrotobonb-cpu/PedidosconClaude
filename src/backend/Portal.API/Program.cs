using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Portal.Application;
using Portal.Infrastructure;
using Portal.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Auth: Azure AD en producción, dev-bypass en Development
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);
}
else
{
    builder.Services.AddAuthentication("DevBypass")
        .AddScheme<DevBypassAuthOptions, DevBypassAuthHandler>(
            "DevBypass", _ => { });
}
builder.Services.AddAuthorization();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

builder.Services.AddControllers();

var allowedOrigins = builder.Environment.IsDevelopment()
    ? new[] { "http://localhost:5173", "https://localhost:5173" }
    : new[] { builder.Configuration["Frontend:BaseUrl"]! };

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PortalDbContext>();
    await db.Database.EnsureCreatedAsync();   // SQLite dev: crea tablas sin migración
    if (app.Environment.IsDevelopment())
        await DevDataSeeder.SeedAsync(db);
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
