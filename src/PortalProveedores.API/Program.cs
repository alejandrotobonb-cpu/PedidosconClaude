using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using PortalProveedores.Application.Interfaces;
using PortalProveedores.Application.UseCases;
using PortalProveedores.Domain.Interfaces;
using PortalProveedores.Infrastructure.Persistence;
using PortalProveedores.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Azure AD authentication
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAd");

// EF Core — Azure SQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IOrdenCompraRepository, OrdenCompraRepository>();
builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();

// Use cases — registered via interface so Controller depends on abstraction, not implementation
builder.Services.AddScoped<IOrdenesPorProveedor, OrdenesPorProveedor>();
builder.Services.AddScoped<IGuardarComentario, GuardarComentario>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS para el frontend React (puerto por defecto de Vite)
builder.Services.AddCors(options =>
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Migración automática al iniciar en desarrollo
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
