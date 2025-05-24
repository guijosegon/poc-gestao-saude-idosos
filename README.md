# Gestão e Monitoramento da Saúde de Idosos (Em desenvolvimento)

Prova de Conceito de um **sistema web** para gestão e monitoramento da saúde mental e física de idosos em asilos ou abrigos, com foco na antecipação e prevenção de doenças neurológicas e físicas.  

Esta solução adota .NET 8 com Razor Pages para a interface web, uma API RESTful em ASP.NET Core e padrões de arquitetura em camadas (Presentation → Application → Domain → Infrastructure), suportando SQL Server e PostgreSQL.

---

## 📁 Estrutura do Repositório

```
gestao-saude-idosos/            ← raiz do mono-repo
├── 0-Presentation
│   ├── GestaoSaudeIdosos.API   ← API RESTful (ASP.NET Core)
│   └── GestaoSaudeIdosos.Web   ← Front-end Razor Pages
│
├── 1-Application
│   ├── GestaoSaudeIdosos.Application
│   │   ├── AppServices         ← orquestração de casos de uso
│   │   └── Interfaces          ← contratos de aplicação
│
├── 2-Domain
│   ├── GestaoSaudeIdosos.Domain
│   │   ├── Entities            ← entidades de domínio
│   │   ├── Interfaces          ← repositórios e serviços de domínio
│   │   └── Validations         ← regras e DTOs
│
├── 3-Infra
│   ├── GestaoSaudeIdosos.Infra
│   │   ├── Contexts            ← DbContext EF Core
│   │   ├── Migrations          ← migrações de banco de dados
│   │   └── Repositories        ← implementação de repositórios
│
└── 4-Tests
    └── GestaoSaudeIdosos.Tests ← testes unitários e de integração
```

---

## 🚀 Tecnologias

- **Runtime & Framework**: .NET 8 (ASP.NET Core)  
- **Front-end**: Razor Pages (Views + Rótulos)  
- **API**: ASP.NET Core Web API  
- **ORM**: Entity Framework Core  
- **Bancos de dados**: SQL Server e PostgreSQL (Render)  
- **Autenticação**: Cookie Auth (web) e JWT Bearer (API)  
- **Visualização de dados**: Chart.js (Canvas) & Google Charts

---

## 🛠️ Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)  
- SQL Server local (ou acesso a PostgreSQL no Render)  
- [DBeaver] ou similar para inspecionar o banco PostgreSQL  

---

## ⚙️ Configuração da base

1. **Cadastrar as _ConnectionStrings_** em cada `appsettings.json`:

   ```jsonc
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=PortalIdososDb;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

   Para PostgreSQL (Render):

   ```jsonc
   "ConnectionStrings": {
     "DefaultConnection": "Host=dpg-...;Port=5432;Database=portalidosos;Username=portalidosos_user;Password=SUA_SENHA"
   }
   ```

2. **(Opcional)** Definir variáveis de ambiente para a seção _Jwt_:

   ```bash
   # Windows (PowerShell)
   $Env:Jwt__Key      = "<sua-chave-secreta-256b>"
   $Env:Jwt__Issuer   = "https://localhost:5001"
   $Env:Jwt__Audience = "https://localhost:5001"
   $Env:Jwt__ExpireMinutes = "60"
   ```

---

## ▶️ Migrar e Popular o Banco

Abra um terminal na pasta `GestaoSaudeIdosos.Infra` e rode:

```bash
cd 3-Infra/GestaoSaudeIdosos.Infra
dotnet ef database update
```

---

## ▶️ Executar localmente

### 1) API

```bash
cd 0-Presentation/GestaoSaudeIdosos.API
dotnet run
```

### 2) Web (Razor)

```bash
cd 0-Presentation/GestaoSaudeIdosos.Web
dotnet run
```

---

## 🔐 Autenticação

- **Web**: Cookie Authentication  
- **API**: JWT Bearer  
  - POST `/api/auth/token` com `{ "email": "...", "senha": "..." }`  
  - Recebe `{ "token": "..." }`  
  - Use `Authorization: Bearer <token>`

---

## 📊 Funcionalidades

- **Usuários**: CRUD, papéis (Admin/Usuário)  
- **Pacientes**: CRUD, históricos médicos  
- **Relatórios**: gráficos dinâmicos com Chart.js & Google Charts
- **Prevenção**: sugestões baseadas em métricas  

---

## 🧪 Testes

```bash
cd 4-Tests/GestaoSaudeIdosos.Tests
dotnet test
```

---

## 📚 Referências

- [ASP.NET Core Docs](https://docs.microsoft.com/aspnet/core)  
- [EF Core Migrations](https://docs.microsoft.com/ef/core/managing-schemas/migrations/)  
- [Render PostgreSQL](https://render.com/docs/postgres)  

---

### 🚩 Licença

MIT  
