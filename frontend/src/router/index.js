import { createRouter, createWebHistory } from "vue-router";
import HomeView from "../views/HomeView.vue";
import ProductDetailsView from "../views/ProductDetailsView.vue";

const routes = [
    {
        path: "/",
        name: "home",
        component: HomeView,
    },
    {
        path: "/products/:id",
        name: "product-details",
        component: ProductDetailsView,
    },
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

export default router;