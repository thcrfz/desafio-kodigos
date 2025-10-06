import axios from "axios";
import router from "./router";

const baseURL = import.meta.env.VITE_API_BASE as string;

export const api = axios.create({
  baseURL,
  timeout: 15000,
});

export function setToken(token?: string) {
  if (token) api.defaults.headers.common["Authorization"] = `Bearer ${token}`;
  else delete api.defaults.headers.common["Authorization"];
}

api.interceptors.response.use(
  (res) => res,
  (err) => {
    if (err?.response?.status === 401) {
      setToken(undefined);
      if (router?.currentRoute?.value?.path !== "/login") router.push("/login");
    }
    return Promise.reject(err);
  }
);
