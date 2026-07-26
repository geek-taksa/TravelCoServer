// pages/country.js — single country detail
import { api } from "../api.js";
import { auth } from "../auth.js";
import { mountChrome, toast, icons } from "../ui.js";
import { fmtNumber, fmtArea, getParam, escapeHtml } from "../utils.js";

mountChrome("country.html");

const code = getParam("code");
const root = document.getElementById("countryRoot");

if (!code) {
    root.innerHTML = `<p class="empty-state">No country specified. <a href="countries.html">Browse countries</a>.</p>`;
} else { load(); }

async function load() {
    root.innerHTML = `<div class="loading-state"><div class="spinner"></div>Loading…</div>`;
    try {
        const c = await api.getCountry(code);
        render(c); loadShares(c);
    } catch (err) {
        root.innerHTML = `<p class="empty-state">${err.message} <a href="countries.html">Back to countries</a>.</p>`;
    }
}

const fact = (label, value) =>
    `<div class="fact"><span class="fact__label">${label}</span><span class="fact__value">${value}</span></div>`;

function render(c) {
    const listButtons = auth.isLoggedIn
        ? `<div class="row" style="gap:var(--space-3); margin-top:var(--space-5)">
         <button class="btn btn--primary" data-list="visited">${icons.check} Mark as visited</button>
         <button class="btn btn--outline" data-list="wishlist">${icons.heart} Want to visit</button>
       </div>`
        : `<p class="card__meta" style="margin-top:var(--space-5)"><a href="login.html">Log in</a> to save this country to your lists.</p>`;

    root.innerHTML = `
    <a href="countries.html" class="btn btn--ghost" style="margin-bottom:var(--space-4)">← All countries</a>
    <div class="detail">
      <div class="detail__flag" style="background-image:url('${c.flag}')"></div>
      <div class="detail__body">
        <h1 class="detail__title">${escapeHtml(c.name)}</h1>
        <span class="badge">${escapeHtml(c.region)}</span>
        <div class="facts">
          ${fact("Capital", escapeHtml(c.capital || "—"))}
          ${fact("Population", fmtNumber(c.population))}
          ${fact("Area", fmtArea(c.area))}
          ${fact("Languages", (c.languages || []).map(escapeHtml).join(", ") || "—")}
          ${fact("Currencies", (c.currencies || []).map(escapeHtml).join(", ") || "—")}
        </div>
        ${listButtons}
      </div>
    </div>
    <section class="section">
      <h2 class="section-title" style="text-align:left">Trips shared about ${escapeHtml(c.name)}</h2>
      <div id="countryShares"><div class="loading-state"><div class="spinner"></div>Loading shares…</div></div>
    </section>`;

    root.querySelectorAll("[data-list]").forEach((btn) =>
        btn.addEventListener("click", async () => {
            const type = btn.dataset.list;
            btn.disabled = true;
            try {
                await api.addToList(type, c.code);
                toast(`Added ${c.name} to your ${type === "visited" ? "Visited" : "Want to visit"} list.`, "success");
            } catch (err) {
                toast(err.message || "Couldn't update your list.", "error");
            } finally { btn.disabled = false; }
        })
    );
}

async function loadShares(c) {
    const box = document.getElementById("countryShares");
    try {
        const shares = await api.getShares(c.code);
        if (!shares.length) {
            box.innerHTML = `<p class="empty-state">No trips shared yet for ${escapeHtml(c.name)}.</p>`;
            return;
        }
        box.innerHTML = shares.map((s) => `
      <div class="card" style="margin-bottom:var(--space-4)">
        <div class="card__body">
          <div class="row row-between">
            <h3 class="card__title">${escapeHtml(s.title)}</h3>
            <span class="badge">${escapeHtml(s.type)}</span>
          </div>
          <p style="margin:var(--space-2) 0">${escapeHtml(s.body)}</p>
          <p class="card__meta">by ${escapeHtml(s.author)} · ${escapeHtml(s.createdAt)}</p>
        </div>
      </div>`).join("");
    } catch (err) {
        box.innerHTML = `<p class="empty-state">Couldn't load shares: ${err.message}</p>`;
    }
}