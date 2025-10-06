<template>
  <div class="container py-4" v-if="os">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h1 class="h5 mb-0">OS #{{ os.id }} — {{ os.titulo }}</h1>
      <router-link to="/os" class="btn btn-outline-secondary">Voltar</router-link>
    </div>

    <div class="card shadow-sm mb-4">
      <div class="card-body">
        <h2 class="h6 text-uppercase text-muted mb-3">Checklist</h2>

        <form @submit.prevent="saveMarks">
          <div v-for="item in catalog" :key="item.id" class="mb-3">
            <label class="form-label mb-1">{{ item.titulo }}
              <span v-if="item.obrigatorio" class="text-danger">*</span>
            </label>

            <!-- Boolean -->
            <div v-if="item.tipo === 0" class="form-check">
              <input class="form-check-input" type="checkbox"
                     :id="`chk-${item.id}`"
                     v-model="marks[item.id].marcado" />
              <label class="form-check-label" :for="`chk-${item.id}`">Sim</label>
            </div>

            <!-- Número -->
            <input v-else-if="item.tipo === 1" type="number" class="form-control"
                   v-model.number="marks[item.id].valorNumero" placeholder="0" />

            <!-- Texto -->
            <input v-else-if="item.tipo === 2" type="text" class="form-control"
                   v-model="marks[item.id].valorTexto" placeholder="Digite aqui..." />

            <!-- Foto -->
            <div v-else class="border rounded p-2">
              <input type="file" class="form-control" accept="image/*"
                     @change="onSelectFile($event, item.id)" />
              <div class="row g-2 mt-2">
                <div class="col-6 col-sm-4 col-md-3" v-for="(f, i) in fotosPorItem[item.id] || []" :key="i">
                  <div class="ratio ratio-1x1 border rounded overflow-hidden">
                    <img :src="f.preview || f.url" class="w-100 h-100" style="object-fit:cover;" />
                  </div>
                </div>
              </div>
              <button v-if="pendingFiles[item.id]?.length"
                      class="btn btn-sm btn-primary mt-2"
                      type="button"
                      :disabled="uploading"
                      @click="uploadForItem(item.id)">
                <span v-if="uploading" class="spinner-border spinner-border-sm me-2"></span>
                Enviar fotos ({{ pendingFiles[item.id].length }})
              </button>
            </div>
          </div>

          <div class="d-flex gap-2 mt-4">
            <button class="btn btn-primary" type="submit" :disabled="saving">
              <span v-if="saving" class="spinner-border spinner-border-sm me-2"></span>
              Salvar marcações
            </button>
            <router-link to="/os" class="btn btn-light">Cancelar</router-link>
          </div>

          <div v-if="error" class="alert alert-danger mt-3 mb-0">{{ error }}</div>
          <div v-if="ok" class="alert alert-success mt-3 mb-0">Salvo!</div>
        </form>
      </div>
    </div>
  </div>

  <div v-else class="container py-5 text-center text-muted">Carregando...</div>

</template>
<script setup lang="ts">
import { onMounted, reactive, ref } from "vue";
import { useRoute } from "vue-router";
import { api } from "../../api";

const route = useRoute();
const osId = Number(route.params.id);

type CatalogItem = {
    id: number;
    titulo: string;
    tipo: 0|1|2|3;
    obrigatorio: boolean;
    ativo: boolean;
    ordem: number;
}


const os = ref<any>(null);
const catalog = ref<CatalogItem[]>([]);
const marks = reactive<Record<number, { 
    itemId: number; 
    marcado?: boolean|null; 
    valorNumero?:number|null;
    valorTexto?: string|null;
}>>({})
const fotosPorItem = reactive<Record<number, Array<{ url?: string, preview?: string }>>>({})
const pendingFiles = reactive<Record<number, File[]>>({})
const uploading = ref(false);
const saving = ref(false);
const error = ref("");
const ok = ref(false);


onMounted(async () => {
    error.value = "";
    try {
        const [catRes, osRes] = await Promise.all([
            api.get("/checklist"),
            api.get(`/os/${osId}`)
        ]);

        catalog.value = catRes.data as CatalogItem[];
        os.value = osRes.data;

        console.log(os);
        

        const existing = new Map<number, any>(
            (os.value.checks ?? []).map((c: any) => [c.checklistItemId, c])
        );

        for(const item of catalog.value) {
            const ex = existing.get(item.id);
            marks[item.id] = {
                itemId: item.id,
                marcado: ex?.marcado ?? null,
                valorNumero: ex?.valorNumero ?? null,
                valorTexto: ex?.valorTexto ?? null,
            }
        }

      const base = import.meta.env.VITE_API_BASE as string;

       for (const f of (os.value.fotos || [])) {
            console.log(f);
            
            const key = f.checklistItemId ?? -1;
            if (!fotosPorItem[key]) fotosPorItem[key] = [];
            const url = typeof f.path === "string" && f.path.startsWith("http")
            ? f.path
            : `${base}${f.path}`;

            fotosPorItem[key].push({ url });
        }
        console.log(fotosPorItem);
        
    } catch (e: any) {
        error.value = e?.response?.data ?? "Falha ao carrega os dados."
    }
});

async function saveMarks() {
    ok.value = false;
    error.value = "";
    saving.value = true;
    try {
        const payload = catalog.value.filter((i) => i.tipo !== 3)
            .map((i) => {
                const m = marks[i.id] || { itemId: i.id };
                return {
                    itemId: i.id,
                    marcado: i.tipo == 0 ? !!m.marcado : null,
                    valorNumero: i.tipo === 1 ? (m.valorNumero ?? null) : null,
                    valorTexto: i.tipo === 2 ? (m.valorTexto ?? "") : null
                }
            })
        await api.post(`/os/${osId}/checklist`, payload);
        ok.value = true;
    } catch(e: any) {
        error.value = e?.response?.data ?? "Falha ao salvar checklist";
    } finally {
        saving.value = false;
    }
}

function onSelectFile(ev: Event, itemId: number) {
  const list = (ev.target as HTMLInputElement).files;
  if (!list || !list.length) return;
  if (!pendingFiles[itemId]) pendingFiles[itemId] = [];
  if (!fotosPorItem[itemId]) fotosPorItem[itemId] = [];

  for (let i = 0; i < list.length; i++) {
    const f = list.item(i)!;
    pendingFiles[itemId].push(f);
    fotosPorItem[itemId].push({ preview: URL.createObjectURL(f) });
  }
  (ev.target as HTMLInputElement).value = "";
}

async function uploadForItem(itemId: number) {
  const queue = pendingFiles[itemId];
  if (!queue?.length) return;
  uploading.value = true;
  error.value = "";
  try {
    for (const f of queue) {
      const fd = new FormData();
      fd.append("file", f);
      await api.post(`/os/${osId}/foto?itemId=${itemId}`, fd, {
        headers: { "Content-Type": "multipart/form-data" },
      });
    
    }
    pendingFiles[itemId] = [];
  } catch (e: any) {
    error.value = e?.response?.data ?? "Falha ao enviar foto.";
  } finally {
    uploading.value = false;
  }
}

</script>