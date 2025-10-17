function obterNomeAbaAtual(elemento) {
    if (!elemento) {
        return null;
    }

    const conteudo = elemento.closest(".conteudo");
    if (!conteudo || !conteudo.id) {
        return null;
    }

    const prefixo = "conteudo-";
    if (!conteudo.id.startsWith(prefixo)) {
        return null;
    }

    return conteudo.id.substring(prefixo.length);
}

function abrirAba(nome, url) {
    if (!nome) {
        return;
    }

    const abasContainer = document.getElementById("abas");
    const conteudosContainer = document.getElementById("conteudos");
    if (!abasContainer || !conteudosContainer) {
        return;
    }

    const abaId = "aba-" + nome;
    const conteudoId = "conteudo-" + nome;

    let aba = document.getElementById(abaId);
    if (!aba) {
        aba = document.createElement("div");
        aba.className = "aba";
        aba.id = abaId;

        const titulo = document.createElement("span");
        titulo.className = "aba-title";
        titulo.textContent = nome;
        aba.appendChild(titulo);

        if (nome !== "Portal") {
            const closeButton = document.createElement("button");
            closeButton.type = "button";
            closeButton.className = "close";
            closeButton.setAttribute("aria-label", `Fechar aba ${nome}`);
            closeButton.textContent = "×";
            closeButton.addEventListener("click", (event) => {
                event.stopPropagation();
                fecharAba(nome);
            });
            aba.appendChild(closeButton);
        }

        aba.addEventListener("click", () => ativarConteudo(nome));
        abasContainer.appendChild(aba);
    }

    let conteudo = document.getElementById(conteudoId);
    if (!conteudo) {
        conteudo = document.createElement("div");
        conteudo.className = "conteudo";
        conteudo.id = conteudoId;
        conteudosContainer.appendChild(conteudo);
    }

    if (url) {
        carregarConteudoEmAba(conteudo, url, nome);
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

function carregarConteudoEmAba(container, url, nome) {
    if (!container || !url) {
        return;
    }

    if (container.dataset.url === url && container.dataset.loaded === "true") {
        return;
    }

    container.dataset.url = url;
    container.dataset.loaded = "false";
    container.classList.add("loading");
    container.innerHTML = "<div class=\"loading-state\">Carregando " + nome + "...</div>";

    fetch(url, {
        headers: {
            "X-Requested-With": "XMLHttpRequest"
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Erro ao carregar aba");
            }
            return response.text();
        })
        .then(html => {
            const parser = new DOMParser();
            const doc = parser.parseFromString(html, "text/html");
            const main = doc.querySelector("main.container") || doc.body;

            container.innerHTML = main.innerHTML;
            container.querySelectorAll("script").forEach(script => script.remove());
            container.dataset.loaded = "true";
            container.classList.remove("loading");

            executarScriptsDoConteudo(doc, container);
            ativarTooltips();
            aplicarValidacao(container);
        })
        .catch(() => {
            container.classList.remove("loading");
            container.innerHTML = "<div class=\"error-state\">Não foi possível carregar esta rotina. Tente novamente mais tarde.</div>";
        });
}

function executarScriptsDoConteudo(documento, container) {
    if (!documento) {
        return;
    }

    const scripts = documento.querySelectorAll("main.container script, body script");
    scripts.forEach(script => {
        const isExternal = script.src && script.src.length > 0;
        if (isExternal) {
            return;
        }

        const novoScript = document.createElement("script");
        Array.from(script.attributes).forEach(attr => {
            if (attr.name !== "src") {
                novoScript.setAttribute(attr.name, attr.value);
            }
        });

        novoScript.textContent = script.textContent;
        container.appendChild(novoScript);
    });
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

function aplicarValidacao(contexto = document) {
    const $contexto = $(contexto);

    $contexto.find("form").each(function () {
        $(this).removeData("validator");
        $(this).removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse($(this));
    });

    $contexto.find("form input").off("input.portal blur.portal").on("input.portal blur.portal", function () {
        $(this).valid();
    });
}

document.addEventListener('DOMContentLoaded', () => {
    ativarTooltips();
    ativarConteudo('Portal');
    setupMenuModalInteractions();
    configurarAtalhosDeAbas();
    aplicarValidacao(document);
});

function configurarAtalhosDeAbas() {
    document.addEventListener("click", (event) => {
        const alvo = event.target.closest("[data-open-tab]");
        if (!alvo) {
            return;
        }

        const nome = alvo.getAttribute("data-tab");
        const url = alvo.getAttribute("data-url") || alvo.getAttribute("href");

        if (!nome || !url) {
            return;
        }

        event.preventDefault();

        if (alvo.hasAttribute("data-close-current-tab")) {
            const abaAtual = obterNomeAbaAtual(alvo);
            if (abaAtual) {
                fecharAba(abaAtual);
            }
        }

        abrirAba(nome, url);
    });
}
