document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".pw-toggle").forEach(btn => {
        btn.addEventListener("click", () => {
            const targetId = btn.getAttribute("data-target");
            const input = document.getElementById(targetId);
            if (!input) return;

            const isHidden = input.type === "password";
            input.type = isHidden ? "text" : "password";
            btn.textContent = isHidden ? "Hide" : "Show";
        });
    });
});
