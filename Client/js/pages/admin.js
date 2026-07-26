// pages/admin.js — admin dashboard. Admin-only (guarded). Shows usage stats
// and a user table with lock/unlock and block-sharing controls.
import { api } from "../api.js";
import { auth } from "../auth.js";
import { mountChrome, toast } from "../ui.js";
import { fmtNumber, escapeHtml } from "../utils.js";

mountChrome("admin.html");

// guard: admins only
if (!auth.requireAuth({ adminOnly: true })) throw new Error("redirecting");

const statsEl = document.getElementById("stats");
const usersEl = document.getElementById("usersTable");

async function loadStats() {
    try {
        const s = await api.getAdminStats();
        const cards = [
            ["Daily logins", s.dailyLogins],
            ["Countries imported", s.countriesImported],
            ["Countries saved", s.countriesSaved],
            ["Shares created", s.sharesCreated],
        ];
        statsEl.innerHTML = cards.map(([label, value]) => `
      <div class="card stat-card">
        <div class="card__body">
          <p class="stat-value">${fmtNumber(value)}</p>
          <p class="stat-label">${label}</p>
        </div>
      </div>`).join("");
    } catch (err) {
        statsEl.innerHTML = `<p class="empty-state">Couldn't load stats: ${err.message}</p>`;
    }
}

async function loadUsers() {
    usersEl.innerHTML = `<div class="loading-state"><div class="spinner"></div>Loading users…</div>`;
    try {
        const users = await api.getAdminUsers();
        usersEl.innerHTML = `
      <table class="admin-table">
        <thead>
          <tr><th>User</th><th>Email</th><th>Role</th><th>Status</th><th>Sharing</th><th>Actions</th></tr>
        </thead>
        <tbody>${users.map(rowHTML).join("")}</tbody>
      </table>`;
    } catch (err) {
        usersEl.innerHTML = `<p class="empty-state">Couldn't load users: ${err.message}</p>`;
    }
}

function rowHTML(u) {
    const isSelf = u.id === auth.user?.id;
    const statusBadge = u.locked
        ? `<span class="badge badge--danger">Locked</span>`
        : `<span class="badge badge--ok">Active</span>`;
    const shareBadge = u.canShare
        ? `<span class="badge badge--ok">Allowed</span>`
        : `<span class="badge badge--danger">Blocked</span>`;

    const actions = isSelf
        ? `<span class="card__meta">— you —</span>`
        : `<button class="btn ${u.locked ? "btn--outline" : "btn--danger"}" data-lock="${u.id}" data-locked="${u.locked}">
         ${u.locked ? "Unlock" : "Lock"}
       </button>
       <button class="btn btn--outline" data-share="${u.id}" data-can="${u.canShare}">
         ${u.canShare ? "Block sharing" : "Allow sharing"}
       </button>`;

    return `
    <tr>
      <td>${escapeHtml(u.username)}</td>
      <td>${escapeHtml(u.email)}</td>
      <td>${escapeHtml(u.role)}</td>
      <td>${statusBadge}</td>
      <td>${shareBadge}</td>
      <td><div class="row" style="gap:var(--space-2); flex-wrap:wrap">${actions}</div></td>
    </tr>`;
}

usersEl.addEventListener("click", async (e) => {
    const lockBtn = e.target.closest("[data-lock]");
    const shareBtn = e.target.closest("[data-share]");
    try {
        if (lockBtn) {
            const locked = lockBtn.dataset.locked === "true";
            await api.updateAdminUser(lockBtn.dataset.lock, { locked: !locked });
            toast(locked ? "User unlocked." : "User locked.", "success");
            loadUsers();
        } else if (shareBtn) {
            const can = shareBtn.dataset.can === "true";
            await api.updateAdminUser(shareBtn.dataset.share, { canShare: !can });
            toast(can ? "Sharing blocked." : "Sharing allowed.", "success");
            loadUsers();
        }
    } catch (err) {
        toast(err.message || "Action failed.", "error");
    }
});

loadStats();
loadUsers();