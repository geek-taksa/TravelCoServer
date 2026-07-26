// pages/shares.js — community shares feed
// Anyone can read; logged-in users can post a share and edit/delete their own
import { api } from "../api.js";
import { auth } from "../auth.js";
import { mountChrome, toast, modal } from "../ui.js";
import { getParam, escapeHtml } from "../utils.js";

mountChrome("shares.html");

const TYPES = ["thought", "recommendation", "review"];

const feedEl = document.getElementById("feed");
const filterEl = document.getElementById("countryFilter");
const newBtn = document.getElementById("newShareBtn");

let countries = [];
let activeCountry = getParam("country") || "";

if (auth.isLoggedIn) {
    newBtn.classList.remove("hidden");
    newBtn.addEventListener("click", () => openForm());
}

async function initCountryFilter() {
    countries = await api.getCountries();
    filterEl.innerHTML = `<option value="">All countries</option>` +
        countries.map((c) => `<option value="${c.code}">${escapeHtml(c.name)}</option>`).join("");
    filterEl.value = activeCountry;
    filterEl.addEventListener("change", () => { activeCountry = filterEl.value; load(); });
}

function shareCardHTML(s) {
    const mine = auth.isLoggedIn && s.author === auth.user?.username;
    const actions = mine
        ? `<div class="row" style="gap:var(--space-2); margin-top:var(--space-3)">
         <button class="btn btn--outline" data-edit="${s.id}">Edit</button>
         <button class="btn btn--danger" data-delete="${s.id}">Delete</button>
       </div>`
        : "";
    return `
    <div class="card share-card">
      <div class="card__body">
        <div class="row row-between">
          <h3 class="card__title">${escapeHtml(s.title)}</h3>
          <span class="badge">${escapeHtml(s.type)}</span>
        </div>
        <p class="card__meta" style="margin-top:2px">
          <a href="country.html?code=${s.countryCode}">${escapeHtml(s.countryName || s.countryCode)}</a>
          · by ${escapeHtml(s.author)} · ${escapeHtml(s.createdAt)}
        </p>
        <p style="margin-top:var(--space-3)">${escapeHtml(s.body)}</p>
        ${actions}
      </div>
    </div>`;
}

async function load() {
    feedEl.innerHTML = `<div class="loading-state"><div class="spinner"></div>Loading shares…</div>`;
    try {
        const shares = await api.getShares(activeCountry);
        feedEl.innerHTML = shares.length
            ? shares.map(shareCardHTML).join("")
            : `<p class="empty-state">No shares yet. ${auth.isLoggedIn ? "Be the first to share a trip!" : ""}</p>`;
    } catch (err) {
        feedEl.innerHTML = `<p class="empty-state">Couldn't load shares: ${err.message}</p>`;
    }
}

function openForm(existing = null) {
    const countryOptions = countries
        .map((c) => `<option value="${c.code}" ${existing?.countryCode === c.code ? "selected" : ""}>${escapeHtml(c.name)}</option>`)
        .join("");
    const typeOptions = TYPES
        .map((t) => `<option ${existing?.type === t ? "selected" : ""}>${t}</option>`)
        .join("");

    const { el, close } = modal(`
    <h2 class="section-title" style="text-align:left; font-size:var(--fs-2xl)">${existing ? "Edit share" : "Share a trip"}</h2>
    <form id="shareForm" class="stack" novalidate>
      <div class="field">
        <label class="label" for="sfCountry">Country</label>
        <select class="select" id="sfCountry" ${existing ? "disabled" : ""}>${countryOptions}</select>
      </div>
      <div class="field">
        <label class="label" for="sfType">Type</label>
        <select class="select" id="sfType">${typeOptions}</select>
      </div>
      <div class="field">
        <label class="label" for="sfTitle">Title</label>
        <input class="input" id="sfTitle" type="text" value="${existing ? escapeHtml(existing.title) : ""}" placeholder="A short headline" />
        <span class="field__error" id="sfErr"></span>
      </div>
      <div class="field">
        <label class="label" for="sfBody">Your thoughts</label>
        <textarea class="textarea" id="sfBody" placeholder="Share a recommendation, review or thought…">${existing ? escapeHtml(existing.body) : ""}</textarea>
      </div>
      <div class="row" style="gap:var(--space-3); justify-content:flex-end">
        <button type="button" class="btn btn--ghost" data-cancel>Cancel</button>
        <button type="submit" class="btn btn--primary">${existing ? "Save changes" : "Post share"}</button>
      </div>
    </form>
  `);

    el.querySelector("[data-cancel]").addEventListener("click", close);
    el.querySelector("#shareForm").addEventListener("submit", async (e) => {
        e.preventDefault();
        const title = el.querySelector("#sfTitle").value.trim();
        const body = el.querySelector("#sfBody").value.trim();
        const code = el.querySelector("#sfCountry").value;
        const type = el.querySelector("#sfType").value;
        if (!title || !body) { el.querySelector("#sfErr").textContent = "Please add a title and some text."; return; }

        const country = countries.find((c) => c.code === code);
        const payload = { countryCode: code, countryName: country?.name, type, title, body, author: auth.user.username };

        try {
            if (existing) await api.updateShare(existing.id, payload);
            else await api.createShare(payload);
            toast(existing ? "Share updated." : "Share posted!", "success");
            close();
            load();
        } catch (err) {
            toast(err.message || "Couldn't save your share.", "error");
        }
    });
}

feedEl.addEventListener("click", async (e) => {
    const editId = e.target.closest("[data-edit]")?.dataset.edit;
    const delId = e.target.closest("[data-delete]")?.dataset.delete;
    if (editId) {
        const share = (await api.getShares()).find((s) => s.id === Number(editId));
        if (share) openForm(share);
    } else if (delId) {
        if (!confirm("Delete this share?")) return;
        try { await api.deleteShare(delId); toast("Share deleted.", "success"); load(); }
        catch (err) { toast(err.message || "Couldn't delete.", "error"); }
    }
});

(async function start() { await initCountryFilter(); load(); })();