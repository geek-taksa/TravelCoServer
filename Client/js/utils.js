// utils.js — small pure helpers used across pages.
export const fmtNumber = (n) =>
    typeof n === "number" ? n.toLocaleString("en-US") : n;

export const fmtArea = (n) => `${fmtNumber(n)} km²`;

// Debounce (used for the search box so we don't filter on every keystroke).
export function debounce(fn, wait = 300) {
    let t;
    return (...args) => { clearTimeout(t); t = setTimeout(() => fn(...args), wait); };
}

// Read a query-string param from the current URL.
export const getParam = (key) => new URLSearchParams(location.search).get(key);

// Escape user text before inserting into innerHTML (prevents broken markup / XSS).
export function escapeHtml(str = "") {
    return String(str)
        .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
}

// Unique sorted values from an array of objects — for filter dropdowns.
export function uniqueValues(list, key) {
    const set = new Set();
    list.forEach((item) => {
        const v = item[key];
        (Array.isArray(v) ? v : [v]).forEach((x) => x && set.add(x));
    });
    return [...set].sort();
}