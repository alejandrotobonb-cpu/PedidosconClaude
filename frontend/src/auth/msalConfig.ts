import { Configuration, PopupRequest } from "@azure/msal-browser";

export const msalConfig: Configuration = {
  auth: {
    clientId: "3a3fd4e8-824b-4a64-b7cf-79e3b4edc70c",
    authority: "https://login.microsoftonline.com/63e8f62a-1fe4-4b6e-b74a-bef036828dd7",
    redirectUri: window.location.origin,
  },
  cache: {
    cacheLocation: "sessionStorage",
    storeAuthStateInCookie: false,
  },
};

export const loginRequest: PopupRequest = {
  scopes: ["api://3a3fd4e8-824b-4a64-b7cf-79e3b4edc70c/access_as_user"],
};
