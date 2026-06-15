using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Portal.Application;
using Portal.Infrastructure;
using Portal.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(builder.Configuration["Frontend:BaseUrl"]!)
              .AllowAnyHeader()
              .AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PortalDbContext>();
    await db.Database.MigrateAsync();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
