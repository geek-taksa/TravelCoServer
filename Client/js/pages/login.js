// pages/login.js — login (users AND admin use this same screen).
import { api } from "../api.js";
import { auth } from "../auth.js";
import { mountChrome, toast } from "../ui.js";
import { getParam } from "../utils.js";

mountChrome("login.html");
if (auth.isLoggedIn) location.href = "index.html";

const setError = (id, msg) => { document.getElementById(id).textContent = msg || ""; };
const emailOk = (v) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v);

const form = document.getElementById("loginForm");
form.addEventListener("submit", async (e) => {
    e.preventDefault();
    ["errEmail", "errPassword"].forEach((id) => setError(id));

    const email = form.email.value.trim();
    const password = form.password.value;

    let valid = true;
    if (!emailOk(email)) { setError("errEmail", "Enter a valid email address."); valid = false; }
    if (!password) { setError("errPassword", "Enter your password."); valid = false; }
    if (!valid) return;

    const btn = form.querySelector('[type="submit"]');
    btn.disabled = true; btn.textContent = "Signing in…";
    try {
        const res = await api.login({ email, password });
        auth.setSession(res);
        toast(`Welcome back, ${res.user.username}!`, "success");
        const redirect = getParam("redirect");
        const dest = redirect || (res.user.role === "admin" ? "admin.html" : "index.html");
        setTimeout(() => (location.href = dest), 400);
    } catch (err) {
        toast(err.message || "Login failed.", "error");
        setError("errPassword", "Invalid email or password.");
        btn.disabled = false; btn.textContent = "Log in";
    }
});