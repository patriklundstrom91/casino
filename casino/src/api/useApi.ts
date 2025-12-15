import { useCallback } from "react";
import { useAuth } from "@clerk/clerk-react";

export const useApi = () => {
  const { getToken } = useAuth();
  const API_URL = import.meta.env.VITE_API_URL || "http://localhost:5136";
  console.log("🛠️ API_URL:", API_URL);
  console.log("🛠️ VITE_API_URL:", import.meta.env.VITE_API_URL);
  const apiFetch = useCallback(
    async (endpoint: string, options: RequestInit = {}) => {
      const token = await getToken({ template: "Casino-JWT" });
      const url = `${API_URL}${endpoint.startsWith("/") ? "" : "/"}${endpoint}`;
      const response = await fetch(url, {
        ...options,
        headers: {
          "Content-Type": "application/json",
          ...(token && { Authorization: `Bearer ${token}` }),
          ...options.headers,
        },
      });

      const data = await response.json();

      if (response.status >= 400) {
        throw new Error(
          data.error || data.message || `API Error: ${response.status}`
        );
      }

      return data;
    },
    [getToken]
  );

  return { apiFetch };
};
