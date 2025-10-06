<template>
  <div class="container min-vh-100 d-flex align-items-center py-4">
    <div class="row w-100 justify-content-center">
      <div class="col-12 col-sm-10 col-md-6 col-lg-4">
        <div class="card shadow-sm">
          <div class="card-body p-4">
            <h1 class="h4 mb-4 text-center">Criar conta</h1>

            <form @submit.prevent="submit" novalidate>
              <div class="mb-3">
                <label class="form-label" for="nome">Nome</label>
                <input
                  id="nome"
                  v-model="nome"
                  type="text"
                  class="form-control"
                  required
                  placeholder="Seu nome"
                />
              </div>

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

              <div class="mb-3">
                <label class="form-label" for="password">Senha</label>
                <input
                  id="password"
                  v-model="password"
                  type="password"
                  class="form-control"
                  required
                  minlength="6"
                  autocomplete="new-password"
                  placeholder="••••••••"
                />
              </div>

              <div class="mb-4">
                <label class="form-label" for="confirm">Confirmar senha</label>
                <input
                  id="confirm"
                  v-model="confirm"
                  type="password"
                  class="form-control"
                  required
                  minlength="6"
                  autocomplete="new-password"
                  placeholder="••••••••"
                />
              </div>

              <button
                type="submit"
                class="btn btn-primary w-100 d-flex justify-content-center align-items-center"
                :disabled="loading"
              >
                <span v-if="loading" class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                {{ loading ? 'Criando...' : 'Registrar' }}
              </button>

              <div v-if="error" class="alert alert-danger mt-3 mb-0 py-2" role="alert">
                {{ error }}
              </div>
              <div v-if="success" class="alert alert-success mt-3 mb-0 py-2" role="alert">
                Conta criada! Faça login.
              </div>

              <div class="d-flex justify-content-between align-items-center mt-3">
                <router-link to="/login" class="small text-decoration-none">Já tem conta? Entrar</router-link>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>


<script setup lang="ts">
import { ref } from 'vue';
import { api } from '../../api';
import { useRouter } from 'vue-router';

const router = useRouter();
const nome = ref('');
const email = ref('');
const password = ref('');
const confirm = ref('');
const loading = ref(false);
const error = ref('');
const success = ref(false);

async function submit() {
  error.value = '';
  success.value = false;

  if (password.value !== confirm.value) {
    error.value = 'As senhas não conferem.';
    return;
  }

  loading.value = true;
  try {
    await api.post('/auth/register', {
      nome: nome.value,
      email: email.value,
      password: password.value,
    });
    success.value = true;
    setTimeout(() => router.push('/login'), 800);
  } catch (e: any) {
    error.value = e?.response?.data ?? 'Falha ao registrar';
  } finally {
    loading.value = false;
  }
}
</script>

<style scoped>
</style>
