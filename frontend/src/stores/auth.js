import { reactive } from "vue";

const savedAuth = JSON.parse(localStorage.getItem("shopsphere_auth") || "null");

export const auth = reactive({
    token: savedAuth?.token || "",
    userId: savedAuth?.userId || "",
    fullName: savedAuth?.fullName || "",
    email: savedAuth?.email || "",
    role: savedAuth?.role || "",
});

export const isAuthenticated = () => {
    return !!auth.token;
};

export const hasRole = (roles) => {
    return roles.includes(auth.role);
};

export const setAuth = (data) => {
    auth.token = data.token;
    auth.userId = data.userId;
    auth.fullName = data.fullName;
    auth.email = data.email;
    auth.role = data.role;

    localStorage.setItem("shopsphere_auth", JSON.stringify(data));
};

export const clearAuth = () => {
    auth.token = "";
    auth.userId = "";
    auth.fullName = "";
    auth.email = "";
    auth.role = "";

    localStorage.removeItem("shopsphere_auth");
};