import { ref } from "vue";
import api from "../services/api";
import { auth, isAuthenticated } from "./auth";

export const cartCount = ref(0);

export const setCartCount = (count) => {
    cartCount.value = Math.max(0, Number(count || 0));
};

export const increaseCartCount = (quantity = 1) => {
    cartCount.value += Number(quantity || 1);
};

export const decreaseCartCount = (quantity = 1) => {
    cartCount.value = Math.max(0, cartCount.value - Number(quantity || 1));
};

export const resetCartCount = () => {
    cartCount.value = 0;
};

export const fetchCartCount = async () => {
    if (!isAuthenticated() || auth.role !== "User" || !auth.userId) {
        resetCartCount();
        return;
    }

    try {
        const response = await api.get(`/cart/${auth.userId}`);

        const items = response.data?.items || response.data?.cartItems || [];

        const totalQuantity = items.reduce((total, item) => {
            return total + Number(item.quantity || 0);
        }, 0);

        setCartCount(totalQuantity);
    } catch (err) {
        resetCartCount();
    }
};