import { Configuration, PopupRequest } from "@azure/msal-browser";

export const msalConfig: Configuration = {
  auth: {
    clientId: import.meta.env.VITE_AAD_CLIENT_ID,
    authority: `https://login.microsoftonline.com/${import.meta.env.VITE_AAD_TENANT_ID}`,
    redirectUri: window.location.origin,
  },
  cache: {
    cacheLocation: "sessionStorage",
    storeAuthStateInCookie: false,
  },
};

export const loginRequest: PopupRequest = {
  scopes: [`api://${import.meta.env.VITE_AAD_API_CLIENT_ID}/user_impersonation`],
};

export const apiConfig = {
  baseUrl: import.meta.env.VITE_API_BASE_URL as string,
};
