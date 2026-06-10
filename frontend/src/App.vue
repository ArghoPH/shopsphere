<script setup>
import { onMounted, ref } from "vue";
import api from "./services/api";

const products = ref([]);
const loading = ref(true);
const error = ref("");

const fetchProducts = async () => {
  try {
    const response = await api.get("/products");
    products.value = response.data;
  } catch (err) {
    error.value = "Failed to load products. Make sure backend is running.";
    console.error(err);
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  fetchProducts();
});
</script>

<template>
  <div class="min-h-screen bg-slate-100 text-slate-950">
    <header class="bg-slate-950 px-6 py-6 text-white md:px-16">
      <nav class="mx-auto flex max-w-7xl items-center justify-between">
        <h2 class="text-2xl font-bold tracking-tight">ShopSphere</h2>

        <div class="hidden items-center gap-8 text-sm text-slate-300 md:flex">
          <a href="#" class="transition hover:text-white">Home</a>
          <a href="#" class="transition hover:text-white">Products</a>
          <a href="#" class="transition hover:text-white">Cart</a>
        </div>
      </nav>

      <section class="mx-auto max-w-7xl py-20 md:py-28">
        <p class="mb-4 font-semibold text-blue-300">
          ASP.NET Core + Vue + Supabase
        </p>

        <h1 class="max-w-3xl text-4xl font-black leading-tight md:text-6xl">
          Build your modern e-commerce store
        </h1>

        <p class="mt-6 max-w-2xl text-base leading-8 text-slate-300 md:text-lg">
          A fully functional free-tier friendly e-commerce project with product
          listing, cart, checkout, Cash on Delivery and Mock Payment.
        </p>

        <div class="mt-8 flex flex-wrap gap-3">
          <span class="rounded-full bg-white/10 px-4 py-2 text-sm text-slate-200">
            Vue Frontend
          </span>
          <span class="rounded-full bg-white/10 px-4 py-2 text-sm text-slate-200">
            .NET API
          </span>
          <span class="rounded-full bg-white/10 px-4 py-2 text-sm text-slate-200">
            Supabase DB
          </span>
        </div>
      </section>
    </header>

    <main class="mx-auto -mt-10 max-w-7xl px-6 pb-16 md:px-16">
      <section class="rounded-3xl bg-white p-6 shadow-xl shadow-slate-200 md:p-8">
        <div class="flex flex-col gap-2 md:flex-row md:items-end md:justify-between">
          <div>
            <h2 class="text-3xl font-black tracking-tight">Featured Products</h2>
            <p class="mt-2 text-slate-500">
              Products loaded from Supabase PostgreSQL through ASP.NET Core API.
            </p>
          </div>

          <button
            class="mt-4 rounded-xl bg-slate-950 px-5 py-3 text-sm font-bold text-white transition hover:bg-blue-600 md:mt-0">
            View All
          </button>
        </div>
      </section>

      <section class="mt-8">
        <div v-if="loading" class="rounded-3xl bg-white p-10 text-center text-slate-500 shadow-sm">
          Loading products...
        </div>

        <div v-else-if="error" class="rounded-3xl bg-white p-10 text-center font-semibold text-red-600 shadow-sm">
          {{ error }}
        </div>

        <div v-else class="grid grid-cols-1 gap-6 md:grid-cols-2 xl:grid-cols-3">
          <article v-for="product in products" :key="product.id"
            class="group overflow-hidden rounded-3xl bg-white shadow-sm ring-1 ring-slate-200 transition hover:-translate-y-1 hover:shadow-xl hover:shadow-slate-200">
            <div class="aspect-square overflow-hidden bg-slate-200">
              <img :src="product.imageUrl" :alt="product.name"
                class="h-full w-full object-cover transition duration-300 group-hover:scale-105" />
            </div>

            <div class="p-6">
              <div class="mb-3 flex items-center justify-between">
                <span class="rounded-full bg-blue-50 px-3 py-1 text-xs font-bold text-blue-700">
                  {{ product.categoryName }}
                </span>

                <span class="text-sm text-slate-500">
                  Stock: {{ product.stock }}
                </span>
              </div>

              <h3 class="text-xl font-black text-slate-950">
                {{ product.name }}
              </h3>

              <p class="mt-3 min-h-14 text-sm leading-6 text-slate-500">
                {{ product.description }}
              </p>

              <div class="mt-6 flex items-center justify-between">
                <strong class="text-2xl font-black text-slate-950">
                  ৳{{ product.price }}
                </strong>
              </div>

              <button
                class="mt-6 w-full rounded-2xl bg-slate-950 px-5 py-4 text-sm font-bold text-white transition hover:bg-blue-600">
                Add to Cart
              </button>
            </div>
          </article>
        </div>
      </section>
    </main>
  </div>
</template>