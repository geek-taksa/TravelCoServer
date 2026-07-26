// pages/register.js — registration form controller.
// Collects account details + preferences (continents, languages with levels),
// validates client-side, calls api.register and starts a session.
import { api } from "../api.js";
import { auth } from "../auth.js";
import { mountChrome, toast } from "../ui.js";

mountChrome("register.html");
if (auth.isLoggedIn) location.href = "index.html";

const CONTINENTS = ["Africa", "Americas", "Asia", "Europe", "Oceania", "Antarctic"];
const LEVELS = ["Beginner", "Intermediate", "Fluent", "Native"];

// continents checkboxes
document.getElementById("continents").innerHTML = CONTINENTS.map(
    (c) => `<label class="checkbox"><input type="checkbox" name="continent" value="${c}" /> ${c}</label>`
).join("");

// dynamic language rows
const langWrap = document.getElementById("languages");
function addLanguageRow(name = "", level = "Fluent") {
    const row = document.createElement("div");
    row.className = "lang-row row";
    row.innerHTML = `
    <input class="input" type="text" placeholder="Language (e.g. English)" value="${name}" aria-label="Language name" />
    <select class="select" aria-label="Proficiency level">
      ${LEVELS.map((l) => `<option ${l === level ? "selected" : ""}>${l}</option>`).join("")}
    </select>
    <button type="button" class="icon-btn" data-remove aria-label="Remove language">✕</button>`;
    row.querySelector("[data-remove]").addEventListener("click", () => row.remove());
    langWrap.appendChild(row);
}
addLanguageRow();
document.getElementById("addLang").addEventListener("click", () => addLanguageRow());

const setError = (id, msg) => { document.getElementById(id).textContent = msg || ""; };
const emailOk = (v) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v);

function collectPreferences() {
    const continents = [...document.querySelectorAll('input[name="continent"]:checked')].map((c) => c.value);
    const languages = [...langWrap.querySelectorAll(".lang-row")]
        .map((row) => ({ name: row.querySelector("input").value.trim(), level: row.querySelector("select").value }))
        .filter((l) => l.name);
    return { continents, languages };
}

const form = document.getElementById("registerForm");
form.addEventListener("submit", async (e) => {
    e.preventDefault();
    ["errUsername", "errEmail", "errPassword", "errConfirm"].forEach((id) => setError(id));

    const username = form.username.value.trim();
    const email = form.email.value.trim();
    const password = form.password.value;
    const confirm = form.confirm.value;

    let valid = true;
    if (username.length < 2) { setError("errUsername", "Please enter your name."); valid = false; }
    if (!emailOk(email)) { setError("errEmail", "Enter a valid email address."); valid = false; }
    if (password.length < 6) { setError("errPassword", "Password must be at least 6 characters."); valid = false; }
    if (password !== confirm) { setError("errConfirm", "Passwords don't match."); valid = false; }
    if (!valid) return;

    const btn = form.querySelector('[type="submit"]');
    btn.disabled = true; btn.textContent = "Creating account…";
    try {
        const res = await api.register({ username, email, password, preferences: collectPreferences() });
        auth.setSession(res);
        toast(`Welcome, ${res.user.username}!`, "success");
        setTimeout(() => (location.href = "index.html"), 500);
    } catch (err) {
        toast(err.message || "Registration failed.", "error");
        btn.disabled = false; btn.textContent = "Create account";
    }
});