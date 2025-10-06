<template>
  <div class="container py-4">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h1 class="h4 mb-0">Nova Ordem de Serviço</h1>
      <router-link to="/os" class="btn btn-outline-secondary">Voltar</router-link>
    </div>

    <div class="card shadow-sm">
      <div class="card-body">
        <form @submit.prevent="submit" novalidate>
          <div class="mb-3">
            <label class="form-label" for="titulo">Título</label>
            <input
              id="titulo"
              v-model.trim="titulo"
              type="text"
              class="form-control"
              required
              placeholder="Ex.: Manutenção preventiva"
            />
          </div>

          <div class="mb-4">
            <label class="form-label" for="descricao">Descrição</label>
            <textarea
              id="descricao"
              v-model.trim="descricao"
              class="form-control"
              rows="4"
              placeholder="Detalhes da OS..."
            ></textarea>
          </div>

          <div class="d-flex gap-2">
            <button type="submit" class="btn btn-primary" :disabled="loading || !titulo">
              <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
              Criar OS
            </button>
            <router-link to="/os" class="btn btn-light">Cancelar</router-link>
          </div>

          <div v-if="error" class="alert alert-danger mt-3 mb-0 py-2">{{ error }}</div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import { api } from "../../api";

const router = useRouter();
const titulo = ref<string>("");
const descricao = ref<string>("");
const loading = ref(false);
const error = ref("");

async function submit() {
  error.value = "";
  if (!titulo.value) return;

  loading.value = true;
  try {
    const { data } = await api.post("/os", {
      titulo: titulo.value,
      descricao: descricao.value || null,
    });

    const newId =
      (data && (data.id ?? data?.os?.id)) ??
      null;

    if (newId) {
      router.push(`/os/${newId}`);
    } else {
      router.push("/os");
    }
  } catch (e: any) {
    error.value = e?.response?.data ?? "Falha ao criar OS";
  } finally {
    loading.value = false;
  }
}
</script>
