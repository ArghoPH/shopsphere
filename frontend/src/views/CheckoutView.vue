<script setup>
import { onMounted, ref } from "vue";
import { RouterLink, useRouter } from "vue-router";
import api from "../services/api";
import { auth, isAuthenticated } from "../stores/auth";
import AppNavbar from "../components/AppNavbar.vue";
import { resetCartCount } from "../stores/cartCounter";

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
        const response = await api.get(`/cart/${auth.userId}`);
        cart.value = response.data;
    } catch (err) {
        error.value = "Failed to load active checkout cart sessions.";
        console.error(err);
    } finally {
        loading.value = false;
    }
};

const openConfirmation = () => {
    error.value = "";

    if (!fullName.value.trim() || !phone.value.trim() || !address.value.trim()) {
        error.value = "Please fill out all delivery fields before proceeding.";
        return;
    }

    if (!cashOnDeliveryChecked.value) {
        error.value = "Please select and confirm Cash on Delivery option.";
        return;
    }

    showConfirmBox.value = true;
};

const placeOrder = async () => {
    placingOrder.value = true;
    error.value = "";

    try {
        const response = await api.post("/orders/checkout", {
            userId: auth.userId,
            fullName: fullName.value,
            phone: phone.value,
            address: address.value,
            cashOnDeliveryChecked: cashOnDeliveryChecked.value,
            confirmOrder: true,
        });

        resetCartCount();

        success.value = response.data.message || "Order placed successfully!";
        showConfirmBox.value = false;

        setTimeout(() => {
            router.push("/orders");
        }, 1500);
    } catch (err) {
        error.value = err.response?.data?.message || "Failed to place your order.";
        console.error(err);
    } finally {
        placingOrder.value = false;
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
                    <i class="fas fa-credit-card"></i>
                </div>
                <div>
                    <h1 class="text-2xl font-black tracking-tight text-slate-800 md:text-3xl">Secure Checkout</h1>
                    <p class="text-xs font-semibold text-slate-400 uppercase mt-0.5">Finalize your personal shipping
                        parameters</p>
                </div>
            </div>

            <div v-if="loading" class="animate-pulse space-y-4 max-w-3xl">
                <div class="rounded-3xl bg-white border border-slate-100 p-8 space-y-4">
                    <div class="h-6 w-1/4 rounded bg-slate-200"></div>
                    <div class="h-12 w-full rounded-xl bg-slate-100"></div>
                    <div class="h-12 w-full rounded-xl bg-slate-100"></div>
                </div>
            </div>

            <div v-else-if="!cart || cart.items.length === 0"
                class="rounded-3xl bg-white p-12 text-center shadow-sm border border-slate-200/80 max-w-xl mx-auto my-12">
                <div
                    class="mx-auto flex h-20 w-20 items-center justify-center rounded-full bg-slate-100 text-slate-400 text-3xl mb-5">
                    <i class="fas fa-shopping-basket"></i>
                </div>
                <h2 class="text-xl font-bold text-slate-800">Your checkout cart session is empty</h2>
                <p class="mt-2 text-sm text-slate-500 max-w-sm mx-auto leading-relaxed">
                    There are no current orders requested for execution. Go back and select products.
                </p>
                <RouterLink to="/products"
                    class="mt-6 inline-flex items-center gap-2 rounded-xl bg-slate-900 px-6 py-3.5 text-sm font-bold text-white transition hover:bg-blue-600 shadow-md">
                    <i class="fas fa-store text-xs"></i> Start Browsing
                </RouterLink>
            </div>

            <section v-else class="grid gap-8 lg:grid-cols-3 items-start">

                <div class="rounded-3xl bg-white p-5 border border-slate-200/60 shadow-sm lg:col-span-2 md:p-8">
                    <h2
                        class="text-lg font-black text-slate-800 flex items-center gap-2 border-b border-slate-100 pb-3">
                        <i class="fas fa-map-marker-alt text-blue-500 text-sm"></i> Shipping & Delivery Info
                    </h2>

                    <div class="mt-6 space-y-5">
                        <div>
                            <label class="mb-1.5 block text-xs font-bold uppercase tracking-wider text-slate-400">
                                Recipient Full Name
                            </label>
                            <div class="relative">
                                <span class="absolute left-4 top-1/2 -translate-y-1/2 text-slate-400 text-sm">
                                    <i class="fas fa-user"></i>
                                </span>
                                <input v-model="fullName" type="text"
                                    class="w-full rounded-xl border border-slate-200 bg-slate-50/50 pl-11 pr-4 py-3.5 text-sm outline-none transition focus:border-blue-500 focus:bg-white"
                                    placeholder="e.g. Abir Hasan" />
                            </div>
                        </div>

                        <div>
                            <label class="mb-1.5 block text-xs font-bold uppercase tracking-wider text-slate-400">
                                Contact Phone Number
                            </label>
                            <div class="relative">
                                <span class="absolute left-4 top-1/2 -translate-y-1/2 text-slate-400 text-sm">
                                    <i class="fas fa-phone-alt"></i>
                                </span>
                                <input v-model="phone" type="text"
                                    class="w-full rounded-xl border border-slate-200 bg-slate-50/50 pl-11 pr-4 py-3.5 text-sm outline-none transition focus:border-blue-500 focus:bg-white"
                                    placeholder="e.g. +8801XXXXXXXXX" />
                            </div>
                        </div>

                        <div>
                            <label class="mb-1.5 block text-xs font-bold uppercase tracking-wider text-slate-400">
                                Full Mailing Address
                            </label>
                            <div class="relative">
                                <span class="absolute left-4 top-4 text-slate-400 text-sm">
                                    <i class="fas fa-home"></i>
                                </span>
                                <textarea v-model="address" rows="3"
                                    class="w-full rounded-xl border border-slate-200 bg-slate-50/50 pl-11 pr-4 py-3.5 text-sm outline-none transition focus:border-blue-500 focus:bg-white resize-none"
                                    placeholder="House, Road, Area, District specifications..."></textarea>
                            </div>
                        </div>

                        <label
                            :class="cashOnDeliveryChecked ? 'border-blue-500 bg-blue-50/30 ring-1 ring-blue-500/20' : 'border-slate-200 bg-slate-50/40'"
                            class="flex cursor-pointer items-start gap-3.5 rounded-2xl border p-4 transition-all duration-200">
                            <input v-model="cashOnDeliveryChecked" type="checkbox"
                                class="mt-1 h-4 w-4 rounded border-slate-300 text-blue-600 focus:ring-blue-500" />
                            <div>
                                <strong class="block text-sm font-bold text-slate-800">Cash on Delivery (COD)</strong>
                                <span class="text-xs font-medium text-slate-400 block mt-0.5">
                                    Handover cash currency notes securely directly upon parcel logistics reception.
                                </span>
                            </div>
                        </label>

                        <div class="space-y-2">
                            <div v-if="error"
                                class="flex items-center gap-2 rounded-xl bg-rose-50 border border-rose-200 px-4 py-3 text-xs font-bold text-rose-600 shadow-inner">
                                <i class="fas fa-exclamation-circle text-sm"></i>
                                <span>{{ error }}</span>
                            </div>

                            <div v-if="success"
                                class="flex items-center gap-2 rounded-xl bg-emerald-50 border border-emerald-200 px-4 py-3 text-xs font-bold text-emerald-700 shadow-inner">
                                <i class="fas fa-check-circle text-sm"></i>
                                <span>{{ success }}</span>
                            </div>
                        </div>

                        <button @click="openConfirmation"
                            class="flex h-12 w-full items-center justify-center gap-2 rounded-xl bg-slate-950 text-sm font-bold text-white transition duration-200 hover:bg-blue-600 shadow-md">
                            <i class="fas fa-lock text-xs"></i>
                            <span>Proceed Order Assembly</span>
                        </button>
                    </div>
                </div>

                <aside
                    class="sticky top-6 rounded-3xl bg-white p-6 border border-slate-200/60 shadow-lg shadow-slate-100 lg:col-span-1">
                    <h2 class="text-base font-black text-slate-800 border-b border-slate-100 pb-3">Review Purchase</h2>

                    <div class="mt-4 space-y-3 max-h-56 overflow-y-auto pr-1 border-b border-slate-100 pb-4">
                        <div v-for="item in cart.items" :key="item.id"
                            class="flex justify-between gap-4 text-xs font-medium text-slate-500">
                            <span class="line-clamp-1 flex-1">{{ item.productName }} <b class="text-slate-800 ml-1">×{{
                                item.quantity }}</b></span>
                            <strong class="text-slate-700">৳{{ item.subtotal }}</strong>
                        </div>
                    </div>

                    <div class="mt-4 space-y-2 text-xs font-semibold text-slate-400">
                        <div class="flex justify-between text-slate-500">
                            <span>Logistics Transit</span>
                            <span class="text-emerald-600 font-bold uppercase text-[10px]">Free Shipping</span>
                        </div>
                        <div class="flex justify-between items-baseline pt-2 text-slate-900 border-t border-slate-50">
                            <span class="text-sm font-bold">Aggregate Gross Total</span>
                            <strong class="text-xl font-black text-blue-600">৳{{ cart.total }}</strong>
                        </div>
                    </div>
                </aside>
            </section>

            <div v-if="showConfirmBox"
                class="fixed inset-0 synchronized z-50 flex items-center justify-center bg-slate-950/40 backdrop-blur-sm px-4">
                <div
                    class="w-full max-w-md rounded-3xl bg-white p-6 shadow-2xl border border-slate-100 text-center relative overflow-hidden">

                    <div
                        class="mx-auto flex h-14 w-14 items-center justify-center rounded-full bg-blue-50 text-blue-600 text-xl mb-4">
                        <i class="fas fa-truck-loading"></i>
                    </div>

                    <h2 class="text-xl font-extrabold text-slate-800 tracking-tight">Dispatch Confirmation?</h2>
                    <p class="mt-2 text-xs leading-relaxed text-slate-400 max-w-xs mx-auto">
                        Are you sure you want to deploy this active retail order profile now using Cash on Delivery
                        logistics routing?
                    </p>

                    <div class="mt-6 flex gap-3">
                        <button @click="showConfirmBox = false"
                            class="flex-1 rounded-xl border border-slate-200 bg-white px-4 py-3 text-xs font-bold text-slate-700 transition hover:bg-slate-50 hover:border-slate-400">
                            Dismiss
                        </button>

                        <button @click="placeOrder" :disabled="placingOrder"
                            class="flex-1 flex items-center justify-center gap-1.5 rounded-xl bg-slate-950 px-4 py-3 text-xs font-bold text-white transition hover:bg-blue-600 shadow-md disabled:cursor-not-allowed disabled:bg-slate-200 disabled:text-slate-400 disabled:shadow-none">
                            <i v-if="placingOrder" class="fas fa-spinner animate-spin"></i>
                            <span>{{ placingOrder ? "Authorizing..." : "Confirm & Place" }}</span>
                        </button>
                    </div>
                </div>
            </div>
        </main>
    </div>
</template>