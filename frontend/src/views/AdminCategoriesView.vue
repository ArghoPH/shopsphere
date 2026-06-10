<script setup>
import { onMounted, reactive, ref } from "vue";
import api from "../services/api";
import AppNavbar from "../components/AppNavbar.vue";

const categories = ref([]);
const loading = ref(true);
const saving = ref(false);
const uploadingImage = ref(false);

const error = ref("");
const success = ref("");
const editingCategoryId = ref(null);

const form = reactive({
    name: "",
    slug: "",
    imageUrl: "",
});

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
    form.imageUrl = "";
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

const uploadCategoryImage = async (event) => {
    const file = event.target.files?.[0];

    if (!file) return;

    uploadingImage.value = true;
    error.value = "";
    success.value = "";

    try {
        const formData = new FormData();
        formData.append("file", file);

        const response = await api.post("/admin/uploads/category-image", formData, {
            headers: {
                "Content-Type": "multipart/form-data",
            },
        });

        form.imageUrl = response.data.imageUrl;
        success.value = "Category image uploaded successfully.";
    } catch (err) {
        error.value =
            err.response?.data?.message ||
            err.response?.data?.details ||
            "Failed to upload category image.";
        console.error(err);
    } finally {
        uploadingImage.value = false;
        event.target.value = "";
    }
};

const removeCategoryImage = () => {
    form.imageUrl = "";
};

const saveCategory = async () => {
    saving.value = true;
    error.value = "";
    success.value = "";

    try {
        const payload = {
            name: form.name,
            slug: form.slug,
            imageUrl: form.imageUrl || null,
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
    form.imageUrl = category.imageUrl || "";
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

        if (editingCategoryId.value === categoryId) {
            resetForm();
        }

        await fetchCategories();
    } catch (err) {
        error.value = err.response?.data?.message || "Failed to delete category.";
        console.error(err);
    }
};

onMounted(() => {
    fetchCategories();
});
</script>

<template>
    <div class="min-h-screen bg-slate-100 text-slate-950">
        <AppNavbar />

        <main class="mx-auto max-w-7xl px-6 py-10 md:px-16">
            <div class="mb-8">
                <p class="text-xs font-black uppercase tracking-[0.25em] text-blue-600">
                    Admin Panel
                </p>

                <h1 class="mt-2 text-4xl font-black">
                    Category Management
                </h1>

                <p class="mt-2 text-slate-500">
                    Add, edit, delete, and manage category images.
                </p>
            </div>

            <div v-if="success" class="mb-6 rounded-2xl bg-green-50 p-4 text-sm font-bold text-green-700">
                {{ success }}
            </div>

            <div v-if="error" class="mb-6 rounded-2xl bg-red-50 p-4 text-sm font-bold text-red-600">
                {{ error }}
            </div>

            <section class="grid gap-8 lg:grid-cols-3">
                <!-- Form -->
                <div class="rounded-3xl bg-white p-6 shadow-sm ring-1 ring-slate-200">
                    <h2 class="text-2xl font-black">
                        {{ editingCategoryId ? "Edit Category" : "Add Category" }}
                    </h2>

                    <p class="mt-2 text-sm text-slate-500">
                        Category image will show on the home page category section.
                    </p>

                    <div class="mt-6 grid gap-4">
                        <div>
                            <label class="mb-2 block text-sm font-bold">
                                Category Name
                            </label>

                            <input v-model="form.name" @input="!editingCategoryId && generateSlug()" type="text"
                                class="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none focus:border-slate-950"
                                placeholder="Example: T-Shirts" />
                        </div>

                        <div>
                            <label class="mb-2 block text-sm font-bold">
                                Slug
                            </label>

                            <input v-model="form.slug" type="text"
                                class="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none focus:border-slate-950"
                                placeholder="example: t-shirts" />
                        </div>

                        <div>
                            <label class="mb-2 block text-sm font-bold">
                                Category Image
                            </label>

                            <input type="file" accept="image/jpeg,image/png,image/webp" @change="uploadCategoryImage"
                                class="w-full rounded-2xl border border-slate-300 bg-white px-4 py-3 text-sm outline-none focus:border-slate-950" />

                            <p v-if="uploadingImage"
                                class="mt-2 rounded-xl bg-blue-50 px-4 py-3 text-sm font-bold text-blue-700">
                                Uploading image...
                            </p>

                            <input v-model="form.imageUrl" type="text"
                                class="mt-3 w-full rounded-2xl border border-slate-300 px-4 py-3 text-sm outline-none focus:border-slate-950"
                                placeholder="Category image URL" />

                            <div v-if="form.imageUrl"
                                class="mt-4 overflow-hidden rounded-2xl border border-slate-200 bg-slate-50">
                                <img :src="form.imageUrl" alt="Category preview" class="h-40 w-full object-cover" />

                                <button type="button" @click="removeCategoryImage"
                                    class="w-full bg-red-50 px-4 py-3 text-sm font-bold text-red-600 transition hover:bg-red-100">
                                    Remove Image
                                </button>
                            </div>
                        </div>

                        <div class="grid grid-cols-2 gap-3">
                            <button type="button" @click="saveCategory" :disabled="saving || uploadingImage"
                                class="rounded-2xl bg-slate-950 px-5 py-4 text-sm font-bold text-white transition hover:bg-blue-600 disabled:cursor-not-allowed disabled:opacity-60">
                                {{
                                    saving
                                        ? "Saving..."
                                        : editingCategoryId
                                            ? "Update"
                                : "Create"
                                }}
                            </button>

                            <button type="button" @click="resetForm"
                                class="rounded-2xl border border-slate-300 px-5 py-4 text-sm font-bold transition hover:border-slate-950">
                                Clear
                            </button>
                        </div>
                    </div>
                </div>

                <!-- Category List -->
                <div class="lg:col-span-2">
                    <div v-if="loading" class="rounded-3xl bg-white p-10 text-center text-slate-500 shadow-sm">
                        Loading categories...
                    </div>

                    <div v-else-if="categories.length === 0"
                        class="rounded-3xl bg-white p-10 text-center shadow-sm ring-1 ring-slate-200">
                        <h2 class="text-2xl font-black">
                            No categories found
                        </h2>

                        <p class="mt-2 text-slate-500">
                            Create your first product category.
                        </p>
                    </div>

                    <div v-else class="grid gap-5 md:grid-cols-2">
                        <article v-for="category in categories" :key="category.id"
                            class="overflow-hidden rounded-3xl bg-white shadow-sm ring-1 ring-slate-200">
                            <div class="relative h-44 bg-slate-100">
                                <img v-if="category.imageUrl" :src="category.imageUrl" :alt="category.name"
                                    class="h-full w-full object-cover" />

                                <div v-else class="flex h-full w-full items-center justify-center text-slate-400">
                                    <i class="fa-solid fa-image text-4xl"></i>
                                </div>

                                <div
                                    class="absolute left-4 top-4 rounded-full bg-white/90 px-3 py-2 text-xs font-black text-slate-950 backdrop-blur">
                                    {{ category.productCount || 0 }} products
                                </div>
                            </div>

                            <div class="p-5">
                                <div class="flex flex-wrap gap-2">
                                    <span
                                        class="inline-flex h-8 items-center rounded-lg bg-blue-50 px-3 text-xs font-bold text-blue-700">
                                        {{ category.slug }}
                                    </span>
                                </div>

                                <h2 class="mt-3 text-2xl font-black">
                                    {{ category.name }}
                                </h2>

                                <div class="mt-5 flex flex-wrap gap-3">
                                    <button type="button" @click="editCategory(category)"
                                        class="rounded-2xl bg-slate-950 px-5 py-3 text-sm font-bold text-white transition hover:bg-blue-600">
                                        Edit
                                    </button>

                                    <button type="button" @click="deleteCategory(category.id)"
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