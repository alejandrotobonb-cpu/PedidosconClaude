using Portal.Domain.Entities;

namespace Portal.Infrastructure.Persistence;

public static class DevDataSeeder
{
    public static async Task SeedAsync(PortalDbContext db)
    {
        if (db.Proveedores.Any()) return;

        var proveedor = new Proveedor
        {
            Nit = "900123456",
            Nombre = "Agroquímicos del Norte S.A.S",
            EmailSac = "proveedor@test.com",
            CompradorEmail = "comprador@ghtcorp.com",
            Activo = true
        };
        db.Proveedores.Add(proveedor);
        await db.SaveChangesAsync();

        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

        db.OrdenesCompra.AddRange(
            new OrdenCompra
            {
                NumeroOc = "OC-2025-001",
                ProveedorNit = "900123456",
                FuenteFinca = "Finca El Rosal",
                CodigoArt = "FERT-001",
                Descripcion = "Fertilizante NPK 50kg",
                FechaPedido = hoy.AddDays(-60),
                FechaEntrega = hoy.AddDays(-45),   // vencida
                CantidadPedida = 100,
                CantidadPend = 60,
                ObsCompras = "Pedido urgente temporada"
            },
            new OrdenCompra
            {
                NumeroOc = "OC-2025-002",
                ProveedorNit = "900123456",
                FuenteFinca = "Finca La Esperanza",
                CodigoArt = "PEST-007",
                Descripcion = "Herbicida glifosato 20L",
                FechaPedido = hoy.AddDays(-20),
                FechaEntrega = hoy.AddDays(3),     // próxima ≤6 días
                CantidadPedida = 50,
                CantidadPend = 50,
                ObsCompras = null
            },
            new OrdenCompra
            {
                NumeroOc = "OC-2025-003",
                ProveedorNit = "900123456",
                FuenteFinca = "Finca El Paraíso",
                CodigoArt = "SEMI-012",
                Descripcion = "Semillas de tomate cherry x1000",
                FechaPedido = hoy.AddDays(-10),
                FechaEntrega = hoy.AddDays(15),    // normal
                CantidadPedida = 200,
                CantidadPend = 200,
                ObsCompras = null
            },
            new OrdenCompra
            {
                NumeroOc = "OC-2025-004",
                ProveedorNit = "900123456",
                FuenteFinca = "Finca El Rosal",
                CodigoArt = "RIEGO-003",
                Descripcion = "Sistema de riego por goteo",
                FechaPedido = hoy.AddDays(-5),
                FechaEntrega = hoy.AddDays(7),     // urgente
                CantidadPedida = 1,
                CantidadPend = 1,
                Urgente = true,
                ObsCompras = "Instalación programada semana próxima"
            }
        );

        await db.SaveChangesAsync();
    }
}
