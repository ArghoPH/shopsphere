<script setup>
import { onMounted, ref } from "vue";
import { RouterLink } from "vue-router";
import api from "../services/api";
import { auth, isAuthenticated } from "../stores/auth";
import AppNavbar from "../components/AppNavbar.vue";

const orders = ref([]);
const loading = ref(true);
const error = ref("");

const fetchOrders = async () => {
    try {
        const response = await api.get(`/orders/user/${auth.userId}`);
        orders.value = response.data;
    } catch (err) {
        error.value = "Failed to load orders.";
        console.error(err);
    } finally {
        loading.value = false;
    }
};

const formatDate = (date) => {
    return new Date(date).toLocaleDateString("en-BD", {
        year: "numeric",
        month: "short",
        day: "numeric",
    });
};

onMounted(fetchOrders);
</script>

<template>
    <div class="min-h-screen bg-slate-100 text-slate-950">
        <AppNavbar />

        <main class="mx-auto max-w-7xl px-6 py-12 md:px-16">
            <div class="mb-8 flex flex-col gap-3 md:flex-row md:items-end md:justify-between">
                <div>
                    <h1 class="text-4xl font-black">Order History</h1>
                    <p class="mt-2 text-slate-500">
                        Track your previous Cash on Delivery orders.
                    </p>
                </div>
            </div>

            <div v-if="loading" class="rounded-3xl bg-white p-10 text-center text-slate-500 shadow-sm">
                Loading orders...
            </div>

            <div v-else-if="error" class="rounded-3xl bg-white p-10 text-center font-semibold text-red-600 shadow-sm">
                {{ error }}
            </div>

            <div v-else-if="orders.length === 0" class="rounded-3xl bg-white p-10 text-center shadow-sm">
                <h2 class="text-2xl font-black">No orders yet</h2>
                <p class="mt-2 text-slate-500">
                    Your placed orders will appear here.
                </p>

                <RouterLink to="/"
                    class="mt-6 inline-block rounded-2xl bg-slate-950 px-6 py-4 text-sm font-bold text-white transition hover:bg-blue-600">
                    Start Shopping
                </RouterLink>
            </div>

            <section v-else class="space-y-6">
                <article v-for="order in orders" :key="order.id"
                    class="overflow-hidden rounded-3xl bg-white shadow-sm ring-1 ring-slate-200">
                    <div class="border-b border-slate-200 p-6">
                        <div class="flex flex-col gap-4 md:flex-row md:items-start md:justify-between">
                            <div>
                                <p class="text-sm font-bold text-slate-400">
                                    Order ID
                                </p>
                                <h2 class="mt-1 break-all text-lg font-black">
                                    {{ order.id }}
                                </h2>

                                <p class="mt-3 text-sm text-slate-500">
                                    Placed on {{ formatDate(order.createdAt) }}
                                </p>
                            </div>

                            <div class="flex flex-wrap gap-2">
                                <span class="rounded-full bg-blue-50 px-4 py-2 text-sm font-bold text-blue-700">
                                    {{ order.orderStatus }}
                                </span>

                                <span class="rounded-full bg-amber-50 px-4 py-2 text-sm font-bold text-amber-700">
                                    {{ order.paymentStatus }}
                                </span>

                                <span class="rounded-full bg-slate-100 px-4 py-2 text-sm font-bold text-slate-700">
                                    {{ order.paymentMethod }}
                                </span>
                            </div>
                        </div>
                    </div>

                    <div class="p-6">
                        <div class="space-y-4">
                            <div v-for="item in order.items" :key="`${order.id}-${item.productName}`"
                                class="flex items-center justify-between gap-4 rounded-2xl bg-slate-50 p-4">
                                <div>
                                    <h3 class="font-black">
                                        {{ item.productName }}
                                    </h3>

                                    <p class="mt-1 text-sm text-slate-500">
                                        Qty: {{ item.quantity }} × ৳{{ item.price }}
                                    </p>
                                </div>

                                <strong class="text-lg font-black">
                                    ৳{{ item.subtotal }}
                                </strong>
                            </div>
                        </div>

                        <div class="mt-6 flex items-center justify-between border-t border-slate-200 pt-5">
                            <span class="text-lg font-bold">Total Amount</span>
                            <strong class="text-3xl font-black">
                                ৳{{ order.totalAmount }}
                            </strong>
                        </div>
                    </div>
                </article>
            </section>
        </main>
    </div>
</template>