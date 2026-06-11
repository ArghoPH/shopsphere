<script setup>
import { onMounted, reactive, ref, computed } from "vue";
import { useRouter } from "vue-router";
import api from "../services/api";
import { auth, clearAuth } from "../stores/auth";
import AppNavbar from "../components/AppNavbar.vue";

const router = useRouter();
const users = ref([]);
const loading = ref(true);
const saving = ref(false);
const error = ref("");
const success = ref("");
const searchQuery = ref(""); // সার্চ কুয়েরির জন্য নতুন স্টেট

const form = reactive({
    fullName: "",
    email: "",
    password: "",
    role: "User",
});

const logout = () => {
    clearAuth();
    router.push("/login");
};

const fetchUsers = async () => {
    loading.value = true;
    error.value = "";

    try {
        const response = await api.get("/master/users");
        users.value = response.data;
    } catch (err) {
        error.value = err.response?.data?.message || "Failed to load users.";
        console.error(err);
    } finally {
        loading.value = false;
    }
};

const createUser = async () => {
    saving.value = true;
    error.value = "";
    success.value = "";

    try {
        await api.post("/master/users", form);

        success.value = "Login created successfully.";

        form.fullName = "";
        form.email = "";
        form.password = "";
        form.role = "User";

        await fetchUsers();
    } catch (err) {
        error.value = err.response?.data?.message || "Failed to create login.";
        console.error(err);
    } finally {
        saving.value = false;
    }
};

const toggleStatus = async (user) => {
    error.value = "";
    success.value = "";

    try {
        await api.put(`/master/users/${user.id}/status`, !user.isActive, {
            headers: {
                "Content-Type": "application/json",
            },
        });

        success.value = "User status updated successfully.";
        await fetchUsers();
    } catch (err) {
        error.value = err.response?.data?.message || "Failed to update status.";
        console.error(err);
    }
};

// রিয়েল-টাইম সার্চ ফিল্টারিং (Name, Email, Role অনুযায়ী সার্চ হবে)
const filteredUsers = computed(() => {
    const query = searchQuery.value.toLowerCase().trim();
    if (!query) return users.value;

    return users.value.filter(user =>
        user.fullName?.toLowerCase().includes(query) ||
        user.email?.toLowerCase().includes(query) ||
        user.role?.toLowerCase().includes(query)
    );
});

// প্রিমিয়াম ড্যাশবোর্ড স্ট্যাটস এর জন্য কম্পিউটেড প্রোপার্টিজ
const totalUsersCount = computed(() => users.value.length);
const activeUsersCount = computed(() => users.value.filter(u => u.isActive).length);
const inactiveUsersCount = computed(() => users.value.filter(u => !u.isActive).length);

onMounted(fetchUsers);
</script>

<template>
    <div class="min-h-screen bg-slate-50 text-slate-900 antialiased selection:bg-slate-900 selection:text-white">
        <AppNavbar />

        <main class="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
            <div class="flex flex-col justify-between gap-4 border-b border-slate-200 pb-6 sm:flex-row sm:items-center">
                <div>
                    <h1 class="text-3xl font-black tracking-tight text-slate-900 sm:text-4xl">User Workspace</h1>
                    <p class="mt-1.5 text-sm text-slate-500 font-medium">
                        Logged in as <span class="font-semibold text-slate-800">{{ auth.fullName }}</span> &middot;
                        <span
                            class="rounded bg-slate-100 px-1.5 py-0.5 text-xs font-bold text-slate-600 uppercase tracking-wider">{{
                            auth.role }}</span>
                    </p>
                </div>
            </div>

            <div class="mt-8 grid grid-cols-1 gap-5 sm:grid-cols-3">
                <div
                    class="relative overflow-hidden rounded-2xl border border-slate-200 bg-white p-6 shadow-sm transition hover:shadow-md">
                    <p class="text-xs font-bold uppercase tracking-wider text-slate-400">Total System Logins</p>
                    <p class="mt-2 text-4xl font-black tracking-tight text-slate-900">{{ totalUsersCount }}</p>
                </div>
                <div
                    class="relative overflow-hidden rounded-2xl border border-slate-200 bg-white p-6 shadow-sm transition hover:shadow-md">
                    <p class="text-xs font-bold uppercase tracking-wider text-green-500">Active Accounts</p>
                    <p class="mt-2 text-4xl font-black tracking-tight text-green-600">{{ activeUsersCount }}</p>
                </div>
                <div
                    class="relative overflow-hidden rounded-2xl border border-slate-200 bg-white p-6 shadow-sm transition hover:shadow-md">
                    <p class="text-xs font-bold uppercase tracking-wider text-rose-500">Inactive Accounts</p>
                    <p class="mt-2 text-4xl font-black tracking-tight text-rose-600">{{ inactiveUsersCount }}</p>
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

                <div class="sticky top-6 rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
                    <h2 class="text-xl font-bold tracking-tight text-slate-900">Provision New Account</h2>
                    <p class="mt-1 text-xs text-slate-500">Configure corporate identity and role access permissions.</p>

                    <form @submit.prevent="createUser" class="mt-6 space-y-4">
                        <div>
                            <label class="mb-1.5 block text-xs font-bold tracking-wide text-slate-700 uppercase">Full
                                Name</label>
                            <input v-model="form.fullName" type="text" required placeholder="John Doe"
                                class="w-full rounded-xl border border-slate-200 bg-slate-50/50 px-4 py-2.5 text-sm outline-none transition focus:border-slate-900 focus:bg-white focus:ring-4 focus:ring-slate-900/5 placeholder:text-slate-400" />
                        </div>

                        <div>
                            <label class="mb-1.5 block text-xs font-bold tracking-wide text-slate-700 uppercase">Email
                                Address</label>
                            <input v-model="form.email" type="email" required placeholder="john@company.com"
                                class="w-full rounded-xl border border-slate-200 bg-slate-50/50 px-4 py-2.5 text-sm outline-none transition focus:border-slate-900 focus:bg-white focus:ring-4 focus:ring-slate-900/5 placeholder:text-slate-400" />
                        </div>

                        <div>
                            <label class="mb-1.5 block text-xs font-bold tracking-wide text-slate-700 uppercase">Secure
                                Password</label>
                            <input v-model="form.password" type="password" required placeholder="••••••••"
                                class="w-full rounded-xl border border-slate-200 bg-slate-50/50 px-4 py-2.5 text-sm outline-none transition focus:border-slate-900 focus:bg-white focus:ring-4 focus:ring-slate-900/5 placeholder:text-slate-400" />
                        </div>

                        <div>
                            <label
                                class="mb-1.5 block text-xs font-bold tracking-wide text-slate-700 uppercase">Security
                                Role</label>
                            <div class="relative">
                                <select v-model="form.role"
                                    class="w-full appearance-none rounded-xl border border-slate-200 bg-slate-50/50 px-4 py-2.5 text-sm outline-none transition focus:border-slate-900 focus:bg-white focus:ring-4 focus:ring-slate-900/5">
                                    <option value="User">Standard User</option>
                                    <option value="Admin">Administrator</option>
                                    <option value="MasterAdmin">Master Admin</option>
                                </select>
                            </div>
                        </div>

                        <button type="submit" :disabled="saving"
                            class="w-full mt-2 rounded-xl bg-slate-900 px-4 py-3 text-sm font-bold text-white shadow-sm transition hover:bg-slate-800 focus:ring-4 focus:ring-slate-900/20 active:scale-[0.98] disabled:opacity-50 disabled:pointer-events-none">
                            <span v-if="saving" class="flex items-center justify-center gap-2">
                                <svg class="animate-spin h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
                                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor"
                                        stroke-width="4"></circle>
                                    <path class="opacity-75" fill="currentColor"
                                        d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
                                    </path>
                                </svg>
                                Authenticating...
                            </span>
                            <span v-else>Deploy Account</span>
                        </button>
                    </form>
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
                            placeholder="Search by name, identity email, or system role..."
                            class="w-full bg-transparent text-sm text-slate-800 outline-none placeholder:text-slate-400" />
                        <span v-if="searchQuery"
                            class="text-xs bg-slate-100 text-slate-500 px-2 py-1 rounded-md font-mono">
                            {{ filteredUsers.length }} found
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
                        Syncing credentials ledger...
                    </div>

                    <div v-else class="max-h-[620px] overflow-y-auto pr-1 space-y-3 custom-scrollbar">
                        <div v-if="filteredUsers.length === 0"
                            class="rounded-2xl border border-dashed border-slate-200 bg-white p-12 text-center text-sm font-medium text-slate-400">
                            No active matching credentials found.
                        </div>

                        <article v-for="user in filteredUsers" :key="user.id"
                            class="group relative rounded-xl border border-slate-200 bg-white p-5 shadow-sm transition-all duration-200 hover:border-slate-300 hover:shadow-md">
                            <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
                                <div class="flex items-start gap-4">
                                    <div
                                        class="hidden sm:flex h-10 w-10 items-center justify-center rounded-full bg-slate-100 font-bold text-sm text-slate-700 border border-slate-200 uppercase">
                                        {{ user.fullName.charAt(0) }}
                                    </div>
                                    <div>
                                        <div class="flex flex-wrap items-center gap-2">
                                            <h3
                                                class="text-base font-bold text-slate-900 group-hover:text-black transition">
                                                {{ user.fullName }}
                                            </h3>

                                            <span
                                                class="inline-flex items-center rounded-md bg-slate-50 border border-slate-200 px-2 py-0.5 text-xs font-semibold text-slate-600 tracking-tight">
                                                {{ user.role }}
                                            </span>

                                            <span
                                                class="inline-flex items-center gap-1 rounded-md px-2 py-0.5 text-xs font-bold"
                                                :class="user.isActive ? 'bg-emerald-50 text-emerald-700 border border-emerald-200' : 'bg-rose-50 text-rose-700 border border-rose-200'">
                                                <span class="h-1.5 w-1.5 rounded-full"
                                                    :class="user.isActive ? 'bg-emerald-500' : 'bg-rose-500'"></span>
                                                {{ user.isActive ? "Active" : "Suspended" }}
                                            </span>
                                        </div>

                                        <p class="mt-1 text-xs font-medium text-slate-500">
                                            {{ user.email }}
                                        </p>
                                    </div>
                                </div>

                                <button @click="toggleStatus(user)"
                                    class="w-full sm:w-auto rounded-xl px-4 py-2 text-xs font-bold tracking-wide transition border focus:outline-none"
                                    :class="user.isActive
                                        ? 'bg-white border-rose-200 text-rose-600 hover:bg-rose-50/50 hover:border-rose-300'
                                        : 'bg-white border-emerald-200 text-emerald-700 hover:bg-emerald-50/50 hover:border-emerald-300'
                                        ">
                                    {{ user.isActive ? "Deactivate Access" : "Activate Access" }}
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
/* Scroller UI optimization for cleaner tracking experience */
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

/* Vue Animations */
.fade-enter-active,
.fade-leave-active {
    transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
    opacity: 0;
}
</style>