function abrirAba(nome) {
    const abasContainer = document.getElementById("abas");
    const conteudosContainer = document.getElementById("conteudos");
    const abaId = "aba-" + nome;
    const conteudoId = "conteudo-" + nome;

    let aba = document.getElementById(abaId);
    if (!aba) {
        aba = document.createElement("div");
        aba.className = "aba";
        aba.id = abaId;
        aba.innerHTML = nome + (nome === "Portal" ? "" : " <span class=\"close\" onclick=\"fecharAba('" + nome + "')\">×</span>");
        aba.onclick = () => ativarConteudo(nome);
        abasContainer.appendChild(aba);
    }

    let conteudo = document.getElementById(conteudoId);
    if (!conteudo) {
        conteudo = document.createElement("div");
        conteudo.className = "conteudo";
        conteudo.id = conteudoId;
        conteudo.innerHTML = "<h2>" + nome + "</h2><p>Conteúdo de " + nome + " em desenvolvimento.</p>";
        conteudosContainer.appendChild(conteudo);
    }

    ativarConteudo(nome);
}

function ativarConteudo(nome) {
    const conteudo = document.getElementById("conteudo-" + nome);
    const aba = document.getElementById("aba-" + nome);

    if (conteudo) {
        document.querySelectorAll(".conteudo").forEach(div => div.classList.remove("active"));
        conteudo.classList.add("active");
    }

    if (aba) {
        document.querySelectorAll(".aba").forEach(div => div.classList.remove("active"));
        aba.classList.add("active");
    }
}

function fecharAba(nome) {
    if (nome === "Portal") return;

    const aba = document.getElementById("aba-" + nome);
    const conteudo = document.getElementById("conteudo-" + nome);

    if (aba) aba.remove();
    if (conteudo) conteudo.remove();

    const abasRestantes = document.querySelectorAll(".aba");
    if (abasRestantes.length > 0) {
        const ultimaAba = abasRestantes[abasRestantes.length - 1];
        const id = ultimaAba.id.replace("aba-", "");
        ativarConteudo(id);
    } else {
        ativarConteudo("Portal");
    }
}

let menuIsOpen = false;
let menuHoverTimeout = null;
let menuModalElement = null;
let menuToggleElement = null;

function openMenu() {
    if (!menuModalElement || !menuToggleElement) return;
    clearTimeout(menuHoverTimeout);
    menuModalElement.classList.add("open");
    menuToggleElement.setAttribute("aria-expanded", "true");
    menuIsOpen = true;
}

function closeMenu() {
    if (!menuModalElement || !menuToggleElement) return;
    clearTimeout(menuHoverTimeout);
    menuModalElement.classList.remove("open");
    menuToggleElement.setAttribute("aria-expanded", "false");
    menuIsOpen = false;
}

function scheduleMenuClose() {
    clearTimeout(menuHoverTimeout);
    menuHoverTimeout = setTimeout(() => {
        closeMenu();
    }, 180);
}

function toggleMenu(evt) {
    if (evt) {
        evt.stopPropagation();
    }

    if (menuIsOpen) {
        closeMenu();
    } else {
        openMenu();
    }
}

function setupMenuModalInteractions() {
    menuModalElement = document.getElementById("menuModal");
    menuToggleElement = document.getElementById("menuToggle");

    if (!menuModalElement || !menuToggleElement) {
        return;
    }

    const keepMenuOpen = () => {
        openMenu();
    };

    menuToggleElement.addEventListener("mouseenter", keepMenuOpen);
    menuToggleElement.addEventListener("focus", keepMenuOpen);
    menuModalElement.addEventListener("mouseenter", keepMenuOpen);

    menuToggleElement.addEventListener("mouseleave", scheduleMenuClose);
    menuToggleElement.addEventListener("blur", scheduleMenuClose);
    menuModalElement.addEventListener("mouseleave", scheduleMenuClose);

    document.addEventListener("click", (event) => {
        if (!menuIsOpen) return;

        if (event.target === menuToggleElement) return;
        if (menuModalElement.contains(event.target)) return;

        closeMenu();
    });

    document.addEventListener("keydown", (event) => {
        if (event.key === "Escape") {
            closeMenu();
        }
    });

    menuModalElement.querySelectorAll(".menu-item").forEach(button => {
        button.addEventListener("click", () => {
            closeMenu();
        });
    });
}

function ativarTooltips() {
    const tooltips = document.querySelectorAll('.tooltip-message, .field-validation-error');

    tooltips.forEach(tooltip => {
        const input = tooltip.previousElementSibling;
        if (!input) return;

        const mostrarTooltip = () => {
            const message = tooltip.textContent.trim();
            if (message.length > 0) {
                tooltip.classList.remove("hidden");
                tooltip.classList.add("active");

                if (tooltip.timeoutId) clearTimeout(tooltip.timeoutId);

                tooltip.timeoutId = setTimeout(() => {
                    tooltip.classList.remove("active");
                    tooltip.classList.add("hidden");
                }, 3000);

                tooltip.addEventListener("click", () => {
                    tooltip.classList.remove("active");
                    tooltip.classList.add("hidden");
                });
            }
        };

        input.addEventListener("blur", () => {
            mostrarTooltip();
        });

        if (tooltip.classList.contains("field-validation-error") && tooltip.textContent.trim() !== "") {
            mostrarTooltip();
        }
    });
}

document.addEventListener('DOMContentLoaded', () => {
    ativarTooltips();
    ativarConteudo('Portal');
    setupMenuModalInteractions();
});

$(document).ready(function () {
    $("form").each(function () {
        $(this).removeData("validator");
        $(this).removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse($(this));
    });

    $("form input").on("input blur", function () {
        $(this).valid();
    });
});
