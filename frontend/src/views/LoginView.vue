<script setup>
import { reactive, ref } from "vue";
import { RouterLink, useRouter } from "vue-router";
import api from "../services/api";
import { setAuth } from "../stores/auth";

const router = useRouter();

const loading = ref(false);
const error = ref("");

const form = reactive({
    email: "",
    password: "",
});

const login = async () => {
    loading.value = true;
    error.value = "";

    try {
        const response = await api.post("/auth/login", form);

        setAuth(response.data);

        if (response.data.role === "MasterAdmin") {
            router.push("/master/users");
        } else if (response.data.role === "Admin") {
            router.push("/admin/orders");
        } else {
            router.push("/");
        }
    } catch (err) {
        error.value = err.response?.data?.message || "Login failed.";
        console.error(err);
    } finally {
        loading.value = false;
    }
};
</script>

<template>
    <div class="flex min-h-screen items-center justify-center bg-slate-100 px-6 text-slate-950">
        <div class="w-full max-w-md rounded-3xl bg-white p-8 shadow-sm ring-1 ring-slate-200">
            <h1 class="text-4xl font-black">Login</h1>
            <p class="mt-2 text-slate-500">Access your ShopSphere account.</p>

            <div v-if="error" class="mt-6 rounded-2xl bg-red-50 p-4 text-sm font-bold text-red-600">
                {{ error }}
            </div>

            <div class="mt-6 grid gap-4">
                <div>
                    <label class="mb-2 block text-sm font-bold">Email</label>
                    <input v-model="form.email" type="email"
                        class="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none focus:border-slate-950"
                        placeholder="email@example.com" />
                </div>

                <div>
                    <label class="mb-2 block text-sm font-bold">Password</label>
                    <input v-model="form.password" type="password"
                        class="w-full rounded-2xl border border-slate-300 px-4 py-3 outline-none focus:border-slate-950"
                        placeholder="Password" />
                </div>

                <button @click="login" :disabled="loading"
                    class="rounded-2xl bg-slate-950 px-5 py-4 font-bold text-white transition hover:bg-blue-600 disabled:opacity-60">
                    {{ loading ? "Logging in..." : "Login" }}
                </button>

                <p class="text-center text-sm text-slate-500">
                    New customer?
                    <RouterLink to="/register" class="font-bold text-slate-950">
                        Create account
                    </RouterLink>
                </p>
            </div>
        </div>
    </div>
</template>