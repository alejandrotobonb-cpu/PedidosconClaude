# Setup Azure AD — Portal de Proveedores GHT

## 1. Registro de la app API (Portal.API)

1. Azure Portal → Azure Active Directory → App registrations → **New registration**
   - Name: `Portal Proveedores API`
   - Supported account types: `Accounts in this organizational directory only`
2. En la app registrada → **Expose an API**
   - Set Application ID URI: `api://{CLIENT_ID}`
   - Add scope: `user_impersonation` (Admin and users can consent)
3. En **Manifest**, agregar roles de aplicación:
   ```json
   "appRoles": [
     { "allowedMemberTypes": ["User"], "displayName": "Proveedor", "id": "GUID1", "value": "proveedor" },
     { "allowedMemberTypes": ["User"], "displayName": "Comprador SAC", "id": "GUID2", "value": "comprador" },
     { "allowedMemberTypes": ["User"], "displayName": "Admin SAC", "id": "GUID3", "value": "admin" }
   ]
   ```
4. Copiar **Application (client) ID** → pegar en `appsettings.json` como `ClientId` y `Audience`
5. Copiar **Directory (tenant) ID** → pegar en `appsettings.json` como `TenantId`

## 2. Registro de la app Frontend (React)

1. Azure AD → App registrations → **New registration**
   - Name: `Portal Proveedores Frontend`
   - Redirect URI: `Single-page application (SPA)` → `https://localhost:5173` (dev)
2. En **API permissions** → Add permission → My APIs → seleccionar `Portal Proveedores API` → `user_impersonation`
3. Copiar **Application (client) ID** → pegar en `.env.local` como `VITE_AAD_CLIENT_ID`

## 3. Asignación de usuarios proveedores

1. Azure AD → Enterprise applications → `Portal Proveedores API`
2. Users and groups → Add user → asignar rol `proveedor`
3. En el perfil del usuario: agregar atributo de extensión `extension_Nit` con el NIT del proveedor
   - Si no tienes extensiones de directorio, usar el campo "Department" o "Employee ID" como fallback temporal

## 4. Valores a completar

| Archivo | Campo | Valor |
|---------|-------|-------|
| `Portal.API/appsettings.json` | `AzureAd.TenantId` | Directory (tenant) ID |
| `Portal.API/appsettings.json` | `AzureAd.ClientId` | API App Client ID |
| `Portal.API/appsettings.json` | `ConnectionStrings.PortalDb` | Azure SQL connection string |
| `Portal.API/appsettings.json` | `AzureCommunicationServices.ConnectionString` | ACS connection string |
| `Portal.API/appsettings.json` | `SagApi.Token` | Token de autenticación SAG |
| `frontend/.env.local` | `VITE_AAD_CLIENT_ID` | Frontend App Client ID |
| `frontend/.env.local` | `VITE_AAD_TENANT_ID` | Directory (tenant) ID |
| `frontend/.env.local` | `VITE_AAD_API_CLIENT_ID` | API App Client ID |

## 5. Recursos Azure a provisionar

```bash
# Grupo de recursos
az group create --name rg-portal-proveedores --location eastus

# Azure SQL Database
az sql server create --name srv-portal-ght --resource-group rg-portal-proveedores \
  --location eastus --admin-user sqladmin --admin-password "TU_PASSWORD"
az sql db create --resource-group rg-portal-proveedores --server srv-portal-ght \
  --name PortalProveedores --service-objective S2

# Azure Blob Storage
az storage account create --name stportalght --resource-group rg-portal-proveedores \
  --location eastus --sku Standard_LRS
az storage container create --name adjuntos-oc --account-name stportalght \
  --public-access off

# Azure Communication Services
az communication create --name acs-portal-ght --resource-group rg-portal-proveedores \
  --data-location unitedstates
```
