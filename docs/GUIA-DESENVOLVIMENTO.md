# 🗺️ Guia Passo a Passo — DematecStock

> **Para quem é este guia?**
> Para desenvolvedores que estão contribuindo com este projeto e precisam de um mapa claro sobre a ordem correta de criação de arquivos, respeitando a arquitetura em camadas adotada.

---

## Visão Geral da Arquitetura

Antes de qualquer coisa, entenda o **fluxo de uma requisição** neste projeto:

```
[Requisição HTTP]
      ↓
  Controller          → DematecStock.Api
      ↓
  Use Case            → DematecStock.Application
      ↓
  Interface Repo      → DematecStock.Domain
      ↓
  Implementação Repo  → DematecStock.Infrastructure
      ↓
  DbContext (EF Core) → DematecStock.Infrastructure
      ↓
  Banco de Dados (SQL Server)
```

E os projetos de suporte transversal:

- **`DematecStock.Exception`** → Erros customizados usados em qualquer camada
- **`DematecStock.Communication`** → Contratos de entrada (Request) e saída (Response) da API
- **`DematecStock.Domain`** → DTOs internos (resultados de queries/views/procedures)

---

## 📦 Mapa dos Projetos e suas Responsabilidades

| Projeto | Camada | O que vive aqui |
|---|---|---|
| `DematecStock.Exception` | Transversal | Exceções customizadas da aplicação |
| `DematecStock.Domain` | Domínio | Entities, Interfaces de Repositório, DTOs internos |
| `DematecStock.Communication` | Contrato | Requests e Responses que a API expõe |
| `DematecStock.Application` | Aplicação | Use Cases, AutoMapper, DI Extension |
| `DematecStock.Infrastructure` | Infraestrutura | DbContext, Repositórios concretos, DI Extension |
| `DematecStock.Api` | Apresentação | Controllers, Filters, Program.cs |

---

## ✅ Passos em Ordem — Para Cada Nova Funcionalidade

---

### 🔴 PASSO 1 — Exception Customizada (se necessário)

**Arquivo:** `NomeDaExcecaoException.cs`  
**Pasta:** `src/DematecStock.Exception/ExceptionsBase/`  
**Projeto:** `DematecStock.Exception`

**Responsabilidade:**  
Representa um tipo específico de erro de negócio que a aplicação pode lançar. Permite que o `ExceptionFilter` da API capture e retorne o código HTTP correto automaticamente.

**O que implementar:**

```csharp
// Exemplo: src/DematecStock.Exception/ExceptionsBase/NotFoundException.cs
public class NotFoundException : DematecStockException
{
    private readonly string _message;

    public NotFoundException(string message) => _message = message;

    // O filtro usa esse StatusCode para montar a resposta HTTP
    public override int StatusCode => StatusCodes.Status404NotFound;

    public override List<string> GetErrors() => new List<string> { _message };
}
```

**Relação com EF Core:** Nenhuma. Este projeto não conhece banco de dados.

**Dependências:** Apenas herda de `DematecStockException` que já existe no projeto.

**❌ Erros comuns:**
- Esquecer de herdar de `DematecStockException` (a classe base do projeto)
- Criar exceção genérica quando já existe uma adequada — sempre verifique `ErrorOnValidationException`, `InvalidLoginException` e `NotFoundException` antes de criar nova

**🧪 Teste antes de avançar:**  
O projeto `DematecStock.Exception` compila sem erros (`Ctrl+Shift+B`).

---

### 🟠 PASSO 2 — Entidade (Entity)

**Arquivo:** `NomeDaEntidade.cs`  
**Pasta:** `src/DematecStock.Domain/Entities/`  
**Projeto:** `DematecStock.Domain`

**Responsabilidade:**  
Representa uma tabela real do banco de dados. O EF Core usa esta classe para saber o nome da tabela, as colunas e o tipo de cada campo.

**O que implementar:**

```csharp
// Modelo baseado em WarehouseLocations.cs existente no projeto
[Table("NomeDaTabelaNoSqlServer")]
public class MinhaEntidade
{
    [Key]
    [Column("CodColunaPK")]
    public int Id { get; set; }

    [Column("NomeColunaNoSqlServer")]
    public string Nome { get; set; } = string.Empty;

    // Use private set para propriedades que só mudam via método
    [Column("Ativo")]
    public string? IsActive { get; private set; }

    // Métodos de domínio para alterar estado (padrão do projeto)
    public void ChangeIsActive(string value) { IsActive = value; }
}
```

**Relação com EF Core:**  
O atributo `[Table("...")]` diz ao EF qual tabela mapear. O `[Column("...")]` mapeia cada propriedade para a coluna correta. O `[Key]` define a chave primária.

**Dependências:** Nenhuma dependência de outros projetos.

**❌ Erros comuns:**
- Usar o nome da classe C# igual ao nome da tabela SQL e esquecer o `[Table]` — perigoso quando os nomes diferem (como `Estocagem` → `WarehouseLocations`)
- Esquecer `= string.Empty` em strings não-nulas (gera warning de nullable)
- Deixar o `set` público em propriedades que deveriam ser imutáveis externamente

**🧪 Teste antes de avançar:**  
`DematecStock.Domain` compila. Não é necessário testar no banco ainda.

---

### 🟠 PASSO 3 — DTO Interno (se usar View ou Procedure)

**Arquivo:** `NomeDaQueryResult.cs`  
**Pasta:** `src/DematecStock.Domain/DTOs/`  
**Projeto:** `DematecStock.Domain`

**Responsabilidade:**  
Quando o dado vem de uma **View** ou **Stored Procedure** do SQL Server (não de uma tabela simples), você não usa uma Entity — usa um DTO. Ele é o "molde" para receber o resultado da query.

**O que implementar:**

```csharp
// Modelo baseado em ProductSearchQueryResult.cs do projeto
public class ProdutoQueryResult
{
    public int IdProduto { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
}
```

**Relação com EF Core:**  
Este DTO será mapeado no `DbContext` com `HasNoKey()` e `ToView(null)` (para procedures) ou `ToView("nome_view")` (para views). Veja o Passo 8.

**Dependências:** Nenhuma.

**❌ Erros comuns:**
- Criar DTO com propriedades com nomes diferentes das colunas retornadas pela procedure/view sem usar `[Column]` — o EF não conseguirá mapear
- Usar Entity (com `[Key]`) para resultado de procedure — causa erro de configuração no EF

**🧪 Teste antes de avançar:**  
`DematecStock.Domain` compila sem erros.

---

### 🟡 PASSO 4 — Interface do Repositório

**Arquivo:** `INomeDaEntidadeReadOnlyRepository.cs` ou `...WriteOnlyRepository.cs`  
**Pasta:** `src/DematecStock.Domain/Repositories/NomeDaEntidade/`  
**Projeto:** `DematecStock.Domain`

**Responsabilidade:**  
Define **o que** o repositório consegue fazer, sem dizer **como**. Esta interface é o "contrato" que a camada de Application usa. O projeto Application nunca conhece EF Core — só conhece esta interface.

**O que implementar:**

```csharp
// Padrão do projeto: separar Read de Write
public interface IWarehouseLocationsReadOnlyRepository
{
    Task<List<WarehouseLocations>> GetAllAsync();
    Task<WarehouseLocations?> GetByIdAsync(int id);
}

public interface IWarehouseLocationsWriteOnlyRepository
{
    Task AddAsync(WarehouseLocations entity);
}
```

**Relação com EF Core:** Nenhuma. A interface não sabe que EF existe.

**Dependências:** Depende da Entity ou DTO criados nos Passos 2 ou 3.

**❌ Erros comuns:**
- Colocar a implementação concreta aqui (só deve ter a assinatura dos métodos)
- Misturar Read e Write na mesma interface — o padrão do projeto separa em arquivos distintos
- Esquecer o `Task<>` — todos os métodos devem ser assíncronos neste projeto

**🧪 Teste antes de avançar:**  
`DematecStock.Domain` compila. Revise se todos os tipos usados na interface existem.

---

### 🟡 PASSO 5 — Request e Response (Contrato da API)

**Arquivo Request:** `RequestNomeDaAcaoJson.cs`  
**Arquivo Response:** `ResponseNomeDaEntidadeJson.cs`  
**Pasta:** `src/DematecStock.Communication/Requests/` e `.../Responses/`  
**Projeto:** `DematecStock.Communication`

**Responsabilidade:**
- **Request** → Molde do corpo JSON que o *cliente* envia para a API (ex: dados para criar uma localização)
- **Response** → Molde do JSON que a *API retorna* para o cliente

**O que implementar:**

```csharp
// Request — o que o cliente envia
public class RequestWriteWarehouseLocationJson
{
    public string LocationName { get; set; } = string.Empty;
    public int Aisle { get; set; }
    public int Building { get; set; }
}

// Response — o que a API devolve
public class ResponseLocationsJson
{
    public int IdLocation { get; set; }
    public string LocationName { get; set; } = string.Empty;
}

// Response paginada (padrão do projeto para listas)
public class ResponseProductSearchPagedJson
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<ResponseProductSearchItemJson> Data { get; set; } = new();
}
```

**Relação com EF Core:** Nenhuma. Este projeto não conhece banco de dados.

**Dependências:** Nenhuma dependência de outros projetos da solução.

**❌ Erros comuns:**
- Expor a Entity diretamente na API em vez de criar um Response — isso quebra o encapsulamento e expõe colunas do banco
- Esquecer de inicializar listas (`= new()`) — gera `NullReferenceException` em runtime
- Colocar validações complexas aqui — validação fica no Use Case

**🧪 Teste antes de avançar:**  
`DematecStock.Communication` compila. Verifique se os nomes fazem sentido do ponto de vista do cliente da API.

---

### 🟢 PASSO 6 — Mapeamento no AutoMapper

**Arquivo:** `AutoMapping.cs` *(já existe — apenas adicionar o mapeamento)*  
**Pasta:** `src/DematecStock.Application/AutoMapper/`  
**Projeto:** `DematecStock.Application`

**Responsabilidade:**  
Ensina ao AutoMapper como converter automaticamente entre tipos. Evita código repetitivo de `entity.Prop = request.Prop`.

**O que adicionar:**

```csharp
// Dentro da classe AutoMapping.cs existente:
private void RequestToEntity()
{
    // Request → Entity (ao criar/atualizar)
    CreateMap<RequestWriteWarehouseLocationJson, WarehouseLocations>();
}

private void EntityToResponse()
{
    // Entity → Response (ao retornar para o cliente)
    CreateMap<WarehouseLocations, ResponseLocationsJson>();
}

private void DTOToResponse()
{
    // DTO interno → Response (para Views e Procedures)
    CreateMap<ProductLocationsQueryResult, ResponseProductLocations>();
}
```

**Relação com EF Core:** Nenhuma.

**Dependências:** Depende dos tipos criados nos Passos 2, 3 e 5.

**❌ Erros comuns:**
- Criar um mapeamento invertido errado — `CreateMap<A, B>()` mapeia A→B, não B→A. Para o caminho inverso, use `.ReverseMap()` ou crie outro `CreateMap`
- Mapear propriedades com nomes diferentes sem configurar — AutoMapper só mapeia automaticamente quando os nomes são iguais (case insensitive)
- Esquecer de registrar o mapeamento e só descobrir o erro em runtime (`AutoMapperMappingException`)

**🧪 Teste antes de avançar:**  
`DematecStock.Application` compila. Se quiser validar mapeamentos, adicione um teste unitário chamando `configuration.AssertConfigurationIsValid()`.

---

### 🔵 PASSO 7 — Implementação do Repositório

**Arquivo:** `NomeDaEntidadeRepository.cs`  
**Pasta:** `src/DematecStock.Infrastructure/Repositories/`  
**Projeto:** `DematecStock.Infrastructure`

**Responsabilidade:**  
Implementa concretamente a interface criada no Passo 4. **Aqui é o único lugar** onde o EF Core é usado diretamente para consultar ou salvar dados.

**O que implementar:**

```csharp
// Implementa a interface do Domain
public class WarehouseLocationRepository :
    IWarehouseLocationsReadOnlyRepository,
    IWarehouseLocationsWriteOnlyRepository
{
    private readonly DematecStockDbContext _dbContext;

    // DbContext é injetado automaticamente pelo ASP.NET Core
    public WarehouseLocationRepository(DematecStockDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Leitura — AsNoTracking() para consultas (melhor performance)
    public async Task<List<WarehouseLocations>> GetAllAsync()
        => await _dbContext.WarehouseLocations.AsNoTracking().ToListAsync();

    // Escrita — Add rastreia a entidade para salvar depois via UnitOfWork
    public async Task AddAsync(WarehouseLocations entity)
        => await _dbContext.WarehouseLocations.AddAsync(entity);

    // Para Stored Procedures:
    public async Task<List<ProductSearchQueryResult>> SearchAsync(string? q, CancellationToken ct)
        => await _dbContext.ProductSearch
            .FromSqlInterpolated($"EXEC dbo.usp_Wms_ProductSearch {q}")
            .AsNoTracking()
            .ToListAsync(ct);
}
```

> ⚠️ **Regra importante do projeto:** O repositório **não chama `SaveChanges()`**. Quem faz isso é o `UnitOfWork`, chamado pelo Use Case. Assim você pode salvar várias operações em uma única transação.

**Relação com EF Core:** ✅ Central. Usa `DbContext`, `DbSet`, `AsNoTracking()`, `ToListAsync()`, `AddAsync()`, `FromSqlInterpolated()`.

**Dependências:** `DematecStockDbContext` (Passo 8), interfaces do Domain (Passo 4).

**❌ Erros comuns:**
- Chamar `_dbContext.SaveChangesAsync()` dentro do repositório — quebra o padrão de `UnitOfWork` do projeto
- Esquecer `AsNoTracking()` em consultas de leitura — o EF fica rastreando objetos desnecessariamente, consumindo memória
- Usar `await` dentro de um `Select()` do LINQ — não funciona; faça o `await` primeiro, depois projete

**🧪 Teste antes de avançar:**  
`DematecStock.Infrastructure` compila. A query SQL está correta (teste direto no SQL Server Management Studio antes de colocar no código).

---

### 🔵 PASSO 8 — Registrar no DbContext (se Entity ou DTO novo)

**Arquivo:** `DematecStockDbContext.cs` *(já existe — apenas adicionar)*  
**Pasta:** `src/DematecStock.Infrastructure/DataAccess/`  
**Projeto:** `DematecStock.Infrastructure`

**Responsabilidade:**  
O `DbContext` é o "gerenciador" de banco de dados do EF Core. Cada tabela que você quer acessar precisa ter um `DbSet<>` registrado aqui.

**O que adicionar:**

Para uma **tabela comum** (Entity):
```csharp
// Adicionar propriedade DbSet
public DbSet<MinhaEntidade> MinhasEntidades { get; set; }
```

Para um **DTO de View** (como `FlatLocationWithProductsQueryResult`):
```csharp
// Em OnModelCreating:
modelBuilder.Entity<MinhaViewQueryResult>(entity =>
{
    entity.HasNoKey()
        .ToView("vw_NomeDaView", "dbo");
    // Configure precisão de decimais se houver
    entity.Property(p => p.Quantidade).HasPrecision(18, 4);
});
```

Para um **DTO de Stored Procedure** (como `ProductSearchQueryResult`):
```csharp
// DbSet exposto como propriedade
public DbSet<ProcedureQueryResult> NomeDaProcedure => Set<ProcedureQueryResult>();

// Em OnModelCreating:
modelBuilder.Entity<ProcedureQueryResult>(entity =>
{
    entity.HasNoKey();
    entity.ToView(null); // null = não é uma view, é resultado de procedure
});
```

**Relação com EF Core:** ✅ Central. Este arquivo É o EF Core no projeto.

**Dependências:** Entity ou DTO criados nos Passos 2 e 3.

**❌ Erros comuns:**
- Esquecer `HasNoKey()` para DTOs de procedure/view — EF exige chave primária por padrão e lança exceção
- Esquecer `ToView(null)` para procedures — EF tentará mapear para uma view inexistente
- Não configurar `HasPrecision()` para decimais — pode gerar warning ou erro de truncamento

**🧪 Teste antes de avançar:**  
Compile e execute a aplicação. Se o EF estiver mal configurado, o erro aparece na inicialização, não em runtime.

---

### 🔵 PASSO 9 — Registrar no DI da Infrastructure

**Arquivo:** `DependencyInjectionExtension.cs` *(já existe — apenas adicionar)*  
**Pasta:** `src/DematecStock.Infrastructure/`  
**Projeto:** `DematecStock.Infrastructure`

**Responsabilidade:**  
"Apresentar" ao ASP.NET Core: *"quando alguém pedir a interface X, entregue a implementação Y"*. Sem isso, a injeção de dependência falha em runtime.

**O que adicionar:**

```csharp
private static void AddRepositories(IServiceCollection services)
{
    // Padrão existente no projeto:
    services.AddScoped<IWarehouseLocationsReadOnlyRepository, WarehouseLocationRepository>();
    services.AddScoped<IWarehouseLocationsWriteOnlyRepository, WarehouseLocationRepository>();

    // Adicione a nova linha aqui:
    services.AddScoped<IMinhaNovaInterface, MinhaNovaImplementacao>();
}
```

> 💡 **`AddScoped`** = uma instância por requisição HTTP. É o padrão deste projeto para repositórios e use cases.

**Relação com EF Core:** Indireta — garante que o `DbContext` seja injetado corretamente no repositório.

**Dependências:** Interface (Passo 4) e Implementação (Passo 7).

**❌ Erros comuns:**
- Usar `AddSingleton` para repositório — o `DbContext` é `Scoped`, e um `Singleton` não pode depender de `Scoped` (erro em runtime)
- Esquecer de registrar e só descobrir com `InvalidOperationException: Unable to resolve service` ao testar o endpoint

**🧪 Teste antes de avançar:**  
`DematecStock.Infrastructure` compila. Execute a aplicação e verifique que não há erros de DI no startup.

---

### 🟣 PASSO 10 — Interface do Use Case

**Arquivo:** `INomeDaAcaoUseCase.cs`  
**Pasta:** `src/DematecStock.Application/UseCases/NomeDaFuncionalidade/NomeDaAcao/`  
**Projeto:** `DematecStock.Application`

**Responsabilidade:**  
Define a assinatura do Use Case. O Controller da API depende desta interface, nunca da implementação concreta.

**O que implementar:**

```csharp
public interface IGetAllLocationsUseCase
{
    // O método sempre retorna Task (assíncrono)
    // Recebe Request ou parâmetros simples
    // Retorna um Response do Communication
    Task<ResponseLocationsJson> Execute();
}
```

**Relação com EF Core:** Nenhuma.

**Dependências:** Tipos de `DematecStock.Communication` (Passo 5).

**❌ Erros comuns:**
- Colocar lógica de negócio na interface — interfaces só têm assinaturas
- Retornar tipos de Entity ou DTO interno — o Controller deve receber apenas tipos do `Communication`

**🧪 Teste antes de avançar:**  
`DematecStock.Application` compila.

---

### 🟣 PASSO 11 — Implementação do Use Case

**Arquivo:** `NomeDaAcaoUseCase.cs`  
**Pasta:** `src/DematecStock.Application/UseCases/NomeDaFuncionalidade/NomeDaAcao/`  
**Projeto:** `DematecStock.Application`

**Responsabilidade:**  
Contém toda a **lógica de negócio**. Orquestra: validar entrada → chamar repositório → transformar resultado → retornar Response.

**O que implementar:**

```csharp
public class GetProductSearchUseCase : IGetProductSearchUseCase
{
    // Depende APENAS de interfaces do Domain — nunca de implementações concretas
    private readonly IProductSearchReadOnlyRepository _repository;

    public GetProductSearchUseCase(IProductSearchReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResponseProductSearchPagedJson> Execute(
        string? q, int page, int pageSize, CancellationToken ct)
    {
        // 1. Sanitizar/validar entrada
        page = Math.Max(1, page);
        pageSize = Math.Min(Math.Max(1, pageSize), 20);

        // 2. Chamar repositório
        var rows = await _repository.SearchAsync(q?.Trim(), page, pageSize, ct);

        // 3. Lançar exceção de negócio se necessário
        if (!rows.Any())
            throw new NotFoundException("Nenhum produto encontrado.");

        // 4. Projetar para Response e retornar
        return new ResponseProductSearchPagedJson
        {
            Page = page,
            PageSize = pageSize,
            Data = rows.Select(x => new ResponseProductSearchItemJson
            {
                IdProduct = x.IdProduct,
                ProductDescription = x.ProductDescription
            }).ToList()
        };
    }
}
```

> 💡 Se o Use Case **salva dados**, injete também `IUnitOfWork` e chame `await _unitOfWork.Commit()` **no final, após todas as operações**.

```csharp
// Exemplo com UnitOfWork para operações de escrita:
public class CreateLocationUseCase : ICreateLocationUseCase
{
    private readonly IWarehouseLocationsWriteOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateLocationUseCase(
        IWarehouseLocationsWriteOnlyRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResponseLocationsJson> Execute(RequestWriteWarehouseLocationJson request)
    {
        var entity = _mapper.Map<WarehouseLocations>(request);
        await _repository.AddAsync(entity);
        await _unitOfWork.Commit(); // ← Salva no banco
        return _mapper.Map<ResponseLocationsJson>(entity);
    }
}
```

**Relação com EF Core:** Nenhuma direta. O Use Case não sabe que EF existe.

**Dependências:** Interfaces de repositório (Passo 4), tipos de Communication (Passo 5), exceções (Passo 1).

**❌ Erros comuns:**
- Injetar `DematecStockDbContext` diretamente no Use Case — viola a arquitetura; use sempre a interface do repositório
- Esquecer de chamar `_unitOfWork.Commit()` em operações de escrita — os dados não são salvos no banco
- Colocar lógica de negócio no Controller em vez de no Use Case

**🧪 Teste antes de avançar:**  
`DematecStock.Application` compila. Escreva um teste unitário básico mockando o repositório se possível.

---

### 🟣 PASSO 12 — Registrar Use Case no DI da Application

**Arquivo:** `DependencyInjectionExtension.cs` *(já existe — apenas adicionar)*  
**Pasta:** `src/DematecStock.Application/`  
**Projeto:** `DematecStock.Application`

**O que adicionar:**

```csharp
private static void AddUseCases(IServiceCollection services)
{
    // Padrão existente:
    services.AddScoped<IGetAllLocationsUseCase, GetAllLocationsUseCase>();

    // Adicione o novo:
    services.AddScoped<IMeuNovoUseCase, MeuNovoUseCase>();
}
```

**❌ Erros comuns:**
- Adicionar no DI da Infrastructure em vez do Application — cada projeto tem seu próprio `DependencyInjectionExtension`
- Registrar a implementação sem a interface (`services.AddScoped<MeuUseCase>()`) — o Controller não conseguirá resolver via interface

**🧪 Teste antes de avançar:**  
Execute a aplicação. Não deve haver erros de DI no startup.

---

### ⚪ PASSO 13 — Controller

**Arquivo:** `NomeDaFuncionalidadeController.cs`  
**Pasta:** `src/DematecStock.Api/Controllers/`  
**Projeto:** `DematecStock.Api`

**Responsabilidade:**  
Receber a requisição HTTP, chamar o Use Case via `[FromServices]` e retornar a resposta com o código HTTP correto. **Nenhuma lógica de negócio aqui.**

**O que implementar:**

```csharp
[Route("api/[controller]")]
[ApiController]
// [Authorize] ← Adicione se o endpoint requer autenticação JWT
public class ProductSearchController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ResponseProductSearchPagedJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(
        // Use Case injetado via [FromServices] — padrão deste projeto
        [FromServices] IGetProductSearchUseCase useCase,
        [FromQuery] string? q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        // Validações simples de contrato HTTP ficam aqui
        if (page <= 0)
            return BadRequest(new ResponseErrorJson("Página deve ser maior que zero."));

        // Delega tudo para o Use Case
        var response = await useCase.Execute(q, page, pageSize, ct);
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(
        [FromServices] ICreateLocationUseCase useCase,
        [FromBody] RequestWriteWarehouseLocationJson request)
    {
        var response = await useCase.Execute(request);
        // 201 Created com a URL do novo recurso
        return Created(string.Empty, response);
    }
}
```

> 💡 O `ExceptionFilter` já registrado no `Program.cs` captura qualquer `DematecStockException` lançada pelo Use Case e retorna o status HTTP correto automaticamente — por isso você **não** precisa de `try/catch` no Controller.

**Relação com EF Core:** Nenhuma.

**Dependências:** Interfaces de Use Case (Passo 10), tipos de Communication (Passo 5).

**❌ Erros comuns:**
- Injetar o Use Case via construtor do Controller em vez de `[FromServices]` no método — o padrão deste projeto usa `[FromServices]` por método
- Adicionar `try/catch` no Controller — o `ExceptionFilter` já faz esse trabalho
- Esquecer `[ApiController]` — sem ele, o model binding automático e as respostas de validação não funcionam

**🧪 Teste antes de avançar:**  
Abra o Swagger (`/swagger`) e teste o endpoint manualmente com dados válidos e inválidos.

---

## 🔄 Resumo Visual — Checklist de Criação

```
Para cada nova funcionalidade, siga esta ordem:

[ ]  1. Exception (se necessário)     → DematecStock.Exception
[ ]  2. Entity                        → DematecStock.Domain/Entities
[ ]  3. DTO interno (se view/proc)    → DematecStock.Domain/DTOs
[ ]  4. Interface Repositório         → DematecStock.Domain/Repositories
[ ]  5. Request + Response            → DematecStock.Communication
[ ]  6. Mapeamento AutoMapper         → DematecStock.Application/AutoMapper
[ ]  7. Implementação Repositório     → DematecStock.Infrastructure/Repositories
[ ]  8. Registro no DbContext         → DematecStock.Infrastructure/DataAccess
[ ]  9. DI da Infrastructure          → DematecStock.Infrastructure/DependencyInjectionExtension.cs
[ ] 10. Interface Use Case            → DematecStock.Application/UseCases
[ ] 11. Implementação Use Case        → DematecStock.Application/UseCases
[ ] 12. DI da Application             → DematecStock.Application/DependencyInjectionExtension.cs
[ ] 13. Controller                    → DematecStock.Api/Controllers
```

> 💡 **Dica final:** A ordem acima garante que, quando você for escrever um arquivo, todas as suas dependências já existem. Você **nunca** terá referência quebrada se seguir essa sequência de baixo para cima na arquitetura.

---

## 🔑 Referências Rápidas — Arquivos-Chave do Projeto

| Arquivo | Caminho | Para que serve |
|---|---|---|
| `DematecStockDbContext.cs` | `Infrastructure/DataAccess/` | Registrar DbSets e configurar EF Core |
| `AutoMapping.cs` | `Application/AutoMapper/` | Adicionar mapeamentos AutoMapper |
| `DependencyInjectionExtension.cs` | `Application/` | Registrar Use Cases no DI |
| `DependencyInjectionExtension.cs` | `Infrastructure/` | Registrar Repositórios no DI |
| `ExceptionFilter.cs` | `Api/Filters/` | Captura exceções e retorna HTTP correto |
| `Program.cs` | `Api/` | Configuração geral da aplicação |
| `IUnitOfWork.cs` | `Domain/Repositories/` | Contrato para commit de transações |
| `UnitOfWork.cs` | `Infrastructure/DataAccess/` | Implementação do SaveChangesAsync |
