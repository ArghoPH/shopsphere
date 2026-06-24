<script setup>
import { onMounted, reactive, ref } from "vue";
import { RouterLink } from "vue-router";
import api from "../services/api";
import AppNavbar from "../components/AppNavbar.vue";

const products = ref([]);
const categories = ref([]);
const uploadingImage = ref(false);

const loading = ref(true);
const saving = ref(false);
const error = ref("");
const success = ref("");

const editingProductId = ref(null);

const form = reactive({
    categoryId: "",
    name: "",
    slug: "",
    description: "",
    price: 0,
    stock: 0,
    imageUrl: "",
    isActive: true,
});

const resetForm = () => {
    editingProductId.value = null;

    form.categoryId = "";
    form.name = "";
    form.slug = "";
    form.description = "";
    form.price = 0;
    form.stock = 0;
    form.imageUrl = "";
    form.isActive = true;
};

const generateSlug = () => {
    form.slug = form.name
        .toLowerCase()
        .trim()
        .replace(/[^a-z0-9]+/g, "-")
        .replace(/(^-|-$)/g, "");
};

const fetchProducts = async () => {
    try {
        const response = await api.get("/admin/products");
        products.value = response.data;
    } catch (err) {
        error.value = "Failed to load products.";
        console.error(err);
    }
};

const fetchCategories = async () => {
    try {
        const response = await api.get("/categories");
        categories.value = response.data;
    } catch (err) {
        error.value = "Failed to load categories.";
        console.error(err);
    }
};

const loadPage = async () => {
    loading.value = true;
    error.value = "";

    await Promise.all([fetchProducts(), fetchCategories()]);

    loading.value = false;
};

const uploadImage = async (event) => {
    const file = event.target.files?.[0];

    if (!file) return;

    uploadingImage.value = true;
    error.value = "";
    success.value = "";

    try {
        const formData = new FormData();
        formData.append("file", file);

        const response = await api.post("/admin/uploads/product-image", formData, {
            headers: {
                "Content-Type": "multipart/form-data",
            },
        });

        form.imageUrl = response.data.imageUrl;
        success.value = "Image uploaded successfully.";
    } catch (err) {
        error.value =
            err.response?.data?.message ||
            err.response?.data?.details ||
            "Failed to upload image.";
        console.error(err);
    } finally {
        uploadingImage.value = false;
    }
};

const saveProduct = async () => {
    saving.value = true;
    error.value = "";
    success.value = "";

    try {
        const payload = {
            categoryId: form.categoryId || null,
            name: form.name,
            slug: form.slug,
            description: form.description,
            price: Number(form.price),
            stock: Number(form.stock),
            imageUrl: form.imageUrl || null,
            isActive: form.isActive,
        };

        if (editingProductId.value) {
            await api.put(`/admin/products/${editingProductId.value}`, payload);
            success.value = "Product updated successfully.";
        } else {
            await api.post("/admin/products", payload);
            success.value = "Product created successfully.";
        }

        resetForm();
        await fetchProducts();
    } catch (err) {
        error.value = err.response?.data?.message || "Failed to save product.";
        console.error(err);
    } finally {
        saving.value = false;
    }
};

const editProduct = (product) => {
    editingProductId.value = product.id;

    form.categoryId = product.categoryId || "";
    form.name = product.name;
    form.slug = product.slug;
    form.description = product.description || "";
    form.price = product.price;
    form.stock = product.stock;
    form.imageUrl = product.imageUrl || "";
    form.isActive = product.isActive;
};

const deactivateProduct = async (productId) => {
    const confirmed = confirm("Are you sure you want to deactivate this product?");

    if (!confirmed) return;

    error.value = "";
    success.value = "";

    try {
        await api.delete(`/admin/products/${productId}`);
        success.value = "Product deactivated successfully.";
        await fetchProducts();
    } catch (err) {
        error.value = err.response?.data?.message || "Failed to deactivate product.";
        console.error(err);
    }
};

onMounted(loadPage);
</script>

<template>
    <div class="min-h-screen bg-slate-100 text-slate-950">
        <AppNavbar />

        <main class="mx-auto  max-w-7xl px-6 py-12 md:px-16">
            <div class="mb-8">
                <h1 class="text-4xl font-black">Admin Product Management</h1>
                <p class="mt-2 text-slate-500">
                    Add, edit, deactivate, and manage product stock.
                </p>
            </div>

            <div v-if="success" class="mb-6 rounded-2xl bg-green-50 p-4 text-sm font-bold text-green-700">
                {{ success }}
            </div>

            <div v-if="error" class="mb-6 rounded-2xl bg-red-50 p-4 text-sm font-bold text-red-600">
                {{ error }}
            </div>

            <div v-if="loading" class="rounded-3xl bg-white p-10 text-center text-slate-500 shadow-sm">
                Loading admin products...
            </div>

            <section v-else class="grid gap-8 lg:grid-cols-3">
                <div class="rounded-3xl bg-white p-6 shadow-sm ring-1 ring-slate-200 lg:col-span-1">
                    <h2 class="text-2xl font-black">
                        {{ editingProductId ? "Edit Product" : "Add Product" }}
                    </h2>

                    <div class="mt-6 grid gap-4">
                        <div>
                            <label class="mb-2 block text-sm font-bold text-slate-700">
                                Product Name
                            </label>
                            <input v-model="form.name" @input="!editingProductId && generateSlug()" type="text"
                                class="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none focus:border-slate-950"
                                placeholder="Product name" />
                        </div>

                        <div>
                            <label class="mb-2 block text-sm font-bold text-slate-700">
                                Slug
                            </label>
                            <input v-model="form.slug" type="text"
                                class="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none focus:border-slate-950"
                                placeholder="product-slug" />
                        </div>

                        <div>
                            <label class="mb-2 block text-sm font-bold text-slate-700">
                                Category
                            </label>
                            <select v-model="form.categoryId"
                                class="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none focus:border-slate-950">
                                <option value="">No Category</option>
                                <option v-for="category in categories" :key="category.id" :value="category.id">
                                    {{ category.name }}
                                </option>
                            </select>
                        </div>

                        <div>
                            <label class="mb-2 block text-sm font-bold text-slate-700">
                                Description
                            </label>
                            <textarea v-model="form.description" rows="4"
                                class="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none focus:border-slate-950"
                                placeholder="Product description"></textarea>
                        </div>

                        <div class="grid grid-cols-2 gap-3">
                            <div>
                                <label class="mb-2 block text-sm font-bold text-slate-700">
                                    Price
                                </label>
                                <input v-model.number="form.price" type="number"
                                    class="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none focus:border-slate-950" />
                            </div>

                            <div>
                                <label class="mb-2 block text-sm font-bold text-slate-700">
                                    Stock
                                </label>
                                <input v-model.number="form.stock" type="number"
                                    class="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none focus:border-slate-950" />
                            </div>
                        </div>

                        <div>
                            <label class="mb-2 block text-sm font-bold text-slate-700">
                                Product Image
                            </label>

                            <input type="file" accept="image/jpeg,image/png,image/webp" @change="uploadImage"
                                class="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm outline-none focus:border-slate-950" />

                            <p v-if="uploadingImage"
                                class="mt-2 rounded-xl bg-blue-50 px-4 py-3 text-sm font-bold text-blue-700">
                                Uploading product image...
                            </p>

                            <input v-model="form.imageUrl" type="text"
                                class="mt-3 w-full rounded-2xl border border-slate-300 px-4 py-3 text-sm outline-none focus:border-slate-950"
                                placeholder="Image URL will appear here" />

                            <img v-if="form.imageUrl" :src="form.imageUrl" alt="Product preview"
                                class="mt-4 h-40 w-full rounded-2xl object-cover ring-1 ring-slate-200" />
                        </div>

                        <label class="flex items-center gap-3 rounded-2xl bg-slate-50 p-4">
                            <input v-model="form.isActive" type="checkbox" class="h-5 w-5" />
                            <span class="font-bold">Active Product</span>
                        </label>

                        <div class="grid grid-cols-2 gap-3">
                            <button @click="saveProduct" :disabled="saving"
                                class="rounded-2xl bg-slate-950 px-5 py-4 text-sm font-bold text-white transition hover:bg-blue-600 disabled:opacity-60">
                                {{ saving ? "Saving..." : editingProductId ? "Update" : "Create" }}
                            </button>

                            <button @click="resetForm"
                                class="rounded-2xl border border-slate-300 px-5 py-4 text-sm font-bold transition hover:border-slate-950">
                                Clear
                            </button>
                        </div>
                    </div>
                </div>

                <div class="space-y-5 lg:col-span-2">
                    <article v-for="product in products" :key="product.id"
                        class="grid gap-5 rounded-3xl bg-white p-5 shadow-sm ring-1 ring-slate-200 md:grid-cols-[140px_1fr]">
                        <img :src="product.imageUrl || 'https://placehold.co/400x400?text=Product'" :alt="product.name"
                            class="h-36 w-full rounded-2xl object-cover md:h-36" />

                        <div>
                            <div class="flex flex-col gap-3 md:flex-row md:items-start md:justify-between">
                                <div>
                                    <div class="flex flex-wrap items-center gap-2">
                                        <span
                                            class="inline-flex h-8 items-center rounded-lg bg-blue-50 px-3 text-xs font-bold text-blue-700">
                                            {{ product.categoryName || "No Category" }}
                                        </span>

                                        <span class="inline-flex h-8 items-center rounded-lg px-3 text-xs font-bold"
                                            :class="product.isActive
                                                ? 'bg-green-50 text-green-700'
                                                : 'bg-red-50 text-red-700'
                                                ">
                                            {{ product.isActive ? "Active" : "Inactive" }}
                                        </span>
                                    </div>

                                    <h2 class="mt-3 text-2xl font-black">
                                        {{ product.name }}
                                    </h2>

                                    <p class="mt-2 text-sm text-slate-500">
                                        {{ product.description }}
                                    </p>
                                </div>

                                <div class="text-left md:text-right">
                                    <strong class="text-2xl font-black">
                                        ৳{{ product.price }}
                                    </strong>
                                    <p class="mt-1 text-sm text-slate-500">
                                        Stock: {{ product.stock }}
                                    </p>
                                </div>
                            </div>

                            <div class="mt-5 flex flex-wrap gap-3">
                                <button @click="editProduct(product)"
                                    class="rounded-2xl bg-slate-950 px-5 py-3 text-sm font-bold text-white transition hover:bg-blue-600">
                                    Edit
                                </button>

                                <button @click="deactivateProduct(product.id)"
                                    class="rounded-2xl bg-red-50 px-5 py-3 text-sm font-bold text-red-600 transition hover:bg-red-100">
                                    Deactivate
                                </button>
                            </div>
                        </div>
                    </article>
                </div>
            </section>
        </main>
    </div>
</template>