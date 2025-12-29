const toggleBtn = document.getElementById("themeToggle");
const html = document.documentElement;

// Load saved theme
const savedTheme = localStorage.getItem("theme") || "dark";
html.setAttribute("data-bs-theme", savedTheme);
toggleBtn.textContent = savedTheme === "dark" ? "🌙" : "☀️";

// Toggle theme
toggleBtn.addEventListener("click", () => {
    const current = html.getAttribute("data-bs-theme");
    const next = current === "dark" ? "light" : "dark";

    html.setAttribute("data-bs-theme", next);
    localStorage.setItem("theme", next);
    toggleBtn.textContent = next === "dark" ? "🌙" : "☀️";
});
