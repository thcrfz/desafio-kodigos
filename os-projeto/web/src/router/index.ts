import { createRouter, createWebHistory } from "vue-router";
const LoginPage = () => import("../pages/login/LoginPage.vue");
const RegisterPage = () => import("../pages/login/RegisterPage.vue");
const OsListPage = () => import("../pages/os/OsListPage.vue");
const OsCreatePage = () => import("../pages/os/OsCreatePage.vue");
const OsEditPage = () => import("../pages/os/OsEditPage.vue");
const ChecklistPage = () => import("../pages/checklist/ChecklistPage.vue");

const routes = [
  { path: "/login", component: LoginPage },
  { path: "/register", component: RegisterPage },
  { path: "/os", component: OsListPage, meta: { auth: true } },
  { path: "/os/new", component: OsCreatePage, meta: { auth: true } },
  { path: "/os/:id", component: OsEditPage, meta: { auth: true } },
  { path: "/checklists", component: ChecklistPage, meta: { auth: true } },
  { path: "/", redirect: "/login" },
  { path: "/:pathMatch(.*)*", redirect: "/login" },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to, _from, next) => {
  const token = localStorage.getItem("auth_token");

  if (to.meta?.auth && !token) return next("/login");

  if ((to.path === "/login" || to.path === "/register") && token) {
    return next("/os");
  }

  next();
});

export default router;
