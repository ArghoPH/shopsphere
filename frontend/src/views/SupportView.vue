<script setup>
import { ref } from "vue";
import AppNavbar from "../components/AppNavbar.vue";

const openFaq = ref(0);

const supportInfo = {
    whatsapp: "01999358618",
    email: "support@shopsphere.com",
    phone: "+8801537366505",
};

const faqs = [
    {
        question: "How can I place an order?",
        answer:
            "Browse products, open the product details page, add your selected item to cart, then proceed to checkout using Cash on Delivery.",
    },
    {
        question: "Can I cancel my order?",
        answer:
            "Order cancellation is possible before the order is processed or delivered. Please contact support as soon as possible with your order details.",
    },
    {
        question: "How can I track my order?",
        answer:
            "After placing an order, go to your Order History page. You can see the latest order status such as Pending, Processing, Delivered, or Cancelled.",
    },
    {
        question: "What payment method is available?",
        answer:
            "Currently, ShopSphere supports Cash on Delivery. Online payment gateway support may be added in the future.",
    },
    {
        question: "How do I contact support?",
        answer:
            "You can contact us through WhatsApp, email, or phone using the support options on this page.",
    },
];

const toggleFaq = (index) => {
    openFaq.value = openFaq.value === index ? null : index;
};

const whatsappLink = `https://wa.me/${supportInfo.whatsapp}`;
const mailLink = `mailto:${supportInfo.email}`;
const phoneLink = `tel:${supportInfo.phone.replace(/\s|-/g, "")}`;
</script>

<template>
    <div class="min-h-screen bg-slate-100 text-slate-950">
        <AppNavbar />

        <main class="mx-auto max-w-7xl px-6 py-10 md:px-16">
            <!-- Header -->
            <section class="overflow-hidden rounded-[2rem] bg-slate-950 p-8 text-white md:p-12">
                <div class="max-w-3xl">
                    <p class="text-xs font-black uppercase tracking-[0.28em] text-blue-400">
                        Contact Support
                    </p>

                    <h1 class="mt-4 text-4xl font-black tracking-tight md:text-6xl">
                        Need help? We are here for you.
                    </h1>

                    <p class="mt-5 max-w-2xl text-base leading-8 text-slate-300 md:text-lg">
                        Find answers to common questions or contact the ShopSphere support
                        team through WhatsApp, email, or phone.
                    </p>
                </div>
            </section>

            <!-- Contact Cards -->
            <section class="mt-8 grid gap-5 md:grid-cols-3">
                <a :href="whatsappLink" target="_blank" rel="noopener noreferrer"
                    class="group rounded-3xl bg-white p-6 shadow-sm ring-1 ring-slate-200 transition hover:-translate-y-1 hover:shadow-xl">
                    <div class="flex h-14 w-14 items-center justify-center rounded-2xl bg-green-50 text-green-600">
                        <i class="fa-brands fa-whatsapp text-3xl"></i>
                    </div>

                    <h2 class="mt-5 text-2xl font-black">WhatsApp</h2>

                    <p class="mt-2 text-sm leading-6 text-slate-500">
                        Chat with us directly on WhatsApp for quick support.
                    </p>

                    <p class="mt-5 font-bold text-green-600">
                        Message Now
                        <i class="fa-solid fa-arrow-right ml-1 transition group-hover:translate-x-1"></i>
                    </p>
                </a>

                <a :href="mailLink"
                    class="group rounded-3xl bg-white p-6 shadow-sm ring-1 ring-slate-200 transition hover:-translate-y-1 hover:shadow-xl">
                    <div class="flex h-14 w-14 items-center justify-center rounded-2xl bg-blue-50 text-blue-600">
                        <i class="fa-solid fa-envelope text-2xl"></i>
                    </div>

                    <h2 class="mt-5 text-2xl font-black">Email</h2>

                    <p class="mt-2 text-sm leading-6 text-slate-500">
                        Send your issue or order details to our support email.
                    </p>

                    <p class="mt-5 break-all font-bold text-blue-600">
                        {{ supportInfo.email }}
                    </p>
                </a>

                <a :href="phoneLink"
                    class="group rounded-3xl bg-white p-6 shadow-sm ring-1 ring-slate-200 transition hover:-translate-y-1 hover:shadow-xl">
                    <div class="flex h-14 w-14 items-center justify-center rounded-2xl bg-orange-50 text-orange-600">
                        <i class="fa-solid fa-phone text-2xl"></i>
                    </div>

                    <h2 class="mt-5 text-2xl font-black">Phone</h2>

                    <p class="mt-2 text-sm leading-6 text-slate-500">
                        Call us during business hours for direct support.
                    </p>

                    <p class="mt-5 font-bold text-orange-600">
                        {{ supportInfo.phone }}
                    </p>
                </a>
            </section>

            <!-- FAQ -->
            <section class="mt-10 grid gap-8 lg:grid-cols-3">
                <div>
                    <p class="text-xs font-black uppercase tracking-[0.25em] text-blue-600">
                        FAQ
                    </p>

                    <h2 class="mt-2 text-3xl font-black md:text-4xl">
                        Frequently Asked Questions
                    </h2>

                    <p class="mt-4 leading-7 text-slate-500">
                        Quick answers for common questions about orders, checkout, payment,
                        delivery and support.
                    </p>

                    <div class="mt-6 rounded-3xl bg-blue-50 p-5 text-blue-800">
                        <p class="font-black">
                            Future Update
                        </p>

                        <p class="mt-2 text-sm leading-6">
                            Live chat and AI assistant will be added here later.
                        </p>
                    </div>
                </div>

                <div class="lg:col-span-2">
                    <div class="grid gap-4">
                        <article v-for="(faq, index) in faqs" :key="faq.question"
                            class="overflow-hidden rounded-3xl bg-white shadow-sm ring-1 ring-slate-200">
                            <button type="button" @click="toggleFaq(index)"
                                class="flex w-full items-center justify-between gap-4 px-6 py-5 text-left">
                                <span class="text-lg font-black">
                                    {{ faq.question }}
                                </span>

                                <span
                                    class="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-slate-100 text-slate-950">
                                    <i class="fa-solid transition"
                                        :class="openFaq === index ? 'fa-minus' : 'fa-plus'"></i>
                                </span>
                            </button>

                            <Transition enter-active-class="transition duration-200 ease-out"
                                enter-from-class="opacity-0 -translate-y-2" enter-to-class="opacity-100 translate-y-0"
                                leave-active-class="transition duration-150 ease-in"
                                leave-from-class="opacity-100 translate-y-0" leave-to-class="opacity-0 -translate-y-2">
                                <div v-if="openFaq === index" class="border-t border-slate-100 px-6 pb-6 pt-1">
                                    <p class="leading-7 text-slate-500">
                                        {{ faq.answer }}
                                    </p>
                                </div>
                            </Transition>
                        </article>
                    </div>
                </div>
            </section>
        </main>
    </div>
</template>