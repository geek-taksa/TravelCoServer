// pages/quizzes.js — two time-limited country quizzes

import { api } from "../api.js";
import { mountChrome, toast } from "../ui.js";
import { escapeHtml } from "../utils.js";

mountChrome("quizzes.html");

const QUIZ_IDS = [1, 2];

const hubEl = document.getElementById("quizHub");
const playEl = document.getElementById("quizPlay");
const pointsEl = document.getElementById("pointsTotal");

async function showPoints() {
    try { pointsEl.textContent = await api.getPoints(); }
    catch { pointsEl.textContent = "0"; }
}

let timer = null;

async function loadHub() {
    showPoints();
    hubEl.innerHTML = `<div class="loading-state"><div class="spinner"></div>Loading quizzes…</div>`;
    try {
        const quizzes = await Promise.all(QUIZ_IDS.map((id) => api.getQuiz(id)));
        hubEl.innerHTML = `<div class="grid quiz-grid">${quizzes.map(hubCard).join("")}</div>`;
        hubEl.querySelectorAll("[data-play]").forEach((b) =>
            b.addEventListener("click", () => startQuiz(Number(b.dataset.play)))
        );
    } catch (err) {
        hubEl.innerHTML = `<p class="empty-state">Couldn't load quizzes: ${err.message}</p>`;
    }
}

function hubCard(q) {
    return `
    <div class="card quiz-card">
      <div class="card__body">
        <h3 class="card__title">${escapeHtml(q.title)}</h3>
        <p class="card__meta">${q.questions.length} questions · ${q.timeLimitSec}s time limit</p>
        <button class="btn btn--primary" style="margin-top:var(--space-4)" data-play="${q.id}">Play</button>
      </div>
    </div>`;
}

async function startQuiz(id) {
    const quiz = await api.getQuiz(id);
    hubEl.classList.add("hidden");
    playEl.classList.remove("hidden");

    let remaining = quiz.timeLimitSec;

    playEl.innerHTML = `
    <div class="quiz-head">
      <h2 class="quiz-title">${escapeHtml(quiz.title)}</h2>
      <span class="timer" id="timer">${fmtTime(remaining)}</span>
    </div>
    <form id="quizForm" class="stack-lg">
      ${quiz.questions.map(questionHTML).join("")}
      <button type="submit" class="btn btn--primary btn--block">Submit answers</button>
    </form>`;

    const timerEl = document.getElementById("timer");
    timer = setInterval(() => {
        remaining--;
        timerEl.textContent = fmtTime(remaining);
        if (remaining <= 10) timerEl.classList.add("timer--low");
        if (remaining <= 0) { clearInterval(timer); finishQuiz(quiz); }
    }, 1000);

    document.getElementById("quizForm").addEventListener("submit", (e) => {
        e.preventDefault();
        clearInterval(timer);
        finishQuiz(quiz);
    });
}

function questionHTML(q, i) {
    return `
    <fieldset class="quiz-q card">
      <div class="card__body">
        <legend class="quiz-q__prompt">${i + 1}. ${escapeHtml(q.prompt)}</legend>
        <div class="quiz-options">
          ${q.options.map((opt, oi) => `
            <label class="quiz-option">
              <input type="radio" name="q${q.id}" value="${oi}" />
              <span>${escapeHtml(opt)}</span>
            </label>`).join("")}
        </div>
      </div>
    </fieldset>`;
}

async function finishQuiz(quiz) {
    const form = document.getElementById("quizForm");

    // build the answers array the API expects: [{ questionId, selectedIndex }]
    const answers = quiz.questions.map((q) => {
        const picked = form.querySelector(`input[name="q${q.id}"]:checked`);
        return { questionId: q.id, selectedIndex: picked ? Number(picked.value) : -1 };
    });

    let result;
    try {
        result = await api.submitQuiz(quiz.id, answers);   // server scores it -> { score, points }
    } catch (err) {
        toast(err.message || "Couldn't submit quiz.", "error");
        return;
    }

    showPoints();

    playEl.innerHTML = `
    <div class="card result-card">
      <div class="card__body text-center">
        <h2 class="quiz-title">Results — ${escapeHtml(quiz.title)}</h2>
        <p class="result-score">${result.score} / ${quiz.questions.length}</p>
        <p class="card__meta">You earned <strong>${result.points}</strong> points.</p>
        <div class="row" style="justify-content:center; gap:var(--space-3); margin-top:var(--space-6)">
          <button class="btn btn--outline" id="againBtn">Play again</button>
          <button class="btn btn--primary" id="backBtn">Back to quizzes</button>
        </div>
      </div>
    </div>`;

    document.getElementById("againBtn").addEventListener("click", () => startQuiz(quiz.id));
    document.getElementById("backBtn").addEventListener("click", backToHub);
    toast(`+${result.points} points!`, "success");
}

function backToHub() {
    playEl.classList.add("hidden");
    playEl.innerHTML = "";
    hubEl.classList.remove("hidden");
    showPoints();
}

function fmtTime(sec) {
    const s = Math.max(0, sec);
    return `${String(Math.floor(s / 60)).padStart(2, "0")}:${String(s % 60).padStart(2, "0")}`;
}

loadHub();