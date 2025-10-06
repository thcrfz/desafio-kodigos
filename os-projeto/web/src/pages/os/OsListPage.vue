<template>
  <div class="container py-4">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h1 class="h4 mb-0">Ordens de Serviço</h1>
      <router-link to="/os/new" class="btn btn-primary">
        Nova OS
      </router-link>
    </div>

    <div class="card shadow-sm">
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
              <tr>
                <th style="width:80px;">ID</th>
                <th>Título</th>
                <th style="width:140px;">Status</th>
                <th style="width:180px;">Técnico</th>
                <th style="width:200px;">Atualizado</th>
                <th style="width:110px;" class="text-end">Ações</th>
              </tr>
            </thead>

            <tbody v-if="items && items.length">
              <tr v-for="item in items" :key="item.id">
                <td>{{ item.id }}</td>
                <td class="text-truncate" style="max-width: 320px;">{{ item.titulo }}</td>
                <td>
                  <span class="badge bg-secondary" v-if="item.status===0">Aberta</span>
                  <span class="badge bg-primary" v-else-if="item.status===1">Em execução</span>
                  <span class="badge bg-success" v-else-if="item.status===2">Fechada</span>
                  <span class="badge bg-light text-dark" v-else>—</span>
                </td>
                <td>{{ item.tecnico?.nome ?? '—' }}</td>
                <td>{{ item.updatedAt }}</td>
                <td class="text-end">
                  <router-link class="btn btn-sm btn-outline-primary" :to="`/os/${item.id}`">
                    Abrir
                  </router-link>
                </td>
              </tr>
            </tbody>

            <tbody v-else>
              <tr>
                <td colspan="6" class="text-center py-4 text-muted">
                  Nenhuma OS encontrada.
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
    import { ref, onMounted } from "vue";
    import { api } from "../../api";
    import { OsListItem } from '../../models';
    const items = ref<OsListItem[]>([]);

    onMounted(async () => {
        const { data } = await api.get("/os");
        items.value = data.items as OsListItem[];
    });
</script>