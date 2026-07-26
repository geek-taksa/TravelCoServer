// auth.js — session state + route guards, backed by localStorage.
// The navbar and protected pages use this. Full login/register UI is Step 4+.
import { CONFIG } from "./config.js";
import { api, token } from "./api.js";

export const auth = {
    get user() {
        try { return JSON.parse(localStorage.getItem(CONFIG.USER_KEY)); }
        catch { return null; }
    },
    get isLoggedIn() { return !!token.get(); },
    get isAdmin() { return this.user?.role === "admin"; },

    // Persist session after a successful login/register.
    setSession({ token: t, user }) {
        token.set(t);
        localStorage.setItem(CONFIG.USER_KEY, JSON.stringify(user));
    },

    async logout() {
        try { await api.logout(); } catch { }
        token.clear();
        localStorage.removeItem(CONFIG.USER_KEY);
        location.href = "./index.html";
    },

    // Call at the top of a protected page.
    requireAuth({ adminOnly = false } = {}) {
        if (!this.isLoggedIn) {
            location.href = `./login.html?redirect=${encodeURIComponent(location.pathname.split("/").pop())}`;
            return false;
        }
        if (adminOnly && !this.isAdmin) { location.href = "./index.html"; return false; }
        return true;
    },
};