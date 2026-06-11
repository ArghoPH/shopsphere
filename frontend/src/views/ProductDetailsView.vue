<script setup>
import { onMounted, ref } from "vue";
import { RouterLink, useRoute } from "vue-router";
import api from "../services/api";
import { auth, isAuthenticated } from "../stores/auth";
import { useRouter } from "vue-router";
import AppNavbar from "../components/AppNavbar.vue";
import { increaseCartCount, fetchCartCount } from "../stores/cartCounter";

const router = useRouter();
const route = useRoute();

const product = ref(null);
const loading = ref(true);
const error = ref("");
const isAdding = ref(false);

const cartMessage = ref(null);

const addToCart = async () => {
    if (!isAuthenticated()) {
        router.push("/login");
        return;
    }

    if (!product.value || product.value.stock <= 0) return;

    isAdding.value = true;
    cartMessage.value = null;

    try {
        await api.post("/cart/add", {
            userId: auth.userId,
            productId: product.value.id,
            quantity: 1,
        });
        increaseCartCount(quantity.value || 1);
        fetchCartCount();

        cartMessage.value = {
            type: "success",
            text: "Success! Product added to your cart."
        };

        // Auto clear message
        setTimeout(() => {
            cartMessage.value = null;
        }, 3000);
    } catch (err) {
        cartMessage.value = {
            type: "error",
            text: err.response?.data?.message || "Failed to add product to cart."
        };
        console.error(err);
    } finally {
        isAdding.value = false;
    }
};

const fetchProduct = async () => {
    try {
        const response = await api.get(`/products/${route.params.id}`);
        product.value = response.data;
    } catch (err) {
        error.value = "Product details could not be retrieved. Please try again.";
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
    <div class="min-h-screen bg-slate-50 text-slate-900 font-sans">
        <AppNavbar />

        <main class="mx-auto max-w-7xl px-4 py-8 md:px-8">

            <div class="mb-6">
                <RouterLink to="/products"
                    class="inline-flex items-center gap-2 text-sm font-semibold text-blue-600 hover:text-blue-700 transition">
                    <i class="fas fa-arrow-left text-xs"></i> Back to Shop Products
                </RouterLink>
            </div>

            <div v-if="loading"
                class="animate-pulse rounded-3xl bg-white border border-slate-100 p-6 md:p-10 shadow-sm grid gap-8 md:grid-cols-2">
                <div class="aspect-square w-full rounded-2xl bg-slate-200"></div>
                <div class="space-y-5 py-4">
                    <div class="h-6 w-24 rounded-full bg-slate-200"></div>
                    <div class="h-10 w-3/4 rounded bg-slate-200"></div>
                    <div class="h-4 w-full rounded bg-slate-200"></div>
                    <div class="h-4 w-full rounded bg-slate-200"></div>
                    <div class="h-12 w-32 rounded bg-slate-200 pt-4"></div>
                    <div class="h-14 w-full rounded-xl bg-slate-200"></div>
                </div>
            </div>

            <div v-else-if="error"
                class="rounded-3xl bg-white p-12 text-center shadow-sm border border-slate-200 max-w-xl mx-auto my-12">
                <div
                    class="mx-auto flex h-16 w-16 items-center justify-center rounded-full bg-rose-50 text-rose-500 text-2xl mb-4">
                    <i class="fas fa-exclamation-triangle"></i>
                </div>
                <h2 class="text-xl font-bold text-slate-800">Oops! Something went wrong</h2>
                <p class="mt-2 text-sm text-slate-500">{{ error }}</p>
                <RouterLink to="/products"
                    class="mt-6 inline-flex rounded-xl bg-slate-900 px-6 py-2.5 text-xs font-bold text-white shadow hover:bg-blue-600 transition">
                    Return to Store
                </RouterLink>
            </div>

            <section v-else
                class="grid gap-8 rounded-3xl bg-white p-5 border border-slate-200/60 shadow-xl shadow-slate-100 md:grid-cols-2 md:p-10">

                <div
                    class="group relative overflow-hidden rounded-2xl bg-slate-50 border border-slate-100 flex items-center justify-center p-4 aspect-square">
                    <img :src="product.imageUrl || 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=800&q=80'"
                        :alt="product.name"
                        class="h-full w-full object-cover rounded-xl transition-transform duration-500 group-hover:scale-102" />

                    <div class="absolute top-4 right-4">
                        <span :class="product.stock > 0 ? 'bg-emerald-500 text-white' : 'bg-rose-500 text-white'"
                            class="inline-flex rounded-lg px-3 py-1.5 text-xs font-bold uppercase tracking-wider shadow-md">
                            <i :class="product.stock > 0 ? 'fas fa-check-circle mr-1.5' : 'fas fa-times-circle mr-1.5'"
                                class="mt-0.5"></i>
                            {{ product.stock > 0 ? "In Stock" : "Out of Stock" }}
                        </span>
                    </div>
                </div>

                <div class="flex flex-col justify-between py-2">
                    <div>
                        <span
                            class="inline-flex items-center gap-2 rounded-full bg-blue-50 border border-blue-100 px-4 py-1.5 text-xs font-bold text-blue-700 uppercase tracking-wide">
                            <i class="fas fa-tags text-[10px]"></i> {{ product.categoryName || 'General Product' }}
                        </span>

                        <h1 class="mt-4 text-3xl font-black tracking-tight text-slate-800 md:text-4xl lg:text-5xl">
                            {{ product.name }}
                        </h1>

                        <div class="mt-6 flex flex-wrap items-baseline gap-4 border-b border-slate-100 pb-5">
                            <strong class="text-3xl font-black text-blue-600 md:text-4xl">
                                ৳{{ product.price }}
                            </strong>
                            <span class="rounded-xl bg-slate-100 px-3 py-1.5 text-xs font-bold text-slate-600">
                                <i class="fas fa-cubes text-slate-400 mr-1"></i> Available Stock: {{ product.stock }}
                                items
                            </span>
                        </div>

                        <div class="mt-6">
                            <h3 class="text-xs font-bold text-slate-400 uppercase tracking-wider mb-2">Product
                                Description</h3>
                            <p class="text-sm leading-relaxed text-slate-600 whitespace-pre-line">
                                {{ product.description ||
                                    'No detailed descriptive specifications provided for this listing asset yet.' }}
                            </p>
                        </div>
                    </div>

                    <div class="mt-8 pt-6 border-t border-slate-100">

                        <div class="grid gap-3 sm:grid-cols-2">
                            <button @click="addToCart" :disabled="product.stock <= 0 || isAdding"
                                class="flex h-14 items-center justify-center gap-2 rounded-xl bg-slate-950 px-6 text-sm font-bold text-white transition duration-200 hover:bg-blue-600 shadow-md disabled:cursor-not-allowed disabled:bg-slate-200 disabled:text-slate-400 disabled:shadow-none">
                                <i v-if="isAdding" class="fas fa-spinner animate-spin"></i>
                                <i v-else class="fas fa-shopping-cart"></i>
                                <span>{{ isAdding ? "Processing..." : "Add to Cart" }}</span>
                            </button>

                            <button :disabled="product.stock <= 0"
                                class="flex h-14 items-center justify-center gap-2 rounded-xl border border-slate-200 bg-white px-6 text-sm font-bold text-slate-800 transition hover:bg-slate-50 hover:border-slate-400 disabled:cursor-not-allowed disabled:bg-slate-50 disabled:text-slate-300 disabled:border-slate-100">
                                <i class="fas fa-bolt"></i>
                                <span>Buy It Now</span>
                            </button>
                        </div>

                        <div
                            class="mt-6 flex flex-wrap gap-y-2 gap-x-6 text-[11px] text-slate-400 font-bold uppercase tracking-wider">
                            <span><i class="fas fa-truck text-emerald-500 mr-1"></i> Cash On Delivery</span>
                            <span><i class="fas fa-undo text-blue-500 mr-1"></i> 7 Days Easy Return</span>
                            <span><i class="fas fa-shield-alt text-amber-500 mr-1"></i> 100% Genuine</span>
                        </div>

                        <div class="relative min-h-[10px]">
                            <Transition enter-active-class="transition duration-300 ease-out"
                                enter-from-class="opacity-0 translate-y-2" enter-to-class="opacity-100 translate-y-0"
                                leave-active-class="transition duration-200 ease-in"
                                leave-from-class="opacity-100 translate-y-0" leave-to-class="opacity-0 translate-y-2">
                                <div v-if="cartMessage"
                                    class="mt-4 flex items-center justify-center gap-2 rounded-xl px-4 py-3 text-center text-xs font-bold shadow-inner border"
                                    :class="cartMessage.type === 'success' ? 'bg-emerald-50 text-emerald-700 border-emerald-200' : 'bg-rose-50 text-rose-600 border-rose-200'">
                                    <i
                                        :class="cartMessage.type === 'success' ? 'fas fa-check-circle' : 'fas fa-times-circle'"></i>
                                    <span>{{ cartMessage.text }}</span>
                                </div>
                            </Transition>
                        </div>

                    </div>
                </div>
            </section>
        </main>
    </div>
</template>