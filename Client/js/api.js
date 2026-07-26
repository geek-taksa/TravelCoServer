// api.js — the single network boundary for the whole client.
// No page calls fetch() directly; everything goes through here.
// While CONFIG.USE_MOCK is true, requests are served from /mock/*.json and
// filtered/sorted in-memory. Flip USE_MOCK in config.js when the API is ready.
import { CONFIG } from "./config.js";

/* token helpers */
export const token = {
    get: () => localStorage.getItem(CONFIG.TOKEN_KEY),
    set: (t) => localStorage.setItem(CONFIG.TOKEN_KEY, t),
    clear: () => localStorage.removeItem(CONFIG.TOKEN_KEY),
};

/* request (backend) */
async function request(path, { method = "GET", body, auth = true } = {})
{
    const headers = { "Content-Type": "application/json" };
    if (auth && token.get()) headers.Authorization = `Bearer ${token.get()}`;
    const res = await fetch(`${CONFIG.BASE_URL}${path}`, {
        method, headers, body: body ? JSON.stringify(body) : undefined,
    });
    if (!res.ok) {
        let message = `Request failed (${res.status})`;
        try { message = (await res.json()).message || message; } catch { }
        throw new Error(message);
    }
    const text = await res.text();
    return text ? JSON.parse(text) : null;
}


/* mock helpers */
const _cache = {};
async function loadMock(name) {
    if (!_cache[name]) {
        const res = await fetch(`./mock/${name}.json`);
        _cache[name] = await res.json();
    }
    return JSON.parse(JSON.stringify(_cache[name])); // deep copy
}
const delay = (ms = CONFIG.MOCK_DELAY) => new Promise((r) => setTimeout(r, ms));

// mock persistence for the user's Visited / Want-to-visit lists
const mockLists = {
    KEY: "travelco_mock_lists",
    read() {
        try { return JSON.parse(localStorage.getItem(this.KEY)) || { visited: [], wishlist: [] }; }
        catch { return { visited: [], wishlist: [] }; }
    },
    write(store) { localStorage.setItem(this.KEY, JSON.stringify(store)); },
};

// mock persistence for community shares (seeded from shares.json on first use)
const mockShares = {
    KEY: "travelco_mock_shares",
    async all() {
        const raw = localStorage.getItem(this.KEY);
        if (raw) return JSON.parse(raw);
        const seed = await loadMock("shares");
        localStorage.setItem(this.KEY, JSON.stringify(seed));
        return seed;
    },
    save(list) { localStorage.setItem(this.KEY, JSON.stringify(list)); },
};

// mock persistence for admin user management (seeded from users.json)
const mockUsers = {
    KEY: "travelco_mock_users",
    async all() {
        const raw = localStorage.getItem(this.KEY);
        if (raw) return JSON.parse(raw);
        const seed = await loadMock("users");
        localStorage.setItem(this.KEY, JSON.stringify(seed));
        return seed;
    },
    save(list) { localStorage.setItem(this.KEY, JSON.stringify(list)); },
};

// search/filter/sort a country array (mock side; real API does this server-side)
function queryCountries(list, params = {}) {
    let out = [...list];
    const { search, region, language, currency, sort, order = "asc" } = params;
    if (search) { const q = search.toLowerCase(); out = out.filter((c) => c.name.toLowerCase().includes(q)); }
    if (region) out = out.filter((c) => c.region === region);
    if (language) out = out.filter((c) => (c.languages || []).includes(language));
    if (currency) out = out.filter((c) => (c.currencies || []).includes(currency));
    if (sort) {
        out.sort((a, b) => {
            const av = a[sort], bv = b[sort];
            const cmp = typeof av === "string" ? av.localeCompare(bv) : (av - bv);
            return order === "desc" ? -cmp : cmp;
        });
    }
    return out;
}

/* Public API */
export const api = {
    /* auth / users */
    async register(data) {
        if (CONFIG.USE_MOCK) { await delay(); return { token: "mock-token", user: { ...data, role: "user" } }; }
        return request("/api/auth/register", { method: "POST", body: data, auth: false });
    },
    async login({ email, password }) {
        if (CONFIG.USE_MOCK) {
            await delay();
            const users = await loadMock("users");
            const user = users.find((u) => u.email === email);
            if (!user) throw new Error("Invalid email or password.");
            return { token: "mock-token", user };
        }
        return request("/api/auth/login", { method: "POST", body: { email, password }, auth: false });
    },
    async logout() {
        if (CONFIG.USE_MOCK) { await delay(80); return null; }
        return request("/api/auth/logout", { method: "POST" });
    },
    async me() {
        if (CONFIG.USE_MOCK) { await delay(); return (await loadMock("users"))[0]; }
        return request("/api/users/me");
    },
    async updateMe(data) {
        if (CONFIG.USE_MOCK) { await delay(); return { ...(await loadMock("users"))[0], ...data }; }
        return request("/api/users/me", { method: "PUT", body: data });
    },

    /* countries */
    async getCountries(params = {}) {
        if (CONFIG.USE_MOCK) { await delay(); return queryCountries(await loadMock("countries"), params); }
        const qs = new URLSearchParams(Object.entries(params).filter(([, v]) => v !== undefined && v !== "")).toString();
        return request(`/api/countries${qs ? `?${qs}` : ""}`);
    },
    async getCountry(code) {
        if (CONFIG.USE_MOCK) {
            await delay();
            const c = (await loadMock("countries")).find((x) => x.code === code);
            if (!c) throw new Error("Country not found.");
            return c;
        }
        return request(`/api/countries/${code}`);
    },
    async getRegionCounts() {
        if (CONFIG.USE_MOCK) {
            await delay();
            const list = await loadMock("countries");
            const counts = {};
            list.forEach((c) => { counts[c.region] = (counts[c.region] || 0) + 1; });
            return counts;
        }
        return request("/api/countries/region-counts");
    },

    /* ---- personal lists ---- */
    async getLists() {
        if (CONFIG.USE_MOCK) {
            await delay();
            const store = mockLists.read();
            const byCode = Object.fromEntries((await loadMock("countries")).map((c) => [c.code, c]));
            const hydrate = (codes) => codes.map((code) => byCode[code]).filter(Boolean);
            return { visited: hydrate(store.visited), wishlist: hydrate(store.wishlist) };
        }
        return request("/api/lists");
    },
    async addToList(type, countryCode) {
        if (CONFIG.USE_MOCK) {
            await delay();
            const store = mockLists.read();
            const other = type === "visited" ? "wishlist" : "visited";
            store[other] = store[other].filter((c) => c !== countryCode);
            if (!store[type].includes(countryCode)) store[type].push(countryCode);
            mockLists.write(store);
            return { ok: true };
        }
        return request(`/api/lists/${type}`, { method: "POST", body: { countryCode } });
    },
    async removeFromList(type, countryCode) {
        if (CONFIG.USE_MOCK) {
            await delay();
            const store = mockLists.read();
            store[type] = store[type].filter((c) => c !== countryCode);
            mockLists.write(store);
            return null;
        }
        return request(`/api/lists/${type}/${countryCode}`, { method: "DELETE" });
    },
    async moveInList({ countryCode, from, to }) {
        if (CONFIG.USE_MOCK) {
            await delay();
            const store = mockLists.read();
            store[from] = store[from].filter((c) => c !== countryCode);
            if (!store[to].includes(countryCode)) store[to].push(countryCode);
            mockLists.write(store);
            return { ok: true };
        }
        return request("/api/lists/move", { method: "PUT", body: { countryCode, from, to } });
    },

    /* ---- shares ---- */
    async getShares(country) {
        if (CONFIG.USE_MOCK) {
            await delay();
            const shares = (await mockShares.all()).slice().reverse();
            return country ? shares.filter((s) => s.countryCode === country) : shares;
        }
        return request(`/api/shares${country ? `?country=${country}` : ""}`);
    },
    async createShare(data) {
        if (CONFIG.USE_MOCK) {
            await delay();
            const all = await mockShares.all();
            const share = { id: Date.now(), createdAt: new Date().toISOString().slice(0, 10), ...data };
            all.push(share);
            mockShares.save(all);
            return share;
        }
        return request("/api/shares", { method: "POST", body: data });
    },
    async updateShare(id, data) {
        if (CONFIG.USE_MOCK) {
            await delay();
            const all = await mockShares.all();
            const i = all.findIndex((s) => s.id === Number(id));
            if (i === -1) throw new Error("Share not found.");
            all[i] = { ...all[i], ...data };
            mockShares.save(all);
            return all[i];
        }
        return request(`/api/shares/${id}`, { method: "PUT", body: data });
    },
    async deleteShare(id) {
        if (CONFIG.USE_MOCK) {
            await delay();
            mockShares.save((await mockShares.all()).filter((s) => s.id !== Number(id)));
            return null;
        }
        return request(`/api/shares/${id}`, { method: "DELETE" });
    },

    /* quizzes */
    async getQuiz(id) {
        if (CONFIG.USE_MOCK) {
            await delay();
            const q = (await loadMock("quizzes")).find((x) => x.id === Number(id));
            if (!q) throw new Error("Quiz not found.");
            return q;
        }
        return request(`/api/quizzes/${id}`);
    },
    async submitQuiz(id, answers) {
        if (CONFIG.USE_MOCK) { await delay(); return { score: 0, points: 0 }; }
        return request(`/api/quizzes/${id}/submit`, { method: "POST", body: { answers } });
    },
    async getPoints() {
        if (CONFIG.USE_MOCK) return Number(localStorage.getItem("travelco_points") || 0);
        return request("/api/quizzes/points");
    },

    /* ---- admin ----*/
     
    async getAdminUsers() {
        if (CONFIG.USE_MOCK) { await delay(); return mockUsers.all(); }
        return request("/api/admin/users");
    },
    async updateAdminUser(id, flags) {
        if (CONFIG.USE_MOCK) {
            await delay();
            const users = await mockUsers.all();
            const u = users.find((x) => x.id === Number(id));
            if (!u) throw new Error("User not found.");
            Object.assign(u, flags);
            mockUsers.save(users);
            return u;
        }
        return request(`/api/admin/users/${id}`, { method: "PUT", body: flags });
    },
    async getAdminStats() {
        if (CONFIG.USE_MOCK) {
            await delay();
            const countries = await loadMock("countries");
            const lists = mockLists.read();
            const shares = await mockShares.all();
            return {
                dailyLogins: 42, // no auth telemetry in mock; static placeholder
                countriesImported: countries.length,
                countriesSaved: lists.visited.length + lists.wishlist.length,
                sharesCreated: shares.length,
            };
        }
        return request("/api/admin/stats");
    },

    // Countries page -- sort functionality by language and currency
    async getCountryLanguages() {
        if (CONFIG.USE_MOCK) {
            const list = await loadMock("countries");
            return [...new Set(list.flatMap((c) => c.languages || []))].sort();
        }
        return request("/api/countries/languages");
    },
    async getCountryCurrencies() {
        if (CONFIG.USE_MOCK) {
            const list = await loadMock("countries");
            return [...new Set(list.flatMap((c) => c.currencies || []))].sort();
        }
        return request("/api/countries/currencies");
    },
};