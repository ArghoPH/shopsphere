<script setup>
import { onMounted, reactive, ref } from "vue";
import { RouterLink, useRouter } from "vue-router";
import api from "../services/api";
import { clearAuth } from "../stores/auth";

const router = useRouter();

const categories = ref([]);
const loading = ref(true);
const saving = ref(false);
const error = ref("");
const success = ref("");
const editingCategoryId = ref(null);

const form = reactive({
    name: "",
    slug: "",
});

const logout = () => {
    clearAuth();
    router.push("/login");
};

const generateSlug = () => {
    form.slug = form.name
        .toLowerCase()
        .trim()
        .replace(/[^a-z0-9]+/g, "-")
        .replace(/(^-|-$)/g, "");
};

const resetForm = () => {
    editingCategoryId.value = null;
    form.name = "";
    form.slug = "";
};

const fetchCategories = async () => {
    loading.value = true;
    error.value = "";

    try {
        const response = await api.get("/admin/categories");
        categories.value = response.data;
    } catch (err) {
        error.value = err.response?.data?.message || "Failed to load categories.";
        console.error(err);
    } finally {
        loading.value = false;
    }
};

const saveCategory = async () => {
    saving.value = true;
    error.value = "";
    success.value = "";

    try {
        const payload = {
            name: form.name,
            slug: form.slug,
        };

        if (editingCategoryId.value) {
            await api.put(`/admin/categories/${editingCategoryId.value}`, payload);
            success.value = "Category updated successfully.";
        } else {
            await api.post("/admin/categories", payload);
            success.value = "Category created successfully.";
        }

        resetForm();
        await fetchCategories();
    } catch (err) {
        error.value = err.response?.data?.message || "Failed to save category.";
        console.error(err);
    } finally {
        saving.value = false;
    }
};

const editCategory = (category) => {
    editingCategoryId.value = category.id;
    form.name = category.name;
    form.slug = category.slug;
};

const deleteCategory = async (categoryId) => {
    const confirmed = confirm(
        "Are you sure you want to delete this category? Products under this category will become uncategorized."
    );

    if (!confirmed) return;

    error.value = "";
    success.value = "";

    try {
        await api.delete(`/admin/categories/${categoryId}`);
        success.value = "Category deleted successfully.";
        await fetchCategories();
    } catch (err) {
        error.value = err.response?.data?.message || "Failed to delete category.";
        console.error(err);
    }
};

onMounted(fetchCategories);
</script>

<template>
    <div class="min-h-screen bg-slate-100 text-slate-950">
        <header class="bg-slate-950 px-6 py-4 text-white md:px-16">
            <nav class="mx-auto flex max-w-7xl items-center justify-between">
                <RouterLink to="/" class="text-2xl font-bold">
                    ShopSphere Admin
                </RouterLink>

                <div class="flex flex-wrap items-center gap-3 text-xs font-bold">
                    <RouterLink to="/admin/orders"
                        class="rounded-lg bg-white/10 px-4 py-2 transition hover:bg-white/20">
                        Orders
                    </RouterLink>

                    <RouterLink to="/admin/products"
                        class="rounded-lg bg-white/10 px-4 py-2 transition hover:bg-white/20">
                        Products
                    </RouterLink>

                    <RouterLink to="/admin/categories"
                        class="rounded-lg bg-blue-600 px-4 py-2 transition hover:bg-blue-700">
                        Categories
                    </RouterLink>

                    <button @click="logout" class="rounded-lg bg-red-500 px-4 py-2 transition hover:bg-red-600">
                        Logout
                    </button>
                </div>
            </nav>
        </header>

        <main class="mx-auto max-w-7xl px-6 py-10 md:px-16">
            <div class="mb-8">
                <h1 class="text-4xl font-black">Admin Category Management</h1>
                <p class="mt-2 text-slate-500">
                    Add, edit, and delete product categories.
                </p>
            </div>

            <div v-if="success" class="mb-6 rounded-2xl bg-green-50 p-4 text-sm font-bold text-green-700">
                {{ success }}
            </div>

            <div v-if="error" class="mb-6 rounded-2xl bg-red-50 p-4 text-sm font-bold text-red-600">
                {{ error }}
            </div>

            <section class="grid gap-8 lg:grid-cols-3">
                <div class="rounded-3xl bg-white p-6 shadow-sm ring-1 ring-slate-200">
                    <h2 class="text-2xl font-black">
                        {{ editingCategoryId ? "Edit Category" : "Add Category" }}
                    </h2>

                    <div class="mt-6 grid gap-4">
                        <div>
                            <label class="mb-2 block text-sm font-bold">Category Name</label>
                            <input v-model="form.name" @input="!editingCategoryId && generateSlug()" type="text"
                                class="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none focus:border-slate-950"
                                placeholder="Example: T-Shirts" />
                        </div>

                        <div>
                            <label class="mb-2 block text-sm font-bold">Slug</label>
                            <input v-model="form.slug" type="text"
                                class="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none focus:border-slate-950"
                                placeholder="example: t-shirts" />
                        </div>

                        <div class="grid grid-cols-2 gap-3">
                            <button @click="saveCategory" :disabled="saving"
                                class="rounded-2xl bg-slate-950 px-5 py-4 text-sm font-bold text-white transition hover:bg-blue-600 disabled:opacity-60">
                                {{ saving ? "Saving..." : editingCategoryId ? "Update" : "Create" }}
                            </button>

                            <button @click="resetForm"
                                class="rounded-2xl border border-slate-300 px-5 py-4 text-sm font-bold transition hover:border-slate-950">
                                Clear
                            </button>
                        </div>
                    </div>
                </div>

                <div class="lg:col-span-2">
                    <div v-if="loading" class="rounded-3xl bg-white p-10 text-center text-slate-500 shadow-sm">
                        Loading categories...
                    </div>

                    <div v-else class="space-y-4">
                        <article v-for="category in categories" :key="category.id"
                            class="rounded-3xl bg-white p-6 shadow-sm ring-1 ring-slate-200">
                            <div class="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
                                <div>
                                    <div class="flex flex-wrap gap-2">
                                        <span
                                            class="inline-flex h-8 items-center rounded-lg bg-blue-50 px-3 text-xs font-bold text-blue-700">
                                            {{ category.slug }}
                                        </span>

                                        <span
                                            class="inline-flex h-8 items-center rounded-lg bg-slate-100 px-3 text-xs font-bold text-slate-700">
                                            {{ category.productCount }} products
                                        </span>
                                    </div>

                                    <h2 class="mt-3 text-2xl font-black">
                                        {{ category.name }}
                                    </h2>
                                </div>

                                <div class="flex flex-wrap gap-3">
                                    <button @click="editCategory(category)"
                                        class="rounded-2xl bg-slate-950 px-5 py-3 text-sm font-bold text-white transition hover:bg-blue-600">
                                        Edit
                                    </button>

                                    <button @click="deleteCategory(category.id)"
                                        class="rounded-2xl bg-red-50 px-5 py-3 text-sm font-bold text-red-600 transition hover:bg-red-100">
                                        Delete
                                    </button>
                                </div>
                            </div>
                        </article>
                    </div>
                </div>
            </section>
        </main>
    </div>
</template>