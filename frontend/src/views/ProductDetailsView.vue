<script setup>
import { onMounted, ref } from "vue";
import { RouterLink, useRoute } from "vue-router";
import api from "../services/api";
import { GUEST_USER_ID } from "../constants/user";

const route = useRoute();

const product = ref(null);
const loading = ref(true);
const error = ref("");

const cartMessage = ref("");

const addToCart = async () => {
    if (!product.value) return;

    try {
        await api.post("/cart/add", {
            userId: GUEST_USER_ID,
            productId: product.value.id,
            quantity: 1,
        });

        cartMessage.value = "Product added to cart!";
    } catch (err) {
        cartMessage.value = "Failed to add product to cart.";
        console.error(err);
    }
};

const fetchProduct = async () => {
    try {
        const response = await api.get(`/products/${route.params.id}`);
        product.value = response.data;
    } catch (err) {
        error.value = "Product not found or backend is not running.";
        console.error(err);
    } finally {
        loading.value = false;
    }
};

onMounted(() => {
    fetchProduct();
});
</script>

<template>
    <div class="min-h-screen bg-slate-100 text-slate-950">
        <header class="bg-slate-950 px-6 py-6 text-white md:px-16">
            <nav class="mx-auto flex max-w-7xl items-center justify-between">
                <RouterLink to="/" class="text-2xl font-bold tracking-tight">
                    ShopSphere
                </RouterLink>

                <RouterLink to="/"
                    class="rounded-xl bg-white/10 px-4 py-2 text-sm font-bold text-white transition hover:bg-white/20">
                    Back Home
                </RouterLink>
            </nav>
        </header>

        <main class="mx-auto max-w-7xl px-6 py-12 md:px-16">
            <div v-if="loading" class="rounded-3xl bg-white p-10 text-center text-slate-500 shadow-sm">
                Loading product...
            </div>

            <div v-else-if="error" class="rounded-3xl bg-white p-10 text-center font-semibold text-red-600 shadow-sm">
                {{ error }}
            </div>

            <section v-else
                class="grid gap-8 rounded-3xl bg-white p-6 shadow-xl shadow-slate-200 md:grid-cols-2 md:p-8">
                <div class="overflow-hidden rounded-3xl bg-slate-200">
                    <img :src="product.imageUrl" :alt="product.name" class="h-full w-full object-cover" />
                </div>

                <div class="flex flex-col justify-center">
                    <span class="mb-4 w-fit rounded-full bg-blue-50 px-4 py-2 text-sm font-bold text-blue-700">
                        {{ product.categoryName }}
                    </span>

                    <h1 class="text-4xl font-black tracking-tight md:text-5xl">
                        {{ product.name }}
                    </h1>

                    <p class="mt-5 text-base leading-8 text-slate-500 md:text-lg">
                        {{ product.description }}
                    </p>

                    <div class="mt-8 flex flex-wrap items-center gap-4">
                        <strong class="text-4xl font-black text-slate-950">
                            ৳{{ product.price }}
                        </strong>

                        <span class="rounded-xl bg-slate-100 px-4 py-2 text-sm font-semibold text-slate-600">
                            Stock: {{ product.stock }}
                        </span>
                    </div>

                    <div class="mt-8 grid gap-3 sm:grid-cols-2">

                        <button @click="addToCart"
                            class="rounded-2xl bg-slate-950 px-6 py-4 text-sm font-bold text-white transition hover:bg-blue-600">
                            Add to Cart
                        </button>

                        <button
                            class="rounded-2xl border border-slate-300 bg-white px-6 py-4 text-sm font-bold text-slate-950 transition hover:border-slate-950">
                            Buy Now
                        </button>
                        <div v-if="cartMessage"
                            class="mt-6 rounded-2xl bg-green-50 p-4 text-sm font-bold text-green-700">
                            {{ cartMessage }}
                        </div>

                    </div>
                </div>
            </section>
        </main>
    </div>
</template>