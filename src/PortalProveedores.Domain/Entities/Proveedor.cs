namespace PortalProveedores.Domain.Entities;

public class Proveedor
{
    public int Id { get; set; }
    public string Nit { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string EmailContacto { get; set; } = string.Empty;
    public string AzureAdObjectId { get; set; } = string.Empty;
    public string CompradorEmail { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
