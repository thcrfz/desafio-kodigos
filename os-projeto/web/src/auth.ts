import { ref } from "vue";
import { setToken as setApiToken } from "./api";

export const authToken = ref<string | null>(localStorage.getItem("auth_token"));
export const authUser = ref<any>(() => {
  try {
    const auth_user = localStorage.getItem("auth_user");
    return JSON.parse(auth_user || "");
  } catch {
    return null;
  }
});

export function setAuth(token: string, user: any): void {
  authToken.value = token;
  authUser.value = user;
  localStorage.setItem("auth_token", token);
  localStorage.setItem("auth_user", JSON.stringify(user));
  setApiToken(token);
}

export function clearAuth() {
  authToken.value = null;
  authToken.value = null;
  localStorage.removeItem("auth_token");
  localStorage.removeItem("auth_user");
  setApiToken(undefined);
}
