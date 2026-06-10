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
        error.value = "Failed to load shopping cart items.";
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
    <div class="min-h-screen bg-slate-50 text-slate-900 font-sans">
        <AppNavbar />

        <main class="mx-auto max-w-7xl px-4 py-8 md:px-8">

            <div class="mb-8 flex items-center gap-3">
                <div
                    class="flex h-10 w-10 items-center justify-center rounded-xl bg-blue-600 text-white shadow-md shadow-blue-100">
                    <i class="fas fa-shopping-basket"></i>
                </div>
                <div>
                    <h1 class="text-2xl font-black tracking-tight text-slate-800 md:text-3xl">Your Shopping Cart</h1>
                    <p v-if="cart && cart.items.length" class="text-xs font-semibold text-slate-400 uppercase mt-0.5">
                        Review your selected premium retail items
                    </p>
                </div>
            </div>

            <div v-if="loading" class="animate-pulse space-y-4 max-w-4xl">
                <div v-for="n in 2" :key="n" class="rounded-3xl bg-white border border-slate-100 p-5 flex gap-5">
                    <div class="h-28 w-28 rounded-2xl bg-slate-200 shrink-0"></div>
                    <div class="flex-1 space-y-3 py-2">
                        <div class="h-5 w-1/3 rounded bg-slate-200"></div>
                        <div class="h-4 w-1/4 rounded bg-slate-200"></div>
                        <div class="h-8 w-24 rounded bg-slate-200"></div>
                    </div>
                </div>
            </div>

            <div v-else-if="error"
                class="rounded-3xl bg-white p-12 text-center shadow-sm border border-slate-200 max-w-xl mx-auto my-12">
                <div
                    class="mx-auto flex h-16 w-16 items-center justify-center rounded-full bg-rose-50 text-rose-500 text-2xl mb-4">
                    <i class="fas fa-exclamation-circle"></i>
                </div>
                <h2 class="text-xl font-bold text-slate-800">Connection Interrupted</h2>
                <p class="mt-2 text-sm text-slate-500">{{ error }}</p>
            </div>

            <div v-else-if="!cart || cart.items.length === 0"
                class="rounded-3xl bg-white p-12 text-center shadow-sm border border-slate-200/80 max-w-xl mx-auto my-12">
                <div
                    class="mx-auto flex h-20 w-20 items-center justify-center rounded-full bg-slate-100 text-slate-400 text-3xl mb-5">
                    <i class="fas fa-shopping-cart"></i>
                </div>
                <h2 class="text-xl font-bold text-slate-800">Your cart is currently empty</h2>
                <p class="mt-2 text-sm text-slate-500 max-w-sm mx-auto leading-relaxed">
                    Looks like you haven't added anything to your cart yet. Explore our marketplace categories to find
                    hot deals.
                </p>
                <RouterLink to="/products"
                    class="mt-6 inline-flex items-center gap-2 rounded-xl bg-slate-900 px-6 py-3.5 text-sm font-bold text-white transition hover:bg-blue-600 shadow-md">
                    <i class="fas fa-store text-xs"></i> Start Shopping
                </RouterLink>
            </div>

            <section v-else class="grid gap-8 lg:grid-cols-3 items-start">

                <div class="space-y-4 lg:col-span-2">
                    <article v-for="item in cart.items" :key="item.id"
                        class="group flex flex-col gap-5 rounded-3xl bg-white p-4 border border-slate-200/60 shadow-sm transition hover:shadow-md md:flex-row">

                        <div
                            class="h-28 w-full rounded-2xl bg-slate-50 border border-slate-100 overflow-hidden shrink-0 md:w-28 relative">
                            <img :src="item.imageUrl || 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=300&q=80'"
                                :alt="item.productName"
                                class="h-full w-full object-cover transition duration-300 group-hover:scale-102" />
                        </div>

                        <div class="flex flex-1 flex-col justify-between py-0.5">
                            <div class="flex items-start justify-between gap-4">
                                <div>
                                    <h2
                                        class="text-base font-bold text-slate-800 tracking-tight transition group-hover:text-blue-600">
                                        {{ item.productName }}
                                    </h2>
                                    <p class="mt-1 text-xs font-semibold text-slate-400">
                                        Unit Price: <span class="text-slate-600 font-bold">৳{{ item.price }}</span>
                                    </p>
                                </div>

                                <button @click="removeItem(item.id)"
                                    class="h-8 w-8 flex items-center justify-center rounded-lg bg-slate-50 text-slate-400 hover:bg-rose-50 hover:text-rose-600 transition"
                                    title="Remove this item bundle from your selection">
                                    <i class="fas fa-trash-alt text-xs"></i>
                                </button>
                            </div>

                            <div
                                class="mt-4 flex flex-wrap items-center justify-between gap-4 border-t border-slate-50 pt-3">
                                <div class="flex items-center rounded-xl bg-slate-100 p-1 border border-slate-200/30">
                                    <button @click="updateQuantity(item, item.quantity - 1)"
                                        :disabled="item.quantity <= 1"
                                        class="h-8 w-8 rounded-lg flex items-center justify-center text-xs font-bold text-slate-600 hover:bg-white hover:text-slate-950 transition disabled:opacity-30 disabled:hover:bg-transparent">
                                        <i class="fas fa-minus"></i>
                                    </button>

                                    <span class="w-10 text-center text-xs font-bold text-slate-800">{{ item.quantity
                                        }}</span>

                                    <button @click="updateQuantity(item, item.quantity + 1)"
                                        class="h-8 w-8 rounded-lg flex items-center justify-center text-xs font-bold text-slate-600 hover:bg-white hover:text-slate-950 transition">
                                        <i class="fas fa-plus"></i>
                                    </button>
                                </div>

                                <div class="flex flex-col text-right">
                                    <span
                                        class="text-[10px] text-slate-400 font-bold uppercase tracking-wider">Subtotal</span>
                                    <strong class="text-base font-black text-slate-800">
                                        ৳{{ item.subtotal }}
                                    </strong>
                                </div>
                            </div>

                        </div>
                    </article>
                </div>

                <aside
                    class="sticky top-6 rounded-3xl bg-white p-6 border border-slate-200/60 shadow-lg shadow-slate-100 lg:col-span-1">
                    <h2 class="text-lg font-black text-slate-800 border-b border-slate-100 pb-3">Order Summary</h2>

                    <div class="mt-4 space-y-3.5 text-xs font-semibold text-slate-500">
                        <div class="flex justify-between items-center">
                            <span>Total Items Selected</span>
                            <span class="rounded-md bg-slate-100 px-2.5 py-1 text-slate-700 font-bold">{{
                                cart.items.length }} unique packs</span>
                        </div>

                        <div class="flex justify-between items-center">
                            <span>Estimated Shipping</span>
                            <span class="text-emerald-600 font-bold uppercase tracking-wider">Free Delivery</span>
                        </div>

                        <div class="flex justify-between items-center border-t border-slate-100 pt-4 text-slate-900">
                            <span class="text-sm font-bold">Total Payable Amount</span>
                            <strong class="text-2xl font-black text-blue-600">৳{{ cart.totalAmount }}</strong>
                        </div>
                    </div>

                    <RouterLink to="/checkout"
                        class="mt-6 flex h-12 items-center justify-center gap-2 w-full rounded-xl bg-slate-950 text-sm font-bold text-white transition duration-200 hover:bg-blue-600 shadow-md">
                        <span>Proceed to Secure Checkout</span>
                        <i class="fas fa-arrow-right text-xs mt-0.5"></i>
                    </RouterLink>

                    <div class="mt-4 text-center">
                        <p class="text-[10px] text-slate-400 font-medium">
                            <i class="fas fa-shield-alt mr-1 text-slate-400"></i> SSL Encrypted Connection Protocols
                            Enforced.
                        </p>
                    </div>
                </aside>

            </section>
        </main>
    </div>
</template>