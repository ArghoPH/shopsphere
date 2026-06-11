<script setup>
const currentYear = new Date().getFullYear();
</script>

<template>
    <footer class="border-t border-slate-900 bg-slate-950 text-white font-sans selection:bg-blue-500/30">
        <div class="mx-auto max-w-7xl px-4 py-16 md:px-8">

            <div class="grid gap-10 md:grid-cols-4 lg:gap-12">

                <div class="md:col-span-2 space-y-5">
                    <RouterLink to="/" class="inline-flex items-center gap-3 group">
                        <div
                            class="flex h-11 w-11 items-center justify-center rounded-xl bg-gradient-to-br from-blue-500 to-blue-600 text-lg font-bold text-white shadow-lg shadow-blue-500/20 transition duration-300 group-hover:scale-105">
                            S
                        </div>

                        <div>
                            <h2
                                class="text-xl font-bold tracking-tight text-slate-100 group-hover:text-blue-400 transition">
                                ShopSphere
                            </h2>
                            <p class="text-xs font-semibold text-slate-400 uppercase tracking-wider mt-0.5">
                                Smart shopping, simple checkout.
                            </p>
                        </div>
                    </RouterLink>

                    <p class="max-w-md text-sm leading-relaxed text-slate-400">
                        ShopSphere is a modern full-stack e-commerce architecture engineered seamlessly with
                        <span class="text-slate-200">ASP.NET Core</span>, <span class="text-slate-200">Vue 3</span>,
                        <span class="text-slate-200">Tailwind CSS</span>, and cloud microservices.
                    </p>

                    <div class="flex items-center gap-3 pt-2">
                        <a href="https://github.com/ArghoPH" target="_blank" rel="noopener noreferrer"
                            class="flex h-9 w-9 items-center justify-center rounded-xl bg-slate-900 border border-slate-800 text-slate-400 transition duration-200 hover:bg-blue-600 hover:text-white hover:border-blue-600 hover:-translate-y-0.5"
                            aria-label="GitHub Repository Profiles">
                            <i class="fab fa-github text-sm"></i>
                        </a>

                        <a href="mailto:arghochakma2023@gmail.com"
                            class="flex h-9 w-9 items-center justify-center rounded-xl bg-slate-900 border border-slate-800 text-slate-400 transition duration-200 hover:bg-blue-600 hover:text-white hover:border-blue-600 hover:-translate-y-0.5"
                            aria-label="Direct Email Gateway Pipeline">
                            <i class="fas fa-envelope text-sm"></i>
                        </a>

                        <a href="https://shopsphere-api-2mb2.onrender.com" target="_blank" rel="noopener noreferrer"
                            class="flex h-9 w-9 items-center justify-center rounded-xl bg-slate-900 border border-slate-800 text-slate-400 transition duration-200 hover:bg-blue-600 hover:text-white hover:border-blue-600 hover:-translate-y-0.5"
                            aria-label="Backend Core Rest API Spec Engine">
                            <i class="fas fa-server text-sm"></i>
                        </a>
                    </div>
                </div>

                <div>
                    <h3
                        class="text-xs font-bold uppercase tracking-widest text-slate-200 border-b border-slate-900 pb-2">
                        Navigation Links
                    </h3>

                    <div class="mt-4 grid gap-2.5 text-sm text-slate-400">
                        <RouterLink to="/" class="hover:text-blue-400 transition inline-fit w-fit group">
                            <span
                                class="relative py-0.5 after:absolute after:bottom-0 after:left-0 after:h-[1px] after:w-0 after:bg-blue-400 after:transition-all group-hover:after:w-full">Home
                                Overview</span>
                        </RouterLink>

                        <RouterLink to="/products" class="hover:text-blue-400 transition inline-fit w-fit group">
                            <span
                                class="relative py-0.5 after:absolute after:bottom-0 after:left-0 after:h-[1px] after:w-0 after:bg-blue-400 after:transition-all group-hover:after:w-full">Product
                                Catalog</span>
                        </RouterLink>

                        <RouterLink to="/cart" class="hover:text-blue-400 transition inline-fit w-fit group">
                            <span
                                class="relative py-0.5 after:absolute after:bottom-0 after:left-0 after:h-[1px] after:w-0 after:bg-blue-400 after:transition-all group-hover:after:w-full">Shopping
                                Cart</span>
                        </RouterLink>

                        <RouterLink to="/orders" class="hover:text-blue-400 transition inline-fit w-fit group">
                            <span
                                class="relative py-0.5 after:absolute after:bottom-0 after:left-0 after:h-[1px] after:w-0 after:bg-blue-400 after:transition-all group-hover:after:w-full">Track
                                Orders</span>
                        </RouterLink>
                    </div>
                </div>

                <div>
                    <h3
                        class="text-xs font-bold uppercase tracking-widest text-slate-200 border-b border-slate-900 pb-2">
                        System Support
                    </h3>

                    <div class="mt-4 grid gap-2.5 text-sm text-slate-400">
                        <a href="mailto:arghochakma2023@gmail.com"
                            class="hover:text-blue-400 transition inline-fit w-fit group">
                            <span
                                class="relative py-0.5 after:absolute after:bottom-0 after:left-0 after:h-[1px] after:w-0 after:bg-blue-400 after:transition-all group-hover:after:w-full">Contact
                                Support</span>
                        </a>

                        <RouterLink to="/login" class="hover:text-blue-400 transition inline-fit w-fit group">
                            <span
                                class="relative py-0.5 after:absolute after:bottom-0 after:left-0 after:h-[1px] after:w-0 after:bg-blue-400 after:transition-all group-hover:after:w-full">Account
                                Login</span>
                        </RouterLink>

                        <RouterLink to="/register" class="hover:text-blue-400 transition inline-fit w-fit group">
                            <span
                                class="relative py-0.5 after:absolute after:bottom-0 after:left-0 after:h-[1px] after:w-0 after:bg-blue-400 after:transition-all group-hover:after:w-full">Create
                                Account</span>
                        </RouterLink>

                        <a href="https://github.com/ArghoPH" target="_blank" rel="noopener noreferrer"
                            class="hover:text-blue-400 transition inline-fit w-fit group">
                            <span
                                class="relative py-0.5 after:absolute after:bottom-0 after:left-0 after:h-[1px] after:w-0 after:bg-blue-400 after:transition-all group-hover:after:w-full">Open
                                Source Core</span>
                        </a>
                    </div>
                </div>
            </div>

            <div
                class="mt-12 flex flex-col gap-4 border-t border-slate-900 pt-6 text-xs text-slate-500 sm:flex-row sm:items-center sm:justify-between">
                <p class="tracking-wide">
                    &copy; {{ currentYear }} <span class="text-slate-400 font-medium">ShopSphere Platform</span>. All
                    rights reserved.
                </p>

                <p class="flex items-center gap-1.5">
                    <span>Engineered with precision by</span>
                    <a href="https://github.com/ArghoPH" target="_blank" rel="noopener noreferrer"
                        class="font-bold text-slate-300 transition hover:text-blue-400">
                        Argho Chakma
                    </a>
                </p>
            </div>

        </div>
    </footer>
</template>