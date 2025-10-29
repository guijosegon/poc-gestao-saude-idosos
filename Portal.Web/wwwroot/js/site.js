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

    if (nome === "Portal") {
        const portalContainer = document.getElementById("conteudo-Portal");
        if (portalContainer && portalContainer.dataset.refreshOnActivate === "true") {
            delete portalContainer.dataset.refreshOnActivate;
            atualizarResumoPortal();
        }
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

function decodeHeaderValue(value) {
    if (!value) {
        return null;
    }

    try {
        return decodeURIComponent(value);
    } catch (error) {
        return value;
    }
}

function obterMensagensTempData(headers) {
    if (!headers || typeof headers.get !== "function") {
        return null;
    }

    const sucesso = decodeHeaderValue(headers.get("X-TempData-Sucesso"));
    const erro = decodeHeaderValue(headers.get("X-TempData-Erro"));

    if (!sucesso && !erro) {
        return null;
    }

    return {
        sucesso: sucesso || null,
        erro: erro || null
    };
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
        .then(async response => {
            if (!response.ok) {
                throw new Error("Erro ao carregar aba");
            }
            const html = await response.text();
            const mensagensTempData = obterMensagensTempData(response.headers);
            return { html, mensagensTempData };
        })
        .then(({ html, mensagensTempData }) => {
            const parser = new DOMParser();
            const doc = parser.parseFromString(html, "text/html");
            atualizarConteudoDaAba(container, doc, mensagensTempData);
        })
        .catch(() => {
            container.classList.remove("loading");
            container.innerHTML = "<div class=\"error-state\">Não foi possível carregar esta rotina. Tente novamente mais tarde.</div>";
        });
}

async function atualizarResumoPortal() {
    const portalContainer = document.getElementById("conteudo-Portal");
    if (!portalContainer) {
        return;
    }

    const url = portalContainer.dataset.refreshUrl || portalContainer.dataset.url;
    if (!url) {
        return;
    }

    const conteudoAnterior = portalContainer.innerHTML;
    const urlAnterior = portalContainer.dataset.url;
    const estadoAnterior = portalContainer.dataset.loaded;

    portalContainer.dataset.url = url;
    portalContainer.dataset.loaded = "false";
    portalContainer.classList.add("loading");
    portalContainer.innerHTML = "<div class=\"loading-state\">Atualizando Portal...</div>";

    try {
        const response = await fetch(url, {
            headers: {
                "X-Requested-With": "XMLHttpRequest",
                "Accept": "text/html"
            },
            credentials: "same-origin"
        });

        if (!response.ok) {
            throw new Error("Erro ao atualizar Portal");
        }

        const html = await response.text();
        const parser = new DOMParser();
        const doc = parser.parseFromString(html, "text/html");

        atualizarConteudoDaAba(portalContainer, doc, null);
    } catch (error) {
        portalContainer.classList.remove("loading");
        portalContainer.innerHTML = conteudoAnterior;
        if (estadoAnterior) {
            portalContainer.dataset.loaded = estadoAnterior;
        } else {
            delete portalContainer.dataset.loaded;
        }

        if (urlAnterior) {
            portalContainer.dataset.url = urlAnterior;
        } else {
            delete portalContainer.dataset.url;
        }
    }
}

function agendarAtualizacaoPortal() {
    const portalContainer = document.getElementById("conteudo-Portal");
    if (!portalContainer) {
        return Promise.resolve();
    }

    if (portalContainer.classList.contains("active")) {
        return atualizarResumoPortal();
    }

    portalContainer.dataset.refreshOnActivate = "true";
    return Promise.resolve();
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

function removerAlerta(alerta) {
    if (!alerta || alerta.dataset.alertDismissed === "true") {
        return;
    }

    alerta.dataset.alertDismissed = "true";
    alerta.classList.add("dismissed");

    setTimeout(() => {
        alerta.remove();
    }, 180);
}

function configurarAlertasGlobais(contexto) {
    let container = null;

    if (contexto instanceof HTMLElement) {
        container = contexto.classList.contains("global-alerts")
            ? contexto
            : contexto.querySelector(".global-alerts");
    } else if (contexto && typeof contexto.querySelector === "function") {
        container = contexto.querySelector(".global-alerts");
    }

    if (!container) {
        container = document.querySelector(".global-alerts");
    }

    if (!container) {
        return;
    }

    container.querySelectorAll(".alert").forEach(alerta => {
        if (alerta.dataset.alertConfigured === "true") {
            return;
        }

        alerta.dataset.alertConfigured = "true";

        setTimeout(() => removerAlerta(alerta), 5000);

        alerta.addEventListener("click", () => removerAlerta(alerta));
    });
}

function atualizarAlertasGlobais(documento, mensagensTempData) {
    const alertasAtuais = document.querySelector(".global-alerts");
    const novosAlertas = documento ? documento.querySelector(".global-alerts") : null;

    if (!alertasAtuais) {
        return;
    }

    if (novosAlertas && novosAlertas.innerHTML.trim().length > 0) {
        alertasAtuais.innerHTML = novosAlertas.innerHTML;
        configurarAlertasGlobais(alertasAtuais);
        return;
    }

    alertasAtuais.innerHTML = "";

    if (!mensagensTempData) {
        configurarAlertasGlobais(alertasAtuais);
        return;
    }

    const { sucesso, erro } = mensagensTempData;

    if (sucesso) {
        const alertaSucesso = document.createElement("div");
        alertaSucesso.className = "alert success";
        alertaSucesso.setAttribute("role", "alert");
        alertaSucesso.textContent = sucesso;
        alertasAtuais.appendChild(alertaSucesso);
    }

    if (erro) {
        const alertaErro = document.createElement("div");
        alertaErro.className = "alert danger";
        alertaErro.setAttribute("role", "alert");
        alertaErro.textContent = erro;
        alertasAtuais.appendChild(alertaErro);
    }

    configurarAlertasGlobais(alertasAtuais);
}

function atualizarConteudoDaAba(conteudo, documento, mensagensTempData) {
    if (!conteudo || !documento) {
        return;
    }

    const alvoPrincipal = documento.querySelector("main.container") || documento.body;
    const fragmento = alvoPrincipal.cloneNode(true);
    fragmento.querySelectorAll("script").forEach(script => script.remove());

    conteudo.innerHTML = fragmento.innerHTML;
    conteudo.dataset.loaded = "true";
    conteudo.classList.remove("loading");

    atualizarAlertasGlobais(documento, mensagensTempData);
    executarScriptsDoConteudo(documento, conteudo);
    ativarTooltips();
    aplicarValidacao(conteudo);
    inicializarMultiSelects(conteudo);
    inicializarGerenciadoresDeFoto(conteudo);
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

function inicializarGerenciadoresDeFoto(contexto = document) {
    const scope = contexto instanceof HTMLElement ? contexto : document;
    const containers = scope.querySelectorAll('[data-photo-manager]');

    containers.forEach(container => {
        if (!(container instanceof HTMLElement)) {
            return;
        }

        if (container.dataset.photoConfigured === 'true') {
            return;
        }

        const valueInput = container.querySelector('[data-photo-value]');
        const removeInput = container.querySelector('[data-photo-remove-flag]');
        const image = container.querySelector('[data-photo-image]');
        const emptyState = container.querySelector('[data-photo-empty]');
        const viewButton = container.querySelector('[data-photo-view]');
        const removeButton = container.querySelector('[data-photo-remove]');
        const defaultImage = container.dataset.photoDefault || '';
        let storedImage = container.dataset.photoStored || '';
        const fileInputSelector = container.dataset.photoFileInput || '';

        let fileInput = null;
        if (fileInputSelector) {
            fileInput = container.closest('form')?.querySelector(fileInputSelector)
                || scope.querySelector(fileInputSelector)
                || document.querySelector(fileInputSelector);
        }

        if (!(fileInput instanceof HTMLInputElement)) {
            fileInput = null;
        }

        let originalValue = valueInput ? valueInput.value : '';
        let objectUrl = null;
        let isMarkedForRemoval = removeInput ? removeInput.value === 'true' : false;

        const clearObjectUrl = () => {
            if (objectUrl) {
                URL.revokeObjectURL(objectUrl);
                objectUrl = null;
            }
        };

        const updateRemoveButtonLabel = (hasFile) => {
            if (!removeButton) {
                return;
            }

            const removeText = removeButton.dataset.removeText || removeButton.textContent;
            const restoreText = removeButton.dataset.restoreText || removeText;

            if (isMarkedForRemoval && !hasFile) {
                removeButton.textContent = restoreText;
            } else {
                removeButton.textContent = removeText;
            }
        };

        const updateState = () => {
            const hasFile = fileInput && fileInput.files && fileInput.files.length > 0;
            const hasOriginal = !!originalValue && !isMarkedForRemoval;
            const hasPreview = hasFile || hasOriginal;

            if (image) {
                if (hasFile && objectUrl) {
                    image.src = objectUrl;
                } else if (hasOriginal && storedImage) {
                    image.src = storedImage;
                } else if (defaultImage) {
                    image.src = defaultImage;
                }
            }

            if (emptyState) {
                emptyState.hidden = !!hasPreview;
            }

            if (viewButton) {
                viewButton.disabled = !hasPreview;
            }

            if (removeButton) {
                const canRemove = !!originalValue || hasFile;
                removeButton.disabled = !canRemove;
            }

            updateRemoveButtonLabel(hasFile);
        };

        const resetRemovalState = () => {
            isMarkedForRemoval = false;
            if (removeInput) {
                removeInput.value = 'false';
            }
            if (valueInput) {
                valueInput.value = originalValue;
            }
        };

        if (fileInput) {
            fileInput.addEventListener('change', () => {
                clearObjectUrl();

                if (fileInput.files && fileInput.files.length > 0) {
                    objectUrl = URL.createObjectURL(fileInput.files[0]);
                }

                resetRemovalState();
                updateState();
            });
        }

        if (removeButton) {
            removeButton.addEventListener('click', () => {
                const hasFile = fileInput && fileInput.files && fileInput.files.length > 0;

                if (hasFile) {
                    clearObjectUrl();
                    if (fileInput) {
                        fileInput.value = '';
                    }
                    updateState();
                    return;
                }

                if (!originalValue) {
                    isMarkedForRemoval = false;
                    if (removeInput) {
                        removeInput.value = 'false';
                    }
                    updateState();
                    return;
                }

                isMarkedForRemoval = !isMarkedForRemoval;

                if (removeInput) {
                    removeInput.value = isMarkedForRemoval ? 'true' : 'false';
                }

                if (valueInput) {
                    valueInput.value = isMarkedForRemoval ? '' : originalValue;
                }

                updateState();
            });
        }

        if (viewButton) {
            viewButton.addEventListener('click', () => {
                if (viewButton.disabled) {
                    return;
                }

                const hasFile = fileInput && fileInput.files && fileInput.files.length > 0;
                let previewUrl = null;

                if (hasFile && objectUrl) {
                    previewUrl = objectUrl;
                } else if (!isMarkedForRemoval && storedImage) {
                    previewUrl = storedImage;
                } else if (defaultImage) {
                    previewUrl = defaultImage;
                }

                if (previewUrl) {
                    window.open(previewUrl, '_blank', 'noopener');
                }
            });
        }

        if (valueInput) {
            valueInput.addEventListener('change', () => {
                originalValue = valueInput.value;
                storedImage = container.dataset.photoStored || storedImage;
                updateState();
            });
        }

        container.dataset.photoConfigured = 'true';
        updateState();
    });
}

function inicializarMultiSelects(contexto = document) {
    const alvo = contexto instanceof HTMLElement ? contexto : document;
    const containers = alvo.querySelectorAll('[data-multi-select]');

    containers.forEach(container => {
        if (!container || container.dataset.multiSelectReady === "true") {
            return;
        }

        container.dataset.multiSelectReady = "true";

        const chipsContainer = container.querySelector('[data-multi-select-chips]');
        const searchInput = container.querySelector('[data-multi-select-search]');
        const optionElements = Array.from(container.querySelectorAll('.multi-select__option'));

        if (!chipsContainer || optionElements.length === 0) {
            return;
        }

        const placeholder = chipsContainer.dataset.placeholder || '';

        const atualizarChips = () => {
            chipsContainer.innerHTML = '';

            const selecionados = optionElements.filter(opcao => {
                const checkbox = opcao.querySelector('input[type="checkbox"]');
                return checkbox && checkbox.checked;
            });

            if (selecionados.length === 0) {
                if (placeholder) {
                    const placeholderElement = document.createElement('span');
                    placeholderElement.className = 'multi-select__placeholder';
                    placeholderElement.textContent = placeholder;
                    chipsContainer.appendChild(placeholderElement);
                }
                return;
            }

            selecionados.forEach(opcao => {
                const checkbox = opcao.querySelector('input[type="checkbox"]');
                const label = opcao.dataset.label || checkbox?.value || '';
                if (!checkbox) {
                    return;
                }

                const chip = document.createElement('button');
                chip.type = 'button';
                chip.className = 'multi-select__chip';
                chip.dataset.value = checkbox.value;
                chip.innerHTML = `<span>${label}</span><span aria-hidden="true">×</span>`;
                chip.addEventListener('click', () => {
                    checkbox.checked = false;
                    checkbox.dispatchEvent(new Event('change', { bubbles: true }));
                });
                chipsContainer.appendChild(chip);
            });
        };

        optionElements.forEach(opcao => {
            const checkbox = opcao.querySelector('input[type="checkbox"]');
            if (!checkbox) {
                return;
            }

            checkbox.addEventListener('change', atualizarChips);
        });

        if (searchInput) {
            searchInput.addEventListener('input', () => {
                const termo = searchInput.value.trim().toLowerCase();
                optionElements.forEach(opcao => {
                    const texto = (opcao.dataset.label || '').toLowerCase();
                    const visivel = termo.length === 0 || texto.includes(termo);
                    opcao.style.display = visivel ? '' : 'none';
                });
            });
        }

        atualizarChips();
    });
}

document.addEventListener('DOMContentLoaded', () => {
    ativarTooltips();
    ativarConteudo('Portal');
    setupMenuModalInteractions();
    configurarAtalhosDeAbas();
    inicializarMultiSelects();
    inicializarGerenciadoresDeFoto();
    aplicarValidacao(document);
    configurarEnvioDeFormularios();
    configurarResetDeFiltros();
    configurarAlertasGlobais();
    configurarSelecaoDeFormularios();
});

function configurarAtalhosDeAbas() {
    document.addEventListener("click", (event) => {
        const alvo = event.target.closest("[data-open-tab],[data-close-current-tab]");
        if (!alvo) {
            return;
        }

        const deveFecharAbaAtual = alvo.hasAttribute("data-close-current-tab");
        const abaAtual = deveFecharAbaAtual ? obterNomeAbaAtual(alvo) : null;

        if (alvo.hasAttribute("data-open-tab")) {
            const nome = alvo.getAttribute("data-tab");
            const url = alvo.getAttribute("data-url") || alvo.getAttribute("href");

            if (!nome || !url) {
                if (deveFecharAbaAtual && abaAtual) {
                    event.preventDefault();
                    fecharAba(abaAtual);
                }

                return;
            }

            event.preventDefault();

            if (deveFecharAbaAtual && abaAtual) {
                fecharAba(abaAtual);
            }

            abrirAba(nome, url);
            return;
        }

        if (deveFecharAbaAtual && abaAtual) {
            event.preventDefault();
            fecharAba(abaAtual);
        }
    });
}

function obterTokenAntifalsificacao(form) {
    if (form) {
        const campoFormulario = form.querySelector('input[name="__RequestVerificationToken"]');
        if (campoFormulario && campoFormulario.value) {
            return campoFormulario.value;
        }
    }

    const campoGlobal = document.querySelector('input[name="__RequestVerificationToken"]');
    if (campoGlobal && campoGlobal.value) {
        return campoGlobal.value;
    }

    const metaToken = document.querySelector('meta[name="request-verification-token"]');
    if (metaToken && metaToken.content) {
        return metaToken.content;
    }

    return null;
}

function atualizarTokenAntifalsificacao(documento) {
    if (!documento) {
        return;
    }

    const metaOrigem = documento.querySelector('meta[name="request-verification-token"]');
    if (!metaOrigem || !metaOrigem.content) {
        return;
    }

    let metaAtual = document.querySelector('meta[name="request-verification-token"]');
    if (!metaAtual) {
        metaAtual = document.createElement('meta');
        metaAtual.setAttribute('name', 'request-verification-token');
        document.head.appendChild(metaAtual);
    }

    metaAtual.setAttribute('content', metaOrigem.content);
}

function configurarEnvioDeFormularios() {
    document.addEventListener("submit", async (event) => {
        const form = event.target;
        if (!(form instanceof HTMLFormElement)) {
            return;
        }

        if (!form.closest("#conteudos")) {
            return;
        }

        if (event.defaultPrevented) {
            return;
        }

        const containerAtual = form.closest(".conteudo");
        if (!containerAtual) {
            return;
        }

        const actionAtributo = (form.getAttribute("action") || "").trim();
        let action;

        if (actionAtributo) {
            action = new URL(actionAtributo, window.location.href).toString();
        } else if (form.action && form.action.trim() !== "") {
            action = form.action;
        } else {
            action = window.location.href;
        }

        const method = ((form.method || "GET").toString()).toUpperCase();
        const dadosFormulario = new FormData(form);
        const nomeAbaAtual = obterNomeAbaAtual(containerAtual) || obterNomeAbaAtual(form);

        let urlRequisicao = action;
        let corpoRequisicao = dadosFormulario;

        if (method === "GET" || method === "HEAD") {
            const parametros = new URLSearchParams();
            dadosFormulario.forEach((valor, chave) => {
                if (valor instanceof File) {
                    if (valor.name) {
                        parametros.append(chave, valor.name);
                    }
                } else if (valor !== undefined && valor !== null) {
                    parametros.append(chave, valor.toString());
                }
            });

            const queryString = parametros.toString();
            if (queryString) {
                const separador = urlRequisicao.includes("?") ? "&" : "?";
                urlRequisicao = `${urlRequisicao}${separador}${queryString}`;
            }

            corpoRequisicao = undefined;
        }

        containerAtual.classList.add("loading");
        containerAtual.innerHTML = "<div class=\"loading-state\">Salvando...</div>";

        try {
            const headers = {
                "X-Requested-With": "XMLHttpRequest",
                "Accept": "text/html"
            };

            const antiForgeryToken = obterTokenAntifalsificacao(form);
            if (antiForgeryToken) {
                headers["RequestVerificationToken"] = antiForgeryToken;

                if (!dadosFormulario.has("__RequestVerificationToken") && method !== "GET" && method !== "HEAD") {
                    dadosFormulario.append("__RequestVerificationToken", antiForgeryToken);
                }
            }

            const response = await fetch(urlRequisicao, {
                method,
                body: corpoRequisicao,
                headers,
                credentials: "same-origin"
            });

            if (!response.ok) {
                if (response.status === 401 || response.status === 403) {
                    window.location.href = response.url || window.location.href;
                    return;
                }

                throw new Error("Erro ao enviar formulário");
            }

            const texto = await response.text();
            const mensagensTempData = obterMensagensTempData(response.headers);
            const parser = new DOMParser();
            const doc = parser.parseFromString(texto, "text/html");
            const foiRedirecionado = response.redirected;

            let destino = containerAtual;
            let abaDestino = nomeAbaAtual;
            let urlDestino;

            if (foiRedirecionado) {
                const urlFinal = new URL(response.url, window.location.origin);
                const abaDesejada = form.dataset.successTab || nomeAbaAtual;
                urlDestino = form.dataset.successUrl || (urlFinal.pathname + urlFinal.search);

                if (abaDesejada) {
                    abaDestino = abaDesejada;

                    if (abaDesejada !== nomeAbaAtual) {
                        let conteudoDestino = document.getElementById(`conteudo-${abaDesejada}`);
                        if (!conteudoDestino) {
                            abrirAba(abaDesejada);
                            conteudoDestino = document.getElementById(`conteudo-${abaDesejada}`);
                        }

                        if (conteudoDestino) {
                            destino = conteudoDestino;
                        }
                    }
                }
            } else {
                const urlAtual = new URL(action, window.location.origin);
                urlDestino = urlAtual.pathname + urlAtual.search;
            }

            atualizarConteudoDaAba(destino, doc, mensagensTempData);

            if (destino && urlDestino) {
                destino.dataset.url = urlDestino;
            }

            atualizarTokenAntifalsificacao(doc);

            if (destino !== containerAtual) {
                containerAtual.classList.remove("loading");
            }

            if (foiRedirecionado && abaDestino) {
                ativarConteudo(abaDestino);

                if (form.dataset.closeOnSuccess === "true" && nomeAbaAtual && nomeAbaAtual !== abaDestino) {
                    fecharAba(nomeAbaAtual);
                }
            }

            await agendarAtualizacaoPortal();
        } catch (error) {
            containerAtual.classList.remove("loading");
            containerAtual.innerHTML = "<div class=\"error-state\">Não foi possível enviar o formulário. Tente novamente.</div>";
        }
    });
}

function limparCamposDeFiltro(form) {
    if (!(form instanceof HTMLFormElement)) {
        return;
    }

    const campos = form.querySelectorAll('input, select, textarea');
    campos.forEach((campo) => {
        if (!(campo instanceof HTMLElement)) {
            return;
        }

        if (campo.tagName === 'INPUT') {
            const input = campo;
            const tipo = (input.getAttribute('type') || '').toLowerCase();

            if (tipo === 'hidden') {
                if (input.name === 'Pagina') {
                    input.value = '1';
                }
                return;
            }

            if (tipo === 'checkbox' || tipo === 'radio') {
                input.checked = false;
                return;
            }

            input.value = '';
            return;
        }

        if (campo.tagName === 'SELECT') {
            const select = campo;
            select.selectedIndex = 0;
            return;
        }

        if (campo.tagName === 'TEXTAREA') {
            const textarea = campo;
            textarea.value = '';
        }
    });
}

function configurarResetDeFiltros() {
    document.addEventListener('click', (event) => {
        const alvo = event.target instanceof Element ? event.target.closest('.filters__reset') : null;
        if (!alvo) {
            return;
        }

        event.preventDefault();

        const form = alvo.closest('form');
        if (form) {
            limparCamposDeFiltro(form);
        }

        const acao = (alvo.getAttribute('href') || (form ? form.getAttribute('action') : null) || (form && form.action) || '').trim();
        if (!acao) {
            return;
        }

        const url = new URL(acao, window.location.origin);
        url.search = '';

        const container = alvo.closest('.conteudo');
        const emAbas = container && container.closest('#conteudos');
        if (container && emAbas) {
            const nomeAba = obterNomeAbaAtual(alvo) || obterNomeAbaAtual(container) || 'lista';
            container.classList.add('loading');
            container.innerHTML = `<div class="loading-state">Carregando ${nomeAba}...</div>`;
            carregarConteudoEmAba(container, url.toString(), nomeAba);
            return;
        }

        window.location.href = url.pathname + url.search;
    });
}

const modalOverlay = document.getElementById('app-modal');
const modalContent = modalOverlay ? modalOverlay.querySelector('[data-modal-content]') : null;
let modalSelectionContext = null;

function configurarSelecaoDeFormularios() {
    if (!modalOverlay || !modalContent) {
        return;
    }

    document.addEventListener('click', async (event) => {
        const botaoFormulario = event.target instanceof Element ? event.target.closest('[data-aplicar-formulario]') : null;
        if (botaoFormulario) {
            event.preventDefault();
            await abrirSelecaoFormulario(botaoFormulario);
            return;
        }

        const botaoPaciente = event.target instanceof Element ? event.target.closest('[data-aplicar-formulario-paciente]') : null;
        if (botaoPaciente) {
            event.preventDefault();
            await abrirSelecaoPaciente(botaoPaciente);
            return;
        }

        const fechar = event.target instanceof Element ? event.target.closest('[data-modal-close]') : null;
        if (fechar) {
            event.preventDefault();
            fecharModalSelecao();
            return;
        }

        const item = event.target instanceof Element ? event.target.closest('.selection-modal__item') : null;
        if (item && modalSelectionContext) {
            event.preventDefault();
            tratarSelecaoModal(item);
        }

        if (event.target === modalOverlay) {
            fecharModalSelecao();
        }
    });

    document.addEventListener('keydown', (event) => {
        if (event.key === 'Escape' && !modalOverlay.hidden) {
            fecharModalSelecao();
        }
    });
}

async function abrirSelecaoFormulario(botao) {
    const formularioId = botao.getAttribute('data-formulario-id');
    const selecaoUrl = botao.getAttribute('data-selecao-url');
    const aplicarUrl = botao.getAttribute('data-aplicar-url');

    if (!formularioId || !selecaoUrl || !aplicarUrl) {
        return;
    }

    const abaOrigem = obterNomeAbaAtual(botao) || 'Formulários';
    const container = botao.closest('.conteudo');
    const urlOrigem = container && container.dataset ? container.dataset.url : window.location.pathname + window.location.search;

    const url = new URL(selecaoUrl, window.location.origin);
    url.searchParams.set('formularioId', formularioId);

    await abrirModalSelecao(url.toString(), {
        tipo: 'formulario',
        formularioId,
        aplicarUrl,
        abaOrigem,
        urlOrigem
    });
}

async function abrirSelecaoPaciente(botao) {
    const pacienteId = botao.getAttribute('data-paciente-id');
    const selecaoUrl = botao.getAttribute('data-selecao-url');
    const aplicarUrl = botao.getAttribute('data-aplicar-url');

    if (!pacienteId || !selecaoUrl || !aplicarUrl) {
        return;
    }

    const abaOrigem = obterNomeAbaAtual(botao) || 'Pacientes';
    const container = botao.closest('.conteudo');
    const urlOrigem = container && container.dataset ? container.dataset.url : window.location.pathname + window.location.search;

    const url = new URL(selecaoUrl, window.location.origin);
    url.searchParams.set('pacienteId', pacienteId);

    await abrirModalSelecao(url.toString(), {
        tipo: 'paciente',
        pacienteId,
        aplicarUrl,
        abaOrigem,
        urlOrigem
    });
}

async function abrirModalSelecao(url, contexto) {
    if (!modalOverlay || !modalContent) {
        return;
    }

    modalSelectionContext = contexto;
    modalOverlay.hidden = false;
    modalContent.innerHTML = '<div class="loading-state">Carregando...</div>';

    try {
        const resposta = await fetch(url, {
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        if (!resposta.ok) {
            throw new Error('Erro ao carregar lista');
        }

        const html = await resposta.text();
        modalContent.innerHTML = html;
        configurarPesquisaSelecao(modalContent);
    } catch (error) {
        modalContent.innerHTML = '<div class="error-state">Não foi possível carregar as opções. Tente novamente.</div>';
    }
}

function fecharModalSelecao() {
    if (!modalOverlay || !modalContent) {
        return;
    }

    modalOverlay.hidden = true;
    modalContent.innerHTML = '';
    modalSelectionContext = null;
}

function configurarPesquisaSelecao(container) {
    if (!container) {
        return;
    }

    const campoBusca = container.querySelector('[data-selection-search]');
    const lista = container.querySelector('[data-selection-list]');

    if (!campoBusca || !lista) {
        return;
    }

    campoBusca.addEventListener('input', () => {
        const termo = campoBusca.value.trim().toLowerCase();
        lista.querySelectorAll('.selection-modal__item').forEach((item) => {
            const texto = `${item.dataset.selectionTitle || ''} ${item.dataset.selectionDescription || ''} ${item.dataset.selectionExtra || ''}`.toLowerCase();
            const visivel = termo.length === 0 || texto.includes(termo);
            const li = item.closest('li');
            if (li) {
                li.style.display = visivel ? '' : 'none';
            }
        });
    });

    window.requestAnimationFrame(() => campoBusca.focus());
}

function tratarSelecaoModal(item) {
    if (!modalSelectionContext) {
        return;
    }

    const idSelecionado = item.getAttribute('data-selection-id');
    if (!idSelecionado) {
        return;
    }

    const url = new URL(modalSelectionContext.aplicarUrl, window.location.origin);

    if (modalSelectionContext.tipo === 'formulario') {
        url.searchParams.set('formularioId', modalSelectionContext.formularioId);
        url.searchParams.set('pacienteId', idSelecionado);
    } else if (modalSelectionContext.tipo === 'paciente') {
        url.searchParams.set('formularioId', idSelecionado);
        url.searchParams.set('pacienteId', modalSelectionContext.pacienteId);
    }

    if (modalSelectionContext.abaOrigem) {
        url.searchParams.set('abaOrigem', modalSelectionContext.abaOrigem);
    }

    if (modalSelectionContext.urlOrigem) {
        url.searchParams.set('urlOrigem', modalSelectionContext.urlOrigem);
    }

    fecharModalSelecao();
    abrirAba('Formulário', url.toString());
}
