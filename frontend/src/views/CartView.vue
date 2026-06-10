<script setup>
import { onMounted, ref } from "vue";
import { RouterLink } from "vue-router";
import api from "../services/api";
import { auth, isAuthenticated } from "../stores/auth";
import AppNavbar from "../components/AppNavbar.vue";

const cart = ref(null);
const loading = ref(true);
const error = ref("");

const fetchCart = async () => {
    try {
        const response = await api.get(`/cart/${auth.userId}`);
        cart.value = response.data;
    } catch (err) {
        error.value = "Failed to load cart.";
        console.error(err);
    } finally {
        loading.value = false;
    }
};

const updateQuantity = async (item, quantity) => {
    if (quantity <= 0) return;

    try {
        await api.put(`/cart/items/${item.id}`, {
            quantity,
        });

        await fetchCart();
    } catch (err) {
        console.error(err);
    }
};

const removeItem = async (itemId) => {
    try {
        await api.delete(`/cart/items/${itemId}`);
        await fetchCart();
    } catch (err) {
        console.error(err);
    }
};

onMounted(fetchCart);
</script>

<template>
    <div class="min-h-screen bg-slate-100 text-slate-950">
        <AppNavbar />

        <main class="mx-auto max-w-7xl px-6 py-12 md:px-16">
            <h1 class="mb-8 text-4xl font-black">Your Cart</h1>

            <div v-if="loading" class="rounded-3xl bg-white p-10 text-center text-slate-500 shadow-sm">
                Loading cart...
            </div>

            <div v-else-if="error" class="rounded-3xl bg-white p-10 text-center font-semibold text-red-600 shadow-sm">
                {{ error }}
            </div>

            <div v-else-if="!cart || cart.items.length === 0" class="rounded-3xl bg-white p-10 text-center shadow-sm">
                <h2 class="text-2xl font-black">Your cart is empty</h2>
                <RouterLink to="/"
                    class="mt-6 inline-block rounded-2xl bg-slate-950 px-6 py-4 text-sm font-bold text-white transition hover:bg-blue-600">
                    Shop Now
                </RouterLink>
            </div>

            <section v-else class="grid gap-8 lg:grid-cols-3">
                <div class="space-y-4 lg:col-span-2">
                    <article v-for="item in cart.items" :key="item.id"
                        class="flex flex-col gap-5 rounded-3xl bg-white p-5 shadow-sm ring-1 ring-slate-200 md:flex-row">
                        <img :src="item.imageUrl" :alt="item.productName"
                            class="h-36 w-full rounded-2xl object-cover md:w-36" />

                        <div class="flex flex-1 flex-col justify-between">
                            <div>
                                <h2 class="text-xl font-black">
                                    {{ item.productName }}
                                </h2>

                                <p class="mt-2 text-slate-500">
                                    Price: ৳{{ item.price }}
                                </p>
                            </div>

                            <div class="mt-5 flex flex-wrap items-center justify-between gap-4">
                                <div class="flex items-center gap-3">
                                    <button @click="updateQuantity(item, item.quantity - 1)"
                                        class="rounded-xl bg-slate-100 px-4 py-2 font-black transition hover:bg-slate-200">
                                        -
                                    </button>

                                    <span class="font-bold">{{ item.quantity }}</span>

                                    <button @click="updateQuantity(item, item.quantity + 1)"
                                        class="rounded-xl bg-slate-100 px-4 py-2 font-black transition hover:bg-slate-200">
                                        +
                                    </button>
                                </div>

                                <div class="flex items-center gap-4">
                                    <strong class="text-lg font-black">
                                        ৳{{ item.subtotal }}
                                    </strong>

                                    <button @click="removeItem(item.id)"
                                        class="rounded-xl bg-red-50 px-4 py-2 text-sm font-bold text-red-600 transition hover:bg-red-100">
                                        Remove
                                    </button>
                                </div>
                            </div>
                        </div>
                    </article>
                </div>

                <aside class="h-fit rounded-3xl bg-white p-6 shadow-sm ring-1 ring-slate-200">
                    <h2 class="text-2xl font-black">Order Summary</h2>

                    <div class="mt-6 space-y-4 text-slate-600">
                        <div class="flex justify-between">
                            <span>Total Items</span>
                            <strong>{{ cart.items.length }}</strong>
                        </div>

                        <div class="flex justify-between border-t border-slate-200 pt-4 text-slate-950">
                            <span class="font-bold">Total Amount</span>
                            <strong class="text-2xl">৳{{ cart.totalAmount }}</strong>
                        </div>
                    </div>

                    <RouterLink to="/checkout"
                        class="mt-6 block w-full rounded-2xl bg-slate-950 px-6 py-4 text-center text-sm font-bold text-white transition hover:bg-blue-600">
                        Proceed to Checkout
                    </RouterLink>
                </aside>
            </section>
        </main>
    </div>
</template>