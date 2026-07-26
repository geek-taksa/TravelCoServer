// pages/profile.js — account details + preferences editor
// Guarded page. Pre-fills from the logged-in session, saves via api.updateMe
import { api, token } from "../api.js";
import { auth } from "../auth.js";
import { mountChrome, toast } from "../ui.js";
import { escapeHtml } from "../utils.js";

mountChrome("profile.html");

if (!auth.requireAuth()) throw new Error("redirecting");

const CONTINENTS = ["Africa", "Americas", "Asia", "Europe", "Oceania", "Antarctic"];
const LEVELS = ["Beginner", "Intermediate", "Fluent", "Native"];

const user = auth.user || {};

const form = document.getElementById("profileForm");
const langWrap = document.getElementById("languages");

function addLanguageRow(name = "", level = "Fluent") {
    const row = document.createElement("div");
    row.className = "lang-row row";
    row.innerHTML = `
    <input class="input" type="text" placeholder="Language (e.g. English)" value="${escapeHtml(name)}" aria-label="Language name" />
    <select class="select" aria-label="Proficiency level">
      ${LEVELS.map((l) => `<option ${l === level ? "selected" : ""}>${l}</option>`).join("")}
    </select>
    <button type="button" class="icon-btn" data-remove aria-label="Remove language">✕</button>`;
    row.querySelector("[data-remove]").addEventListener("click", () => row.remove());
    langWrap.appendChild(row);
}
document.getElementById("addLang").addEventListener("click", () => addLanguageRow());

async function loadProfile() {
    let profile;
    try { profile = await api.me(); }   // GET /api/users/me — includes preferences
    catch { profile = user; }

    const prefs = profile.preferences || { continents: [], languages: [] };

    form.username.value = profile.username || "";
    form.email.value = profile.email || "";

    document.getElementById("continents").innerHTML = CONTINENTS.map((c) => `
    <label class="checkbox">
      <input type="checkbox" name="continent" value="${c}" ${prefs.continents?.includes(c) ? "checked" : ""} /> ${c}
    </label>`).join("");

    langWrap.innerHTML = "";
    if (prefs.languages?.length) prefs.languages.forEach((l) => addLanguageRow(l.name, l.level));
    else addLanguageRow();
}
loadProfile();

const setError = (id, msg) => { document.getElementById(id).textContent = msg || ""; };
const emailOk = (v) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v);

function collectPreferences() {
    const continents = [...document.querySelectorAll('input[name="continent"]:checked')].map((c) => c.value);
    const languages = [...langWrap.querySelectorAll(".lang-row")]
        .map((row) => ({ name: row.querySelector("input").value.trim(), level: row.querySelector("select").value }))
        .filter((l) => l.name);
    return { continents, languages };
}

form.addEventListener("submit", async (e) => {
    e.preventDefault();
    setError("errUsername"); setError("errEmail");

    const username = form.username.value.trim();
    const email = form.email.value.trim();

    let valid = true;
    if (username.length < 2) { setError("errUsername", "Please enter your name."); valid = false; }
    if (!emailOk(email)) { setError("errEmail", "Enter a valid email address."); valid = false; }
    if (!valid) return;

    const btn = form.querySelector('[type="submit"]');
    btn.disabled = true; btn.textContent = "Saving…";

    try {
        const updated = await api.updateMe({ username, email, preferences: collectPreferences() });
        auth.setSession({ token: token.get(), user: { ...user, ...updated } });
        toast("Profile saved.", "success");
    } catch (err) {
        toast(err.message || "Couldn't save profile.", "error");
    } finally {
        btn.disabled = false; btn.textContent = "Save changes";
    }
});