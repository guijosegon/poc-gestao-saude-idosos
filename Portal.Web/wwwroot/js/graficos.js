(function (window, document) {
    const GOOGLE_CHARTS_URL = 'https://www.gstatic.com/charts/loader.js';
    let chartsLoaderPromise = null;

    function ensureGoogleCharts() {
        if (window.google && window.google.charts && window.google.visualization) {
            return Promise.resolve(window.google);
        }

        if (!chartsLoaderPromise) {
            chartsLoaderPromise = new Promise((resolve, reject) => {
                const existingScript = document.querySelector(`script[src="${GOOGLE_CHARTS_URL}"]`);
                const script = existingScript || document.createElement('script');

                const onLoaded = () => {
                    if (!window.google || !window.google.charts) {
                        reject(new Error('Google Charts não disponível.'));
                        return;
                    }

                    window.google.charts.load('current', { packages: ['corechart'] });
                    window.google.charts.setOnLoadCallback(() => resolve(window.google));
                };

                if (!existingScript) {
                    script.src = GOOGLE_CHARTS_URL;
                    script.async = true;
                    script.onload = onLoaded;
                    script.onerror = () => reject(new Error('Não foi possível carregar o Google Charts.'));
                    document.head.appendChild(script);
                } else if (window.google && window.google.charts && window.google.visualization) {
                    resolve(window.google);
                } else {
                    existingScript.addEventListener('load', onLoaded, { once: true });
                    existingScript.addEventListener('error', () => reject(new Error('Não foi possível carregar o Google Charts.')), { once: true });
                }
            });
        }

        return chartsLoaderPromise;
    }

    function createChart(element, tipo) {
        if (!window.google || !window.google.visualization) {
            return null;
        }

        switch (tipo) {
            case 1:
                return new window.google.visualization.PieChart(element);
            case 2:
                return new window.google.visualization.ColumnChart(element);
            case 3:
                return new window.google.visualization.BarChart(element);
            case 4:
                return new window.google.visualization.LineChart(element);
            case 5:
                return new window.google.visualization.AreaChart(element);
            case 6:
                return new window.google.visualization.SteppedAreaChart(element);
            case 7:
                return new window.google.visualization.ComboChart(element);
            default:
                return new window.google.visualization.ColumnChart(element);
        }
    }

    function drawChart(element, tipo, dataset) {
        ensureGoogleCharts()
            .then(() => {
                if (!dataset || !Array.isArray(dataset.columnLabels)) {
                    element.innerHTML = '<p class="chart-error">Não foi possível carregar os dados.</p>';
                    return;
                }

                const rows = Array.isArray(dataset.rows) ? dataset.rows : [];
                if (rows.length === 0) {
                    element.innerHTML = '<p class="chart-empty">Nenhum dado disponível para este gráfico.</p>';
                    return;
                }

                const data = [dataset.columnLabels, ...rows];
                const dataTable = window.google.visualization.arrayToDataTable(data);
                const chart = createChart(element, tipo);

                if (!chart) {
                    element.innerHTML = '<p class="chart-error">Visualização não suportada neste ambiente.</p>';
                    return;
                }

                element.innerHTML = '';
                chart.draw(dataTable, dataset.options || {});
                element.dataset.graficoRendered = 'true';
            })
            .catch(() => {
                element.innerHTML = '<p class="chart-error">Não foi possível carregar o gráfico.</p>';
            });
    }

    function renderChartElement(element) {
        if (!(element instanceof HTMLElement)) {
            return;
        }

        const url = element.dataset.graficoDadosUrl;
        const tipo = parseInt(element.dataset.graficoTipo || '0', 10);

        if (!url || Number.isNaN(tipo)) {
            element.innerHTML = '<p class="chart-error">Configuração inválida do gráfico.</p>';
            return;
        }

        element.innerHTML = '<p class="chart-placeholder">Atualizando dados...</p>';

        fetch(url, {
            headers: {
                'Accept': 'application/json'
            },
            credentials: 'same-origin'
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Erro ao carregar dados do gráfico.');
                }

                return response.json();
            })
            .then(payload => {
                if (!payload || payload.success !== true) {
                    throw new Error('Retorno inválido para o gráfico.');
                }

                drawChart(element, tipo, payload.dados);
            })
            .catch(() => {
                element.innerHTML = '<p class="chart-error">Não foi possível atualizar este gráfico.</p>';
            });
    }

    function renderChartsIn(container) {
        const scope = container instanceof HTMLElement ? container : document;
        const elements = scope.querySelectorAll('[data-grafico-element]');

        elements.forEach(element => {
            renderChartElement(element);
        });
    }

    function obterTokenAntifalsificacao(form) {
        if (form) {
            const tokenField = form.querySelector('input[name="__RequestVerificationToken"]');
            if (tokenField && tokenField.value) {
                return tokenField.value;
            }
        }

        const meta = document.querySelector('meta[name="request-verification-token"]');
        return meta ? meta.content : null;
    }

    function buildPreviewPayload(form) {
        const origemSelect = form.querySelector('[data-grafico-origem]');
        const tipoSelect = form.querySelector('[data-grafico-tipo]');
        const descricaoInput = form.querySelector('input[name="Descricao"]');
        const legendaCheckbox = form.querySelector('[data-grafico-legenda]');
        const eixoHorizontalInput = form.querySelector('input[name="TituloEixoHorizontal"]');
        const eixoVerticalInput = form.querySelector('input[name="TituloEixoVertical"]');
        const payload = {
            descricao: descricaoInput ? descricaoInput.value : null,
            origem: origemSelect ? parseInt(origemSelect.value || '0', 10) : 0,
            tipo: tipoSelect ? parseInt(tipoSelect.value || '0', 10) : 0,
            mostrarLegenda: legendaCheckbox ? legendaCheckbox.checked : true,
            tituloEixoHorizontal: eixoHorizontalInput ? eixoHorizontalInput.value : null,
            tituloEixoVertical: eixoVerticalInput ? eixoVerticalInput.value : null
        };

        const origemPaciente = payload.origem === 1;
        const origemFormulario = payload.origem === 2;

        if (origemPaciente) {
            const campoPaciente = form.querySelector('[data-grafico-campo-paciente]');
            payload.pacienteCampoCategoria = campoPaciente && campoPaciente.value ? parseInt(campoPaciente.value, 10) : null;
        }

        if (origemFormulario) {
            const formularioSelect = form.querySelector('[data-grafico-formulario]');
            const campoFormularioSelect = form.querySelector('[data-grafico-campo-formulario]');
            payload.formularioId = formularioSelect && formularioSelect.value ? parseInt(formularioSelect.value, 10) : null;
            payload.formularioCampoId = campoFormularioSelect && campoFormularioSelect.value ? parseInt(campoFormularioSelect.value, 10) : null;
        }

        return payload;
    }

    function atualizarSecoesOrigem(form) {
        const origemSelect = form.querySelector('[data-grafico-origem]');
        const valor = origemSelect ? origemSelect.value : '';
        const secoes = form.querySelectorAll('[data-grafico-origem-section]');

        secoes.forEach(secao => {
            const alvo = secao.getAttribute('data-grafico-origem-section');
            if (alvo === valor) {
                secao.removeAttribute('hidden');
            } else {
                secao.setAttribute('hidden', 'hidden');
            }
        });
    }

    function preencherCamposFormulario(select, campos) {
        if (!(select instanceof HTMLSelectElement)) {
            return;
        }

        const valorAtual = select.value;
        select.innerHTML = '';

        const optionDefault = document.createElement('option');
        optionDefault.value = '';
        optionDefault.textContent = 'Selecione o campo';
        select.appendChild(optionDefault);

        campos.forEach(campo => {
            const option = document.createElement('option');
            option.value = campo.value;
            option.textContent = campo.text;
            if (valorAtual && campo.value === valorAtual) {
                option.selected = true;
            }
            select.appendChild(option);
        });
    }

    function fetchCamposFormulario(form, formularioId) {
        const select = form.querySelector('[data-grafico-campo-formulario]');
        const urlBase = form.dataset.camposUrl;

        if (!select || !urlBase) {
            return;
        }

        const container = select.parentElement;
        if (container) {
            const existente = container.querySelector('.field-validation-error');
            if (existente) {
                existente.remove();
            }
        }

        if (!formularioId) {
            preencherCamposFormulario(select, []);
            return;
        }

        const url = `${urlBase}?formularioId=${encodeURIComponent(formularioId)}`;

        fetch(url, {
            headers: {
                'Accept': 'application/json'
            },
            credentials: 'same-origin'
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Erro ao carregar campos do formulário.');
                }

                return response.json();
            })
            .then(payload => {
                if (!payload || payload.success !== true) {
                    throw new Error('Retorno inválido.');
                }

                preencherCamposFormulario(select, Array.isArray(payload.campos) ? payload.campos : []);
            })
            .catch(() => {
                preencherCamposFormulario(select, []);
                if (container) {
                    const erro = document.createElement('span');
                    erro.className = 'field-validation-error';
                    erro.textContent = 'Não foi possível carregar os campos.';
                    container.appendChild(erro);
                }
            });
    }

    function handlePreview(form) {
        const previewButton = form.querySelector('[data-grafico-preview]');
        const previewArea = form.querySelector('[data-grafico-preview-area]');

        if (!previewArea) {
            return;
        }

        const payload = buildPreviewPayload(form);
        const url = form.dataset.previewUrl;

        if (!url) {
            previewArea.innerHTML = '<p class="chart-error">Endpoint de pré-visualização não configurado.</p>';
            return;
        }

        previewArea.innerHTML = '<p class="chart-placeholder">Gerando pré-visualização...</p>';

        if (previewButton) {
            previewButton.disabled = true;
        }

        const token = obterTokenAntifalsificacao(form);

        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                ...(token ? { 'RequestVerificationToken': token } : {})
            },
            body: JSON.stringify(payload),
            credentials: 'same-origin'
        })
            .then(response => {
                if (!response.ok) {
                    return response.json().catch(() => ({ success: false, message: 'Erro ao gerar pré-visualização.' }));
                }

                return response.json();
            })
            .then(data => {
                if (!data || data.success !== true) {
                    const mensagem = data && data.errors && data.errors.length
                        ? data.errors.join(' ')
                        : (data && data.message) || 'Não foi possível gerar a pré-visualização.';
                    previewArea.innerHTML = `<p class="chart-error">${mensagem}</p>`;
                    return;
                }

                drawChart(previewArea, payload.tipo, data.dados);
            })
            .catch(() => {
                previewArea.innerHTML = '<p class="chart-error">Não foi possível gerar a pré-visualização.</p>';
            })
            .finally(() => {
                if (previewButton) {
                    previewButton.disabled = false;
                }
            });
    }

    function initializeForms(container) {
        const scope = container instanceof HTMLElement ? container : document;
        const forms = scope.querySelectorAll('[data-grafico-form]');

        forms.forEach(form => {
            if (!(form instanceof HTMLFormElement) || form.dataset.graficoFormReady === 'true') {
                return;
            }

            const origemSelect = form.querySelector('[data-grafico-origem]');
            const formularioSelect = form.querySelector('[data-grafico-formulario]');
            const previewButton = form.querySelector('[data-grafico-preview]');

            if (origemSelect) {
                origemSelect.addEventListener('change', () => atualizarSecoesOrigem(form));
            }

            if (formularioSelect) {
                formularioSelect.addEventListener('change', () => {
                    const valor = formularioSelect.value ? parseInt(formularioSelect.value, 10) : 0;
                    fetchCamposFormulario(form, valor);
                });
            }

            if (previewButton) {
                previewButton.addEventListener('click', event => {
                    event.preventDefault();
                    handlePreview(form);
                });
            }

            atualizarSecoesOrigem(form);

            if (formularioSelect && formularioSelect.value) {
                fetchCamposFormulario(form, parseInt(formularioSelect.value, 10));
            }

            form.dataset.graficoFormReady = 'true';
        });
    }

    window.Graficos = {
        renderChartsIn,
        initializeForms
    };

    document.addEventListener('DOMContentLoaded', () => {
        renderChartsIn(document);
        initializeForms(document);
    });
})(window, document);
