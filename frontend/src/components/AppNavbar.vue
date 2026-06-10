<script setup>
import { RouterLink, useRouter } from "vue-router";
import { auth, clearAuth, isAuthenticated } from "../stores/auth";

const router = useRouter();

const logout = () => {
    clearAuth();
    router.push("/login");
};

const isAdmin = () => {
    return auth.role === "Admin" || auth.role === "MasterAdmin";
};

const isMasterAdmin = () => {
    return auth.role === "MasterAdmin";
};
</script>

<template>
    <header class="bg-slate-950 px-6 py-6 text-white md:px-16">
        <nav class="mx-auto flex max-w-7xl items-center justify-between">
            <RouterLink to="/" class="text-2xl font-bold tracking-tight">
                ShopSphere
            </RouterLink>

            <div class="flex flex-wrap items-center gap-4 text-sm font-bold text-slate-300">
                <RouterLink to="/" class="transition hover:text-white">
                    Home
                </RouterLink>

                <RouterLink to="/products" class="transition hover:text-white">
                    Products
                </RouterLink>

                <RouterLink v-if="isAuthenticated()" to="/cart" class="transition hover:text-white">
                    Cart
                </RouterLink>

                <RouterLink v-if="isAuthenticated()" to="/orders" class="transition hover:text-white">
                    My Orders
                </RouterLink>

                <RouterLink v-if="isAdmin()" to="/admin/orders" class="transition hover:text-white">
                    Admin Orders
                </RouterLink>

                <RouterLink v-if="isAdmin()" to="/admin/products" class="transition hover:text-white">
                    Admin Products
                </RouterLink>

                <RouterLink v-if="isMasterAdmin()" to="/master/users" class="transition hover:text-white">
                    Master Users
                </RouterLink>

                <RouterLink v-if="!isAuthenticated()" to="/login" class="transition hover:text-white">
                    Login
                </RouterLink>

                <RouterLink v-if="!isAuthenticated()" to="/register" class="transition hover:text-white">
                    Register
                </RouterLink>

                <span v-if="isAuthenticated()" class="rounded-xl bg-white/10 px-3 py-2 text-white">
                    {{ auth.fullName }}
                </span>

                <button v-if="isAuthenticated()" @click="logout"
                    class="rounded-xl bg-red-500 px-4 py-2 text-white transition hover:bg-red-600">
                    Logout
                </button>
            </div>
        </nav>
    </header>
</template>