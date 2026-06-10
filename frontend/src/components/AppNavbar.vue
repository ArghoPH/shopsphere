<script setup>
import { ref } from "vue";
import { RouterLink, useRouter } from "vue-router";
import { auth, clearAuth, isAuthenticated } from "../stores/auth";

const router = useRouter();

const manageOpen = ref(false);
const accountOpen = ref(false);

const isUser = () => auth.role === "User";
const isAdmin = () => auth.role === "Admin";
const isMasterAdmin = () => auth.role === "MasterAdmin";

const canManage = () => isAdmin() || isMasterAdmin();

const closeMenus = () => {
    manageOpen.value = false;
    accountOpen.value = false;
};

const goTo = (path) => {
    closeMenus();
    router.push(path);
};

const logout = () => {
    clearAuth();
    closeMenus();
    router.push("/login");
};
</script>

<template>
    <header class="sticky top-0 z-50 border-b border-white/10 bg-slate-950 text-white">
        <nav class="mx-auto flex max-w-7xl items-center justify-between px-6 py-4 md:px-16">
            <RouterLink to="/" @click="closeMenus" class="text-xl font-black tracking-tight md:text-2xl">
                ShopSphere
            </RouterLink>

            <div class="flex items-center gap-2 text-xs font-bold md:gap-3">
                <RouterLink to="/" @click="closeMenus"
                    class="rounded-lg px-3 py-2 text-slate-300 transition hover:bg-white/10 hover:text-white">
                    Home
                </RouterLink>

                <RouterLink to="/products" @click="closeMenus"
                    class="rounded-lg px-3 py-2 text-slate-300 transition hover:bg-white/10 hover:text-white">
                    Products
                </RouterLink>

                <RouterLink v-if="isAuthenticated() && isUser()" to="/cart" @click="closeMenus"
                    class="rounded-lg px-3 py-2 text-slate-300 transition hover:bg-white/10 hover:text-white">
                    Cart
                </RouterLink>

                <!-- <RouterLink v-if="isAuthenticated() && isUser()" to="/orders" @click="closeMenus"
                    class="rounded-lg px-3 py-2 text-slate-300 transition hover:bg-white/10 hover:text-white">
                    My Orders
                </RouterLink> -->

                <div v-if="isAuthenticated() && canManage()" class="relative">
                    <button @click="manageOpen = !manageOpen; accountOpen = false"
                        class="rounded-lg bg-white/10 px-3 py-2 text-white transition hover:bg-white/20">
                        Manage
                    </button>

                    <div v-if="manageOpen"
                        class="absolute right-0 mt-3 w-52 overflow-hidden rounded-2xl bg-white p-2 text-slate-950 shadow-xl ring-1 ring-slate-200">
                        <RouterLink to="/admin/orders" @click="closeMenus"
                            class="block rounded-xl px-4 py-3 transition hover:bg-slate-100">
                            Orders
                        </RouterLink>

                        <RouterLink to="/admin/products" @click="closeMenus"
                            class="block rounded-xl px-4 py-3 transition hover:bg-slate-100">
                            Products
                        </RouterLink>

                        <RouterLink to="/admin/categories" @click="closeMenus"
                            class="block rounded-xl px-4 py-3 transition hover:bg-slate-100">
                            Categories
                        </RouterLink>
                    </div>
                </div>

                <div v-if="isAuthenticated() && isMasterAdmin()" class="relative">
                    <button @click="accountOpen = false; manageOpen = false; router.push('/master/users')"
                        class="rounded-lg bg-blue-600 px-3 py-2 text-white transition hover:bg-blue-700">
                        Master
                    </button>
                </div>

                <RouterLink v-if="!isAuthenticated()" to="/login"
                    class="rounded-lg px-3 py-2 text-slate-300 transition hover:bg-white/10 hover:text-white">
                    Login
                </RouterLink>

                <RouterLink v-if="!isAuthenticated()" to="/register"
                    class="rounded-lg bg-blue-600 px-3 py-2 text-white transition hover:bg-blue-700">
                    Register
                </RouterLink>

                <div v-if="isAuthenticated()" class="relative">
                    <button @click="accountOpen = !accountOpen; manageOpen = false"
                        class="flex items-center gap-2 rounded-lg bg-white/10 px-3 py-2 text-white transition hover:bg-white/20">
                        <span class="hidden max-w-32 truncate md:inline">
                            {{ auth.fullName }}
                        </span>
                        <span class="md:hidden">Account</span>
                    </button>

                    <div v-if="accountOpen"
                        class="absolute right-0 mt-3 w-56 overflow-hidden rounded-2xl bg-white p-2 text-slate-950 shadow-xl ring-1 ring-slate-200">
                        <div class="border-b border-slate-100 px-4 py-3">
                            <p class="font-black mb-2">{{ auth.fullName }}</p>
                            <p class="mt-1 text-xs text-slate-500">{{ auth.role }}</p>
                        </div>
                        <div class="border-b border-slate-100 px-4 py-3">
                            <RouterLink v-if="isAuthenticated() && isUser()" to="/orders" @click="closeMenus"
                                class="rounded-lg text-black-300 transition hover:bg-white/10 hover:text-black mt-5">
                                My Orders
                            </RouterLink>
                        </div>
                        <button @click="logout"
                            class="block w-full rounded-xl px-4 py-3 text-left font-bold text-red-600 transition hover:bg-red-50 mt-5">
                            Logout
                        </button>
                    </div>
                </div>
            </div>
        </nav>
    </header>
</template>