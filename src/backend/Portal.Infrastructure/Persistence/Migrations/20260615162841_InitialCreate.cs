using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portal.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EmailSac = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CompradorEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.Id);
                    table.UniqueConstraint("AK_Proveedores_Nit", x => x.Nit);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesCompra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroOc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ProveedorNit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FuenteFinca = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CodigoArt = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaPedido = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaEntrega = table.Column<DateOnly>(type: "date", nullable: true),
                    CantidadPedida = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CantidadPend = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ObsCompras = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Urgente = table.Column<bool>(type: "bit", nullable: false),
                    SincronizadoEn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesCompra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesCompra_Proveedores_ProveedorNit",
                        column: x => x.ProveedorNit,
                        principalTable: "Proveedores",
                        principalColumn: "Nit",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comentarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdenCompraId = table.Column<int>(type: "int", nullable: false),
                    ProveedorNit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCompromiso = table.Column<DateOnly>(type: "date", nullable: true),
                    NumeroGuia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notificado = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comentarios_OrdenesCompra_OrdenCompraId",
                        column: x => x.OrdenCompraId,
                        principalTable: "OrdenesCompra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comentarios_Proveedores_ProveedorNit",
                        column: x => x.ProveedorNit,
                        principalTable: "Proveedores",
                        principalColumn: "Nit",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Adjuntos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComentarioId = table.Column<int>(type: "int", nullable: false),
                    NombreArchivo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TipoMime = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TamanioBytes = table.Column<int>(type: "int", nullable: false),
                    BlobUri = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adjuntos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Adjuntos_Comentarios_ComentarioId",
                        column: x => x.ComentarioId,
                        principalTable: "Comentarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adjuntos_ComentarioId",
                table: "Adjuntos",
                column: "ComentarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_OrdenCompraId",
                table: "Comentarios",
                column: "OrdenCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_ProveedorNit",
                table: "Comentarios",
                column: "ProveedorNit");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompra_NumeroOc_CodigoArt",
                table: "OrdenesCompra",
                columns: new[] { "NumeroOc", "CodigoArt" },
                unique: true,
                filter: "[CodigoArt] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesCompra_ProveedorNit",
                table: "OrdenesCompra",
                column: "ProveedorNit");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_Nit",
                table: "Proveedores",
                column: "Nit",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adjuntos");

            migrationBuilder.DropTable(
                name: "Comentarios");

            migrationBuilder.DropTable(
                name: "OrdenesCompra");

            migrationBuilder.DropTable(
                name: "Proveedores");
        }
    }
}
