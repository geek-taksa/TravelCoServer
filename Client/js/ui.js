// ui.js — shared, framework-free UI: inline SVG icons, navbar, footer,
// toast and modal. Pages import { mountChrome, icons, toast, modal }.
import { auth } from "./auth.js";

/* inline SVG icons (stroke = currentColor) */
const svg = (paths, extra = "") =>
    `<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" ${extra}>${paths}</svg>`;

export const icons = {
    chevron: svg('<path d="m6 9 6 6 6-6"/>'),
    search: svg('<circle cx="11" cy="11" r="8"/><path d="m21 21-4.3-4.3"/>'),
    filter: svg('<path d="M22 3H2l8 9.46V19l4 2v-8.54L22 3z"/>'),
    sort: svg('<path d="m3 16 4 4 4-4"/><path d="M7 20V4"/><path d="m21 8-4-4-4 4"/><path d="M17 4v16"/>'),
    menu: svg('<path d="M3 12h18M3 6h18M3 18h18"/>'),
    heart: svg('<path d="M20.8 4.6a5.5 5.5 0 0 0-7.8 0L12 5.6l-1-1a5.5 5.5 0 0 0-7.8 7.8l1 1L12 21l7.8-7.6 1-1a5.5 5.5 0 0 0 0-7.8z"/>'),
    check: svg('<path d="M20 6 9 17l-5-5"/>'),
};

/* navbar — links adapt to auth state; `active` = current page filename */
function navbarHTML(active) {
    const links = [
        { href: "index.html", label: "Home" },
        { href: "countries.html", label: "Countries" },
        { href: "shares.html", label: "Community" },
        { href: "quizzes.html", label: "Quizzes" },
    ];
    if (auth.isLoggedIn) links.push({ href: "lists.html", label: "My Lists" });
    if (auth.isAdmin) links.push({ href: "admin.html", label: "Admin" });

    const authLinks = auth.isLoggedIn
        ? `<a href="profile.html" ${active === "profile.html" ? 'aria-current="page"' : ""}>Settings</a>
       <a href="#" data-action="logout">Log-out</a>`
        : `<a href="login.html" ${active === "login.html" ? 'aria-current="page"' : ""}>Login</a>
       <a href="register.html" class="btn btn--primary" style="color:#fff">Sign up</a>`;

    const mainLinks = links
        .map((l) => `<a href="${l.href}" ${active === l.href ? 'aria-current="page"' : ""}>${l.label}</a>`)
        .join("");

    return `
    <nav class="navbar">
      <div class="navbar__inner">
        <a href="index.html" class="brand">TravelCo</a>
        <button class="nav-toggle" aria-label="Menu" data-action="nav-toggle">${icons.menu}</button>
        <div class="nav-links hidden" id="navLinks">${mainLinks}${authLinks}</div>
      </div>
    </nav>`;
}

function footerHTML() {
    return `
    <footer class="footer">
      <div class="footer__inner">
        <span class="brand">TravelCo</span>
        <p class="footer__credit">Created by Maria Dotsenko</p>
      </div>
    </footer>`;
}

// Inject navbar + footer and wire shared behaviour. Call once per page.
export function mountChrome(active = "") {
    const nav = document.getElementById("nav-root");
    const foot = document.getElementById("footer-root");
    if (nav) nav.innerHTML = navbarHTML(active);
    if (foot) foot.innerHTML = footerHTML();

    document.body.addEventListener("click", (e) => {
        const el = e.target.closest("[data-action]");
        if (!el) return;
        if (el.dataset.action === "logout") { e.preventDefault(); auth.logout(); }
        if (el.dataset.action === "nav-toggle") document.getElementById("navLinks")?.classList.toggle("hidden");
    });

    const sync = () => {
        const links = document.getElementById("navLinks");
        if (window.innerWidth > 860) links?.classList.remove("hidden");
        else links?.classList.add("hidden");
    };
    sync();
    window.addEventListener("resize", sync);
}

/* toast */
export function toast(message, type = "") {
    let wrap = document.querySelector(".toast-wrap");
    if (!wrap) { wrap = document.createElement("div"); wrap.className = "toast-wrap"; document.body.appendChild(wrap); }
    const el = document.createElement("div");
    el.className = `toast ${type ? `toast--${type}` : ""}`;
    el.textContent = message;
    wrap.appendChild(el);
    setTimeout(() => el.remove(), 3200);
}

/* modal */
export function modal(contentHTML) {
    const backdrop = document.createElement("div");
    backdrop.className = "modal-backdrop";
    backdrop.innerHTML = `<div class="modal">${contentHTML}</div>`;
    backdrop.addEventListener("click", (e) => { if (e.target === backdrop) close(); });
    document.body.appendChild(backdrop);
    function close() { backdrop.remove(); }
    return { el: backdrop, close };
}