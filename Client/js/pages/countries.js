// pages/countries.js — country list with search, filter, sort, pagination

import { api } from "../api.js";
import { mountChrome } from "../ui.js";
import { fmtNumber, debounce, getParam, uniqueValues, escapeHtml } from "../utils.js";

mountChrome("countries.html");

const PAGE_SIZE = 12;

const state = {
    search: getParam("search") || "",
    region: getParam("region") || "",
    language: "", currency: "",
    sort: "name", order: "asc", page: 1,
};

let allCountries = [];

const els = {
    search: document.getElementById("searchInput"),
    filterBtn: document.getElementById("filterBtn"),
    panel: document.getElementById("filterPanel"),
    region: document.getElementById("filterRegion"),
    language: document.getElementById("filterLanguage"),
    currency: document.getElementById("filterCurrency"),
    sort: document.getElementById("sortField"),
    order: document.getElementById("sortOrder"),
    results: document.getElementById("results"),
    count: document.getElementById("resultCount"),
    pager: document.getElementById("pager"),
};

els.search.value = state.search;

async function initFilters() {
    allCountries = await api.getCountries();
    const fill = (sel, values, label) => {
        sel.innerHTML = `<option value="">${label}</option>` +
            values.map((v) => `<option value="${escapeHtml(v)}">${escapeHtml(v)}</option>`).join("");
    };
    fill(els.region, uniqueValues(allCountries, "region"), "All regions");
    fill(els.language, await api.getCountryLanguages(), "All languages");
    fill(els.currency, await api.getCountryCurrencies(), "All currencies");
    els.region.value = state.region;
}

function cardHTML(c) {
    return `
    <a class="card card--interactive country-card" href="country.html?code=${c.code}">
      <div class="country-card__flag" style="background-image:url('${c.flag}')"></div>
      <div class="card__body">
        <h3 class="card__title">${escapeHtml(c.name)}</h3>
        <span class="badge">${escapeHtml(c.region)}</span>
        <p class="card__meta">Capital: ${escapeHtml(c.capital || "—")}</p>
        <p class="card__meta">Population: ${fmtNumber(c.population)}</p>
      </div>
    </a>`;
}

async function render() {
    els.results.innerHTML = `<div class="loading-state"><div class="spinner"></div>Loading countries…</div>`;
    try {
        const list = await api.getCountries({
            search: state.search, region: state.region,
            language: state.language, currency: state.currency,
            sort: state.sort, order: state.order,
        });
        els.count.textContent = `${list.length} ${list.length === 1 ? "country" : "countries"}`;

        if (!list.length) {
            els.results.innerHTML = `<p class="empty-state">No countries match your filters.</p>`;
            els.pager.innerHTML = ""; return;
        }

        const pages = Math.ceil(list.length / PAGE_SIZE);
        state.page = Math.min(state.page, pages);
        const start = (state.page - 1) * PAGE_SIZE;
        const pageItems = list.slice(start, start + PAGE_SIZE);

        els.results.innerHTML = `<div class="grid">${pageItems.map(cardHTML).join("")}</div>`;
        renderPager(pages);
    } catch (err) {
        els.results.innerHTML = `<p class="empty-state">Couldn't load countries: ${err.message}</p>`;
    }
}

function renderPager(pages) {
    if (pages <= 1) { els.pager.innerHTML = ""; return; }
    let html = "";
    for (let p = 1; p <= pages; p++) {
        html += `<button class="btn ${p === state.page ? "btn--primary" : "btn--outline"}" data-page="${p}">${p}</button>`;
    }
    els.pager.innerHTML = html;
    els.pager.querySelectorAll("[data-page]").forEach((b) =>
        b.addEventListener("click", () => { state.page = Number(b.dataset.page); render(); window.scrollTo({ top: 0, behavior: "smooth" }); })
    );
}

els.search.addEventListener("input", debounce((e) => {
    state.search = e.target.value.trim(); state.page = 1; render();
}, 300));

els.filterBtn.addEventListener("click", () => els.panel.classList.toggle("hidden"));

[["region", els.region], ["language", els.language], ["currency", els.currency]].forEach(([key, el]) => {
    el.addEventListener("change", () => { state[key] = el.value; state.page = 1; render(); });
});

els.sort.addEventListener("change", () => { state.sort = els.sort.value; render(); });
els.order.addEventListener("click", () => {
    state.order = state.order === "asc" ? "desc" : "asc";
    els.order.querySelector("span").textContent = state.order === "asc" ? "Ascending" : "Descending";
    render();
});

(async function start() { await initFilters(); render(); })();