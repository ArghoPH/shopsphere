<script setup>
import { onMounted, reactive, ref, computed } from "vue";
import api from "../services/api";
import AppNavbar from "../components/AppNavbar.vue";

const categories = ref([]);
const loading = ref(true);
const saving = ref(false);
const uploadingImage = ref(false);
const draggingCategoryImage = ref(false);

const error = ref("");
const success = ref("");
const editingCategoryId = ref(null);
const searchQuery = ref(""); // সার্চ কুয়েরির জন্য নতুন স্টেট

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

const uploadCategoryImageFile = async (file) => {
    if (!file) return;

    if (!file.type.startsWith("image/")) {
        error.value = "Please drop a valid image file.";
        return;
    }

    uploadingImage.value = true;
    draggingCategoryImage.value = false;
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
    }
};

const handleCategoryImageDragEnter = (event) => {
    event.preventDefault();

    draggingCategoryImage.value = true;
};

const handleCategoryImageDragLeave = (event) => {
    event.preventDefault();

    const currentTarget = event.currentTarget;
    const nextTarget = event.relatedTarget;

    if (currentTarget && nextTarget && currentTarget.contains(nextTarget)) {
        return;
    }

    draggingCategoryImage.value = false;
};

const handleCategoryImageDrop = async (event) => {
    event.preventDefault();

    draggingCategoryImage.value = false;

    const file = event.dataTransfer.files?.[0];

    await uploadCategoryImageFile(file);
};






const uploadCategoryImage = async (event) => {
    const file = event.target.files?.[0];

    await uploadCategoryImageFile(file);

    event.target.value = "";
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

// রিয়েল-টাইম সার্চ ফিল্টার (Name এবং Slug দিয়ে ক্যাটাগরি খোঁজা যাবে)
const filteredCategories = computed(() => {
    const query = searchQuery.value.toLowerCase().trim();
    if (!query) return categories.value;
    return categories.value.filter(cat =>
        cat.name?.toLowerCase().includes(query) ||
        cat.slug?.toLowerCase().includes(query)
    );
});

// ড্যাশবোর্ড স্ট্যাটস গণনার জন্য Computed Variables
const totalCategories = computed(() => categories.value.length);
const totalLinkedProducts = computed(() => {
    return categories.value.reduce((sum, cat) => sum + (cat.productCount || 0), 0);
});
onMounted(() => {
    fetchCategories();
});
</script>

<template>
    <div class="min-h-screen bg-slate-50 text-slate-900 antialiased selection:bg-slate-900 selection:text-white">
        <AppNavbar />

        <main class="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
            <div class="flex flex-col justify-between gap-4 border-b border-slate-200 pb-6 sm:flex-row sm:items-center">
                <div>
                    <span class="text-xs font-bold uppercase tracking-[0.2em] text-blue-600 block mb-1">Admin
                        Ledger</span>
                    <h1 class="text-3xl font-black tracking-tight text-slate-900 sm:text-4xl">Category Management</h1>
                    <p class="mt-1 text-sm text-slate-500 font-medium">
                        Structure your inventory taxonomy, slug pathways, and storefront visual assets.
                    </p>
                </div>
            </div>

            <div class="mt-8 grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-3">
                <div
                    class="relative overflow-hidden rounded-2xl border border-slate-200 bg-white p-6 shadow-sm transition hover:shadow-md">
                    <p class="text-xs font-bold uppercase tracking-wider text-slate-400">Total Catalog Segments</p>
                    <p class="mt-2 text-4xl font-black tracking-tight text-slate-900">{{ totalCategories }}</p>
                </div>
                <div
                    class="relative overflow-hidden rounded-2xl border border-slate-200 bg-white p-6 shadow-sm transition hover:shadow-md">
                    <p class="text-xs font-bold uppercase tracking-wider text-blue-500">Categorized Products</p>
                    <p class="mt-2 text-4xl font-black tracking-tight text-blue-600">{{ totalLinkedProducts }}</p>
                </div>
            </div>

            <div class="mt-6">
                <Transition name="fade">
                    <div v-if="success"
                        class="flex items-center gap-3 rounded-xl border border-emerald-200 bg-emerald-50/70 p-4 text-sm font-semibold text-emerald-800 backdrop-blur-sm">
                        <svg class="h-5 w-5 text-emerald-600 flex-shrink-0" fill="none" viewBox="0 0 24 24"
                            stroke-width="2.5" stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round"
                                d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                        </svg>
                        {{ success }}
                    </div>
                </Transition>
                <Transition name="fade">
                    <div v-if="error"
                        class="flex items-center gap-3 rounded-xl border border-rose-200 bg-rose-50/70 p-4 text-sm font-semibold text-rose-700 backdrop-blur-sm">
                        <svg class="h-5 w-5 text-rose-600 flex-shrink-0" fill="none" viewBox="0 0 24 24"
                            stroke-width="2.5" stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round"
                                d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                        </svg>
                        {{ error }}
                    </div>
                </Transition>
            </div>

            <section class="mt-8 grid gap-8 lg:grid-cols-3 items-start">

                <div @dragover.prevent @dragenter.prevent="handleCategoryImageDragEnter"
                    @dragleave.prevent="handleCategoryImageDragLeave" @drop.prevent="handleCategoryImageDrop"
                    class="sticky top-6 relative overflow-hidden rounded-2xl border bg-white p-6 shadow-sm transition-all duration-200"
                    :class="draggingCategoryImage
                        ? 'border-blue-500 ring-4 ring-blue-100 shadow-xl shadow-blue-100'
                        : 'border-slate-200'
                        ">
                    <Transition enter-active-class="transition duration-200 ease-out"
                        enter-from-class="opacity-0 scale-95" enter-to-class="opacity-100 scale-100"
                        leave-active-class="transition duration-150 ease-in" leave-from-class="opacity-100 scale-100"
                        leave-to-class="opacity-0 scale-95">
                        <div v-if="draggingCategoryImage"
                            class="pointer-events-none absolute inset-0 z-30 flex flex-col items-center justify-center rounded-2xl border-2 border-dashed border-blue-500 bg-blue-50/95 text-center backdrop-blur-sm">
                            <div
                                class="flex h-20 w-20 items-center justify-center rounded-full bg-blue-600 text-white shadow-lg shadow-blue-200">
                                <i class="fa-solid fa-cloud-arrow-up text-4xl"></i>
                            </div>

                            <h3 class="mt-5 text-2xl font-black text-blue-700">
                                Drop image here
                            </h3>

                            <p class="mt-2 max-w-xs text-sm font-semibold leading-6 text-blue-500">
                                Release your image anywhere inside this category form to upload it.
                            </p>
                        </div>
                    </Transition>
                    <h2 class="text-xl font-bold tracking-tight text-slate-900">
                        {{ editingCategoryId ? "Modify Category" : "Provision Category" }}
                    </h2>
                    <p class="mt-1 text-xs text-slate-500">
                        Visual thumbnails dynamically serve the public home page grids.
                    </p>

                    <div class="mt-6 space-y-4">
                        <div>
                            <label
                                class="mb-1.5 block text-xs font-bold tracking-wide text-slate-700 uppercase">Category
                                Name</label>
                            <input v-model="form.name" @input="!editingCategoryId && generateSlug()" type="text"
                                class="w-full rounded-xl border border-slate-200 bg-slate-50/50 px-4 py-2.5 text-sm outline-none transition focus:border-slate-900 focus:bg-white focus:ring-4 focus:ring-slate-900/5 placeholder:text-slate-400"
                                placeholder="Example: Essential T-Shirts" />
                        </div>

                        <div>
                            <label class="mb-1.5 block text-xs font-bold tracking-wide text-slate-700 uppercase">URL
                                Slug (SEO Pathway)</label>
                            <input v-model="form.slug" type="text"
                                class="w-full rounded-xl border border-slate-200 bg-slate-50/50 px-4 py-2.5 text-sm outline-none transition focus:border-slate-900 focus:bg-white focus:ring-4 focus:ring-slate-900/5 placeholder:text-slate-400"
                                placeholder="example-t-shirts" />
                        </div>

                        <div>
                            <label
                                class="mb-1.5 block text-xs font-bold tracking-wide text-slate-700 uppercase">Category
                                Media Thumbnail</label>

                            <label
                                class="group flex flex-col items-center justify-center w-full h-24 border-2 border-dashed border-slate-200 rounded-xl cursor-pointer bg-slate-50/40 hover:bg-slate-50 hover:border-blue-500 transition-all">
                                <div class="flex flex-col items-center justify-center pt-2 pb-2 text-center px-4">
                                    <svg class="w-6 h-6 mb-1 text-slate-400 group-hover:text-slate-600 transition"
                                        fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                            d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 002-2H4a2 2 0 00-2 2v12a2 2 0 002 2z" />
                                    </svg>
                                    <p class="text-xs text-slate-500 font-medium"><span
                                            class="font-bold text-slate-800">Click to upload</span> or drag image</p>
                                    <p class="text-[10px] text-slate-400 mt-0.5">WEBP, PNG, JPG</p>
                                </div>
                                <input type="file" accept="image/jpeg,image/png,image/webp"
                                    @change="uploadCategoryImage" class="hidden" />
                            </label>

                            <div v-if="uploadingImage"
                                class="mt-2 flex items-center gap-2 rounded-xl bg-blue-50/80 px-4 py-2.5 text-xs font-bold text-blue-700">
                                <svg class="animate-spin h-3.5 w-3.5 text-blue-600" fill="none" viewBox="0 0 24 24">
                                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor"
                                        stroke-width="4"></circle>
                                    <path class="opacity-75" fill="currentColor"
                                        d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
                                    </path>
                                </svg>
                                Uploading image asset...
                            </div>

                            <input v-model="form.imageUrl" type="text"
                                class="mt-3 w-full rounded-xl border border-slate-200 bg-slate-50/50 px-4 py-2 text-xs outline-none transition focus:border-slate-900 focus:bg-white placeholder:text-slate-400"
                                placeholder="Or inject static asset URL string directly..." />

                            <div v-if="form.imageUrl"
                                class="mt-4 overflow-hidden rounded-xl border border-slate-200 bg-slate-50 relative group">
                                <img :src="form.imageUrl" alt="Category preview" class="h-32 w-full object-cover" />
                                <button type="button" @click="removeCategoryImage"
                                    class="absolute top-2 right-2 rounded-lg bg-white/90 px-2.5 py-1 text-xs font-bold text-rose-600 shadow-sm backdrop-blur border border-rose-100 hover:bg-rose-50 transition">
                                    Remove
                                </button>
                            </div>
                        </div>

                        <div class="grid grid-cols-2 gap-3 pt-2">
                            <button type="button" @click="saveCategory" :disabled="saving || uploadingImage"
                                class="rounded-xl bg-slate-900 px-4 py-3 text-sm font-bold text-white shadow-sm transition hover:bg-slate-800 focus:ring-4 focus:ring-slate-900/20 active:scale-[0.98] disabled:opacity-50 disabled:pointer-events-none">
                                <span v-if="saving" class="flex items-center justify-center gap-1.5">
                                    <svg class="animate-spin h-3.5 w-3.5 text-white" fill="none" viewBox="0 0 24 24">
                                        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor"
                                            stroke-width="4"></circle>
                                        <path class="opacity-75" fill="currentColor"
                                            d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
                                        </path>
                                    </svg>
                                    Syncing...
                                </span>
                                <span v-else>{{ editingCategoryId ? "Save Changes" : "Commit Segment" }}</span>
                            </button>

                            <button type="button" @click="resetForm"
                                class="rounded-xl border border-slate-200 px-4 py-3 text-sm font-bold text-slate-600 bg-white hover:bg-slate-50 hover:border-slate-300 transition active:scale-[0.98]">
                                Clear Form
                            </button>
                        </div>
                    </div>
                </div>

                <div class="lg:col-span-2 space-y-4">

                    <div
                        class="relative rounded-2xl border border-slate-200 bg-white p-3 shadow-sm flex items-center gap-3">
                        <svg class="h-5 w-5 text-slate-400 ml-2 flex-shrink-0" fill="none" viewBox="0 0 24 24"
                            stroke-width="2" stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round"
                                d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                        </svg>
                        <input v-model="searchQuery" type="text"
                            placeholder="Filter active structures by name, search keyword or slug string..."
                            class="w-full bg-transparent text-sm text-slate-800 outline-none placeholder:text-slate-400" />
                        <span v-if="searchQuery"
                            class="text-xs bg-slate-100 text-slate-500 px-2 py-1 rounded-md font-mono flex-shrink-0">
                            {{ filteredCategories.length }} matching
                        </span>
                    </div>

                    <div v-if="loading"
                        class="rounded-2xl border border-slate-200 bg-white p-16 text-center text-sm font-medium text-slate-400 shadow-sm flex flex-col items-center justify-center gap-3">
                        <svg class="animate-spin h-6 w-6 text-slate-600" fill="none" viewBox="0 0 24 24">
                            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4">
                            </circle>
                            <path class="opacity-75" fill="currentColor"
                                d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
                            </path>
                        </svg>
                        Resolving schema records...
                    </div>

                    <div v-else class="max-h-[680px] overflow-y-auto pr-1 space-y-3 custom-scrollbar">
                        <div v-if="filteredCategories.length === 0"
                            class="rounded-2xl border border-dashed border-slate-200 bg-white p-16 text-center text-sm font-medium text-slate-400">
                            No matching ecosystem categories registered.
                        </div>

                        <article v-for="category in filteredCategories" :key="category.id"
                            class="group relative rounded-xl border border-slate-200 bg-white p-4 shadow-sm transition-all duration-200 hover:border-slate-300 hover:shadow-md flex flex-col sm:flex-row sm:items-center justify-between gap-4">

                            <div class="flex items-center gap-4">
                                <div
                                    class="h-16 w-16 rounded-xl overflow-hidden bg-slate-100 border border-slate-150 flex-shrink-0 relative">
                                    <img v-if="category.imageUrl" :src="category.imageUrl" :alt="category.name"
                                        class="h-full w-full object-cover transition duration-300 group-hover:scale-105" />
                                    <div v-else class="flex h-full w-full items-center justify-center text-slate-300">
                                        <svg class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"
                                                d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 002-2H4a2 2 0 00-2 2v12a2 2 0 002 2z" />
                                        </svg>
                                    </div>
                                </div>

                                <div>
                                    <div class="flex flex-wrap items-center gap-2">
                                        <h3
                                            class="text-base font-bold text-slate-900 group-hover:text-black transition">
                                            {{ category.name }}
                                        </h3>
                                        <span
                                            class="inline-flex h-5 items-center rounded-md bg-slate-100 border border-slate-200 px-1.5 text-[10px] font-mono text-slate-500">
                                            /{{ category.slug }}
                                        </span>
                                    </div>
                                    <p class="mt-1 text-xs font-semibold text-blue-600 flex items-center gap-1">
                                        <svg class="h-3.5 w-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                                d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
                                        </svg>
                                        {{ category.productCount || 0 }} linked items
                                    </p>
                                </div>
                            </div>

                            <div
                                class="flex items-center gap-2 sm:justify-end border-t border-slate-100 pt-3 sm:border-0 sm:pt-0">
                                <button type="button" @click="editCategory(category)"
                                    class="rounded-lg bg-white border border-slate-200 px-3 py-1.5 text-xs font-bold text-slate-700 shadow-sm hover:bg-slate-50 hover:border-slate-300 transition focus:outline-none">
                                    Edit
                                </button>
                                <button type="button" @click="deleteCategory(category.id)"
                                    class="rounded-lg bg-white border border-rose-200 px-3 py-1.5 text-xs font-bold text-rose-600 shadow-sm hover:bg-rose-50/60 hover:border-rose-300 transition focus:outline-none">
                                    Delete
                                </button>
                            </div>

                        </article>
                    </div>
                </div>
            </section>
        </main>
    </div>
</template>

<style scoped>
/* Custom Scroller CSS styling setup */
.custom-scrollbar::-webkit-scrollbar {
    width: 6px;
}

.custom-scrollbar::-webkit-scrollbar-track {
    background: transparent;
}

.custom-scrollbar::-webkit-scrollbar-thumb {
    background-color: #cbd5e1;
    border-radius: 20px;
}

.custom-scrollbar::-webkit-scrollbar-thumb:hover {
    background-color: #94a3b8;
}

/* Vue Transitions animations module */
.fade-enter-active,
.fade-leave-active {
    transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
    opacity: 0;
}
</style>