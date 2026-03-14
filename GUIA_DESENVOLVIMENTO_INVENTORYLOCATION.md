# Guia de Desenvolvimento — Ordem de Criação de Arquivos (DematecStock)

> Fluxo principal da solução: **API → UseCase/Service → Repository → DbContext → Banco**

Este guia foi feito para servir como mapa de consulta durante desenvolvimento solo, com foco especial em `InventoryLocation` (`POST`, `DELETE`, `PATCH`) e validações de erro.

---

## 1) Visão geral da arquitetura da solução

A solução está organizada em camadas:

- `DematecStock.Api`: recebe requisições HTTP e retorna respostas.
- `DematecStock.Application`: regras de negócio (Use Cases).
- `DematecStock.Domain`: entidades e contratos (interfaces) de repositórios.
- `DematecStock.Infrastructure`: implementação de acesso a dados com EF Core.
- `DematecStock.Communication`: DTOs de entrada/saída (Requests/Responses).
- `DematecStock.Exception`: exceções de negócio padronizadas.

**Ordem recomendada de criação (sem pular etapas):**
1. Exception
2. Domain
3. Communication
4. Application
5. Infrastructure
6. Api

---

## 2) Passo a passo completo (com os 8 itens obrigatórios)

## PASSO 1 — Exceção base do projeto

1. **Nome do arquivo**: `DematecStockException.cs`  
2. **Pasta**: `src/DematecStock.Exception/ExceptionsBase/`  
3. **Responsabilidade**: Classe abstrata base para exceções da aplicação com `StatusCode` e lista de erros.  
4. **Implementar primeiro**: assinatura abstrata de `StatusCode` e `GetErrors()`.  
5. **Relação com EF Core**: nenhuma direta.  
6. **Dependências**: nenhuma.  
7. **Erros comuns**:
   - esquecer `abstract`;
   - não padronizar retorno de erros.  
8. **O que testar antes de seguir**: build da camada `Exception`.

---

## PASSO 2 — Exceção de validação (`400`)

1. **Nome do arquivo**: `ErrorOnValidationException.cs`  
2. **Pasta**: `src/DematecStock.Exception/ExceptionsBase/`  
3. **Responsabilidade**: representar erro de regra de negócio (HTTP 400).  
4. **Implementar primeiro**: construtor com lista de erros e com erro único + `StatusCode = 400`.  
5. **Relação com EF Core**: indireta (evita persistir dados inválidos).  
6. **Dependências**: `DematecStockException`.  
7. **Erros comuns**:
   - não inicializar lista de erros no construtor de string;
   - usar mensagens genéricas demais.  
8. **O que testar**: instanciar exceção e validar `GetErrors()`.

---

## PASSO 3 — Exceção de não encontrado (`404`)

1. **Nome do arquivo**: `NotFoundException.cs`  
2. **Pasta**: `src/DematecStock.Exception/ExceptionsBase/`  
3. **Responsabilidade**: retornar `404` quando entidade não existe.  
4. **Implementar primeiro**: construtor com mensagem + `StatusCode = 404`.  
5. **Relação com EF Core**: ocorre após busca no banco retornar `null`.  
6. **Dependências**: `DematecStockException`.  
7. **Erros comuns**:
   - lançar `404` no repository (o ideal é usar no UseCase);
   - mensagem de erro sem contexto.  
8. **O que testar**: `GetErrors()` retorna lista com 1 item.

---

## PASSO 4 — Entidade de domínio de estoque por localização

1. **Nome do arquivo**: `InventoryLocation.cs`  
2. **Pasta**: `src/DematecStock.Domain/Entities/`  
3. **Responsabilidade**: mapear tabela `ProdEstocagem` e campos (`CodEst`, `CodProduto`, etc.).  
4. **Implementar primeiro**: propriedades-chave (`IdLocation`, `IdProduct`) e método de domínio `UpdateOnHandQuantity`.  
5. **Relação com EF Core**:
   - `[Table("ProdEstocagem")]`;
   - `[Column(...)]`;
   - chave composta será configurada no `DbContext`.  
6. **Dependências**: `System.ComponentModel.DataAnnotations.Schema`.  
7. **Erros comuns**:
   - não respeitar nomes de colunas reais;
   - deixar `OnHandQuantity` sem proteção de domínio.  
8. **O que testar**: compilar projeto `Domain`.

---

## PASSO 5 — Contrato de transação

1. **Nome do arquivo**: `IUnitOfWork.cs`  
2. **Pasta**: `src/DematecStock.Domain/Repositories/`  
3. **Responsabilidade**: padronizar `Commit()` para persistência final.  
4. **Implementar primeiro**: método `Task Commit()`.  
5. **Relação com EF Core**: será implementado com `SaveChangesAsync()`.  
6. **Dependências**: nenhuma.  
7. **Erros comuns**:
   - salvar no repository em vez de salvar no UnitOfWork;
   - esquecer `await` no commit.  
8. **O que testar**: compilação.

---

## PASSO 6 — Contrato de escrita (`Add`/`Delete`)

1. **Nome do arquivo**: `IInventoryLocationWriteOnlyRepository.cs`  
2. **Pasta**: `src/DematecStock.Domain/Repositories/InventoryLocation/`  
3. **Responsabilidade**: operações que alteram dados (`Add`, `Delete`).  
4. **Implementar primeiro**: assinatura de `Add` e `Delete`.  
5. **Relação com EF Core**: implementação usará `AddAsync` e `Remove`.  
6. **Dependências**: entidade `InventoryLocation`.  
7. **Erros comuns**:
   - acoplar regra de negócio no repository;
   - chamar commit dentro do repository.  
8. **O que testar**: compilação.

---

## PASSO 7 — Contrato de busca/atualização

1. **Nome do arquivo**: `IInventoryLocationUpdateOnlyRepository.cs`  
2. **Pasta**: `src/DematecStock.Domain/Repositories/InventoryLocation/`  
3. **Responsabilidade**: buscar por chave composta e atualizar entidade.  
4. **Implementar primeiro**: `GetByKey` e `Update`.  
5. **Relação com EF Core**: `FirstOrDefaultAsync` + `Update`.  
6. **Dependências**: entidade `InventoryLocation`.  
7. **Erros comuns**:
   - esquecer chave composta na busca;
   - tornar `Update` assíncrono sem necessidade.  
8. **O que testar**: compilação.

---

## PASSO 8 — DTO de entrada do POST

1. **Nome do arquivo**: `RequestAddInventoryLocationJson.cs`  
2. **Pasta**: `src/DematecStock.Communication/Requests/InventoryLocation/`  
3. **Responsabilidade**: payload de criação na API.  
4. **Implementar primeiro**: campos obrigatórios (`IdLocation`, `IdProduct`, `OnHandQuantity`, etc.).  
5. **Relação com EF Core**: nenhuma direta (será mapeado para entidade).  
6. **Dependências**: nenhuma.  
7. **Erros comuns**:
   - usar entidade de domínio direto no controller;
   - não tratar valores padrão.  
8. **O que testar**: serialização JSON e binding no Swagger.

---

## PASSO 9 — DTO de entrada do PATCH

1. **Nome do arquivo**: `RequestUpdateOnHandQuantityJson.cs`  
2. **Pasta**: `src/DematecStock.Communication/Requests/InventoryLocation/`  
3. **Responsabilidade**: payload mínimo para atualizar saldo.  
4. **Implementar primeiro**: `OnHandQuantity`.  
5. **Relação com EF Core**: nenhuma direta.  
6. **Dependências**: nenhuma.  
7. **Erros comuns**:
   - usar DTO grande para atualização parcial;
   - confundir PATCH com PUT.  
8. **O que testar**: binding em endpoint PATCH.

---

## PASSO 10 — DTO de resposta de erro

1. **Nome do arquivo**: `ResponseErrorJson.cs`  
2. **Pasta**: `src/DematecStock.Communication/Responses/`  
3. **Responsabilidade**: padronizar retorno de erro da API.  
4. **Implementar primeiro**: propriedade `Errors` e construtores.  
5. **Relação com EF Core**: nenhuma.  
6. **Dependências**: usado pelo filtro de exceção.  
7. **Erros comuns**:
   - retornar texto solto em vez de objeto padrão.  
8. **O que testar**: resposta JSON de erro em endpoint com validação.

---

## PASSO 11 — Mapeamento AutoMapper

1. **Nome do arquivo**: `AutoMapping.cs`  
2. **Pasta**: `src/DematecStock.Application/AutoMapper/`  
3. **Responsabilidade**: mapear DTOs para entidades e vice-versa.  
4. **Implementar primeiro**: `CreateMap<RequestAddInventoryLocationJson, InventoryLocation>()`.  
5. **Relação com EF Core**: indireta (monta entidade que será persistida).  
6. **Dependências**: DTOs + entidades.  
7. **Erros comuns**:
   - esquecer map e quebrar em runtime;
   - mapear tipo invertido por engano.  
8. **O que testar**: fluxo POST no Swagger.

---

## PASSO 12 — UseCase de criação (POST)

1. **Nome dos arquivos**:
   - `IAddInventoryLocationUseCase.cs`
   - `AddInventoryLocationUseCase.cs`  
2. **Pasta**: `src/DematecStock.Application/UseCases/InventoryLocation/AddInventoryLocation/`  
3. **Responsabilidade**: validar e criar registro de localização de estoque.  
4. **Implementar primeiro**:
   - validação `OnHandQuantity < 0`;
   - map DTO → entidade;
   - repository `Add`;
   - `Commit`.  
5. **Relação com EF Core**: indireta via repository e unit of work.  
6. **Dependências**:
   - `IInventoryLocationWriteOnlyRepository`
   - `IUnitOfWork`
   - `IMapper`
   - `ErrorOnValidationException`.  
7. **Erros comuns**:
   - validar depois de acessar banco;
   - esquecer commit;
   - não usar `await`.  
8. **O que testar**:
   - POST com saldo negativo → `400`;
   - POST válido → `201`.

---

## PASSO 13 — UseCase de remoção (DELETE)

1. **Nome dos arquivos**:
   - `IDeleteInventoryLocationUseCase.cs`
   - `DeleteInventoryLocationUseCase.cs`  
2. **Pasta**: `src/DematecStock.Application/UseCases/InventoryLocation/DeleteInventoryLocation/`  
3. **Responsabilidade**: remover registro apenas se existir e saldo for zero.  
4. **Implementar primeiro**:
   - buscar por chave;
   - `NotFoundException` quando `null`;
   - bloquear exclusão se saldo diferente de zero;
   - delete + commit.  
5. **Relação com EF Core**: indireta via busca e remoção no repository.  
6. **Dependências**:
   - `IInventoryLocationWriteOnlyRepository`
   - `IInventoryLocationUpdateOnlyRepository`
   - `IUnitOfWork`
   - `NotFoundException`, `ErrorOnValidationException`.  
7. **Erros comuns**:
   - remover sem verificar saldo;
   - retornar sucesso para item inexistente sem decisão explícita.  
8. **O que testar**:
   - DELETE inexistente → `404`;
   - DELETE com saldo ≠ 0 → `400`;
   - DELETE com saldo 0 → `204`.

---

## PASSO 14 — UseCase de atualização de saldo (PATCH)

1. **Nome dos arquivos**:
   - `IUpdateOnHandQuantityUseCase.cs`
   - `UpdateOnHandQuantityUseCase.cs`  
2. **Pasta**: `src/DematecStock.Application/UseCases/InventoryLocation/UpdateOnHandQuantity/`  
3. **Responsabilidade**: atualizar somente o saldo (`OnHandQuantity`).  
4. **Implementar primeiro**:
   - validar saldo não-negativo;
   - buscar por chave;
   - lançar `404` se não existir;
   - chamar `UpdateOnHandQuantity()` na entidade;
   - `Update` no repository + `Commit`.  
5. **Relação com EF Core**: indireta via tracking e `SaveChangesAsync`.  
6. **Dependências**:
   - `IInventoryLocationUpdateOnlyRepository`
   - `IUnitOfWork`
   - DTO do PATCH
   - exceções de validação e not found.  
7. **Erros comuns**:
   - tentar setar propriedade protegida direto;
   - esquecer `Commit` após `Update`.  
8. **O que testar**:
   - PATCH saldo negativo → `400`;
   - PATCH item inexistente → `404`;
   - PATCH válido → `204` e valor alterado no banco.

---

## PASSO 15 — Registro de UseCases no DI (Application)

1. **Nome do arquivo**: `DependencyInjectionExtension.cs`  
2. **Pasta**: `src/DematecStock.Application/`  
3. **Responsabilidade**: registrar AutoMapper e UseCases no container.  
4. **Implementar primeiro**: `AddApplication()` com `AddScoped<Interface, Classe>()`.  
5. **Relação com EF Core**: nenhuma direta.  
6. **Dependências**: todos os UseCases.  
7. **Erros comuns**:
   - esquecer registro de um UseCase;
   - lifecycle incorreto.  
8. **O que testar**: inicialização da API sem erro de DI.

---

## PASSO 16 — DbContext (EF Core)

1. **Nome do arquivo**: `DematecStockDbContext.cs`  
2. **Pasta**: `src/DematecStock.Infrastructure/DataAccess/`  
3. **Responsabilidade**: centralizar mapeamentos e acesso EF Core.  
4. **Implementar primeiro**:
   - `DbSet<InventoryLocation>`;
   - `HasKey(e => new { e.IdLocation, e.IdProduct })`;
   - `HasPrecision(18,4)` para saldo.  
5. **Relação com EF Core**: total (classe principal do EF).  
6. **Dependências**: entidades do Domain.  
7. **Erros comuns**:
   - esquecer chave composta;
   - não configurar precisão de decimal.  
8. **O que testar**: migrations (se houver) e startup do contexto.

---

## PASSO 17 — UnitOfWork (Infrastructure)

1. **Nome do arquivo**: `UnitOfWork.cs`  
2. **Pasta**: `src/DematecStock.Infrastructure/DataAccess/`  
3. **Responsabilidade**: implementar `Commit()` com `SaveChangesAsync()`.  
4. **Implementar primeiro**: injeção de `DematecStockDbContext` + método `Commit`.  
5. **Relação com EF Core**: grava alterações rastreadas no contexto.  
6. **Dependências**: `IUnitOfWork`, `DematecStockDbContext`.  
7. **Erros comuns**:
   - usar `SaveChanges` síncrono em API assíncrona.  
8. **O que testar**: persistência real após operações de Add/Update/Delete.

---

## PASSO 18 — Repository de InventoryLocation

1. **Nome do arquivo**: `InventoryLocationRepository.cs`  
2. **Pasta**: `src/DematecStock.Infrastructure/Repositories/`  
3. **Responsabilidade**: implementar contratos de escrita, busca e atualização.  
4. **Implementar primeiro**:
   - `GetByKey`;
   - `Add`;
   - `Update`;
   - `Delete`.  
5. **Relação com EF Core**:
   - `AddAsync`, `FirstOrDefaultAsync`, `Update`, `Remove`.  
6. **Dependências**:
   - `DematecStockDbContext`
   - interfaces `IInventoryLocationWriteOnlyRepository` e `IInventoryLocationUpdateOnlyRepository`.  
7. **Erros comuns**:
   - salvar no repository;
   - query incompleta sem chave composta.  
8. **O que testar**: chamadas dos UseCases refletindo corretamente no banco.

---

## PASSO 19 — Registro de infraestrutura no DI

1. **Nome do arquivo**: `DependencyInjectionExtension.cs`  
2. **Pasta**: `src/DematecStock.Infrastructure/`  
3. **Responsabilidade**: registrar DbContext, UnitOfWork e Repositories.  
4. **Implementar primeiro**: `AddDbContext` e `AddRepositories`.  
5. **Relação com EF Core**: configuração `UseSqlServer(connectionString)`.  
6. **Dependências**: classes Infrastructure + interfaces Domain.  
7. **Erros comuns**:
   - lifetime errado;
   - esquecer de registrar uma interface.  
8. **O que testar**: API sobe sem erro de injeção.

---

## PASSO 20 — Filtro global de exceção

1. **Nome do arquivo**: `ExceptionFilter.cs`  
2. **Pasta**: `src/DematecStock.Api/Filters/`  
3. **Responsabilidade**: converter exceções em resposta HTTP padronizada (`ResponseErrorJson`).  
4. **Implementar primeiro**:
   - tratar `DematecStockException`;
   - fallback de erro desconhecido (`500`).  
5. **Relação com EF Core**: indireta (captura exceções de persistência não tratadas).  
6. **Dependências**: exceções e `ResponseErrorJson`.  
7. **Erros comuns**:
   - não registrar filtro global;
   - vazar exceções internas para cliente.  
8. **O que testar**: provocar erro e validar payload de retorno.

---

## PASSO 21 — Controller de InventoryLocation

1. **Nome do arquivo**: `InventoryLocationController.cs`  
2. **Pasta**: `src/DematecStock.Api/Controllers/`  
3. **Responsabilidade**: endpoints HTTP (`POST`, `DELETE`, `PATCH`) sem regra de negócio.  
4. **Implementar primeiro**: assinatura dos endpoints e injeção dos UseCases.  
5. **Relação com EF Core**: nenhuma direta.  
6. **Dependências**:
   - UseCases de Add/Delete/Update;
   - DTOs de request/response.  
7. **Erros comuns**:
   - colocar regra de negócio no controller;
   - injetar repository direto (quebrando arquitetura).  
8. **O que testar**: Swagger com rotas:
   - `POST /api/inventorylocation`
   - `DELETE /api/inventorylocation/{idLocation}/{idProduct}`
   - `PATCH /api/inventorylocation/{idLocation}/{idProduct}/saldo`

---

## PASSO 22 — `Program.cs`

1. **Nome do arquivo**: `Program.cs`  
2. **Pasta**: `src/DematecStock.Api/`  
3. **Responsabilidade**: bootstrap da aplicação, DI e pipeline HTTP.  
4. **Implementar primeiro**:
   - `AddInfrastructure(...)`;
   - `AddApplication()`;
   - registro do `ExceptionFilter`;
   - `UseAuthentication()` e `UseAuthorization()`.  
5. **Relação com EF Core**: ativa registro do DbContext via `AddInfrastructure`.  
6. **Dependências**: todas as camadas.  
7. **Erros comuns**:
   - ordem errada de middlewares;
   - esquecer chamada de uma extensão de DI.  
8. **O que testar**: aplicação sobe e endpoints respondem conforme esperado.

---

## 3) Fluxo minucioso de `POST`, `DELETE`, `PATCH` em `InventoryLocation`

## POST — `Add`

1. Requisição chega no controller com `RequestAddInventoryLocationJson`.
2. Controller chama `IAddInventoryLocationUseCase.Execute(request)`.
3. UseCase valida `OnHandQuantity`:
   - `< 0` → lança `ErrorOnValidationException` (`400`).
4. UseCase mapeia DTO para entidade com AutoMapper.
5. UseCase chama repository `Add`.
6. UseCase chama `Commit` no UnitOfWork.
7. Controller retorna `201 Created`.

### Testes mínimos do POST
- payload com saldo negativo (`-1`) → `400`.
- payload válido → `201` e registro persistido.

---

## DELETE — `Delete`

1. Requisição chega no controller com `idLocation` e `idProduct` na rota.
2. Controller chama `IDeleteInventoryLocationUseCase.Execute(idLocation, idProduct)`.
3. UseCase busca item (`GetByKey`).
4. Se não existir → `NotFoundException` (`404`).
5. Se existir com `OnHandQuantity != 0` → `ErrorOnValidationException` (`400`).
6. Se existir com saldo `0`, executa `Delete` no repository.
7. Commit no UnitOfWork.
8. Controller retorna `204 NoContent`.

### Testes mínimos do DELETE
- item inexistente → `404`.
- item com saldo → `400`.
- item com saldo zero → `204` e remoção no banco.

---

## PATCH — `UpdateOnHandQuantity`

1. Requisição chega com rota + body (`RequestUpdateOnHandQuantityJson`).
2. Controller chama `IUpdateOnHandQuantityUseCase.Execute(...)`.
3. UseCase valida saldo:
   - `< 0` → `ErrorOnValidationException` (`400`).
4. UseCase busca por chave composta.
5. Não encontrado → `NotFoundException` (`404`).
6. Encontrado → chama `inventoryLocation.UpdateOnHandQuantity(request.OnHandQuantity)`.
7. Chama `Update` no repository.
8. Chama `Commit` no UnitOfWork.
9. Controller retorna `204 NoContent`.

### Testes mínimos do PATCH
- saldo negativo → `400`.
- item inexistente → `404`.
- atualização válida → `204` e saldo atualizado no banco.

---

## 4) Checklist de qualidade antes de concluir

- [ ] `ExceptionFilter` registrado globalmente.
- [ ] todos os UseCases registrados no `Application.DependencyInjectionExtension`.
- [ ] repositories + UnitOfWork + DbContext registrados no `Infrastructure.DependencyInjectionExtension`.
- [ ] chave composta de `InventoryLocation` configurada no `DbContext`.
- [ ] sem regra de negócio no controller.
- [ ] sem `SaveChangesAsync` dentro de repository.
- [ ] validações de saldo negativo em `POST` e `PATCH`.
- [ ] regra de bloqueio de `DELETE` com saldo aplicado.

---

## 5) Resumo rápido de responsabilidades por camada

- **API**: recebe/retorna HTTP.
- **Application**: decide regras e fluxo.
- **Domain**: define contratos e entidades.
- **Infrastructure**: executa acesso a dados (EF Core).
- **Communication**: DTOs da API.
- **Exception**: padrão de erros da aplicação.

Se seguir esta ordem, você evita retrabalho, reduz acoplamento e mantém a arquitetura limpa e escalável.