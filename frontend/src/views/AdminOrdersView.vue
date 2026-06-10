<script setup>
import { onMounted, reactive, ref } from "vue";
import { RouterLink } from "vue-router";
import api from "../services/api";

const orders = ref([]);
const loading = ref(true);
const error = ref("");
const success = ref("");

const selectedStatuses = reactive({});

const statuses = [
    "Processing",
    "Confirmed",
    "Shipped",
    "Delivered",
    "Cancelled",
];

const fetchOrders = async () => {
    loading.value = true;
    error.value = "";

    try {
        const response = await api.get("/admin/orders");
        orders.value = response.data;

        orders.value.forEach((order) => {
            selectedStatuses[order.id] = order.orderStatus;
        });
    } catch (err) {
        error.value = "Failed to load admin orders.";
        console.error(err);
    } finally {
        loading.value = false;
    }
};

const updateOrderStatus = async (orderId) => {
    success.value = "";
    error.value = "";

    try {
        const response = await api.put(`/admin/orders/${orderId}/status`, {
            orderStatus: selectedStatuses[orderId],
        });

        success.value = response.data.message;
        await fetchOrders();
    } catch (err) {
        error.value = err.response?.data?.message || "Failed to update order status.";
        console.error(err);
    }
};

const formatDate = (date) => {
    return new Date(date).toLocaleDateString("en-BD", {
        year: "numeric",
        month: "short",
        day: "numeric",
    });
};
const formatPaymentMethod = (method) => {
    if (method === "CashOnDelivery") {
        return "Cash on Delivery";
    }

    return method;
};

onMounted(fetchOrders);
</script>

<template>
    <div class="min-h-screen bg-slate-100 text-slate-950">
        <header class="bg-slate-950 px-6 py-6 text-white md:px-16">
            <nav class="mx-auto flex max-w-7xl items-center justify-between">
                <RouterLink to="/" class="text-2xl font-bold tracking-tight">
                    ShopSphere Admin
                </RouterLink>

                <div class="flex items-center gap-3">
                    <RouterLink to="/orders"
                        class="rounded-xl bg-white/10 px-4 py-2 text-sm font-bold text-white transition hover:bg-white/20">
                        User Orders
                    </RouterLink>

                    <RouterLink to="/"
                        class="rounded-xl bg-white/10 px-4 py-2 text-sm font-bold text-white transition hover:bg-white/20">
                        Shop
                    </RouterLink>

                    <RouterLink to="/admin/products"
                        class="rounded-xl bg-white/10 px-4 py-2 text-sm font-bold text-white transition hover:bg-white/20">
                        Products
                    </RouterLink>
                </div>
            </nav>
        </header>

        <main class="mx-auto max-w-7xl px-6 py-12 md:px-16">
            <div class="mb-8 flex flex-col gap-3 md:flex-row md:items-end md:justify-between">
                <div>
                    <h1 class="text-4xl font-black">Admin Order Management</h1>
                    <p class="mt-2 text-slate-500">
                        View orders, customer details, items, payment status, and update order status.
                    </p>
                </div>

                <button @click="fetchOrders"
                    class="rounded-2xl bg-slate-950 px-6 py-4 text-sm font-bold text-white transition hover:bg-blue-600">
                    Refresh
                </button>
            </div>

            <div v-if="success" class="mb-6 rounded-2xl bg-green-50 p-4 text-sm font-bold text-green-700">
                {{ success }}
            </div>

            <div v-if="error" class="mb-6 rounded-2xl bg-red-50 p-4 text-sm font-bold text-red-600">
                {{ error }}
            </div>

            <div v-if="loading" class="rounded-3xl bg-white p-10 text-center text-slate-500 shadow-sm">
                Loading admin orders...
            </div>

            <div v-else-if="orders.length === 0" class="rounded-3xl bg-white p-10 text-center shadow-sm">
                <h2 class="text-2xl font-black">No orders found</h2>
                <p class="mt-2 text-slate-500">
                    New customer orders will appear here.
                </p>
            </div>

            <section v-else class="space-y-6">
                <article v-for="order in orders" :key="order.id"
                    class="overflow-hidden rounded-3xl bg-white shadow-sm ring-1 ring-slate-200">
                    <div class="border-b border-slate-200 p-6">
                        <div class="grid gap-6 lg:grid-cols-3">
                            <div class="lg:col-span-2">
                                <p class="text-sm font-bold text-slate-400">Order ID</p>
                                <h2 class="mt-1 break-all text-lg font-black">
                                    {{ order.id }}
                                </h2>

                                <p class="mt-3 text-sm text-slate-500">
                                    Placed on {{ formatDate(order.createdAt) }}
                                </p>
                            </div>

                            <div class="flex flex-wrap items-center gap-2">
                                <span
                                    class="inline-flex h-8 items-center rounded-lg bg-blue-50 px-3 text-xs font-bold text-blue-700">
                                    {{ order.orderStatus }}
                                </span>

                                <span
                                    class="inline-flex h-8 items-center rounded-lg bg-amber-50 px-3 text-xs font-bold text-amber-700">
                                    {{ order.paymentStatus }}
                                </span>

                                <span
                                    class="inline-flex h-8 items-center rounded-lg bg-slate-100 px-3 text-xs font-bold text-slate-700">
                                    {{ formatPaymentMethod(order.paymentMethod) }}
                                </span>
                            </div>
                        </div>
                    </div>

                    <div class="grid gap-6 p-6 lg:grid-cols-3">
                        <div class="rounded-2xl bg-slate-50 p-5">
                            <h3 class="text-lg font-black">Customer Info</h3>

                            <div class="mt-4 space-y-3 text-sm text-slate-600">
                                <p>
                                    <strong class="text-slate-950">Name:</strong>
                                    {{ order.fullName }}
                                </p>

                                <p>
                                    <strong class="text-slate-950">Phone:</strong>
                                    {{ order.phone }}
                                </p>

                                <p>
                                    <strong class="text-slate-950">Address:</strong>
                                    {{ order.address }}
                                </p>
                            </div>
                        </div>

                        <div class="rounded-2xl bg-slate-50 p-5 lg:col-span-2">
                            <h3 class="text-lg font-black">Order Items</h3>

                            <div class="mt-4 space-y-3">
                                <div v-for="item in order.items" :key="`${order.id}-${item.productName}`"
                                    class="flex items-center justify-between gap-4 rounded-xl bg-white p-4">
                                    <div>
                                        <h4 class="font-black">
                                            {{ item.productName }}
                                        </h4>

                                        <p class="mt-1 text-sm text-slate-500">
                                            Qty: {{ item.quantity }} × ৳{{ item.price }}
                                        </p>
                                    </div>

                                    <strong class="text-lg font-black">
                                        ৳{{ item.subtotal }}
                                    </strong>
                                </div>
                            </div>

                            <div class="mt-5 flex justify-between border-t border-slate-200 pt-5">
                                <span class="font-bold">Total Amount</span>
                                <strong class="text-2xl font-black">
                                    ৳{{ order.totalAmount }}
                                </strong>
                            </div>
                        </div>

                        <div class="rounded-2xl bg-white p-5 ring-1 ring-slate-200 lg:col-span-3">
                            <h3 class="text-lg font-black">Update Order Status</h3>

                            <div class="mt-4 flex flex-col gap-3 md:flex-row">
                                <select v-model="selectedStatuses[order.id]"
                                    class="w-full rounded-2xl border border-slate-300 px-4 py-4 font-bold outline-none focus:border-slate-950 md:max-w-xs">
                                    <option v-for="status in statuses" :key="status" :value="status">
                                        {{ status }}
                                    </option>
                                </select>

                                <button @click="updateOrderStatus(order.id)"
                                    class="rounded-2xl bg-slate-950 px-6 py-4 text-sm font-bold text-white transition hover:bg-blue-600">
                                    Update Status
                                </button>
                            </div>
                        </div>
                    </div>
                </article>
            </section>
        </main>
    </div>
</template>