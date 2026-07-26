// pages/lists.js — the user's personal Visited / Want-to-visit lists
// removing, and moving a country between the two lists
import { api } from "../api.js";
import { auth } from "../auth.js";
import { mountChrome, toast } from "../ui.js";
import { fmtNumber, escapeHtml } from "../utils.js";

mountChrome("lists.html");

if (!auth.requireAuth()) throw new Error("redirecting");

const visitedEl = document.getElementById("visitedList");
const wishlistEl = document.getElementById("wishlistList");
const visitedCount = document.getElementById("visitedCount");
const wishlistCount = document.getElementById("wishlistCount");

function cardHTML(c, list) {
    const moveTo = list === "visited" ? "wishlist" : "visited";
    const moveLabel = list === "visited" ? "Move to Want to visit" : "Move to Visited";
    return `
    <div class="card list-card">
      <a href="country.html?code=${c.code}" class="list-card__flag" style="background-image:url('${c.flag}')" aria-label="${escapeHtml(c.name)}"></a>
      <div class="card__body">
        <a href="country.html?code=${c.code}"><h3 class="card__title">${escapeHtml(c.name)}</h3></a>
        <p class="card__meta">${escapeHtml(c.region)} · ${fmtNumber(c.population)}</p>
        <div class="row" style="margin-top:var(--space-3); gap:var(--space-2)">
          <button class="btn btn--outline" data-move data-code="${c.code}" data-from="${list}" data-to="${moveTo}">${moveLabel}</button>
          <button class="btn btn--danger" data-remove data-code="${c.code}" data-list="${list}">Remove</button>
        </div>
      </div>
    </div>`;
}

function renderSection(el, items, list) {
    if (!items.length) {
        el.innerHTML = `<p class="empty-state">Nothing here yet. <a href="countries.html">Explore countries</a> to add some.</p>`;
        return;
    }
    el.innerHTML = items.map((c) => cardHTML(c, list)).join("");
}

async function load() {
    visitedEl.innerHTML = wishlistEl.innerHTML = `<div class="loading-state"><div class="spinner"></div>Loading…</div>`;
    try {
        const { visited, wishlist } = await api.getLists();
        visitedCount.textContent = visited.length;
        wishlistCount.textContent = wishlist.length;
        renderSection(visitedEl, visited, "visited");
        renderSection(wishlistEl, wishlist, "wishlist");
    } catch (err) {
        visitedEl.innerHTML = wishlistEl.innerHTML = `<p class="empty-state">Couldn't load your lists: ${err.message}</p>`;
    }
}

document.querySelector("main").addEventListener("click", async (e) => {
    const moveBtn = e.target.closest("[data-move]");
    const removeBtn = e.target.closest("[data-remove]");
    try {
        if (moveBtn) {
            await api.moveInList({ countryCode: moveBtn.dataset.code, from: moveBtn.dataset.from, to: moveBtn.dataset.to });
            toast("Moved.", "success"); load();
        } else if (removeBtn) {
            await api.removeFromList(removeBtn.dataset.list, removeBtn.dataset.code);
            toast("Removed.", "success"); load();
        }
    } catch (err) {
        toast(err.message || "Something went wrong.", "error");
    }
});

load();