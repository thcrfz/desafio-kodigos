<script setup lang="ts">
import { useRoute } from 'vue-router';
import AppHeader from './components/AppHeader.vue';
import { computed } from 'vue';
import { authToken } from './auth';

const route = useRoute();
const publicPaths = ["/login", "/register"];

const showHeader = computed(() => {
  return !!authToken.value && !publicPaths.includes(route.path)
})
</script>

<template>
  <AppHeader v-if="showHeader"></AppHeader>
  <router-view />
</template>



<style scoped>
</style>
