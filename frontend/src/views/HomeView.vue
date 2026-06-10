<script setup>
import { RouterLink } from "vue-router";
import AppNavbar from "../components/AppNavbar.vue";
import { auth, isAuthenticated } from "../stores/auth";

const isAdmin = () => {
    return auth.role === "Admin" || auth.role === "MasterAdmin";
};
</script>

<template>
    <div class="min-h-screen bg-slate-100 text-slate-950">
        <AppNavbar />

        <main>
            <section class="mx-auto max-w-7xl px-6 py-16 md:px-16">
                <div class="rounded-[2rem] bg-slate-950 px-8 py-16 text-white shadow-sm md:px-14">
                    <p class="text-sm font-bold uppercase tracking-[0.3em] text-blue-300">
                        Welcome to ShopSphere
                    </p>

                    <h1 class="mt-5 max-w-3xl text-5xl font-black leading-tight md:text-7xl">
                        Modern shopping experience built with ASP.NET Core and Vue.
                    </h1>

                    <p class="mt-6 max-w-2xl text-lg leading-8 text-slate-300">
                        Browse products, manage cart, place cash-on-delivery orders, and track your order history from
                        one clean platform.
                    </p>

                    <div class="mt-8 flex flex-wrap gap-4">
                        <RouterLink to="/products"
                            class="rounded-2xl bg-blue-600 px-6 py-4 text-sm font-bold text-white transition hover:bg-blue-700">
                            Shop Products
                        </RouterLink>

                        <RouterLink v-if="!isAuthenticated()" to="/login"
                            class="rounded-2xl bg-white px-6 py-4 text-sm font-bold text-slate-950 transition hover:bg-slate-200">
                            Login
                        </RouterLink>

                        <RouterLink v-if="isAuthenticated() && !isAdmin()" to="/orders"
                            class="rounded-2xl bg-white/10 px-6 py-4 text-sm font-bold text-white transition hover:bg-white/20">
                            My Orders
                        </RouterLink>

                        <RouterLink v-if="isAuthenticated() && isAdmin()" to="/admin/orders"
                            class="rounded-2xl bg-white/10 px-6 py-4 text-sm font-bold text-white transition hover:bg-white/20">
                            Manage Store
                        </RouterLink>
                    </div>
                </div>

                <section class="mt-10 grid gap-5 md:grid-cols-3">
                    <div class="rounded-3xl bg-white p-6 shadow-sm ring-1 ring-slate-200">
                        <h2 class="text-xl font-black">For Customers</h2>
                        <p class="mt-3 text-sm leading-6 text-slate-500">
                            Login, add products to cart, checkout with Cash on Delivery, and track order history.
                        </p>
                    </div>

                    <div class="rounded-3xl bg-white p-6 shadow-sm ring-1 ring-slate-200">
                        <h2 class="text-xl font-black">For Admin</h2>
                        <p class="mt-3 text-sm leading-6 text-slate-500">
                            Manage orders, products, stock, price, categories, and product status.
                        </p>
                    </div>

                    <div class="rounded-3xl bg-white p-6 shadow-sm ring-1 ring-slate-200">
                        <h2 class="text-xl font-black">For Master Admin</h2>
                        <p class="mt-3 text-sm leading-6 text-slate-500">
                            Create and manage Admin/User logins with role-based access control.
                        </p>
                    </div>
                </section>
            </section>
        </main>
    </div>
</template>