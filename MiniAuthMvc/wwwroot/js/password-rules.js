
document.addEventListener("DOMContentLoaded", () => {
    const pw = document.getElementById("Password");
    const cpw = document.getElementById("ConfirmPassword");
    const confirmMsg = document.getElementById("confirmMsg");

    const rules = [
        { id: "rule-length", test: v => v.length >= 8 },
        { id: "rule-upper", test: v => /[A-Z]/.test(v) },
        { id: "rule-lower", test: v => /[a-z]/.test(v) },
        { id: "rule-digit", test: v => /\d/.test(v) },
        { id: "rule-special", test: v => /[^a-zA-Z0-9]/.test(v) }
    ];

    function setRuleState(ruleId, ok) {
        const el = document.getElementById(ruleId);
        if (!el) return;
        el.classList.toggle("valid", ok);
    }

    function updatePasswordRules() {
        const v = pw.value || "";
        rules.forEach(r => setRuleState(r.id, r.test(v)));
    }

    function updateConfirmMessage() {
        const p1 = pw.value || "";
        const p2 = cpw.value || "";

        confirmMsg.className = "small mt-1";
        confirmMsg.textContent = "";

        if (p2.length === 0) return;

        if (p1 === p2) {
            confirmMsg.classList.add("text-success");
            confirmMsg.textContent = "Password matched ✅";
        } else {
            confirmMsg.classList.add("text-danger");
            confirmMsg.textContent = "Passwords do not match ❌";
        }
    }

    pw.addEventListener("input", () => {
        updatePasswordRules();
        updateConfirmMessage();
    });

    cpw.addEventListener("input", updateConfirmMessage);

    // initial render
    updatePasswordRules();
    updateConfirmMessage();
});
