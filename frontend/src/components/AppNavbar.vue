<script setup>
import { onBeforeUnmount, onMounted, ref } from "vue";
import { RouterLink, useRouter } from "vue-router";
import api from "../services/api";
import { auth, clearAuth, isAuthenticated } from "../stores/auth";

const router = useRouter();

const navRef = ref(null);
const categoryOpen = ref(false);
const manageOpen = ref(false);
const accountOpen = ref(false);
const categories = ref([]);

const isUser = () => auth.role === "User";
const isAdmin = () => auth.role === "Admin";
const isMasterAdmin = () => auth.role === "MasterAdmin";
const canManage = () => isAdmin() || isMasterAdmin();

const closeMenus = () => {
    categoryOpen.value = false;
    manageOpen.value = false;
    accountOpen.value = false;
};

const handleOutsideClick = (event) => {
    if (navRef.value && !navRef.value.contains(event.target)) {
        closeMenus();
    }
};

const fetchCategories = async () => {
    try {
        const response = await api.get("/categories");
        categories.value = response.data;
    } catch (err) {
        console.error("Failed to load categories", err);
    }
};

const logout = () => {
    clearAuth();
    closeMenus();
    router.push("/login");
};

onMounted(() => {
    document.addEventListener("click", handleOutsideClick);
    fetchCategories();
});

onBeforeUnmount(() => {
    document.removeEventListener("click", handleOutsideClick);
});
</script>

<template>
    <header class="sticky top-0 z-50 border-b border-white/10 bg-slate-950 text-white">
        <nav ref="navRef" class="mx-auto flex max-w-7xl items-center justify-between px-6 py-4 md:px-16">
            <RouterLink to="/" @click="closeMenus" class="text-xl font-black tracking-tight md:text-2xl">
                ShopSphere
            </RouterLink>

            <div class="flex flex-wrap items-center gap-2 text-xs font-bold md:gap-3">
                <RouterLink to="/" @click="closeMenus"
                    class="rounded-lg px-3 py-2 text-slate-300 transition hover:bg-white/10 hover:text-white">
                    Home
                </RouterLink>

                <RouterLink to="/products" @click="closeMenus"
                    class="rounded-lg px-3 py-2 text-slate-300 transition hover:bg-white/10 hover:text-white">
                    Products
                </RouterLink>

                <!-- Categories Dropdown -->
                <div class="relative">
                    <button type="button" @click.stop="
                        categoryOpen = !categoryOpen;
                    manageOpen = false;
                    accountOpen = false;
                    " class="rounded-lg bg-white/10 px-3 py-2 text-white transition hover:bg-white/20">
                        Categories
                    </button>

                    <Transition enter-active-class="transition duration-200 ease-out"
                        enter-from-class="opacity-0 scale-95 -translate-y-2"
                        enter-to-class="opacity-100 scale-100 translate-y-0"
                        leave-active-class="transition duration-150 ease-in"
                        leave-from-class="opacity-100 scale-100 translate-y-0"
                        leave-to-class="opacity-0 scale-95 -translate-y-2">
                        <div v-if="categoryOpen" @click.stop
                            class="absolute left-0 mt-3 w-60 origin-top-left overflow-hidden rounded-2xl bg-white p-2 text-slate-950 shadow-xl ring-1 ring-slate-200">
                            <RouterLink to="/products" @click="closeMenus"
                                class="block rounded-xl px-4 py-3 transition hover:bg-slate-100">
                                All Products
                            </RouterLink>

                            <RouterLink v-for="category in categories" :key="category.id"
                                :to="`/products?category=${category.slug}`" @click="closeMenus"
                                class="block rounded-xl px-4 py-3 transition hover:bg-slate-100">
                                {{ category.name }}
                            </RouterLink>
                        </div>
                    </Transition>
                </div>

                <RouterLink v-if="isAuthenticated() && isUser()" to="/cart" @click="closeMenus"
                    class="rounded-lg px-3 py-2 text-slate-300 transition hover:bg-white/10 hover:text-white">
                    Cart
                </RouterLink>

                <!-- <RouterLink v-if="isAuthenticated() && isUser()" to="/orders" @click="closeMenus"
                    class="rounded-lg px-3 py-2 text-slate-300 transition hover:bg-white/10 hover:text-white">
                    My Orders
                </RouterLink> -->

                <!-- Admin Manage Dropdown -->
                <div v-if="isAuthenticated() && canManage()" class="relative">
                    <button type="button" @click.stop="
                        manageOpen = !manageOpen;
                    categoryOpen = false;
                    accountOpen = false;
                    " class="rounded-lg bg-white/10 px-3 py-2 text-white transition hover:bg-white/20">
                        Manage
                    </button>

                    <Transition enter-active-class="transition duration-200 ease-out"
                        enter-from-class="opacity-0 scale-95 -translate-y-2"
                        enter-to-class="opacity-100 scale-100 translate-y-0"
                        leave-active-class="transition duration-150 ease-in"
                        leave-from-class="opacity-100 scale-100 translate-y-0"
                        leave-to-class="opacity-0 scale-95 -translate-y-2">
                        <div v-if="manageOpen" @click.stop
                            class="absolute right-0 mt-3 w-52 origin-top-right overflow-hidden rounded-2xl bg-white p-2 text-slate-950 shadow-xl ring-1 ring-slate-200">
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
                    </Transition>
                </div>

                <RouterLink v-if="isAuthenticated() && isMasterAdmin()" to="/master/users" @click="closeMenus"
                    class="rounded-lg bg-blue-600 px-3 py-2 text-white transition hover:bg-blue-700">
                    Master
                </RouterLink>

                <RouterLink v-if="!isAuthenticated()" to="/login" @click="closeMenus"
                    class="rounded-lg px-3 py-2 text-slate-300 transition hover:bg-white/10 hover:text-white">
                    Login
                </RouterLink>

                <RouterLink v-if="!isAuthenticated()" to="/register" @click="closeMenus"
                    class="rounded-lg bg-blue-600 px-3 py-2 text-white transition hover:bg-blue-700">
                    Register
                </RouterLink>

                <!-- Account Dropdown -->
                <div v-if="isAuthenticated()" class="relative">
                    <button type="button" @click.stop="
                        accountOpen = !accountOpen;
                    categoryOpen = false;
                    manageOpen = false;
                    "
                        class="flex items-center gap-2 rounded-lg bg-white/10 px-3 py-2 text-white transition hover:bg-white/20">
                        <span class="hidden max-w-32 truncate md:inline">
                            {{ auth.fullName }}
                        </span>
                        <span class="md:hidden">Account</span>
                    </button>

                    <Transition enter-active-class="transition duration-200 ease-out"
                        enter-from-class="opacity-0 scale-95 -translate-y-2"
                        enter-to-class="opacity-100 scale-100 translate-y-0"
                        leave-active-class="transition duration-150 ease-in"
                        leave-from-class="opacity-100 scale-100 translate-y-0"
                        leave-to-class="opacity-0 scale-95 -translate-y-2">
                        <div v-if="accountOpen" @click.stop
                            class="absolute right-0 mt-3 w-56 origin-top-right overflow-hidden rounded-2xl bg-white p-2 text-slate-950 shadow-xl ring-1 ring-slate-200">
                            <div class="border-b border-slate-100 px-4 py-3">
                                <p class="font-black">{{ auth.fullName }}</p>
                                <p class="mt-1 text-xs text-slate-500">{{ auth.role }}</p>
                            </div>

                            <RouterLink v-if="isUser()" to="/orders" @click="closeMenus"
                                class="block rounded-xl px-4 py-3 transition hover:bg-slate-100">
                                My Orders
                            </RouterLink>

                            <button type="button" @click="logout"
                                class="block w-full rounded-xl px-4 py-3 text-left font-bold text-red-600 transition hover:bg-red-50">
                                Logout
                            </button>
                        </div>
                    </Transition>
                </div>
            </div>
        </nav>
    </header>
</template>