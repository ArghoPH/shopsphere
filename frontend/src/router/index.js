import { createRouter, createWebHistory } from "vue-router";

import HomeView from "../views/HomeView.vue";
import ProductDetailsView from "../views/ProductDetailsView.vue";
import CartView from "../views/CartView.vue";
import CheckoutView from "../views/CheckoutView.vue";
import OrdersView from "../views/OrdersView.vue";
import AdminOrdersView from "../views/AdminOrdersView.vue";
import AdminProductsView from "../views/AdminProductsView.vue";
import LoginView from "../views/LoginView.vue";
import RegisterView from "../views/RegisterView.vue";
import MasterUsersView from "../views/MasterUsersView.vue";
import AdminCategoriesView from "../views/AdminCategoriesView.vue";

import { isAuthenticated, hasRole } from "../stores/auth";

const router = createRouter({
    history: createWebHistory(),
    routes: [
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
            path: "/login",
            name: "login",
            component: LoginView,
        },
        {
            path: "/register",
            name: "register",
            component: RegisterView,
        },
        {
            path: "/cart",
            name: "cart",
            component: CartView,
            meta: {
                requiresAuth: true,
            },
        },
        {
            path: "/checkout",
            name: "checkout",
            component: CheckoutView,
            meta: {
                requiresAuth: true,
            },
        },
        {
            path: "/orders",
            name: "orders",
            component: OrdersView,
            meta: {
                requiresAuth: true,
            },
        },
        {
            path: "/admin/orders",
            name: "admin-orders",
            component: AdminOrdersView,
            meta: {
                requiresAuth: true,
                roles: ["Admin", "MasterAdmin"],
            },
        },
        {
            path: "/admin/products",
            name: "admin-products",
            component: AdminProductsView,
            meta: {
                requiresAuth: true,
                roles: ["Admin", "MasterAdmin"],
            },
        },
        {
            path: "/master/users",
            name: "master-users",
            component: MasterUsersView,
            meta: {
                requiresAuth: true,
                roles: ["MasterAdmin"],
            },
        },
        {
            path: "/products",
            name: "products",
            component: HomeView,
        },
        {
            path: "/admin/categories",
            name: "admin-categories",
            component: AdminCategoriesView,
            meta: {
                requiresAuth: true,
                roles: ["Admin", "MasterAdmin"],
            },
        },
    ],
});

router.beforeEach((to) => {
    if (to.meta.requiresAuth && !isAuthenticated()) {
        return "/login";
    }

    if (to.meta.roles && !hasRole(to.meta.roles)) {
        return "/";
    }
});

export default router;