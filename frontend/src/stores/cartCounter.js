import { ref } from "vue";
import api from "../services/api";
import { auth, isAuthenticated } from "./auth";

export const cartCount = ref(0);

export const fetchCartCount = async () => {
    if (!isAuthenticated() || auth.role !== "User" || !auth.userId) {
        cartCount.value = 0;
        return;
    }

    try {
        const response = await api.get(`/cart/${auth.userId}`);

        const items =
            response.data?.items ||
            response.data?.cartItems ||
            [];

        cartCount.value = items.reduce((total, item) => {
            return total + Number(item.quantity || 0);
        }, 0);
    } catch (err) {
        cartCount.value = 0;
    }
};

export const resetCartCount = () => {
    cartCount.value = 0;
};