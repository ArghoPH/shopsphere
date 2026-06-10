<script setup>
import { RouterLink } from "vue-router";
import AppNavbar from "../components/AppNavbar.vue";
import { ref, onMounted, onUnmounted } from 'vue';
import { isAuthenticated } from "../stores/auth";

// --- IMAGE SLIDER / ADVERTISEMENT LOGIC ---
const currentSlide = ref(0);
const slides = ref([
    {
        title: "Fresh Groceries & Food",
        subtitle: "Get up to 30% OFF on daily organic essentials",
        image: "https://images.unsplash.com/photo-1542838132-92c53300491e?auto=format&fit=crop&w=1200&q=80",
        badge: "🍱 DAILY FRESH",
        btnText: "Shop Groceries",
        link: "/category/food"
    },
    {
        title: "Next-Gen Electronics Store",
        subtitle: "Upgrade your lifestyle with the latest smart gadgets",
        image: "https://images.unsplash.com/photo-1468436139062-f60a71c5c892?auto=format&fit=crop&w=1200&q=80",
        badge: "⚡ TECH UPGRADE",
        btnText: "Explore Tech",
        link: "/category/electronics"
    },
    {
        title: "Trendy Fashion Collections",
        subtitle: "Express your style with our premium lifestyle outfits",
        image: "https://images.unsplash.com/photo-1441986300917-64674bd600d8?auto=format&fit=crop&w=1200&q=80",
        badge: "🔥 TRENDING NOW",
        btnText: "View Fashion",
        link: "/category/fashion"
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
});

onUnmounted(() => {
    clearInterval(sliderInterval);
});

// --- CATEGORIES WITH FONTAWESOME ---
const categories = ref([
    { name: 'Groceries & Food', icon: 'fas fa-utensils', color: 'bg-emerald-100 text-emerald-600', link: '/category/food' },
    { name: 'Electronics', icon: 'fas fa-laptop', color: 'bg-blue-100 text-blue-600', link: '/category/electronics' },
    { name: 'Fashion & Wear', icon: 'fas fa-shirt', color: 'bg-pink-100 text-pink-600', link: '/category/fashion' },
    { name: 'Home & Decor', icon: 'fas fa-couch', color: 'bg-amber-100 text-amber-600', link: '/category/home' },
    { name: 'Beauty & Health', icon: 'fas fa-sparkles', color: 'bg-rose-100 text-rose-600', link: '/category/beauty' },
    { name: 'Sports & Toys', icon: 'fas fa-gamepad', color: 'bg-indigo-100 text-indigo-600', link: '/category/sports' },
]);

// --- TRENDING PRODUCTS ---
const trendingProducts = ref([
    { id: 1, name: 'Sony Wireless Headphones', price: '৳ 2,500', image: 'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=400&q=80' },
    { id: 2, name: 'Fresh Organic Apples (1kg)', price: '৳ 350', image: 'https://images.unsplash.com/photo-1560806887-1e4cd0b6cbd6?auto=format&fit=crop&w=400&q=80' },
    { id: 3, name: 'Smart LED TV Ultra HD', price: '৳ 35,000', image: 'https://images.unsplash.com/photo-1593305841991-05c297ba4575?auto=format&fit=crop&w=400&q=80' },
    { id: 4, name: 'Men\'s Casual Sneakers', price: '৳ 1,800', image: 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=400&q=80' },
]);
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
                <div class="flex items-center justify-between mb-6">
                    <h2 class="text-xl font-bold text-slate-800 md:text-2xl">Shop by Category</h2>
                    <RouterLink to="/categories" class="text-sm font-semibold text-blue-600 hover:underline">View All
                        &rarr;</RouterLink>
                </div>

                <div class="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-6">
                    <RouterLink v-for="cat in categories" :key="cat.name" :to="cat.link"
                        class="group flex flex-col items-center justify-center rounded-2xl bg-white p-5 shadow-sm border border-slate-100 transition-all hover:shadow-md hover:-translate-y-1">
                        <div
                            :class="['flex h-14 w-14 items-center justify-center rounded-full text-xl transition-transform group-hover:scale-110', cat.color]">
                            <i :class="cat.icon"></i>
                        </div>
                        <span class="mt-3 text-center text-sm font-semibold text-slate-700">{{ cat.name }}</span>
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

            <section class="mx-auto max-w-7xl px-4 py-8 pb-16 md:px-8">
                <div class="flex items-center justify-between mb-6">
                    <h2 class="text-xl font-bold text-slate-800 md:text-2xl">Trending Right Now <i
                            class="fas fa-fire text-orange-500 ml-1"></i></h2>
                </div>

                <div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
                    <div v-for="product in trendingProducts" :key="product.id"
                        class="group overflow-hidden rounded-2xl bg-white shadow-sm transition-all hover:shadow-xl border border-slate-200/60">
                        <div class="relative h-56 overflow-hidden bg-slate-100">
                            <img :src="product.image" :alt="product.name"
                                class="h-full w-full object-cover transition-transform duration-300 group-hover:scale-105" />
                            <div
                                class="absolute right-3 top-3 rounded-full bg-rose-500 px-2.5 py-1 text-[10px] font-bold text-white uppercase tracking-wider">
                                Hot
                            </div>
                        </div>
                        <div class="p-4">
                            <h3
                                class="text-sm font-semibold text-slate-800 truncate group-hover:text-blue-600 transition-colors">
                                {{ product.name }}</h3>
                            <p class="mt-1 text-lg font-black text-blue-600">{{ product.price }}</p>

                            <button
                                class="mt-3 flex items-center justify-center gap-2 w-full rounded-xl bg-slate-900 py-2.5 text-xs font-bold text-white transition-colors hover:bg-blue-600">
                                <i class="fas fa-shopping-cart"></i> Add to Cart
                            </button>
                        </div>
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