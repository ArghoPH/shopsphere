<script setup>
import { onMounted, ref, watch } from "vue";
import { RouterLink, useRoute, useRouter } from "vue-router";
import api from "../services/api";
import AppNavbar from "../components/AppNavbar.vue";
import { auth, isAuthenticated } from "../stores/auth";

const addingProducts = ref({});

const router = useRouter();
const route = useRoute();

const products = ref([]);
const loading = ref(true);
const error = ref("");
const productMessages = ref({});

const fetchProducts = async () => {
    loading.value = true;
    error.value = "";

    try {
        const categorySlug = route.query.category;

        const response = await api.get("/products", {
            params: {
                categorySlug: categorySlug || undefined,
            },
        });

        products.value = response.data;
    } catch (err) {
        error.value = "Failed to load products.";
        console.error(err);
    } finally {
        loading.value = false;
    }
};

const addToCart = async (productId) => {
    if (!isAuthenticated()) {
        router.push("/login");
        return;
    }

    addingProducts.value = {
        ...addingProducts.value,
        [productId]: true,
    };

    try {
        await api.post("/cart/add", {
            userId: auth.userId,
            productId,
            quantity: 1,
        });

        productMessages.value = {
            ...productMessages.value,
            [productId]: {
                type: "success",
                text: "Added to cart successfully!",
            },
        };

        setTimeout(() => {
            const updatedMessages = { ...productMessages.value };
            delete updatedMessages[productId];
            productMessages.value = updatedMessages;
        }, 2000);
    } catch (err) {
        productMessages.value = {
            ...productMessages.value,
            [productId]: {
                type: "error",
                text: err.response?.data?.message || "Failed to add product.",
            },
        };

        setTimeout(() => {
            const updatedMessages = { ...productMessages.value };
            delete updatedMessages[productId];
            productMessages.value = updatedMessages;
        }, 2500);

        console.error(err);
    } finally {
        addingProducts.value = {
            ...addingProducts.value,
            [productId]: false,
        };
    }
};

watch(
    () => route.query.category,
    () => {
        fetchProducts();
    }
);

onMounted(() => {
    fetchProducts();
});
</script>

<template>
    <div class="min-h-screen bg-slate-50 text-slate-900 font-sans">
        <AppNavbar />

        <main class="mx-auto max-w-7xl px-4 py-8 md:px-8">
            <section
                class="mb-10 rounded-3xl bg-gradient-to-r from-blue-600 to-indigo-700 p-8 md:p-12 text-white shadow-lg relative overflow-hidden">
                <div class="absolute -right-10 -top-10 h-40 w-40 rounded-full bg-white/10 blur-2xl"></div>

                <div class="relative z-10 max-w-2xl">
                    <span
                        class="inline-flex items-center gap-2 rounded-full bg-white/20 px-3 py-1 text-xs font-bold tracking-wider uppercase backdrop-blur-sm">
                        <i class="fas fa-shopping-bag text-amber-300"></i> ShopSphere Catalog
                    </span>

                    <h1 class="mt-4 text-3xl font-extrabold md:text-4xl tracking-tight">
                        {{ route.query.category ? `Category: ${route.query.category}` : 'Explore Our Collections' }}
                    </h1>

                    <p class="mt-3 text-sm md:text-base text-indigo-100">
                        Discover top-quality items tailored just for you. Quick ordering with Cash on Delivery support.
                    </p>

                    <div v-if="route.query.category" class="mt-4 inline-flex">
                        <RouterLink to="/products"
                            class="flex items-center gap-2 rounded-xl bg-white px-4 py-2 text-xs font-bold text-blue-700 shadow-sm transition hover:bg-blue-50">
                            <i class="fas fa-arrow-left"></i> Clear Filter / View All
                        </RouterLink>
                    </div>
                </div>
            </section>

            <div v-if="error"
                class="mb-8 flex items-center gap-3 rounded-2xl bg-rose-50 border border-rose-200 p-4 text-sm font-bold text-rose-600 shadow-sm">
                <i class="fas fa-exclamation-circle text-lg"></i>
                <span>{{ error }}</span>
            </div>

            <div v-if="loading" class="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
                <div v-for="n in 6" :key="n"
                    class="animate-pulse overflow-hidden rounded-3xl bg-white border border-slate-100 shadow-sm p-5 space-y-4">
                    <div class="h-56 w-full rounded-2xl bg-slate-200"></div>
                    <div class="h-4 w-1/3 rounded bg-slate-200"></div>
                    <div class="h-6 w-3/4 rounded bg-slate-200"></div>
                    <div class="h-4 w-full rounded bg-slate-200"></div>
                    <div class="flex justify-between items-center pt-2">
                        <div class="h-6 w-1/4 rounded bg-slate-200"></div>
                        <div class="h-10 w-1/3 rounded-xl bg-slate-200"></div>
                    </div>
                </div>
            </div>

            <div v-else-if="products.length === 0"
                class="rounded-3xl bg-white p-12 text-center shadow-sm border border-slate-200 max-w-xl mx-auto my-12">
                <div
                    class="mx-auto flex h-16 w-16 items-center justify-center rounded-full bg-amber-50 text-amber-500 text-2xl mb-4">
                    <i class="fas fa-box-open"></i>
                </div>
                <h2 class="text-xl font-bold text-slate-800">No products discovered</h2>
                <p class="mt-2 text-sm text-slate-500">
                    We couldn't find any items available in this category right now.
                </p>

                <RouterLink to="/products"
                    class="mt-6 inline-flex items-center gap-2 rounded-xl bg-slate-900 px-6 py-3 text-sm font-bold text-white transition hover:bg-blue-600 shadow-md">
                    <i class="fas fa-th-large"></i> Explore All Categories
                </RouterLink>
            </div>

            <section v-else class="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
                <article v-for="product in products" :key="product.id"
                    class="group flex flex-col justify-between overflow-hidden rounded-3xl bg-white shadow-sm border border-slate-200/60 transition-all duration-300 hover:-translate-y-1.5 hover:shadow-xl">

                    <div>
                        <div class="relative h-56 overflow-hidden bg-slate-50 border-b border-slate-100">
                            <RouterLink :to="`/products/${product.id}`" class="block h-full w-full">
                                <img :src="product.imageUrl || 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=600&q=80'"
                                    :alt="product.name"
                                    class="h-full w-full object-cover transition-transform duration-500 group-hover:scale-105" />
                            </RouterLink>

                            <div class="absolute left-3 top-3 flex flex-col gap-1.5">
                                <span
                                    class="inline-flex items-center gap-1.5 rounded-lg bg-white/90 backdrop-blur-sm px-2.5 py-1 text-[11px] font-bold text-slate-700 shadow-sm">
                                    <i class="fas fa-tags text-blue-500 text-[10px]"></i> {{ product.categoryName ||
                                        "General" }}
                                </span>
                            </div>

                            <div class="absolute right-3 top-3">
                                <span
                                    :class="product.stock > 0 ? 'bg-emerald-500 text-white' : 'bg-rose-500 text-white'"
                                    class="inline-flex rounded-lg px-2.5 py-1 text-[10px] font-bold uppercase tracking-wider shadow-sm">
                                    {{ product.stock > 0 ? "In Stock" : "Out of Stock" }}
                                </span>
                            </div>
                        </div>

                        <div class="p-5 pb-0">
                            <RouterLink :to="`/products/${product.id}`">
                                <h2
                                    class="text-lg font-bold tracking-tight text-slate-800 transition duration-200 group-hover:text-blue-600 line-clamp-1">
                                    {{ product.name }}
                                </h2>
                            </RouterLink>

                            <p class="mt-2 line-clamp-2 text-xs leading-relaxed text-slate-500">
                                {{ product.description ||
                                    'No detailed description available for this selected retail product asset.' }}
                            </p>
                        </div>
                    </div>

                    <div class="p-5 pt-4">
                        <div class="flex items-end justify-between border-t border-slate-100 pt-3 mb-4">
                            <div class="flex flex-col">
                                <span class="text-[10px] text-slate-400 font-bold uppercase tracking-wider">Price</span>
                                <strong class="text-2xl font-black text-blue-600">৳{{ product.price }}</strong>
                            </div>
                            <span class="text-xs font-semibold text-slate-500 bg-slate-100 px-2 py-1 rounded-md">
                                <i class="fas fa-layer-group text-slate-400 mr-1"></i> Stock: {{ product.stock }}
                            </span>
                        </div>

                        <div class="flex gap-2">
                            <RouterLink :to="`/products/${product.id}`"
                                class="flex h-11 items-center justify-center rounded-xl border border-slate-200 bg-white px-3 text-slate-700 text-xs font-bold transition hover:bg-slate-50 hover:border-slate-400"
                                title="View Product Details">
                                <i class="fas fa-eye text-sm"></i>
                            </RouterLink>

                            <button type="button" @click="addToCart(product.id)"
                                :disabled="product.stock <= 0 || addingProducts[product.id]"
                                class="flex-1 flex h-11 items-center justify-center gap-2 rounded-xl bg-slate-900 text-xs font-bold text-white transition duration-200 hover:bg-blue-600 disabled:cursor-not-allowed disabled:bg-slate-200 disabled:text-slate-400">
                                <i v-if="addingProducts[product.id]" class="fas fa-spinner animate-spin"></i>
                                <i v-else class="fas fa-shopping-cart"></i>
                                <span>
                                    {{ addingProducts[product.id] ? "Adding..." : product.stock <= 0 ? "Out of Stock"
                                        : "Add to Cart" }} </span>
                            </button>
                        </div>

                        <div class="relative min-h-[10px]">
                            <Transition enter-active-class="transition duration-300 ease-out"
                                enter-from-class="opacity-0 translate-y-2" enter-to-class="opacity-100 translate-y-0"
                                leave-active-class="transition duration-200 ease-in"
                                leave-from-class="opacity-100 translate-y-0" leave-to-class="opacity-0 translate-y-2">
                                <div v-if="productMessages[product.id]"
                                    class="mt-3 flex items-center justify-center gap-2 rounded-xl px-3 py-2 text-center text-xs font-bold shadow-inner"
                                    :class="productMessages[product.id].type === 'success' ? 'bg-emerald-50 text-emerald-700 border border-emerald-200' : 'bg-rose-50 text-rose-600 border border-rose-200'">
                                    <i
                                        :class="productMessages[product.id].type === 'success' ? 'fas fa-check-circle' : 'fas fa-times-circle'"></i>
                                    <span>{{ productMessages[product.id].text }}</span>
                                </div>
                            </Transition>
                        </div>
                    </div>
                </article>
            </section>
        </main>
    </div>
</template>