# DematecStock API

API RESTful para gerenciamento de estoque de armazém, desenvolvida com **ASP.NET Core 10** seguindo os princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**.

---

## 📋 Sumário

- [Sobre o Projeto](#sobre-o-projeto)
- [Arquitetura](#arquitetura)
- [Tecnologias](#tecnologias)
- [Pré-requisitos](#pré-requisitos)
- [Configuração](#configuração)
- [Executando o Projeto](#executando-o-projeto)
- [Endpoints da API](#endpoints-da-api)
- [Testes](#testes)
- [Estrutura de Pastas](#estrutura-de-pastas)

---

## Sobre o Projeto

O **DematecStock** é uma API de controle de estoque que permite gerenciar:

- **Localizações de armazém** — cadastro e manutenção de endereços físicos (rua, prédio, nível, apto)
- **Estocagem de produtos** — associação de produtos a localizações com controle de saldo
- **Movimentações de estoque** — registro de entradas e saídas
- **Consulta de produtos** — busca paginada e rastreamento de endereços por produto ou localização
- **Autenticação** — acesso seguro via JWT Bearer

---

## Arquitetura

O projeto segue **Clean Architecture** com separação por camadas:

```
┌──────────────────────────────────────┐
│           DematecStock.Api           │  ← Controllers, Filters, Program.cs
├──────────────────────────────────────┤
│       DematecStock.Application       │  ← Use Cases, AutoMapper
├──────────────────────────────────────┤
│       DematecStock.Communication     │  ← Requests / Responses (DTOs)
├──────────────────────────────────────┤
│         DematecStock.Domain          │  ← Entidades, Interfaces, DTOs internos
├──────────────────────────────────────┤
│      DematecStock.Infrastructure     │  ← EF Core, Repositórios, JWT, BCrypt
├──────────────────────────────────────┤
│        DematecStock.Exception        │  ← Exceções de domínio
├──────────────────────────────────────┤
│         DematecStock.Tests           │  ← Testes unitários e de integração
└──────────────────────────────────────┘
```

---

## Tecnologias

| Tecnologia | Versão |
|---|---|
| .NET / ASP.NET Core | 10 |
| Entity Framework Core (SQL Server) | 10.0.5 |
| AutoMapper | 16.1.1 |
| JWT Bearer Authentication | 10.0.5 |
| BCrypt.Net-Next | 4.1.0 |
| Scalar (OpenAPI UI) | 2.13.8 |
| xUnit | 2.9.3 |
| Moq | 4.20.72 |
| FluentAssertions | 8.8.0 |

---

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server (local ou remoto)
- Visual Studio 2022+ ou VS Code

---

## Configuração

1. Clone o repositório:

```bash
git clone https://github.com/dematecsistemas/api.dotnetcore.apiestoque.git
cd api.dotnetcore.apiestoque
```

2. Configure o arquivo `src/DematecStock.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Connection": "Server=SEU_SERVIDOR;Database=SEU_BANCO;User Id=SEU_USUARIO;Password=SUA_SENHA;TrustServerCertificate=True;"
  },
  "Settings": {
    "Jwt": {
      "SigningKey": "SUA_CHAVE_SECRETA_MINIMO_32_CARACTERES",
      "ExpiresMinutes": 1000
    }
  }
}
```

> ⚠️ **Nunca versione credenciais reais.** Utilize `appsettings.Development.json` (já ignorado no `.gitignore`) ou variáveis de ambiente em produção.

3. O banco de dados deve conter a view `vw_LocationProducts` (schema `dbo`) utilizada para consulta de produtos por localização.

---

## Executando o Projeto

```bash
cd src/DematecStock.Api
dotnet run
```

A documentação interativa estará disponível em:

```
https://localhost:{porta}/scalar/v1
```

---

## Endpoints da API

### 🔐 Autenticação

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/v1/login` | Realiza login e retorna o token JWT |

**Body:**
```json
{
  "email": "usuario@email.com",
  "password": "senha"
}
```

---

### 🏭 Warehouse Locations — Localizações de Armazém

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/warehouselocations` | Lista todas as localizações com produtos |
| `POST` | `/api/warehouselocations` | Cria uma nova localização |
| `PATCH` | `/api/warehouselocations/{id}` | Atualiza parcialmente uma localização |

---

### 📦 Inventory Location — Estocagem de Produtos

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/inventorylocation` | Associa um produto a uma localização |
| `DELETE` | `/api/inventorylocation/{idLocation}/{idProduct}` | Remove a associação produto-localização |
| `PATCH` | `/api/inventorylocation/{idLocation}/{idProduct}/saldo` | Atualiza o saldo em estoque |

---

### 🔄 Inventory Movements — Movimentações de Estoque

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/inventorymovements` | Registra uma movimentação de estoque |

---

### 🔍 Product Search — Pesquisa de Produtos

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/productsearch` | Busca paginada de produtos |

---

### 📍 Products Locations — Endereços de Produtos

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/productslocations` | Lista localizações por produto ou produto por localização |

---

## Testes

O projeto utiliza **xUnit**, **Moq** e **FluentAssertions** para testes unitários e de integração.

Execute os testes com:

```bash
dotnet test
```

Coberturas implementadas:

- Use Cases: `Login`, `WarehouseLocations`, `InventoryLocation`, `InventoryMovement`, `ProductSearch`, `ProductsAddress`
- Controllers: `InventoryMovements`, `ProductSearch`, `ProductsLocations`

---

## Estrutura de Pastas

```
DematecStock/
├── src/
│   ├── DematecStock.Api/               # Camada de apresentação (Web API)
│   │   ├── Controllers/
│   │   └── Filters/
│   ├── DematecStock.Application/       # Casos de uso e AutoMapper
│   │   └── UseCases/
│   ├── DematecStock.Communication/     # DTOs de Request e Response
│   │   ├── Requests/
│   │   └── Responses/
│   ├── DematecStock.Domain/            # Entidades, repositórios (interfaces) e DTOs
│   │   ├── Entities/
│   │   ├── Repositories/
│   │   └── Security/
│   ├── DematecStock.Exception/         # Exceções customizadas
│   └── DematecStock.Infrastructure/    # Implementações: EF Core, JWT, BCrypt
│       ├── DataAccess/
│       ├── Repositories/
│       └── Security/
└── tests/
    └── DematecStock.Tests/             # Testes unitários e de integração
```

---

## Licença

Este projeto é de propriedade da **DEMATEC Sistemas**. Todos os direitos reservados.
