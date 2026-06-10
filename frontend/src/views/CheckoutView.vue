<script setup>
import { onMounted, ref } from "vue";
import { RouterLink, useRouter } from "vue-router";
import api from "../services/api";
import { GUEST_USER_ID } from "../constants/user";

const router = useRouter();

const cart = ref(null);
const loading = ref(true);
const error = ref("");
const success = ref("");

const fullName = ref("");
const phone = ref("");
const address = ref("");
const cashOnDeliveryChecked = ref(false);

const showConfirmBox = ref(false);
const placingOrder = ref(false);

const fetchCart = async () => {
    try {
        const response = await api.get(`/cart/${GUEST_USER_ID}`);
        cart.value = response.data;
    } catch (err) {
        error.value = "Failed to load cart.";
        console.error(err);
    } finally {
        loading.value = false;
    }
};

const openConfirmation = () => {
    error.value = "";

    if (!fullName.value.trim() || !phone.value.trim() || !address.value.trim()) {
        error.value = "Please fill in all delivery information.";
        return;
    }

    if (!cashOnDeliveryChecked.value) {
        error.value = "Please check Cash on Delivery before placing order.";
        return;
    }

    showConfirmBox.value = true;
};

const placeOrder = async () => {
    placingOrder.value = true;
    error.value = "";

    try {
        const response = await api.post("/orders/checkout", {
            userId: GUEST_USER_ID,
            fullName: fullName.value,
            phone: phone.value,
            address: address.value,
            cashOnDeliveryChecked: cashOnDeliveryChecked.value,
            confirmOrder: true,
        });

        success.value = response.data.message;
        showConfirmBox.value = false;

        setTimeout(() => {
            router.push("/");
        }, 1500);
    } catch (err) {
        error.value = err.response?.data?.message || "Failed to place order.";
        console.error(err);
    } finally {
        placingOrder.value = false;
    }
};

onMounted(fetchCart);
</script>

<template>
    <div class="min-h-screen bg-slate-100 text-slate-950">
        <header class="bg-slate-950 px-6 py-6 text-white md:px-16">
            <nav class="mx-auto flex max-w-7xl items-center justify-between">
                <RouterLink to="/" class="text-2xl font-bold tracking-tight">
                    ShopSphere
                </RouterLink>

                <RouterLink to="/cart"
                    class="rounded-xl bg-white/10 px-4 py-2 text-sm font-bold text-white transition hover:bg-white/20">
                    Back to Cart
                </RouterLink>
            </nav>
        </header>

        <main class="mx-auto max-w-7xl px-6 py-12 md:px-16">
            <h1 class="mb-8 text-4xl font-black">Checkout</h1>

            <div v-if="loading" class="rounded-3xl bg-white p-10 text-center text-slate-500 shadow-sm">
                Loading checkout...
            </div>

            <div v-else-if="!cart || cart.items.length === 0" class="rounded-3xl bg-white p-10 text-center shadow-sm">
                <h2 class="text-2xl font-black">Your cart is empty</h2>
                <RouterLink to="/"
                    class="mt-6 inline-block rounded-2xl bg-slate-950 px-6 py-4 text-sm font-bold text-white transition hover:bg-blue-600">
                    Shop Now
                </RouterLink>
            </div>

            <section v-else class="grid gap-8 lg:grid-cols-3">
                <div class="rounded-3xl bg-white p-6 shadow-sm ring-1 ring-slate-200 lg:col-span-2">
                    <h2 class="text-2xl font-black">Delivery Information</h2>

                    <div class="mt-6 grid gap-5">
                        <div>
                            <label class="mb-2 block text-sm font-bold text-slate-700">
                                Full Name
                            </label>
                            <input v-model="fullName" type="text"
                                class="w-full rounded-2xl border border-slate-300 px-4 py-4 outline-none focus:border-slate-950"
                                placeholder="Enter your full name" />
                        </div>

                        <div>
                            <label class="mb-2 block text-sm font-bold text-slate-700">
                                Phone Number
                            </label>
                            <input v-model="phone" type="text"
                                class="w-full rounded-2xl border border-slate-300 px-4 py-4 outline-none focus:border-slate-950"
                                placeholder="Enter your phone number" />
                        </div>

                        <div>
                            <label class="mb-2 block text-sm font-bold text-slate-700">
                                Delivery Address
                            </label>
                            <textarea v-model="address" rows="4"
                                class="w-full rounded-2xl border border-slate-300 px-4 py-4 outline-none focus:border-slate-950"
                                placeholder="Enter your full address"></textarea>
                        </div>

                        <label class="flex cursor-pointer items-start gap-3 rounded-2xl border border-slate-300 p-4">
                            <input v-model="cashOnDeliveryChecked" type="checkbox" class="mt-1 h-5 w-5" />
                            <span>
                                <strong class="block">Cash on Delivery</strong>
                                <span class="text-sm text-slate-500">
                                    I will pay in cash when I receive the product.
                                </span>
                            </span>
                        </label>

                        <div v-if="error" class="rounded-2xl bg-red-50 p-4 text-sm font-bold text-red-600">
                            {{ error }}
                        </div>

                        <div v-if="success" class="rounded-2xl bg-green-50 p-4 text-sm font-bold text-green-700">
                            {{ success }}
                        </div>

                        <button @click="openConfirmation"
                            class="rounded-2xl bg-slate-950 px-6 py-4 text-sm font-bold text-white transition hover:bg-blue-600">
                            Place Order
                        </button>
                    </div>
                </div>

                <aside class="h-fit rounded-3xl bg-white p-6 shadow-sm ring-1 ring-slate-200">
                    <h2 class="text-2xl font-black">Order Summary</h2>

                    <div class="mt-6 space-y-4">
                        <div v-for="item in cart.items" :key="item.id"
                            class="flex justify-between gap-4 text-sm text-slate-600">
                            <span>{{ item.productName }} × {{ item.quantity }}</span>
                            <strong>৳{{ item.subtotal }}</strong>
                        </div>

                        <div class="flex justify-between border-t border-slate-200 pt-4 text-slate-950">
                            <span class="font-bold">Total</span>
                            <strong class="text-2xl">৳{{ cart.totalAmount }}</strong>
                        </div>
                    </div>
                </aside>
            </section>

            <div v-if="showConfirmBox" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 px-6">
                <div class="w-full max-w-md rounded-3xl bg-white p-6 shadow-2xl">
                    <h2 class="text-2xl font-black">Confirm Order?</h2>
                    <p class="mt-3 text-slate-500">
                        Are you sure you want to place this order with Cash on Delivery?
                    </p>

                    <div class="mt-6 flex gap-3">
                        <button @click="showConfirmBox = false"
                            class="flex-1 rounded-2xl border border-slate-300 px-5 py-4 text-sm font-bold transition hover:border-slate-950">
                            No
                        </button>

                        <button @click="placeOrder" :disabled="placingOrder"
                            class="flex-1 rounded-2xl bg-slate-950 px-5 py-4 text-sm font-bold text-white transition hover:bg-blue-600 disabled:cursor-not-allowed disabled:opacity-60">
                            {{ placingOrder ? "Placing..." : "Yes" }}
                        </button>
                    </div>
                </div>
            </div>
        </main>
    </div>
</template>