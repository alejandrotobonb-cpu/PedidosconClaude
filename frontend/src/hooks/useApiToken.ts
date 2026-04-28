import { useMsal } from "@azure/msal-react";
import { useEffect } from "react";
import { loginRequest } from "../auth/msalConfig";
import { setAuthToken } from "../api/ordenesApi";

export function useApiToken() {
  const { instance, accounts } = useMsal();

  useEffect(() => {
    if (accounts.length === 0) return;
    instance
      .acquireTokenSilent({ ...loginRequest, account: accounts[0] })
      .then((res) => setAuthToken(res.accessToken))
      .catch(() => instance.acquireTokenPopup(loginRequest)
        .then((res) => setAuthToken(res.accessToken)));
  }, [instance, accounts]);
}
