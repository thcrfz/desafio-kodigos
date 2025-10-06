import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap"
import { createApp } from 'vue'
import App from './App.vue'
import router from "./router";
import { setToken } from "./api"; 

const saved = localStorage.getItem("auth_token");
if (saved) setToken(saved);

createApp(App).use(router).mount('#app')
