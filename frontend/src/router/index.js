import { createRouter, createWebHistory } from "vue-router";
import HomeView from "../views/HomeView.vue";
import ProductDetailsView from "../views/ProductDetailsView.vue";
import CartView from "../views/CartView.vue";
import CheckoutView from "../views/CheckoutView.vue";
import OrdersView from "../views/OrdersView.vue";
import AdminOrdersView from "../views/AdminOrdersView.vue";

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

    {
        path: "/checkout",
        name: "checkout",
        component: CheckoutView,
    },

    {
        path: "/orders",
        name: "orders",
        component: OrdersView,
    },
    {
        path: "/admin/orders",
        name: "admin-orders",
        component: AdminOrdersView,
    },

];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

export default router;