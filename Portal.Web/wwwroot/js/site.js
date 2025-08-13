function abrirAba(nome) {
    if (nome === "Portal") return;

    if (!document.getElementById("aba-" + nome)) {
        let aba = document.createElement("div");
        aba.className = "aba";
        aba.id = "aba-" + nome;
        aba.innerHTML = nome + ' <span class="close" onclick="fecharAba(\'' + nome + '\')">×</span>';
        aba.onclick = () => ativarConteudo(nome);
        document.getElementById("abas").appendChild(aba);

        let conteudo = document.createElement("div");
        conteudo.className = "conteudo active";
        conteudo.id = "conteudo-" + nome;
        conteudo.innerHTML = "<h2>" + nome + "</h2><p>Conteúdo de " + nome + " em desenvolvimento.</p>";
        document.getElementById("conteudos").appendChild(conteudo);
    }

    ativarConteudo(nome);
}

function ativarConteudo(nome) {
    const conteudo = document.getElementById("conteudo-" + nome);

    if (conteudo) {
        document.querySelectorAll(".conteudo").forEach(div => div.classList.remove("active"));
        conteudo.classList.add("active");
    }
}

function fecharAba(nome) {
    if (nome === "Portal") return; // nunca fecha a aba fixa

    const aba = document.getElementById("aba-" + nome);
    const conteudo = document.getElementById("conteudo-" + nome);

    if (aba) aba.remove();
    if (conteudo) conteudo.remove();

    // buscar as abas restantes
    const abasRestantes = document.querySelectorAll(".aba");
    if (abasRestantes.length > 0) {
        // pega a última aba da lista e ativa
        const ultimaAba = abasRestantes[abasRestantes.length - 1];
        const id = ultimaAba.id.replace("aba-", "");
        ativarConteudo(id);
    } else {
        // fallback: ativa o Portal
        ativarConteudo("Portal");
    }
}

function toggleMenu() {
    const menu = document.getElementById("menuLateral");
    menu.classList.toggle("hidden");
}

// Mensagem de erro dos campos
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

                // cancela timeout anterior, se existir
                if (tooltip.timeoutId) clearTimeout(tooltip.timeoutId);

                // autoesconde após 3s
                tooltip.timeoutId = setTimeout(() => {
                    tooltip.classList.remove("active");
                    tooltip.classList.add("hidden");
                }, 3000);

                // fecha no clique
                tooltip.addEventListener("click", () => {
                    tooltip.classList.remove("active");
                    tooltip.classList.add("hidden");
                });
            }
        };

        // ativa quando sair do campo
        input.addEventListener("blur", () => {
            mostrarTooltip();
        });

        // ⚠️ se a página já veio com erro do servidor, mostra também (mas temporário)
        if (tooltip.classList.contains("field-validation-error") && tooltip.textContent.trim() !== "") {
            mostrarTooltip();
        }
    });
}

document.addEventListener('DOMContentLoaded', ativarTooltips);

$(document).ready(function () {
    $("form").each(function () {
        $(this).removeData("validator"); // remove validação antiga
        $(this).removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse($(this)); // reativa validação
    });

    // Ativa exibição do balão de erro dinâmico
    $("form input").on("input blur", function () {
        $(this).valid();
    });
});