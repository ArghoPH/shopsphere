<script setup>
import { onMounted, reactive, ref } from "vue";
import { RouterLink, useRouter } from "vue-router";
import api from "../services/api";
import { auth, clearAuth } from "../stores/auth";
import AppNavbar from "../components/AppNavbar.vue";

const router = useRouter();

const users = ref([]);
const loading = ref(true);
const saving = ref(false);
const error = ref("");
const success = ref("");

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

onMounted(fetchUsers);
</script>

<template>
    <div class="min-h-screen bg-slate-100 text-slate-950">
        <AppNavbar />

        <main class="mx-auto max-w-7xl px-6 py-12 md:px-16">
            <div class="mb-8">
                <h1 class="text-4xl font-black">Master User Management</h1>
                <p class="mt-2 text-slate-500">
                    Logged in as {{ auth.fullName }} — {{ auth.role }}
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
                    <h2 class="text-2xl font-black">Create Login</h2>
                    <p class="mt-2 text-sm text-slate-500">
                        Create Admin or User login credentials.
                    </p>

                    <div class="mt-6 grid gap-4">
                        <div>
                            <label class="mb-2 block text-sm font-bold">Full Name</label>
                            <input v-model="form.fullName" type="text"
                                class="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none focus:border-slate-950" />
                        </div>

                        <div>
                            <label class="mb-2 block text-sm font-bold">Email</label>
                            <input v-model="form.email" type="email"
                                class="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none focus:border-slate-950" />
                        </div>

                        <div>
                            <label class="mb-2 block text-sm font-bold">Password</label>
                            <input v-model="form.password" type="password"
                                class="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none focus:border-slate-950" />
                        </div>

                        <div>
                            <label class="mb-2 block text-sm font-bold">Role</label>
                            <select v-model="form.role"
                                class="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none focus:border-slate-950">
                                <option value="User">User</option>
                                <option value="Admin">Admin</option>
                                <option value="MasterAdmin">MasterAdmin</option>
                            </select>
                        </div>

                        <button @click="createUser" :disabled="saving"
                            class="rounded-2xl bg-slate-950 px-5 py-4 font-bold text-white transition hover:bg-blue-600 disabled:opacity-60">
                            {{ saving ? "Creating..." : "Create Login" }}
                        </button>
                    </div>
                </div>

                <div class="lg:col-span-2">
                    <div v-if="loading" class="rounded-3xl bg-white p-10 text-center text-slate-500 shadow-sm">
                        Loading users...
                    </div>

                    <div v-else class="space-y-4">
                        <article v-for="user in users" :key="user.id"
                            class="rounded-3xl bg-white p-6 shadow-sm ring-1 ring-slate-200">
                            <div class="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
                                <div>
                                    <div class="flex flex-wrap gap-2">
                                        <span
                                            class="inline-flex h-8 items-center rounded-lg bg-blue-50 px-3 text-xs font-bold text-blue-700">
                                            {{ user.role }}
                                        </span>

                                        <span class="inline-flex h-8 items-center rounded-lg px-3 text-xs font-bold"
                                            :class="user.isActive
                                                ? 'bg-green-50 text-green-700'
                                                : 'bg-red-50 text-red-700'
                                                ">
                                            {{ user.isActive ? "Active" : "Inactive" }}
                                        </span>
                                    </div>

                                    <h2 class="mt-3 text-2xl font-black">
                                        {{ user.fullName }}
                                    </h2>

                                    <p class="mt-1 text-sm text-slate-500">
                                        {{ user.email }}
                                    </p>
                                </div>

                                <button @click="toggleStatus(user)"
                                    class="rounded-2xl px-5 py-3 text-sm font-bold transition" :class="user.isActive
                                        ? 'bg-red-50 text-red-600 hover:bg-red-100'
                                        : 'bg-green-50 text-green-700 hover:bg-green-100'
                                        ">
                                    {{ user.isActive ? "Deactivate" : "Activate" }}
                                </button>
                            </div>
                        </article>
                    </div>
                </div>
            </section>
        </main>
    </div>
</template>