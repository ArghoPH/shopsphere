<script setup>
import { onMounted, ref, watch } from "vue";
import { RouterLink, useRoute, useRouter } from "vue-router";
import api from "../services/api";
import AppNavbar from "../components/AppNavbar.vue";
import { auth, isAuthenticated } from "../stores/auth";

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
                text: "Product added to cart!",
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
    <div class="min-h-screen bg-slate-100 text-slate-950">
        <AppNavbar />

        <main class="mx-auto max-w-7xl px-6 py-10 md:px-16">
            <section class="mb-8">
                <p class="text-sm font-bold uppercase tracking-[0.25em] text-blue-600">
                    ShopSphere
                </p>

                <h1 class="mt-3 text-4xl font-black md:text-5xl">
                    Featured Products
                </h1>

                <p class="mt-3 max-w-2xl text-slate-500">
                    Browse our latest products and add your favorite items to cart.
                </p>

                <div v-if="route.query.category"
                    class="mt-5 inline-flex rounded-xl bg-blue-50 px-4 py-2 text-sm font-bold text-blue-700">
                    Category: {{ route.query.category }}
                </div>
            </section>



            <div v-if="error" class="mb-6 rounded-2xl bg-red-50 p-4 text-sm font-bold text-red-600">
                {{ error }}
            </div>

            <div v-if="loading" class="rounded-3xl bg-white p-10 text-center text-slate-500 shadow-sm">
                Loading products...
            </div>

            <div v-else-if="products.length === 0"
                class="rounded-3xl bg-white p-10 text-center shadow-sm ring-1 ring-slate-200">
                <h2 class="text-2xl font-black">No products found</h2>
                <p class="mt-2 text-slate-500">
                    No products are available in this category.
                </p>

                <RouterLink to="/products"
                    class="mt-6 inline-flex rounded-2xl bg-slate-950 px-6 py-4 text-sm font-bold text-white transition hover:bg-blue-600">
                    View All Products
                </RouterLink>
            </div>

            <section v-else class="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
                <article v-for="product in products" :key="product.id"
                    class="overflow-hidden rounded-3xl bg-white shadow-sm ring-1 ring-slate-200 transition hover:-translate-y-1 hover:shadow-md">
                    <RouterLink :to="`/products/${product.id}`">
                        <img :src="product.imageUrl || 'https://placehold.co/600x600?text=Product'" :alt="product.name"
                            class="h-64 w-full object-cover" />
                    </RouterLink>

                    <div class="p-5">
                        <div class="flex flex-wrap items-center gap-2">
                            <span
                                class="inline-flex h-8 items-center rounded-lg bg-blue-50 px-3 text-xs font-bold text-blue-700">
                                {{ product.categoryName || "No Category" }}
                            </span>

                            <span class="inline-flex h-8 items-center rounded-lg px-3 text-xs font-bold" :class="product.stock > 0
                                ? 'bg-green-50 text-green-700'
                                : 'bg-red-50 text-red-700'
                                ">
                                {{ product.stock > 0 ? "In Stock" : "Out of Stock" }}
                            </span>
                        </div>

                        <RouterLink :to="`/products/${product.id}`">
                            <h2 class="mt-4 text-2xl font-black transition hover:text-blue-600">
                                {{ product.name }}
                            </h2>
                        </RouterLink>

                        <p class="mt-2 line-clamp-2 text-sm leading-6 text-slate-500">
                            {{ product.description }}
                        </p>

                        <div class="mt-5 flex items-center justify-between">
                            <strong class="text-2xl font-black">
                                ৳{{ product.price }}
                            </strong>

                            <span class="text-sm font-bold text-slate-500">
                                Stock: {{ product.stock }}
                            </span>
                        </div>

                        <div class="mt-5 flex gap-3">
                            <RouterLink :to="`/products/${product.id}`"
                                class="flex-1 rounded-2xl border border-slate-300 px-4 py-3 text-center text-sm font-bold transition hover:border-slate-950">
                                View Details
                            </RouterLink>

                            <button type="button" @click="addToCart(product.id)" :disabled="product.stock <= 0"
                                class="flex-1 rounded-2xl bg-slate-950 px-4 py-3 text-sm font-bold text-white transition hover:bg-blue-600 disabled:cursor-not-allowed disabled:opacity-50">
                                Add to Cart
                            </button>
                        </div>
                        <Transition enter-active-class="transition duration-200 ease-out"
                            enter-from-class="opacity-0 translate-y-1" enter-to-class="opacity-100 translate-y-0"
                            leave-active-class="transition duration-150 ease-in"
                            leave-from-class="opacity-100 translate-y-0" leave-to-class="opacity-0 translate-y-1">
                            <p v-if="productMessages[product.id]"
                                class="mt-3 rounded-xl px-4 py-3 text-center text-xs font-bold" :class="productMessages[product.id].type === 'success'
                                        ? 'bg-green-50 text-green-700'
                                        : 'bg-red-50 text-red-600'
                                    ">
                                {{ productMessages[product.id].text }}
                            </p>
                        </Transition>
                    </div>
                </article>
            </section>
        </main>
    </div>
</template>