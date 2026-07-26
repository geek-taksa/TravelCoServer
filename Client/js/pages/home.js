// pages/home.js — homepage controller. Renders "Countries by Region" from
// live region counts and wires the search box to the countries page.
import { api } from "../api.js";
import { mountChrome, icons } from "../ui.js";

mountChrome("index.html");

const REGION_ORDER = ["Africa", "Americas", "Asia", "Europe", "Oceania", "Polar", "Antarctic Ocean", "Antarctic"];

// Search box -> countries.html?search=
document.getElementById("searchForm")?.addEventListener("submit", (e) => {
    e.preventDefault();
    const q = document.getElementById("searchInput").value.trim();
    location.href = `countries.html${q ? `?search=${encodeURIComponent(q)}` : ""}`;
});
document.getElementById("filterBtn")?.addEventListener("click", () => (location.href = "countries.html"));
document.getElementById("sortBtn")?.addEventListener("click", () => (location.href = "countries.html"));

// Region list
const listEl = document.getElementById("regionList");
(async function loadRegions() {
    try {
        const counts = await api.getRegionCounts();
        const regions = REGION_ORDER.filter((r) => counts[r]);
        Object.keys(counts).forEach((r) => { if (!regions.includes(r)) regions.push(r); });

        listEl.innerHTML = regions.map((region) => `
      <details class="region-item">
        <summary class="region-item__header">
          <span class="region-item__chevron">${icons.chevron}</span>
          <span class="region-item__name">${region}</span>
          <span class="count">${counts[region]}</span>
        </summary>
        <div class="region-item__body">
          <a href="countries.html?region=${encodeURIComponent(region)}">View all in ${region} →</a>
        </div>
      </details>
    `).join("");
    } catch (err) {
        listEl.innerHTML = `<p class="empty-state">Couldn't load regions: ${err.message}</p>`;
    }
})();