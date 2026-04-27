# SDD — Portal de Proveedores GHT
**Spec Driven Development**
**Versión:** 2.0
**Fecha:** 2026-04-27
**Proyecto:** Control Pendientes de Entrega – Proveedores SAC
**Empresa:** GHT – Growers Hub Trading

---

## 1. Introducción

### 1.1 Propósito
Este documento especifica el diseño completo del portal web "Control Pendientes de Entrega GHT – Proveedores", una aplicación que permite a los proveedores activos de GHT consultar sus órdenes de compra (OC) pendientes de entrega y registrar comentarios, fechas compromiso y documentos de soporte, facilitando el seguimiento interno del área de SAC (Servicio de Abastecimiento y Compras).

### 1.2 Alcance
El sistema cubre:
- Autenticación de proveedores mediante Directorio Activo corporativo (AD)
- Consulta de OC pendientes sincronizadas desde SAG vía API
- Registro de comentarios y fechas compromiso por parte del proveedor
- Adjunto de documentos de soporte (facturas, guías, fotos, etc.)
- Notificación automática por correo al comprador asignado cuando hay una actualización
- Exportación de comentarios a Excel para integración con herramientas internas
- Panel interno básico para el equipo SAC

El sistema **no** reemplaza SAG; es una capa de colaboración que consume datos de SAG y almacena únicamente los comentarios y adjuntos del proveedor.

### 1.3 Definiciones y acrónimos
| Término | Significado |
|---------|------------|
| OC | Orden de Compra |
| SAG | Sistema de gestión interno de GHT (ERP/WMS) |
| SAC | Servicio de Abastecimiento y Compras |
| AD | Active Directory – directorio de usuarios corporativo de GHT |
| Fuente/Finca | Sede o punto de entrega destino del pedido |
| Proveedor | Empresa externa que suministra productos a GHT |
| Comprador | Funcionario interno de SAC asignado a cada proveedor |
| Fecha compromiso | Fecha que el proveedor declara como nueva fecha de entrega |
| Novedad | Cualquier situación que afecte la entrega normal de un pedido |

### 1.4 Referencias
- Documento fuente: *Pendientes de entrega Proveedores SAC.docx*
- Sistema origen de datos: SAG (API interna GHT)
- Estándar de accesibilidad: WCAG 2.1 nivel AA
- Identidad visual: Manual de marca GHT (verde corporativo)

---

## 2. Descripción General del Sistema

### 2.1 Perspectiva del producto
El portal es una aplicación web independiente que se conecta a SAG mediante API para leer las OC pendientes. Los comentarios y adjuntos del proveedor se almacenan en la base de datos propia del portal. No escribe datos de vuelta a SAG.

```
[SAG API] ──read──► [Portal GHT] ◄──login── [Proveedor vía AD]
                         │
                    [Base de datos portal]
                         │
              ┌──────────┴──────────┐
         [Notificación email]   [Exportación Excel]
              │
         [Comprador SAC]
```

### 2.2 Funciones principales
1. **Autenticación SSO** mediante credenciales del Directorio Activo GHT
2. **Dashboard del proveedor:** lista de OC pendientes propias, ordenadas de más antigua a más reciente, con alerta visual de días vencidos y pedidos próximos a vencer (≤ 6 días)
3. **Detalle de OC:** información completa del pedido (solo lectura)
4. **Registro de comentario:** texto libre + fecha compromiso + adjuntos
5. **Alerta de pedidos urgentes:** marcación visual diferenciada (etiqueta roja)
6. **Notificación automática:** correo al comprador asignado al guardar un comentario
7. **Exportación Excel:** descarga de todos los comentarios registrados por el proveedor
8. **Panel SAC (interno):** vista consolidada de todos los proveedores y sus comentarios

### 2.3 Usuarios del sistema
| Tipo | Descripción | Acceso |
|------|-------------|--------|
| Proveedor | Contacto SAC asignado por GHT (1 usuario por proveedor) | Solo sus propias OC |
| Comprador SAC | Funcionario interno responsable de proveedores asignados | OC de sus proveedores |
| Administrador SAC | Coordinador del área | Vista global + exportación |

### 2.4 Restricciones y suposiciones
- El proveedor accede desde PC (diseño prioritario desktop, responsive secundario)
- Solo existe un usuario activo por proveedor (el contacto SAC asignado)
- Los proveedores se cargan desde un archivo maestro Excel inicial provisto por GHT
- El valor monetario de las OC **no** es visible para el proveedor
- La sincronización con SAG es responsabilidad del equipo de TI de GHT (este documento especifica el contrato de la API, no su implementación en SAG)
- El idioma del sistema es exclusivamente español

---

## 3. Arquitectura del Sistema

### 3.1 Diagrama de arquitectura

```
┌──────────────────────────────────────────────────────────────┐
│                    AZURE ACTIVE DIRECTORY                    │
│            Autenticación SSO (OAuth 2.0 / OIDC)             │
└───────────────────────────┬──────────────────────────────────┘
                            │ Token
          ┌─────────────────┼──────────────────┐
          ▼                 ▼                  ▼
┌──────────────────┐ ┌──────────────┐ ┌────────────────────┐
│  FRONTEND        │ │  BACKEND     │ │  SAG API           │
│  React (TypeScript)  │ │  ASP.NET Core│ │  GET /oc/pendientes│
│  Azure App Service   │ │  Web API     │ │  (sistema interno) │
│  (Static Web App)│ │  App Service /│ │                    │
└────────┬─────────┘ │  Container App│ └─────────┬──────────┘
         │ HTTPS     └──────┬───────┘           │
         └──────────────────┘         ┌──────────┘
                            │         │ (sync c/15 min)
               ┌────────────▼─────────▼──────────────────────┐
               │              AZURE DATA LAYER                │
               │  ┌──────────────────┐  ┌──────────────────┐ │
               │  │  Azure SQL DB    │  │  Azure Blob       │ │
               │  │  (comentarios,   │  │  Storage          │ │
               │  │   proveedores,   │  │  (adjuntos:       │ │
               │  │   caché OC)      │  │   PDF, IMG, Excel)│ │
               │  └──────────────────┘  └──────────────────┘ │
               └─────────────────────────────────────────────┘
                            │
               ┌────────────▼────────────────────┐
               │  Azure Communication Services   │
               │  Notificación email al comprador │
               └─────────────────────────────────┘
```

### 3.2 Componentes y responsabilidades
| Componente | Tecnología | Responsabilidad |
|-----------|-----------|----------------|
| **Frontend SPA** | React 18 + TypeScript | Renderizado de pantallas, navegación entre paneles, llamadas a API |
| **Backend API** | ASP.NET Core 8 Web API | Lógica de negocio, autenticación, integración SAG, exportación Excel |
| **Auth Service** | Azure AD + Microsoft.Identity.Web | SSO con Directorio Activo corporativo (OAuth 2.0 / OIDC) |
| **SAG Adapter** | Servicio C# + HttpClient | Consulta periódica a SAG API y caché local en Azure SQL |
| **Comment Service** | C# + EF Core | CRUD de comentarios, fechas compromiso y referencia a adjuntos |
| **Storage Service** | Azure Blob Storage SDK | Upload, descarga y eliminación de adjuntos |
| **Email Service** | Azure Communication Services | Notificación al comprador al guardar un comentario |
| **Excel Export** | ClosedXML (C#) | Generación de archivo .xlsx con comentarios |
| **Base de datos** | Azure SQL Database | Proveedores, caché de OC, comentarios y metadata de adjuntos |
| **Archivos adjuntos** | Azure Blob Storage | Almacenamiento de documentos subidos por el proveedor |

### 3.3 Patrones de diseño
- **Clean Architecture:** separación en capas — Domain, Application, Infrastructure, API
- **Repository + Unit of Work:** acceso a datos con Entity Framework Core sobre Azure SQL
- **Adapter:** para aislar la integración con SAG API del resto del sistema
- **Mediator (MediatR):** desacoplamiento de comandos y queries (CQRS)
- **Observer:** publicación de evento `ComentarioGuardado` que dispara notificación email
- **Cache-aside:** OC leídas de SAG se cachean en Azure SQL para disponibilidad ante caídas de SAG

---

## 4. Diseño de la Interfaz

### 4.1 Estructura de pantallas y navegación

```
[Login AD]
    │
    ▼
[Hoja 1 – Instructivo / Bienvenida]
    │  (botón ➡️)
    ▼
[Hoja 2 – Panel Principal del Proveedor]
    │  Panel 1: Indicador de días
    │  Panel 2: Lista de OC (seleccionable)
    │  Panel 3: Detalle OC (readonly)
    │  Panel 4: Comentario + adjuntos + guardar
    │
    ├── [Exportar Excel]
    └── [Cerrar sesión]

[Panel SAC – solo usuarios internos]
    │
    ├── Vista consolidada todos los proveedores
    └── Exportación global
```

### 4.2 Guía de estilos

| Elemento | Valor |
|---------|-------|
| Color primario | Verde corporativo GHT `#2E7D32` |
| Color fondo header | `#2E7D32` texto blanco |
| Color alerta vencido | Rojo `#D32F2F` |
| Color alerta próximo (≤6 días) | Naranja `#F57C00` |
| Color urgente | Rojo intenso `#B71C1C` con etiqueta "URGENTE" |
| Color panel seleccionado | Azul claro `#E3F2FD` |
| Tipografía | Inter o Roboto, sans-serif |
| Tamaño base | 14px (desktop), mínimo 12px |
| Logo | GHT – Growers Hub Trading (esquina superior izquierda) |
| Usuario activo | Mostrado en header (nombre + empresa proveedor) |

### 4.3 Diseño responsivo
- **Prioritario:** Desktop ≥ 1024px (proveedores acceden desde PC)
- **Secundario:** Tablet ≥ 768px (layout adaptado, paneles en columna)
- **Mínimo soportado:** Mobile 375px (solo consulta, sin adjuntos)

### 4.4 Pantalla 1 — Instructivo / Bienvenida

```
┌──────────────────────────────────────────────────────────────────┐
│ 🟢 [Logo GHT]   Control Pendientes de Entrega GHT – Proveedores  ➡️ │
├──────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ¡Bienvenido, [Nombre Proveedor]!                               │
│                                                                  │
│  Esta aplicación te permite:                                     │
│  • Consultar tus pedidos pendientes de entrega                   │
│  • Registrar comentarios y fechas compromiso                     │
│  • Adjuntar documentos de soporte                                │
│                                                                  │
│  👉 Cómo usar la app:                                            │
│     1. Selecciona un pedido de la lista                          │
│     2. Revisa el detalle del pedido                              │
│     3. Ingresa tu comentario y guarda (💾)                       │
│                                                                  │
│  ⚠️ Importante:                                                  │
│     • Debes hacer clic en 💾 para guardar tu comentario         │
│     • Tu equipo de compras verá la información registrada        │
│     • Solo ves la información de tu empresa                      │
│     • Para novedades urgentes contacta a tu comprador asignado   │
│                                                                  │
│                          [Logo GHT]                              │
│                  Usuario activo: usuario@ghtcorp.com             │
└──────────────────────────────────────────────────────────────────┘
```

### 4.5 Pantalla 2 — Panel Principal del Proveedor

```
┌──────────────────────────────────────────────────────────────────┐
│ 🟢 [Logo GHT]   Control Pendientes de Entrega GHT – Proveedores  │
│                                         [Exportar Excel] [Salir] │
├────────────┬───────────────────────────────────────────────────── │
│            │                                                      │
│ PANEL 2    │  PANEL 3 – Detalle del pedido seleccionado          │
│ Lista OC   │  ┌────────────────────────────────────────────────┐ │
│            │  │ Fuente/Finca:    [valor]                       │ │
│ [🔴 -45d]  │  │ Documento OC:   [valor]                       │ │
│ Finca El   │  │ Código artículo: [valor]                      │ │
│ Rosal      │  │ Descripción:    [valor]                       │ │
│ OC 12345   │  │ Fecha pedido:   [valor]                       │ │
│ Artículo X │  │ Fecha entrega:  [valor]                       │ │
│            │  │ Cantidad pedida: [valor]                      │ │
│ [🟠 -3d]   │  │ Cantidad pend.: [valor]                       │ │
│ Finca La   │  │ Obs. compras:   [valor]                       │ │
│ Esperanza  │  └────────────────────────────────────────────────┘ │
│ OC 12346   │                                                      │
│ Artículo Y │  PANEL 4 – Comentario del proveedor                 │
│            │  ┌────────────────────────────────────────────────┐ │
│ [🔴URGENTE]│  │ Comentario: __________________________________ │ │
│ OC 12347   │  │             __________________________________ │ │
│ Artículo Z │  │                                                │ │
│            │  │ Fecha compromiso: [selector de fecha]          │ │
│            │  │                                                │ │
│            │  │ Adjuntos:  [+ Agregar archivo]                 │ │
│            │  │  • N° Guía/Remisión: [campo texto]             │ │
│            │  │  • Archivo (PDF/PNG/JPG/Excel): [subir]        │ │
│            │  │                                                │ │
│            │  │                          [💾 Guardar]          │ │
│            │  └────────────────────────────────────────────────┘ │
│            │                                                      │
│ PANEL 1    │                                                      │
│ [Días:     │                                                      │
│  -45 días] │                                                      │
└────────────┴──────────────────────────────────────────────────────┘
```

---

## 5. Diseño de Componentes

### 5.1 `<OrderList />` — Panel 2
| Propiedad | Descripción |
|-----------|-------------|
| **Datos** | Lista de OC del proveedor autenticado |
| **Orden** | Más antigua primero (mayor días negativos arriba) |
| **Ítem** | Días vs fecha entrega · Fuente/Finca · N° OC · Artículo |
| **Estados visuales** | Vencido (🔴 rojo) · Próximo ≤6 días (🟠 naranja) · Normal (⚪) · Urgente (🔴 etiqueta URGENTE) |
| **Interacción** | Click en ítem → carga detalle en Panel 3 y limpia Panel 4 |
| **Paginación** | Virtual scroll o paginación de 50 ítems (máx. 1,808 OC) |

### 5.2 `<OrderDetail />` — Panel 3
| Propiedad | Descripción |
|-----------|-------------|
| **Datos** | Fuente, N° OC, código artículo, descripción, fecha pedido, fecha entrega, cantidad pedida, cantidad pendiente, observaciones compras |
| **Editable** | Ningún campo |
| **Valor dinero** | No se muestra |

### 5.3 `<CommentForm />` — Panel 4
| Campo | Tipo | Validación |
|-------|------|-----------|
| Comentario | Textarea libre | Requerido, mín. 10 chars |
| Fecha compromiso | Date picker | Opcional, no puede ser fecha pasada |
| N° Guía/Remisión | Input texto | Opcional |
| Archivo adjunto | File upload | Opcional, máx. 10 MB por archivo, formatos: PDF, PNG, JPG, XLSX, DOCX |
| **Guardar** | Botón primario | Activo solo si comentario completado |

**Comportamiento al guardar:**
1. Persiste comentario + adjuntos en BD portal
2. Dispara email de notificación al comprador asignado
3. Muestra confirmación visual en pantalla
4. Panel 2 actualiza el ítem (indicador de último comentario)

### 5.4 `<AlertIndicator />` — Panel 1
- Muestra los días vs fecha de entrega del pedido seleccionado
- Número grande, color según estado (rojo / naranja / verde)
- Etiqueta "URGENTE" en rojo intenso si aplica

### 5.5 `<InstructivoPage />` — Pantalla 1
- Se muestra una sola vez por sesión (o siempre al login, configurable)
- Botón ➡️ avanza al Panel Principal
- Muestra nombre del proveedor y usuario activo

### 5.6 `<ExportButton />` — Exportar Excel
- Disponible en header del Panel Principal
- Genera y descarga automáticamente el archivo .xlsx
- Nombre del archivo: `comentarios_[NIT]_[YYYYMMDD].xlsx`

### 5.7 `<SACDashboard />` — Panel interno SAC
| Función | Descripción |
|---------|-------------|
| Vista de proveedores | Lista todos los proveedores con resumen de OC pendientes y último comentario |
| Filtros | Por proveedor, por estado de OC, por comprador asignado |
| Exportación global | Excel con todos los comentarios de todos los proveedores |
| Acceso | Solo usuarios con rol `comprador` o `admin` en AD |

---

## 6. Diseño de Datos

### 6.1 Modelo de datos del portal (Azure SQL Database)

```sql
-- Maestro de proveedores (cargado desde Excel inicial)
CREATE TABLE Proveedores (
  Id              INT           IDENTITY(1,1) PRIMARY KEY,
  Nit             VARCHAR(20)   NOT NULL UNIQUE,
  Nombre          VARCHAR(200)  NOT NULL,
  EmailSac        VARCHAR(100)  NOT NULL,   -- usuario AD asignado (contacto SAC)
  CompradorEmail  VARCHAR(100)  NOT NULL,   -- comprador interno asignado
  Activo          BIT           NOT NULL DEFAULT 1,
  CreatedAt       DATETIME2     NOT NULL DEFAULT GETUTCDATE()
);

-- Caché de OC sincronizadas desde SAG (se refresca cada 15 min)
CREATE TABLE OrdenesCompra (
  Id              INT           IDENTITY(1,1) PRIMARY KEY,
  NumeroOc        VARCHAR(20)   NOT NULL,
  ProveedorNit    VARCHAR(20)   NOT NULL REFERENCES Proveedores(Nit),
  FuenteFinca     VARCHAR(100)  NULL,
  CodigoArt       VARCHAR(50)   NULL,
  Descripcion     NVARCHAR(MAX) NULL,
  FechaPedido     DATE          NULL,
  FechaEntrega    DATE          NULL,
  CantidadPedida  DECIMAL(10,2) NULL,
  CantidadPend    DECIMAL(10,2) NULL,
  ObsCompras      NVARCHAR(MAX) NULL,
  Urgente         BIT           NOT NULL DEFAULT 0,
  DiasVencimiento AS DATEDIFF(DAY, GETUTCDATE(), FechaEntrega) PERSISTED,
  SincronizadoEn  DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
  CONSTRAINT UQ_OC_Art UNIQUE (NumeroOc, CodigoArt)
);

-- Comentarios registrados por el proveedor
CREATE TABLE Comentarios (
  Id              INT           IDENTITY(1,1) PRIMARY KEY,
  OrdenCompraId   INT           NOT NULL REFERENCES OrdenesCompra(Id),
  ProveedorNit    VARCHAR(20)   NOT NULL REFERENCES Proveedores(Nit),
  Texto           NVARCHAR(MAX) NOT NULL,
  FechaCompromiso DATE          NULL,
  NumeroGuia      VARCHAR(100)  NULL,
  Notificado      BIT           NOT NULL DEFAULT 0,
  CreatedAt       DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
  UpdatedAt       DATETIME2     NOT NULL DEFAULT GETUTCDATE()
);

-- Metadata de adjuntos (el archivo físico vive en Azure Blob Storage)
CREATE TABLE Adjuntos (
  Id              INT           IDENTITY(1,1) PRIMARY KEY,
  ComentarioId    INT           NOT NULL REFERENCES Comentarios(Id),
  NombreArchivo   VARCHAR(255)  NOT NULL,
  TipoMime        VARCHAR(100)  NOT NULL,
  TamanioBytes    INT           NOT NULL,
  BlobUri         VARCHAR(500)  NOT NULL,  -- URI en Azure Blob Storage
  CreatedAt       DATETIME2     NOT NULL DEFAULT GETUTCDATE()
);
```

**Notas de infraestructura de datos:**
- **Azure SQL Database:** tier Standard S2 (mínimo), escalable según crecimiento
- **Azure Blob Storage:** container privado `adjuntos-oc`, acceso mediante SAS tokens de corta duración generados por el backend
- **Entity Framework Core 8** como ORM con migraciones versionadas en el repositorio

### 6.2 Contrato de la API SAG

**Endpoint esperado de SAG:**

```
GET /api/oc/pendientes?nit={nit_proveedor}

Response 200:
{
  "proveedor_nit": "900123456",
  "ordenes": [
    {
      "numero_oc": "12345",
      "fuente_finca": "Finca El Rosal",
      "codigo_articulo": "ART-001",
      "descripcion": "Fertilizante NPK 50kg",
      "fecha_pedido": "2026-03-01",
      "fecha_entrega": "2026-03-15",
      "cantidad_pedida": 100,
      "cantidad_pendiente": 60,
      "observaciones": "Pedido urgente temporada",
      "urgente": true
    }
  ]
}
```

**Frecuencia de sincronización:** Cada 15 minutos (configurable)
**Autenticación con SAG:** Token interno GHT (variable de entorno `SAG_API_TOKEN`)

### 6.3 Formato de exportación Excel

| Columna | Fuente |
|---------|--------|
| Proveedor | `proveedores.nombre` |
| NIT | `proveedores.nit` |
| Fuente/Finca | `ordenes_compra.fuente_finca` |
| N° OC | `ordenes_compra.numero_oc` |
| Artículo | `ordenes_compra.descripcion` |
| Comentario proveedor | `comentarios.texto` |
| Fecha compromiso | `comentarios.fecha_compromiso` |
| Fecha registro comentario | `comentarios.created_at` |

### 6.4 Plantilla de correo de notificación al comprador

```
Asunto: [Portal GHT] Actualización de pedido – [Proveedor] – OC [N° OC]

El proveedor [Nombre Proveedor] ha registrado un comentario en el portal:

  Fuente/Finca : [fuente_finca]
  N° OC        : [numero_oc]
  Artículo     : [descripcion]
  Comentario   : [texto_comentario]
  Fecha compromiso: [fecha_compromiso o "No indicada"]
  N° Guía      : [numero_guia o "No indicado"]
  Adjuntos     : [lista de nombres de archivos o "Sin adjuntos"]

Registrado el: [created_at]

Ingresa al portal para ver el detalle completo.
```

---

## 7. Requisitos No Funcionales

### 7.1 Rendimiento
| Métrica | Objetivo |
|---------|---------|
| Carga inicial del portal | < 3 segundos |
| Carga de lista de OC | < 2 segundos (hasta 1,808 ítems) |
| Guardado de comentario | < 1 segundo de respuesta |
| Concurrencia esperada | ~286 usuarios simultáneos (máximo) |

### 7.2 Accesibilidad
- WCAG 2.1 nivel AA
- Contraste mínimo 4.5:1 en texto
- Navegación por teclado funcional
- Labels en todos los campos de formulario

### 7.3 Seguridad
| Riesgo | Mitigación |
|--------|-----------|
| Acceso entre proveedores | Claim de NIT extraído del token Azure AD; filtro obligatorio en cada query de EF Core |
| XSS | Sanitización en frontend (React escapa por defecto); validación adicional en API |
| File upload malicioso | Validación de tipo MIME + extensión + tamaño en ASP.NET Core antes del upload a Blob |
| Sesión | JWT emitido por Azure AD con expiración 8 horas; refresh token automático |
| HTTPS | Forzado en Azure App Service / Container Apps (TLS 1.2+) |
| Secretos | Cadenas de conexión y tokens almacenados en Azure Key Vault; nunca en código o config |
| Acceso a Blob Storage | SAS tokens de corta duración (15 min) generados por el backend; container privado |
| Exposición de datos financieros | El campo de valor monetario de OC nunca se consulta ni expone al proveedor |

### 7.4 Compatibilidad de navegadores
- Chrome ≥ 100 (prioridad)
- Edge ≥ 100
- Firefox ≥ 100
- Safari ≥ 15 (secundario)

### 7.5 Disponibilidad
- Objetivo: 99% en horario laboral (lunes a sábado 6am–8pm)
- Mantenimiento: fuera de horario laboral

### 7.6 Retención de datos
- Comentarios y adjuntos: mínimo 5 años (auditoría interna)
- Caché de OC SAG: solo activas; se eliminan al cerrar la OC en SAG

---

## 8. Plan de Implementación

### Etapa 1 — MVP (Fecha límite: 2026-05-01)

> **Nota:** El plazo es de 4 días hábiles. El MVP se limita estrictamente a las funciones críticas del flujo principal.

| Tarea | Prioridad |
|-------|-----------|
| Setup solución ASP.NET Core 8 Web API + React 18 TS | Alta |
| Configurar Azure AD (registro de app, scopes, roles) | Alta |
| Provisionar Azure SQL Database + migraciones EF Core | Alta |
| Carga del maestro de proveedores desde Excel | Alta |
| Integración con SAG API (HttpClient + SAG Adapter) | Alta |
| Pantalla Instructivo / Bienvenida (Hoja 1) | Alta |
| Panel Principal 4 paneles (Hoja 2) — React components | Alta |
| Endpoint POST comentarios + guardado en Azure SQL | Alta |
| Notificación email con Azure Communication Services | Alta |
| Despliegue en Azure App Service (entorno de pruebas) | Alta |
| Alerta visual: vencidos (🔴), próximos ≤6 días (🟠), urgentes | Alta |

**Fuera del MVP Etapa 1:**
- Adjuntos de archivos (Azure Blob Storage)
- Exportación Excel
- Panel SAC interno

### Etapa 2 — Completitud (estimado: 2026-05-15)

| Tarea | Prioridad |
|-------|-----------|
| Upload de adjuntos a Azure Blob Storage + SAS tokens | Alta |
| Campo N° Guía/Remisión | Alta |
| Exportación Excel con ClosedXML (.xlsx) | Alta |
| Panel SAC interno (vista consolidada por comprador) | Media |
| Filtros en Panel SAC (proveedor, estado, comprador) | Media |
| Mover a Azure Container Apps (si escala lo requiere) | Media |

### Etapa 3 — Mejoras y analítica (estimado: 2026-06-01)

| Tarea | Prioridad |
|-------|-----------|
| Dashboard de métricas: causales de demora, cumplimiento por proveedor | Media |
| % líneas entregadas sin entrada almacén | Media |
| Historial de comentarios por OC | Baja |
| Exportación global para administrador SAC | Baja |
| Notificación recordatorio automática (OC vencidas sin comentario) | Baja |
| Integración con Azure Monitor + Application Insights | Baja |

### Stack tecnológico oficial

| Capa | Tecnología | Razón |
|------|-----------|-------|
| **Backend** | ASP.NET Core 8 Web API (C#) | Stack corporativo GHT; alto rendimiento, tipado fuerte |
| **Frontend** | React 18 + TypeScript | Stack corporativo GHT; SPA con experiencia fluida |
| **Autenticación** | Azure Active Directory + Microsoft.Identity.Web | SSO corporativo; integración nativa con AD de GHT |
| **Base de datos** | Azure SQL Database + EF Core 8 | Stack corporativo GHT; datos relacionales, migraciones versionadas |
| **Archivos adjuntos** | Azure Blob Storage | Stack corporativo GHT; almacenamiento seguro y escalable |
| **Email** | Azure Communication Services | Integrado en ecosistema Azure; sin dependencia de SMTP externo |
| **Excel** | ClosedXML (C#) | Librería .NET madura; sin licencias adicionales |
| **Secrets** | Azure Key Vault | Gestión segura de cadenas de conexión y tokens |
| **Hosting Backend** | Azure App Service | Stack corporativo GHT; despliegue simple con CI/CD |
| **Hosting Frontend** | Azure Static Web Apps | Optimizado para SPA React; CDN global integrado |
| **Contenedores** | Azure Container Apps | Stack corporativo GHT; opción de escalado en Etapa 2+ |
| **CI/CD** | GitHub Actions → Azure | Automatización de build, test y despliegue |
| **Monitoreo** | Azure Application Insights | Trazabilidad, errores y métricas de performance |

---

*Documento generado bajo metodología SDD — Spec Driven Development*
*Próximo paso: revisión y aprobación del SDD antes de iniciar desarrollo*
