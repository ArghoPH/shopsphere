<script setup>
import { RouterLink } from "vue-router";
import AppNavbar from "../components/AppNavbar.vue";
import { ref, onMounted, onUnmounted } from 'vue';
import { isAuthenticated } from "../stores/auth";
import api from "../services/api";


const trendingProducts = ref([]);
const trendingLoading = ref(true);
const trendingScrollRef = ref(null);

const fetchTrendingProducts = async () => {
    trendingLoading.value = true;

    try {
        const response = await api.get("/products/trending");
        trendingProducts.value = response.data;
    } catch (err) {
        console.error("Failed to load trending products", err);
    } finally {
        trendingLoading.value = false;
    }
};

const scrollTrendingProducts = (direction) => {
    if (!trendingScrollRef.value) return;

    trendingScrollRef.value.scrollBy({
        left: direction === "left" ? -360 : 360,
        behavior: "smooth",
    });
};

// --- IMAGE SLIDER / ADVERTISEMENT LOGIC ---
const currentSlide = ref(0);
const slides = ref([
    {
        title: "Fresh Groceries & Food",
        subtitle: "Get up to 30% OFF on daily organic essentials",
        image: "https://images.unsplash.com/photo-1542838132-92c53300491e?auto=format&fit=crop&w=1200&q=80",
        badge: "🍱 DAILY FRESH",
        btnText: "Shop Groceries",
        link: "/products"
    },
    {
        title: "Next-Gen Electronics Store",
        subtitle: "Upgrade your lifestyle with the latest smart gadgets",
        image: "https://images.unsplash.com/photo-1468436139062-f60a71c5c892?auto=format&fit=crop&w=1200&q=80",
        badge: "⚡ TECH UPGRADE",
        btnText: "Explore Tech",
        link: "/products?category=electronics"
    },
    {
        title: "Trendy Fashion Collections",
        subtitle: "Express your style with our premium lifestyle outfits",
        image: "https://images.unsplash.com/photo-1441986300917-64674bd600d8?auto=format&fit=crop&w=1200&q=80",
        badge: "🔥 TRENDING NOW",
        btnText: "View Fashion",
        link: "/products?category=t-shirts"
    }
]);

let sliderInterval = null;

const nextSlide = () => {
    currentSlide.value = (currentSlide.value + 1) % slides.value.length;
};

const prevSlide = () => {
    currentSlide.value = (currentSlide.value - 1 + slides.value.length) % slides.value.length;
};

onMounted(() => {
    // Auto-slide every 5 seconds
    sliderInterval = setInterval(nextSlide, 5000);
    fetchCategories();
    fetchTrendingProducts();
});

onUnmounted(() => {
    clearInterval(sliderInterval);

});

// --- CATEGORIES LOGIC ---
const categories = ref([]);
const categoryLoading = ref(true);

const fetchCategories = async () => {
    categoryLoading.value = true;

    try {
        const response = await api.get("/categories");
        categories.value = response.data;
    } catch (err) {
        console.error("Failed to load categories", err);
    } finally {
        categoryLoading.value = false;
    }
};
</script>

<template>
    <div class="min-h-screen bg-slate-50 text-slate-900 font-sans">
        <AppNavbar />

        <main>
            <section class="mx-auto max-w-7xl px-4 py-6 md:px-8">
                <div
                    class="relative overflow-hidden rounded-[2rem] shadow-xl bg-slate-900 aspect-[16/9] md:aspect-[21/9]">

                    <Transition enter-active-class="transition-opacity duration-700 ease-in-out"
                        enter-from-class="opacity-0" enter-to-class="opacity-100"
                        leave-active-class="transition-opacity duration-700 ease-in-out" leave-from-class="opacity-100"
                        leave-to-class="opacity-0" mode="out-in">
                        <div :key="currentSlide" class="absolute inset-0 w-full h-full">

                            <template v-if="slides[currentSlide]">
                                <img :src="slides[currentSlide].image" :alt="slides[currentSlide].title"
                                    class="absolute inset-0 w-full h-full object-cover" />
                                <div
                                    class="absolute inset-0 bg-gradient-to-r from-slate-950/80 via-slate-950/50 to-transparent">
                                </div>

                                <div
                                    class="absolute inset-0 flex flex-col justify-center px-8 md:px-16 text-white z-10">
                                    <span
                                        class="inline-block self-start rounded-full bg-blue-600 px-3 py-1 text-xs font-bold uppercase tracking-wider mb-3">
                                        {{ slides[currentSlide].badge }}
                                    </span>
                                    <h1 class="text-2xl font-extrabold leading-tight md:text-4xl max-w-xl">
                                        {{ slides[currentSlide].title }}
                                    </h1>
                                    <p class="mt-2 text-sm md:text-base text-slate-200 max-w-md">
                                        {{ slides[currentSlide].subtitle }}
                                    </p>
                                    <div class="mt-5">
                                        <RouterLink :to="slides[currentSlide].link"
                                            class="inline-block rounded-full bg-amber-400 px-6 py-3 text-sm font-bold text-slate-900 shadow-md transition-transform hover:-translate-y-0.5 hover:bg-amber-300">
                                            {{ slides[currentSlide].btnText }} <i class="fas fa-arrow-right ml-2"></i>
                                        </RouterLink>
                                    </div>
                                </div>
                            </template>
                        </div>
                    </Transition>

                    <button @click="prevSlide"
                        class="absolute left-4 top-1/2 -translate-y-1/2 z-30 flex h-10 w-10 items-center justify-center rounded-full bg-black/30 text-white backdrop-blur-sm hover:bg-black/50 transition">
                        <i class="fas fa-chevron-left"></i>
                    </button>
                    <button @click="nextSlide"
                        class="absolute right-4 top-1/2 -translate-y-1/2 z-30 flex h-10 w-10 items-center justify-center rounded-full bg-black/30 text-white backdrop-blur-sm hover:bg-black/50 transition">
                        <i class="fas fa-chevron-right"></i>
                    </button>

                    <div class="absolute bottom-4 left-1/2 -translate-x-1/2 z-30 flex gap-2">
                        <button v-for="(_, index) in slides" :key="index" @click="currentSlide = index"
                            :class="['h-2 rounded-full transition-all duration-300', currentSlide === index ? 'w-6 bg-amber-400' : 'w-2 bg-white/50']">
                        </button>
                    </div>
                </div>
            </section>

            <section class="mx-auto max-w-7xl px-4 py-8 md:px-8">
                <div class="mb-6 flex items-center justify-between">
                    <div>
                        <p class="text-xs font-black uppercase tracking-[0.25em] text-blue-600">
                            Explore Collections
                        </p>
                        <h2 class="mt-2 text-2xl font-black text-slate-900 md:text-3xl">
                            Shop by Category
                        </h2>
                    </div>

                    <RouterLink to="/products"
                        class="rounded-full bg-slate-950 px-5 py-3 text-xs font-bold text-white transition hover:bg-blue-600">
                        View All
                        <i class="fa-solid fa-arrow-right ml-2"></i>
                    </RouterLink>
                </div>

                <div v-if="categoryLoading"
                    class="rounded-3xl bg-white p-10 text-center text-sm font-bold text-slate-500 shadow-sm">
                    Loading categories...
                </div>

                <div v-else class="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-5">
                    <RouterLink v-for="category in categories" :key="category.id"
                        :to="`/products?category=${category.slug}`"
                        class="group relative h-48 overflow-hidden rounded-3xl bg-slate-900 shadow-sm transition-all duration-300 hover:-translate-y-1 hover:shadow-xl">
                        <img :src="category.imageUrl || 'https://images.unsplash.com/photo-1441986300917-64674bd600d8?auto=format&fit=crop&w=900&q=80'"
                            :alt="category.name"
                            class="absolute inset-0 h-full w-full object-cover transition duration-500 group-hover:scale-110" />

                        <div
                            class="absolute inset-0 bg-gradient-to-t from-slate-950/85 via-slate-950/30 to-transparent">
                        </div>

                        <div class="absolute inset-x-0 bottom-0 p-5">
                            <div
                                class="mb-3 inline-flex h-10 w-10 items-center justify-center rounded-full bg-white/15 text-white backdrop-blur-md">
                                <i class="fa-solid fa-layer-group"></i>
                            </div>

                            <h3 class="text-xl font-black text-white">
                                {{ category.name }}
                            </h3>

                            <p class="mt-1 text-xs font-bold uppercase tracking-wider text-slate-300">
                                Explore Products
                            </p>
                        </div>
                    </RouterLink>
                </div>
            </section>

            <section class="mx-auto max-w-7xl px-4 py-4 md:px-8">
                <div
                    class="rounded-2xl bg-gradient-to-r from-rose-500 to-orange-500 px-6 py-8 text-center text-white shadow-md">
                    <h3 class="text-xl font-bold md:text-2xl"><i class="fas fa-gift mr-2"></i> Get 20% Off on Your First
                        Order!</h3>
                    <p class="mt-1 text-sm text-rose-100">Use coupon code <span
                            class="rounded bg-white px-2 py-0.5 text-xs font-bold text-rose-600">WELCOME20</span>
                        at checkout.</p>
                </div>
            </section>

            <section class="mx-auto max-w-7xl px-4 py-16 md:px-8 bg-slate-50/30 font-sans">

                <div class="mb-8 flex items-end justify-between gap-6 border-b border-slate-100 pb-5">
                    <div>
                        <p class="text-xs font-bold uppercase tracking-wider text-blue-600 flex items-center gap-1.5">
                            <i class="fas fa-fire text-amber-500"></i> Best Sellers
                        </p>

                        <h2 class="mt-2 text-2xl font-bold text-slate-900 tracking-tight md:text-3xl">
                            Trending Products
                        </h2>

                        <p class="mt-1.5 max-w-2xl text-sm text-slate-500">
                            Most selling products sorted from highest to lowest sales.
                        </p>
                    </div>

                    <div class="hidden gap-3 md:flex">
                        <button type="button" @click="scrollTrendingProducts('left')"
                            class="flex h-10 w-10 items-center justify-center rounded-xl border border-slate-200 bg-white text-slate-600 shadow-sm transition hover:bg-slate-950 hover:text-white hover:border-slate-950 active:scale-95">
                            <i class="fas fa-chevron-left text-xs"></i>
                        </button>

                        <button type="button" @click="scrollTrendingProducts('right')"
                            class="flex h-10 w-10 items-center justify-center rounded-xl border border-slate-200 bg-white text-slate-600 shadow-sm transition hover:bg-slate-950 hover:text-white hover:border-slate-950 active:scale-95">
                            <i class="fas fa-chevron-right text-xs"></i>
                        </button>
                    </div>
                </div>

                <div v-if="trendingLoading"
                    class="rounded-3xl bg-white border border-slate-100 p-10 text-center text-sm font-medium text-slate-500 shadow-sm max-w-md mx-auto">
                    <i class="fas fa-spinner fa-spin mr-2 text-blue-500"></i> Loading trending products...
                </div>

                <div v-else-if="trendingProducts.length === 0"
                    class="rounded-3xl bg-white border border-slate-200 p-10 text-center text-slate-500 shadow-sm max-w-md mx-auto">
                    <div
                        class="h-10 w-10 rounded-full bg-slate-50 flex items-center justify-center text-slate-400 mx-auto mb-3">
                        <i class="fas fa-box-open text-base"></i>
                    </div>
                    <span class="text-sm font-medium block">No trending products found.</span>
                </div>

                <div v-else class="relative">
                    <div ref="trendingScrollRef"
                        class="flex gap-6 overflow-x-auto scroll-smooth pb-6 [scrollbar-width:none] [&::-webkit-scrollbar]:hidden snap-x">
                        <article v-for="product in trendingProducts" :key="product.id"
                            class="min-w-[280px] max-w-[280px] snap-start overflow-hidden rounded-3xl bg-white/70 border border-white/60 shadow-md shadow-slate-100/50 backdrop-blur-md transition-all duration-300 hover:-translate-y-1 hover:shadow-xl hover:border-blue-300/30 group relative">

                            <div
                                class="absolute -top-10 -right-10 w-24 h-24 bg-gradient-to-br from-blue-400/10 to-purple-400/0 rounded-full blur-xl transition duration-500 group-hover:scale-150">
                            </div>

                            <RouterLink :to="`/products/${product.id}`"
                                class="block h-full flex flex-col justify-between">

                                <div
                                    class="relative h-56 m-3 rounded-2xl overflow-hidden bg-slate-50 border border-slate-100 shrink-0">
                                    <img v-if="product.imageUrl" :src="product.imageUrl" :alt="product.name"
                                        class="h-full w-full object-cover transition-transform duration-500 group-hover:scale-102" />

                                    <div v-else
                                        class="flex h-full w-full items-center justify-center bg-slate-50 text-slate-400">
                                        <i class="fas fa-image text-3xl"></i>
                                    </div>

                                    <div
                                        class="absolute left-3 top-3 inline-flex items-center gap-1.5 rounded-xl bg-white/90 px-3 py-1.5 text-xs font-bold text-slate-800 backdrop-blur shadow-sm border border-white/40">
                                        <i class="fas fa-bolt text-amber-500"></i>
                                        <span>{{ product.soldQuantity }} sold</span>
                                    </div>
                                </div>

                                <div class="p-5 pt-2 flex-1 flex flex-col justify-between">
                                    <div>
                                        <p class="text-xs font-bold uppercase tracking-wider text-blue-600">
                                            {{ product.categoryName || "Product" }}
                                        </p>

                                        <h3
                                            class="mt-2 line-clamp-2 text-base font-bold text-slate-950 tracking-tight group-hover:text-blue-600 transition">
                                            {{ product.name }}
                                        </h3>

                                        <p class="mt-1.5 line-clamp-2 text-sm leading-relaxed text-slate-500">
                                            {{ product.description }}
                                        </p>
                                    </div>

                                    <div class="mt-4 pt-3 border-t border-slate-100 flex items-center justify-between">
                                        <p class="text-lg font-bold tracking-tight text-slate-950">
                                            ৳{{ Number(product.price).toLocaleString() }}
                                        </p>

                                        <span
                                            class="rounded-full bg-slate-100 px-3 py-1 text-xs font-medium text-slate-600">
                                            Stock {{ product.stock }}
                                        </span>
                                    </div>

                                </div>
                            </RouterLink>
                        </article>
                    </div>

                    <div class="mt-4 flex justify-center gap-3 md:hidden">
                        <button type="button" @click="scrollTrendingProducts('left')"
                            class="flex h-10 w-10 items-center justify-center rounded-xl border border-slate-200 bg-white text-slate-600 shadow-sm active:bg-slate-900 active:text-white">
                            <i class="fas fa-chevron-left text-xs"></i>
                        </button>

                        <button type="button" @click="scrollTrendingProducts('right')"
                            class="flex h-10 w-10 items-center justify-center rounded-xl border border-slate-200 bg-white text-slate-600 shadow-sm active:bg-slate-900 active:text-white">
                            <i class="fas fa-chevron-right text-xs"></i>
                        </button>
                    </div>
                </div>
            </section>

            <section class="border-t border-slate-200 bg-white py-10">
                <div class="mx-auto max-w-7xl px-4 md:px-8">
                    <div class="grid grid-cols-1 gap-6 md:grid-cols-3 text-center">
                        <div class="p-4">
                            <div
                                class="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-emerald-100 text-emerald-600 text-lg">
                                <i class="fas fa-truck"></i>
                            </div>
                            <h4 class="mt-3 text-base font-bold">Super Fast Delivery</h4>
                            <p class="mt-1 text-xs text-slate-500">Products delivered directly to your doorstep safely.
                            </p>
                        </div>
                        <div class="p-4">
                            <div
                                class="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-blue-100 text-blue-600 text-lg">
                                <i class="fas fa-credit-card"></i>
                            </div>
                            <h4 class="mt-3 text-base font-bold">Secure Payments</h4>
                            <p class="mt-1 text-xs text-slate-500">Cash on Delivery, Digital Wallets, and Card support.
                            </p>
                        </div>
                        <div class="p-4">
                            <div
                                class="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-rose-100 text-rose-600 text-lg">
                                <i class="fas fa-headset"></i>
                            </div>
                            <h4 class="mt-3 text-base font-bold">24/7 Dedicated Support</h4>
                            <p class="mt-1 text-xs text-slate-500">We are always ready to solve your queries instantly.
                            </p>
                        </div>
                    </div>
                </div>
            </section>
        </main>
    </div>
</template>