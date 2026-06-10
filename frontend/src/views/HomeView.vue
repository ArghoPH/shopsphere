<script setup>
import { onMounted, ref } from "vue";
import { RouterLink } from "vue-router";
import api from "../services/api";
import { GUEST_USER_ID } from "../constants/user";

const products = ref([]);
const loading = ref(true);
const error = ref("");
const cartMessage = ref("");

const fetchProducts = async () => {
    try {
        const response = await api.get("/products");
        products.value = response.data;
    } catch (err) {
        error.value = "Failed to load products. Make sure backend is running.";
        console.error(err);
    } finally {
        loading.value = false;
    }
};

const addToCart = async (productId) => {
    try {
        await api.post("/cart/add", {
            userId: GUEST_USER_ID,
            productId: productId,
            quantity: 1,
        });

        cartMessage.value = "Product added to cart!";
    } catch (err) {
        cartMessage.value = "Failed to add product to cart.";
        console.error(err);
    }
};

onMounted(() => {
    fetchProducts();
});
</script>

<template>
    <div class="min-h-screen bg-slate-100 text-slate-950">
        <header class="bg-slate-950 px-6 py-6 text-white md:px-16">
            <nav class="mx-auto flex max-w-7xl items-center justify-between">
                <RouterLink to="/" class="text-2xl font-bold tracking-tight">
                    ShopSphere
                </RouterLink>

                <div class="hidden items-center gap-8 text-sm text-slate-300 md:flex">
                    <RouterLink to="/" class="transition hover:text-white">Home</RouterLink>
                    <a href="#" class="transition hover:text-white">Products</a>
                    <RouterLink to="/cart" class="transition hover:text-white">Cart</RouterLink>
                    <RouterLink to="/orders" class="transition hover:text-white">My Orders</RouterLink>
                    <RouterLink to="/admin/orders" class="transition hover:text-white">
                        Admin
                    </RouterLink>
                </div>
            </nav>

            <section class="mx-auto max-w-7xl py-20 md:py-28">
                <p class="mb-4 font-semibold text-blue-300">
                    ASP.NET Core + Vue + Supabase
                </p>

                <h1 class="max-w-3xl text-4xl font-black leading-tight md:text-6xl">
                    Build your modern e-commerce store
                </h1>

                <p class="mt-6 max-w-2xl text-base leading-8 text-slate-300 md:text-lg">
                    A fully functional free-tier friendly e-commerce project with product
                    listing, cart, checkout, Cash on Delivery and Mock Payment.
                </p>
            </section>
        </header>

        <main class="mx-auto -mt-10 max-w-7xl px-6 pb-16 md:px-16">
            <section class="rounded-3xl bg-white p-6 shadow-xl shadow-slate-200 md:p-8">
                <div class="flex flex-col gap-2 md:flex-row md:items-end md:justify-between">
                    <div>
                        <h2 class="text-3xl font-black tracking-tight">
                            Featured Products
                        </h2>
                        <p class="mt-2 text-slate-500">
                            Products loaded from Supabase PostgreSQL through ASP.NET Core API.
                        </p>
                    </div>
                </div>
            </section>

            <section class="mt-8">
                <div v-if="loading" class="rounded-3xl bg-white p-10 text-center text-slate-500 shadow-sm">
                    <div
                        class="mx-auto mb-4 h-10 w-10 animate-spin rounded-full border-4 border-slate-200 border-t-slate-950">
                    </div>
                    Loading products...
                </div>

                <div v-else-if="error"
                    class="rounded-3xl bg-white p-10 text-center font-semibold text-red-600 shadow-sm">
                    {{ error }}
                </div>

                <div v-if="cartMessage" class="mb-6 rounded-2xl bg-green-50 p-4 text-sm font-bold text-green-700">
                    {{ cartMessage }}
                </div>

                <div v-else class="grid grid-cols-1 gap-6 md:grid-cols-2 xl:grid-cols-3">
                    <article v-for="product in products" :key="product.id"
                        class="group overflow-hidden rounded-3xl bg-white shadow-sm ring-1 ring-slate-200 transition hover:-translate-y-1 hover:shadow-xl hover:shadow-slate-200">
                        <RouterLink :to="`/products/${product.id}`">
                            <div class="aspect-square overflow-hidden bg-slate-200">
                                <img :src="product.imageUrl" :alt="product.name"
                                    class="h-full w-full object-cover transition duration-300 group-hover:scale-105" />
                            </div>
                        </RouterLink>

                        <div class="p-6">
                            <div class="mb-3 flex items-center justify-between">
                                <span class="rounded-full bg-blue-50 px-3 py-1 text-xs font-bold text-blue-700">
                                    {{ product.categoryName }}
                                </span>

                                <span class="text-sm text-slate-500">
                                    Stock: {{ product.stock }}
                                </span>
                            </div>

                            <RouterLink :to="`/products/${product.id}`">
                                <h3 class="text-xl font-black text-slate-950 transition hover:text-blue-600">
                                    {{ product.name }}
                                </h3>
                            </RouterLink>

                            <p class="mt-3 min-h-14 text-sm leading-6 text-slate-500">
                                {{ product.description }}
                            </p>

                            <div class="mt-6 flex items-center justify-between">
                                <strong class="text-2xl font-black text-slate-950">
                                    ৳{{ product.price }}
                                </strong>
                            </div>

                            <RouterLink :to="`/products/${product.id}`"
                                class="mt-6 block w-full rounded-2xl bg-slate-950 px-5 py-4 text-center text-sm font-bold text-white transition hover:bg-blue-600">
                                View Details
                            </RouterLink>
                        </div>
                    </article>
                </div>
            </section>
        </main>
    </div>
</template>