import axios from "axios";
import { auth } from "../stores/auth";

const api = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL,
    timeout: 15000,
});

api.interceptors.request.use((config) => {
    if (auth.token) {
        config.headers.Authorization = `Bearer ${auth.token}`;
    }

    return config;
});

export default api;