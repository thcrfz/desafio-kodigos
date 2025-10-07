<template>
    <nav class="navbar navbar-expand-lg navabar-light bg-light border bottom">
        <div class="container">
            <router-link class="navbar-brand" to="/os">OS Project</router-link>

            <button
                class="navbar-toggle"
                type="button"
                data-bs-toggle="collapse"
                data-bs-target="#mainNav"
                aria-controls="mainNav"
                aria-expanded="false"
                aria-label="Toggle Navigation"
            >
                <span class="navba-toggler-icon"></span>
            </button>

            <div id="#mainNav">
                <ul class="navbar-nav me-auto mb-2 mb-lg-0">
                    <li class="nav-item">
                        <router-link class="nav-link" to="/os">Ordens</router-link>
                    </li>
                    <li class="nav-item">
                        <router-link class="nav-link" to="/os/new">Nova OS</router-link>
                    </li>
                </ul>
                <div class="d-flex align-items-center gap-2">
                    <span class="text-muted small" v-if="user?.nome">Olá, {{ user.nome }}</span>
                    <button class="btn btn-outline-danger btn-sm" @click="logout">Sair</button>
                </div>
            </div>
        </div>
    </nav>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useRouter } from 'vue-router';
import { setToken } from "../api";
import { authUser, clearAuth } from '../auth';

const router = useRouter();

const user = computed(() => authUser.value);

console.log(user);


function logout() {
    clearAuth();
    router.push("/login");
}
</script>