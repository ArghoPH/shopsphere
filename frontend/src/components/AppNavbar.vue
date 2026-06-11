<script setup>
import { computed, onBeforeUnmount, onMounted, ref, watch } from "vue";
import { RouterLink, useRoute, useRouter } from "vue-router";
import api from "../services/api";
import { auth, clearAuth, isAuthenticated } from "../stores/auth";
import { cartCount, fetchCartCount, resetCartCount } from "../stores/cartCounter";

const router = useRouter();
const route = useRoute();

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
    resetCartCount();
    closeMenus();
    router.push("/login");
};

const isActivePath = (path) => {
    if (path === "/") {
        return route.path === "/";
    }

    return route.path.startsWith(path);
};

const isAllProductsActive = computed(() => {
    return route.path === "/products" && !route.query.category;
});

const isCategoryMenuActive = computed(() => {
    return route.path === "/products" && !!route.query.category;
});

const isActiveCategory = (slug) => {
    return route.path === "/products" && route.query.category === slug;
};

const isAdminActive = computed(() => {
    return route.path.startsWith("/admin") || route.path.startsWith("/master");
});

onMounted(() => {
    document.addEventListener("click", handleOutsideClick);
    fetchCategories();
    fetchCartCount();
});

onBeforeUnmount(() => {
    document.removeEventListener("click", handleOutsideClick);
});

watch(
    () => [auth.token, auth.userId, auth.role],
    () => {
        fetchCartCount();
    }
);
</script>

<template>
    <header class="sticky top-0 z-50 overflow-visible border-b border-white/10 bg-slate-950 text-white">
        <nav ref="navRef"
            class="relative mx-auto flex max-w-7xl flex-nowrap items-center gap-4 overflow-visible whitespace-nowrap px-6 py-4 md:px-16">
            <!-- Brand -->
            <div class="flex shrink-0 items-center">
                <RouterLink to="/" @click="closeMenus"
                    class="inline-flex items-center gap-3 text-xl font-black tracking-tight text-white transition hover:text-blue-400 md:text-2xl">
                    <span>ShopSphere</span>
                </RouterLink>
            </div>

            <!-- Middle Links -->
            <div class="flex min-w-0 flex-1 flex-nowrap items-center justify-center gap-2 text-xs font-bold md:gap-3">
                <RouterLink to="/" @click="closeMenus" class="rounded-full px-4 py-2 text-sm font-bold transition"
                    :class="isActivePath('/')
                        ? 'bg-white text-slate-950 shadow-sm'
                        : 'text-slate-300 hover:bg-white/10 hover:text-white'
                        ">
                    <i class="fa-solid fa-house mr-2"></i>
                    Home
                </RouterLink>

                <RouterLink to="/products" @click="closeMenus"
                    class="rounded-full px-4 py-2 text-sm font-bold transition" :class="isActivePath('/products') && !route.query.category
                        ? 'bg-white text-slate-950 shadow-sm'
                        : 'text-slate-300 hover:bg-white/10 hover:text-white'
                        ">
                    <i class="fa-solid fa-bag-shopping mr-2"></i>
                    Products
                </RouterLink>

                <div class="relative">
                    <button type="button" @click.stop="
                        categoryOpen = !categoryOpen;
                    manageOpen = false;
                    accountOpen = false;
                    " class="rounded-full px-4 py-2 text-sm font-bold transition" :class="isCategoryMenuActive
                        ? 'bg-white text-slate-950 shadow-sm'
                        : 'text-slate-300 hover:bg-white/10 hover:text-white'
                        ">
                        <i class="fa-solid fa-layer-group mr-2"></i>
                        Categories
                        <i class="fa-solid fa-chevron-down ml-2 text-xs"></i>
                    </button>

                    <Transition enter-active-class="transition duration-200 ease-out"
                        enter-from-class="opacity-0 scale-95 -translate-y-2"
                        enter-to-class="opacity-100 scale-100 translate-y-0"
                        leave-active-class="transition duration-150 ease-in"
                        leave-from-class="opacity-100 scale-100 translate-y-0"
                        leave-to-class="opacity-0 scale-95 -translate-y-2">
                        <div v-if="categoryOpen" @click.stop
                            class="absolute left-1/2 z-[9999] mt-3 w-60 -translate-x-1/2 origin-top overflow-hidden rounded-2xl bg-white p-2 text-slate-950 shadow-xl ring-1 ring-slate-200">
                            <RouterLink to="/products" @click="closeMenus"
                                class="block rounded-xl px-4 py-3 text-sm font-bold transition" :class="isAllProductsActive
                                    ? 'bg-slate-950 text-white'
                                    : 'text-slate-700 hover:bg-slate-100 hover:text-slate-950'
                                    ">
                                All Products
                            </RouterLink>

                            <RouterLink v-for="category in categories" :key="category.id"
                                :to="{ path: '/products', query: { category: category.slug } }" @click="closeMenus"
                                class="block rounded-xl px-4 py-3 text-sm font-bold transition" :class="isActiveCategory(category.slug)
                                    ? 'bg-blue-600 text-white'
                                    : 'text-slate-700 hover:bg-slate-100 hover:text-slate-950'
                                    ">
                                {{ category.name }}
                            </RouterLink>
                        </div>
                    </Transition>
                </div>

                <RouterLink to="/support" @click="closeMenus"
                    class="rounded-full px-4 py-2 text-sm font-bold transition" :class="isActivePath('/support')
                        ? 'bg-white text-slate-950 shadow-sm'
                        : 'text-slate-300 hover:bg-white/10 hover:text-white'
                        ">
                    <i class="fa-solid fa-headset mr-2"></i>
                    Support
                </RouterLink>
            </div>

            <!-- Right Links -->
            <div class="flex shrink-0 flex-nowrap items-center justify-end gap-2 text-xs font-bold md:gap-3">
                <!-- Manage -->
                <div v-if="isAuthenticated() && canManage()" class="relative">
                    <button type="button" @click.stop="
                        manageOpen = !manageOpen;
                    categoryOpen = false;
                    accountOpen = false;
                    " class="rounded-full px-4 py-2 text-sm font-bold transition" :class="isAdminActive
                        ? 'bg-white text-slate-950 shadow-sm'
                        : 'text-slate-300 hover:bg-white/10 hover:text-white'
                        ">
                        <i class="fa-solid fa-gear mr-2"></i>
                        Manage
                        <i class="fa-solid fa-chevron-down ml-2 text-xs"></i>
                    </button>

                    <Transition enter-active-class="transition duration-200 ease-out"
                        enter-from-class="opacity-0 scale-95 -translate-y-2"
                        enter-to-class="opacity-100 scale-100 translate-y-0"
                        leave-active-class="transition duration-150 ease-in"
                        leave-from-class="opacity-100 scale-100 translate-y-0"
                        leave-to-class="opacity-0 scale-95 -translate-y-2">
                        <div v-if="manageOpen" @click.stop
                            class="absolute right-0 z-[9999] mt-3 w-52 origin-top-right overflow-hidden rounded-2xl bg-white p-2 text-slate-950 shadow-xl ring-1 ring-slate-200">
                            <RouterLink to="/admin/orders" @click="closeMenus"
                                class="block rounded-xl px-4 py-3 text-sm font-bold transition" :class="isActivePath('/admin/orders')
                                    ? 'bg-slate-950 text-white'
                                    : 'text-slate-700 hover:bg-slate-100 hover:text-slate-950'
                                    ">
                                Orders
                            </RouterLink>

                            <RouterLink to="/admin/products" @click="closeMenus"
                                class="block rounded-xl px-4 py-3 text-sm font-bold transition" :class="isActivePath('/admin/products')
                                    ? 'bg-slate-950 text-white'
                                    : 'text-slate-700 hover:bg-slate-100 hover:text-slate-950'
                                    ">
                                Products
                            </RouterLink>

                            <RouterLink to="/admin/categories" @click="closeMenus"
                                class="block rounded-xl px-4 py-3 text-sm font-bold transition" :class="isActivePath('/admin/categories')
                                    ? 'bg-slate-950 text-white'
                                    : 'text-slate-700 hover:bg-slate-100 hover:text-slate-950'
                                    ">
                                Categories
                            </RouterLink>

                            <RouterLink v-if="isMasterAdmin()" to="/master/users" @click="closeMenus"
                                class="block rounded-xl px-4 py-3 text-sm font-bold transition" :class="isActivePath('/master/users')
                                    ? 'bg-slate-950 text-white'
                                    : 'text-slate-700 hover:bg-slate-100 hover:text-slate-950'
                                    ">
                                Users
                            </RouterLink>
                        </div>
                    </Transition>
                </div>

                <!-- Guest -->
                <RouterLink v-if="!isAuthenticated()" to="/login" @click="closeMenus"
                    class="rounded-full px-4 py-2 text-sm font-bold transition" :class="isActivePath('/login')
                        ? 'bg-white text-slate-950 shadow-sm'
                        : 'text-slate-300 hover:bg-white/10 hover:text-white'
                        ">
                    Login
                </RouterLink>

                <RouterLink v-if="!isAuthenticated()" to="/register" @click="closeMenus"
                    class="rounded-full bg-blue-600 px-4 py-2 text-sm font-bold text-white transition hover:bg-blue-700"
                    :class="isActivePath('/register') ? 'ring-2 ring-white' : ''">
                    Register
                </RouterLink>

                <!-- Account -->
                <div v-if="isAuthenticated()" class="relative">
                    <button type="button" @click.stop="
                        accountOpen = !accountOpen;
                    categoryOpen = false;
                    manageOpen = false;
                    " class="flex items-center gap-2 rounded-full bg-white/10 px-4 py-2 text-sm font-bold text-white transition hover:bg-white/20"
                        :class="isActivePath('/orders') ? 'ring-2 ring-white' : ''">
                        <span class="hidden max-w-24 truncate lg:inline">
                            {{ auth.fullName }}
                        </span>
                        <span class="lg:hidden">Account</span>
                        <i class="fa-solid fa-chevron-down text-xs"></i>
                    </button>

                    <Transition enter-active-class="transition duration-200 ease-out"
                        enter-from-class="opacity-0 scale-95 -translate-y-2"
                        enter-to-class="opacity-100 scale-100 translate-y-0"
                        leave-active-class="transition duration-150 ease-in"
                        leave-from-class="opacity-100 scale-100 translate-y-0"
                        leave-to-class="opacity-0 scale-95 -translate-y-2">
                        <div v-if="accountOpen" @click.stop
                            class="absolute right-0 z-[9999] mt-3 w-56 origin-top-right overflow-hidden rounded-2xl bg-white p-2 text-slate-950 shadow-xl ring-1 ring-slate-200">
                            <div class="border-b border-slate-100 px-4 py-3">
                                <p class="font-black">{{ auth.fullName }}</p>
                                <p class="mt-1 text-xs text-slate-500">{{ auth.role }}</p>
                            </div>

                            <RouterLink v-if="isUser()" to="/orders" @click="closeMenus"
                                class="block rounded-xl px-4 py-3 text-sm font-bold transition" :class="isActivePath('/orders')
                                    ? 'bg-slate-950 text-white'
                                    : 'text-slate-700 hover:bg-slate-100 hover:text-slate-950'
                                    ">
                                My Orders
                            </RouterLink>

                            <button type="button" @click="logout"
                                class="block w-full rounded-xl px-4 py-3 text-left text-sm font-bold text-red-600 transition hover:bg-red-50">
                                Logout
                            </button>
                        </div>
                    </Transition>
                </div>

                <!-- Cart -->
                <RouterLink v-if="isAuthenticated() && isUser()" to="/cart" @click="closeMenus"
                    class="relative ml-2 flex items-center justify-center rounded-full p-2.5 transition hover:scale-105"
                    :class="isActivePath('/cart')
                        ? 'bg-white text-slate-950 shadow-sm'
                        : 'bg-white/10 text-white hover:bg-blue-600'
                        " title="View Cart">
                    <i class="fa-solid fa-cart-shopping text-base"></i>

                    <span v-if="cartCount > 0"
                        class="absolute -right-1.5 -top-1.5 flex h-5 min-w-5 items-center justify-center rounded-full bg-red-600 px-1 text-[10px] font-black leading-none text-white ring-2 ring-slate-950">
                        {{ cartCount > 99 ? "99+" : cartCount }}
                    </span>
                </RouterLink>
            </div>
        </nav>
    </header>
</template>