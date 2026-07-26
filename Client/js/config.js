// config.js — central configuration.
// Flip USE_MOCK to false and set BASE_URL to your ASP.NET API when the
// backend is ready. Nothing else in the client needs to change.
export const CONFIG = {
    USE_MOCK: false,
    BASE_URL: "https://localhost:7000",   // your backend's URL — use the exact one from your Swagger address bar
    TOKEN_KEY: "travelco_token",
    USER_KEY: "travelco_user",
    MOCK_DELAY: 250,                // simulated latency so loading states show
};