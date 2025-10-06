<template>
  <div class="container min-vh-100 d-flex align-items-center py-4">
    <div class="row w-100 justify-content-center">
      <div class="col-12 col-sm-10 col-md-6 col-lg-4">
        <div class="card shadow-sm">
          <div class="card-body p-4">
            <h1 class="h4 mb-4 text-center">Entrar</h1>

            <form @submit.prevent="submit" novalidate>
              <div class="mb-3">
                <label class="form-label" for="email">Email</label>
                <input
                  id="email"
                  v-model="email"
                  type="email"
                  class="form-control"
                  required
                  autocomplete="username"
                  placeholder="voce@empresa.com"
                />
              </div>

              <div class="mb-4">
                <label class="form-label" for="password">Senha</label>
                <input
                  id="password"
                  v-model="password"
                  type="password"
                  class="form-control"
                  required
                  autocomplete="current-password"
                  placeholder="••••••••"
                />
              </div>

              <button
                type="submit"
                class="btn btn-primary w-100 d-flex justify-content-center align-items-center"
                :disabled="loading"
              >
                <span
                  v-if="loading"
                  class="spinner-border spinner-border-sm me-2"
                  role="status"
                  aria-hidden="true"
                ></span>
                {{ loading ? "Entrando..." : "Entrar" }}
              </button>

              <div class="d-flex justify-content-between align-items-center mt-3">
                <router-link to="/register" class="small text-decoration-none">Criar conta</router-link>
              </div>

              <div v-if="error" class="alert alert-danger mt-3 mb-0 py-2" role="alert">
                {{ error }}
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>


<script setup lang="ts">
import { ref } from "vue";
import { api, setToken } from "../../api";
import { useRouter } from "vue-router";

const router = useRouter();
const email = ref("");
const password = ref("");
const loading = ref(false);
const error = ref("");

async function submit() {
  error.value = "";
  loading.value = true;
  try {
    const { data } = await api.post("/auth/login", {
      email: email.value,
      password: password.value,
    });
    setToken(data.token);
    localStorage.setItem("auth_token", data.token);
    localStorage.setItem("auth_user", JSON.stringify(data.user));
    router.push("/os");
  } catch (e: any) {
    error.value = e?.response?.data ?? "Falha ao entrar";
  } finally {
    loading.value = false;
  }
}
</script>

<style scoped>
</style>
