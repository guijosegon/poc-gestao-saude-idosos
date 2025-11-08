# Gestão e Monitoramento da Saúde de Idosos

> Prova de Conceito (PoC) de uma plataforma web para monitorar indicadores físicos e cognitivos de pessoas idosas em instituições de longa permanência, permitindo decisões clínicas mais rápidas e embasadas.

![Tela de Login](https://raw.githubusercontent.com/guijosegon/project-assets/master/GestaoIdosos/login.png)
![Painel Inicial](https://raw.githubusercontent.com/guijosegon/project-assets/master/GestaoIdosos/inicio.png)
![Tela de Login](https://raw.githubusercontent.com/guijosegon/project-assets/master/GestaoIdosos/grafico.png)

## 📚 Sumário
- [Visão Geral](#visão-geral)
- [Objetivos da Plataforma](#objetivos-da-plataforma)
- [Arquitetura da Solução](#arquitetura-da-solução)
  - [Visão em Camadas](#visão-em-camadas)
  - [Módulos do Repositório](#módulos-do-repositório)
  - [Fluxo de Dados](#fluxo-de-dados)
- [Modelagem de Domínio](#modelagem-de-domínio)
- [Recursos Funcionais](#recursos-funcionais)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Preparação do Ambiente](#preparação-do-ambiente)
  - [Requisitos](#requisitos)
  - [Clonagem e restauração de pacotes](#clonagem-e-restauração-de-pacotes)
  - [Configuração de variáveis e secrets](#configuração-de-variáveis-e-secrets)
- [Banco de Dados](#banco-de-dados)
  - [Execução das migrações](#execução-das-migrações)
  - [Reset do banco](#reset-do-banco)
- [Execução das Aplicações](#execução-das-aplicações)
  - [API REST (GestaoSaudeIdosos.API)](#api-rest-gestaosaudeidososapi)
  - [Portal Web (GestaoSaudeIdosos.Web)](#portal-web-gestaosaudeidososweb)
- [Principais Endpoints da API](#principais-endpoints-da-api)
- [Testes Automatizados](#testes-automatizados)
- [Boas Práticas e Segurança](#boas-práticas-e-segurança)
- [Referências e Leituras Complementares](#referências-e-leituras-complementares)
- [Licença](#licença)

---

## Visão Geral
Esta PoC entrega um ecossistema completo para acompanhamento contínuo da saúde de idosos, com foco na prevenção de doenças físicas e neurológicas. A solução integra:

1. **Portal web** para profissionais de saúde cadastrarem pacientes, responderem formulários clínicos, anexarem evidências e visualizarem dashboards.
2. **API RESTful** que disponibiliza os dados para integrações externas, aplicativos móveis ou relatórios corporativos.
3. **Camadas de domínio e infraestrutura** que centralizam regras de negócio, orquestração e persistência em banco PostgreSQL.

---

## Objetivos da Plataforma
- Consolidar informações clínicas, nutricionais e sociais dos residentes em um único local.
- Acompanhar métricas de risco (quedas, mobilidade, cognição) com indicadores visuais.
- Facilitar a comunicação da equipe multiprofissional a partir de históricos e formulários personalizados.
- Permitir que o TCC demonstre uma arquitetura corporativa moderna, escalável e segura com .NET 8.

---

## Arquitetura da Solução

![Tela de Login](https://raw.githubusercontent.com/guijosegon/project-assets/master/GestaoIdosos/solucao.png)
![Tela de Login](https://raw.githubusercontent.com/guijosegon/project-assets/master/GestaoIdosos/banco.png)


### Visão em Camadas
```
Apresentação → GestaoSaudeIdosos.Web (ASP.NET Core MVC) + GestaoSaudeIdosos.API (ASP.NET Core Web API)
Aplicação → Application/ (AppServices, validações de fluxo, políticas de senha)
Domínio → Portal.Domain/ (Entidades, agregados, enums, regras ricas)
Infraestrutura → Portal.Infra/ (EF Core, repositórios, migrations, providers externos)
Testes Automatizados → Portal.Tests/ (xUnit)
```
Cada camada é carregada via injeção de dependências (`AddInfraServices`, `AddApplicationServices`, `AddDomainServices`) nos projetos de apresentação, garantindo baixo acoplamento e alta testabilidade.

### Módulos do Repositório
| Diretório | Descrição |
|-----------|-----------|
| `Portal.API/` (projeto `GestaoSaudeIdosos.API`) | API RESTful com autenticação JWT para integrações e aplicativos terceiros. Exposição de CRUDs de usuários, pacientes e formulários. |
| `Portal.Web/` (projeto `GestaoSaudeIdosos.Web`) | Portal web MVC com autenticação cookie, filtros antifalsificação e páginas para operação diária. |
| `Application/` | Serviços de aplicação, contratos (`Interfaces/`) e políticas de segurança (Argon2 para hashing de senha). |
| `Portal.Domain/` | Entidades, enums e validações do negócio. Representa o modelo rico de pacientes, formulários e gráficos. |
| `Portal.Infra/` | Contexto do Entity Framework Core, migrations e implementações de repositórios. |
| `Portal.Tests/` | Testes de unidade (xUnit) focados em regras críticas, como políticas de senha. |
| `GestaoSaudeIdosos.sln` | Solution file para abrir no Visual Studio / Rider e orquestrar os projetos. |

### Fluxo de Dados
1. O usuário autentica-se no portal web (`GestaoSaudeIdosos.Web`); cookies seguros protegem a sessão.
2. As ações do portal acionam `AppServices` que orquestram regras e validam dados antes da persistência.
3. O `AppContext` do EF Core grava e consulta dados no PostgreSQL, mantendo relacionamentos entre pacientes, formulários, usuários e gráficos.
4. Integrações externas obtêm dados pela API (`GestaoSaudeIdosos.API`) autenticando-se via JWT emitido pelo `AuthController`.

---

## Modelagem de Domínio
| Entidade | Responsabilidade | Destaques |
|----------|------------------|-----------|
| `Usuario` | Representa profissionais e administradores do abrigo. | Guarda perfil (Administrador/Profissional), status ativo, login seguro com Argon2 e controle de criação/edição. |
| `Paciente` | Registra dados cadastrais e clínicos do idoso. | Controla chave pública, idade, risco de queda, mobilidade, vínculo com responsável e histórico de formulários respondidos. |
| `Formulario` / `FormularioCampo` | Modelam instrumentos de avaliação, perguntas e possíveis respostas. | Permitem configurar questionários reutilizáveis para diferentes áreas (nutrição, cognitivo, funcional). |
| `FormularioResultado` | Persistem respostas de avaliações periódicas. | Conectam paciente, formulário e profissional responsável. |
| `Grafico` | Configura dashboards (tipo, origem, exibição no portal). | Integra com Google Charts / Chart.js para monitoramento visual. |

---

## Recursos Funcionais
- **Gestão de Usuários (GestaoSaudeIdosos.API):** cadastro, edição, desativação e atribuição de perfis (Administrador ou Profissional).
- **Gestão de Pacientes (GestaoSaudeIdosos.API):** CRUD completo, associação de responsável e registro de indicadores clínicos.
- **Formulários Clínicos:** construção de formulários dinâmicos e registro de resultados periódicos para análises longitudinais.
- **Dashboards Dinâmicos:** configuração de gráficos customizados a partir dos dados coletados, exibidos no portal web.
- **Autenticação Segura:** JWT na API (`GestaoSaudeIdosos.API`) e cookies com expiração deslizante no portal (`GestaoSaudeIdosos.Web`), além de hashing Argon2 para senhas fortes.
- **Alertas e UX:** filtros de TempData e validação antifalsificação protegem formulários e exibem mensagens amigáveis para o usuário final.

---

## Tecnologias Utilizadas
- **Back-end:** .NET 8, ASP.NET Core MVC/Web API, Entity Framework Core.
- **Front-end:** Razor Views, Bootstrap, jQuery, Chart.js e Google Charts para visualizações.
- **Banco de Dados:** PostgreSQL 14+ (modo relacional, suporte a migrações EF Core).
- **Autenticação/Autorização:** Cookies (`GestaoSaudeIdosos.Web`), JWT Bearer (`GestaoSaudeIdosos.API`), políticas de autorização por perfil.
- **Segurança:** Hash de senha Argon2, antifalsificação (AntiForgery), validação de entrada pelo ModelState.
- **Testes:** xUnit com cenários cobrindo regras críticas de domínio e segurança (ex.: política de senha).
- **Ferramentas de apoio:** DBeaver ou psql para inspeção do banco, Visual Studio Code/Visual Studio/Rider para desenvolvimento.

---

## Preparação do Ambiente

### Requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL instalado (local ou serviço gerenciado)
- Ferramenta de gerenciamento de banco (psql, DBeaver, TablePlus, etc.)
- Git instalado para clonar o repositório

### Clonagem e restauração de pacotes
```bash
# Clonar o projeto
git clone https://github.com/SEU_USUARIO/poc-gestao-saude-idosos.git
cd poc-gestao-saude-idosos

# Restaurar dependências
dotnet restore GestaoSaudeIdosos.sln
```

### Configuração de variáveis e secrets
1. Atualize a `ConnectionStrings:DefaultConnection` nos arquivos `appsettings.json` da API e do portal web:
   ```jsonc
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=portalidosos;Username=usuario;Password=SUA_SENHA"
   }
   ```
2. Defina os parâmetros JWT compartilhados entre API e portal:
   ```bash
   # PowerShell
   $Env:Jwt__Key="sua-chave-com-32-caracteres" \
   $Env:Jwt__Issuer="https://localhost:5001" \
   $Env:Jwt__Audience="https://localhost:5001" \
   $Env:Jwt__ExpireMinutes="60"

   # Bash
   export Jwt__Key="sua-chave-com-32-caracteres"
   export Jwt__Issuer="https://localhost:5001"
   export Jwt__Audience="https://localhost:5001"
   export Jwt__ExpireMinutes="60"
   ```
3. Opcional: utilize o [dotnet user-secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) durante o desenvolvimento para manter credenciais fora do controle de versão no projeto `GestaoSaudeIdosos.Web`:
   ```bash
   cd Portal.Web
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=..."
   dotnet user-secrets set "Jwt:Key" "sua-chave"
   ```

---

## Banco de Dados

### Execução das migrações
```bash
cd Portal.Infra
# aplica migrations no banco configurado
dotnet ef database update --project GestaoSaudeIdosos.Infra.csproj --startup-project ../Portal.Web/GestaoSaudeIdosos.Web.csproj
```
> É possível utilizar o projeto `GestaoSaudeIdosos.API` (diretório `Portal.API/`) como inicialização caso o foco seja apenas a API.

### Reset do banco
Para recriar o esquema a partir do zero:
```bash
cd Portal.Infra
dotnet ef database drop --force --project GestaoSaudeIdosos.Infra.csproj --startup-project ../Portal.Web/GestaoSaudeIdosos.Web.csproj
dotnet ef database update --project GestaoSaudeIdosos.Infra.csproj --startup-project ../Portal.Web/GestaoSaudeIdosos.Web.csproj
```

---

## Execução das Aplicações

### API REST (GestaoSaudeIdosos.API)
```bash
cd Portal.API
dotnet run
```
- Swagger disponível em `https://localhost:5001/swagger` (ambiente Development).【F:Portal.API/Program.cs†L33-L44】
- Autenticação via `POST /api/autenticar` retornando o token JWT.

### Portal Web (GestaoSaudeIdosos.Web)
```bash
cd Portal.Web
dotnet run
```
- Aplicação disponível em `https://localhost:5002` (porta configurada pelo ASP.NET).
- A rota padrão direciona para o painel após login (`/Home/Index`).
- Para primeiro acesso, crie um usuário administrador via API (endpoint `/api/usuarios`) ou migração/semente dedicada.

---

## Principais Endpoints da API
| Verbo & Rota | Descrição | Autorização |
|--------------|-----------|-------------|
| `POST /api/autenticar` | Gera token JWT a partir de credenciais válidas. | Público (retorna 401 para credenciais inválidas). |
| `GET /api/usuarios` | Lista usuários cadastrados. | Requer token JWT válido. |
| `POST /api/usuarios` | Cria um novo usuário. | Perfil `Administrador`. |
| `PUT /api/usuarios/{id}` | Atualiza usuário existente. | Perfil `Administrador`. |
| `DELETE /api/usuarios/{id}` | Remove usuário. | Perfil `Administrador`. |
| `GET /api/pacientes` | Lista pacientes com seus responsáveis. | Qualquer usuário autenticado. |
| `POST /api/pacientes` | Cadastra paciente. | Qualquer usuário autenticado. |
| `PUT /api/pacientes/{id}` | Edita paciente existente. | Qualquer usuário autenticado. |
| `DELETE /api/pacientes/{id}` | Remove paciente. | Apenas `Administrador`. |

> Consulte o Swagger da aplicação para detalhes de payloads e códigos de resposta.

---

## Testes Automatizados
```bash
cd Portal.Tests
dotnet test
```
- Os testes atuais validam, por exemplo, a política de senhas fortes implementada com Argon2, garantindo critérios de complexidade para novos cadastros.

---

## Boas Práticas e Segurança
- **HTTPS obrigatório**: tanto `GestaoSaudeIdosos.Web` quanto `GestaoSaudeIdosos.API` aplicam redirecionamento HTTPS por padrão.
- **Proteção antifalsificação**: todas as ações `POST` do portal web (`GestaoSaudeIdosos.Web`) exigem tokens antiforgery automaticamente adicionados aos formulários.
- **Sessões seguras**: cookies marcados como `HttpOnly` e com expiração deslizante reduzem o risco de sequestro de sessão no portal (`GestaoSaudeIdosos.Web`).
- **Validação de entrada**: controllers conferem `ModelState` antes de processar dados, retornando erros claros para o front-end.
- **Regras de autorização**: endpoints utilizam `[Authorize]` e restrições de perfil para evitar acessos indevidos.

---

## Referências e Leituras Complementares
- [Documentação oficial do ASP.NET Core](https://learn.microsoft.com/aspnet/core/)
- [Entity Framework Core Migrations](https://learn.microsoft.com/ef/core/managing-schemas/migrations/)
- [OWASP Cheat Sheet Series](https://cheatsheetseries.owasp.org/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)

---

## Licença
Distribuído sob a licença MIT. Consulte o arquivo [LICENSE.txt](LICENSE.txt) para mais detalhes.
