<script setup>
import { onMounted, ref } from "vue";
import { RouterLink } from "vue-router";
import api from "../services/api";
import { auth } from "../stores/auth";
import AppNavbar from "../components/AppNavbar.vue";
import { fetchCartCount } from "../stores/cartCounter";

const cart = ref(null);
const loading = ref(true);
const error = ref("");

// নির্দিষ্ট আইটেম আপডেট বা ডিলিট করার সময় অ্যানিমেশন ট্র্যাক করার জন্য
const processingItems = ref(new Set());

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
    if (quantity <= 0 || processingItems.value.has(item.id)) return;

    try {
        processingItems.value.add(item.id);

        await api.put(`/cart/items/${item.id}`, {
            quantity,
        });

        await fetchCart();
        await fetchCartCount();
    } catch (err) {
        console.error(err);
    } finally {
        processingItems.value.delete(item.id);
    }
};

const removeItem = async (itemId) => {
    if (processingItems.value.has(itemId)) return;

    try {
        processingItems.value.add(itemId);

        await api.delete(`/cart/items/${itemId}`);

        await fetchCart();
        await fetchCartCount();
    } catch (err) {
        console.error(err);
    } finally {
        processingItems.value.delete(itemId);
    }
};

onMounted(async () => {
    await fetchCart();
    await fetchCartCount();
});
</script>

<template>
    <div class="min-h-screen bg-slate-50 text-slate-900 font-sans antialiased">
        <AppNavbar />

        <main class="mx-auto max-w-7xl px-4 py-8 md:px-8">

            <div class="mb-8 flex items-center gap-3 animate-fade-in">
                <div
                    class="flex h-10 w-10 items-center justify-center rounded-xl bg-blue-600 text-white shadow-md shadow-blue-100 transition-transform hover:scale-110 duration-300">
                    <i class="fas fa-shopping-basket"></i>
                </div>
                <div>
                    <h1 class="text-2xl font-black tracking-tight text-slate-800 md:text-3xl">Your Shopping Cart</h1>
                    <p v-if="cart && cart.items.length"
                        class="text-xs font-semibold text-slate-400 uppercase mt-0.5 tracking-wider">
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
                class="rounded-3xl bg-white p-12 text-center shadow-sm border border-slate-200 max-w-xl mx-auto my-12 transform transition-all duration-500 hover:scale-102">
                <div
                    class="mx-auto flex h-16 w-16 items-center justify-center rounded-full bg-rose-50 text-rose-500 text-2xl mb-4 animate-bounce">
                    <i class="fas fa-exclamation-circle"></i>
                </div>
                <h2 class="text-xl font-bold text-slate-800">Connection Interrupted</h2>
                <p class="mt-2 text-sm text-slate-500">{{ error }}</p>
            </div>

            <div v-else-if="!cart || cart.items.length === 0"
                class="rounded-3xl bg-white p-12 text-center shadow-sm border border-slate-200/80 max-w-xl mx-auto my-12 transition-all duration-500 hover:shadow-xl">
                <div
                    class="mx-auto flex h-20 w-20 items-center justify-center rounded-full bg-slate-100 text-slate-400 text-3xl mb-5 transition-transform duration-700 hover:rotate-12">
                    <i class="fas fa-shopping-cart"></i>
                </div>
                <h2 class="text-xl font-bold text-slate-800">Your cart is currently empty</h2>
                <p class="mt-2 text-sm text-slate-500 max-w-sm mx-auto leading-relaxed">
                    Looks like you haven't added anything to your cart yet. Explore our marketplace categories to find
                    hot deals.
                </p>
                <RouterLink to="/products"
                    class="mt-6 inline-flex items-center gap-2 rounded-xl bg-slate-900 px-6 py-3.5 text-sm font-bold text-white transition-all duration-300 hover:bg-blue-600 hover:shadow-lg hover:-translate-y-0.5 shadow-md">
                    <i class="fas fa-store text-xs"></i> Start Shopping
                </RouterLink>
            </div>

            <section v-else class="grid gap-8 lg:grid-cols-3 items-start">

                <div class="lg:col-span-2 relative">
                    <TransitionGroup name="cart-list" tag="div" class="space-y-4">
                        <article v-for="item in cart.items" :key="item.id"
                            class="group relative flex flex-col gap-5 rounded-3xl bg-white p-4 border border-slate-200/60 shadow-sm transition-all duration-300 hover:shadow-md md:flex-row"
                            :class="{ 'opacity-60 pointer-events-none select-none': processingItems.has(item.id) }">

                            <div v-if="processingItems.has(item.id)"
                                class="absolute inset-0 z-10 flex items-center justify-center bg-white/40 rounded-3xl backdrop-blur-[1px]">
                                <div
                                    class="h-7 w-7 animate-spin rounded-full border-3 border-blue-600 border-t-transparent">
                                </div>
                            </div>

                            <div
                                class="h-28 w-full rounded-2xl bg-slate-50 border border-slate-100 overflow-hidden shrink-0 md:w-28 relative">
                                <img :src="item.imageUrl || 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=300&q=80'"
                                    :alt="item.productName"
                                    class="h-full w-full object-cover transition duration-500 group-hover:scale-105" />
                            </div>

                            <div class="flex flex-1 flex-col justify-between py-0.5">
                                <div class="flex items-start justify-between gap-4">
                                    <div>
                                        <h2
                                            class="text-base font-bold text-slate-800 tracking-tight transition duration-300 group-hover:text-blue-600">
                                            {{ item.productName }}
                                        </h2>
                                        <p class="mt-1 text-xs font-semibold text-slate-400">
                                            Unit Price: <span class="text-slate-600 font-bold">৳{{ item.price }}</span>
                                        </p>
                                    </div>

                                    <button @click="removeItem(item.id)"
                                        class="h-8 w-8 flex items-center justify-center rounded-lg bg-slate-50 text-slate-400 hover:bg-rose-50 hover:text-rose-600 transition-all duration-200 active:scale-90"
                                        title="Remove this item bundle from your selection">
                                        <i class="fas fa-trash-alt text-xs"></i>
                                    </button>
                                </div>

                                <div
                                    class="mt-4 flex flex-wrap items-center justify-between gap-4 border-t border-slate-50 pt-3">
                                    <div
                                        class="flex items-center rounded-xl bg-slate-100 p-1 border border-slate-200/30">
                                        <button @click="updateQuantity(item, item.quantity - 1)"
                                            :disabled="item.quantity <= 1 || processingItems.has(item.id)"
                                            class="h-8 w-8 rounded-lg flex items-center justify-center text-xs font-bold text-slate-600 hover:bg-white hover:text-slate-950 transition-all duration-150 active:scale-90 disabled:opacity-30 disabled:hover:bg-transparent">
                                            <i class="fas fa-minus"></i>
                                        </button>

                                        <span
                                            class="w-10 text-center text-xs font-bold text-slate-800 transition-all duration-300">{{
                                                item.quantity }}</span>

                                        <button @click="updateQuantity(item, item.quantity + 1)"
                                            :disabled="processingItems.has(item.id)"
                                            class="h-8 w-8 rounded-lg flex items-center justify-center text-xs font-bold text-slate-600 hover:bg-white hover:text-slate-950 transition-all duration-150 active:scale-90">
                                            <i class="fas fa-plus"></i>
                                        </button>
                                    </div>

                                    <div class="flex flex-col text-right">
                                        <span
                                            class="text-[10px] text-slate-400 font-bold uppercase tracking-wider">Subtotal</span>
                                        <strong class="text-base font-black text-slate-800 transition-all duration-300">
                                            ৳{{ item.subtotal }}
                                        </strong>
                                    </div>
                                </div>

                            </div>
                        </article>
                    </TransitionGroup>
                </div>

                <aside
                    class="sticky top-24 rounded-3xl bg-white p-6 border border-slate-200/60 shadow-lg shadow-slate-100/50 lg:col-span-1 transition-all duration-300 hover:shadow-xl animate-fade-in-up">
                    <h2 class="text-lg font-black text-slate-800 border-b border-slate-100 pb-3">Order Summary</h2>

                    <div class="mt-4 space-y-3.5 text-xs font-semibold text-slate-500">
                        <div class="flex justify-between items-center">
                            <span>Total Items Selected</span>
                            <span
                                class="rounded-md bg-slate-100 px-2.5 py-1 text-slate-700 font-bold transition-all duration-300">
                                {{ cart.items.length }} unique {{ cart.items.length === 1 ? 'pack' : 'packs' }}
                            </span>
                        </div>

                        <div class="flex justify-between items-center">
                            <span>Estimated Shipping</span>
                            <span class="text-emerald-600 font-bold uppercase tracking-wider animate-pulse">Free
                                Delivery</span>
                        </div>

                        <div class="flex justify-between items-center border-t border-slate-100 pt-4 text-slate-900">
                            <span class="text-sm font-bold">Total Payable Amount</span>
                            <strong
                                class="text-2xl font-black text-blue-600 tracking-tight transition-all duration-300">৳
                                {{ cart.totalAmount }}</strong>
                        </div>
                    </div>

                    <RouterLink to="/checkout"
                        class="mt-6 flex h-12 items-center justify-center gap-2 w-full rounded-xl bg-slate-950 text-sm font-bold text-white transition-all duration-300 hover:bg-blue-600 hover:shadow-lg hover:-translate-y-0.5 active:translate-y-0 shadow-md group">
                        <span>Proceed to Secure Checkout</span>
                        <i
                            class="fas fa-arrow-right text-xs mt-0.5 transition-transform duration-300 group-hover:translate-x-1"></i>
                    </RouterLink>

                    <div class="mt-4 text-center">
                        <p class="text-[10px] text-slate-400 font-medium">
                            <i class="fas fa-shield-alt mr-1 text-emerald-500"></i> SSL Encrypted Connection Protocols
                            Enforced.
                        </p>
                    </div>
                </aside>

            </section>
        </main>
    </div>
</template>

<style scoped>
/* 1. Page Load Animations */
@keyframes fadeIn {
    from {
        opacity: 0;
        transform: translateY(-4px);
    }

    to {
        opacity: 1;
        transform: translateY(0);
    }
}

@keyframes fadeInUp {
    from {
        opacity: 0;
        transform: translateY(15px);
    }

    to {
        opacity: 1;
        transform: translateY(0);
    }
}

.animate-fade-in {
    animation: fadeIn 0.4s ease-out forwards;
}

.animate-fade-in-up {
    animation: fadeInUp 0.5s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}

/* 2. Cart List Transition Group Animations (Delete/Move) */
.cart-list-enter-from {
    opacity: 0;
    transform: translateX(-20px);
}

.cart-list-leave-to {
    opacity: 0;
    transform: translateX(30px);
}

.cart-list-leave-active {
    position: absolute;
    width: 100%;
    z-index: 0;
}

.cart-list-move {
    transition: transform 0.4s cubic-bezier(0.55, 0, 0.1, 1);
}

article {
    transition: all 0.4s cubic-bezier(0.55, 0, 0.1, 1);
}

/* Custom border width helper */
.border-3 {
    border-width: 3px;
}
</style>