import { createRouter, createWebHistory } from "vue-router";
import HomeView from "../views/HomeView.vue";
import ProductDetailsView from "../views/ProductDetailsView.vue";
import CartView from "../views/CartView.vue";

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
    {
        path: "/cart",
        name: "cart",
        component: CartView,
    },
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

export default router;