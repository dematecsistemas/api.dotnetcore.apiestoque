# .NET 10 Upgrade Plan - DematecStock Solution

## Table of Contents

- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Project-by-Project Plans](#project-by-project-plans)
  - [DematecStock.Communication](#dematecstockcommunication)
  - [DematecStock.Domain](#dematecstockdomain)
  - [DematecStock.Exception](#dematecstockexception)
  - [DematecStock.Infrastructure](#dematecstockinfrastructure)
  - [DematecStock.Application](#dematecstockapplication)
  - [DematecStock.Api](#dematecstockapi)
- [Package Update Reference](#package-update-reference)
- [Breaking Changes Catalog](#breaking-changes-catalog)
- [Risk Management](#risk-management)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

### Scenario Description
Upgrade DematecStock solution from **.NET 8.0** to **.NET 10.0 (Long Term Support)** to leverage the latest platform improvements, security updates, and performance enhancements.

### Scope
- **Total Projects**: 6
- **Current State**: All projects target `net8.0`
- **Target State**: All projects will target `net10.0`
- **Total Code Files**: 60
- **Total Lines of Code**: 1,651
- **Estimated LOC to Modify**: 15+ (0.9% of codebase)

### Discovered Metrics
| Metric | Value | Classification |
|--------|-------|----------------|
| Project Count | 6 | Small solution |
| Dependency Depth | 3 levels | Simple structure |
| High-Risk Projects | 0 | Low risk |
| Security Vulnerabilities | 0 | No critical issues |
| Circular Dependencies | 0 | Clean architecture |
| Target Heterogeneity | Homogeneous (all net8.0→net10.0) | Simple upgrade path |

### Complexity Classification
**Simple Solution** - All indicators point to a straightforward upgrade:
- Small project count (≤5 threshold)
- Shallow dependency tree (depth 3)
- All projects rated as Low difficulty
- Clean dependency structure with no cycles
- Homogeneous target framework (all net8.0 → net10.0)
- No security vulnerabilities
- All SDK-style projects

### Selected Strategy
**All-At-Once Strategy** - All 6 projects will be upgraded simultaneously in a single coordinated operation.

**Rationale**:
- Small solution size enables atomic upgrade
- All projects currently on modern .NET (8.0)
- Clean dependency structure supports simultaneous update
- All required packages have known net10.0 versions
- Low risk profile across all projects
- Faster completion time vs incremental approach

### Critical Issues
- **API Incompatibilities**: 8 binary incompatible + 7 source incompatible APIs (primarily JWT/Identity-related)
- **Package Updates Required**: 5 packages need updates to net10.0-compatible versions
- **Main Technology Challenge**: IdentityModel & JWT Bearer authentication (4 affected APIs)

### Iteration Strategy
**Fast Batch Approach** (2-3 detail iterations):
- Iteration 1: Foundation (dependency analysis, strategy, stubs)
- Iteration 2: Batch all project details (all 6 projects in one iteration given low complexity)
- Iteration 3: Finalize (testing strategy, success criteria, source control)

## Migration Strategy

### Approach Selection: All-At-Once

**Selected Strategy**: All-At-Once Strategy

**Justification**:

✅ **Ideal Conditions Met**:
- Small solution (6 projects < 30 threshold)
- All projects currently on .NET 8.0 (modern .NET)
- Homogeneous codebase (consistent patterns, all SDK-style)
- Low external dependency complexity (10 total packages, 5 need updates)
- All NuGet packages have known net10.0 versions available
- Assessment shows clear upgrade path for all packages

✅ **Advantages for This Solution**:
- **Fastest Completion**: Single coordinated operation
- **No Multi-Targeting**: Avoid complexity of supporting multiple frameworks
- **Clean Dependency Resolution**: All projects benefit simultaneously, no version conflicts
- **Unified Testing**: Single comprehensive validation pass
- **Simple Coordination**: All developers upgrade at same time

⚠️ **Acknowledged Challenges**:
- **Higher Initial Risk**: All changes applied at once (mitigated by low complexity and good tests)
- **Larger Testing Surface**: Must validate entire solution (but only 1,651 LOC)
- **Coordinated Deployment**: All projects deploy together (acceptable for this solution structure)

### All-At-Once Strategy Principles

**Simultaneity**: All project files and package references updated in a single atomic operation. No intermediate states where some projects are on net8.0 and others on net10.0.

**Execution Sequence**:
1. Update all 6 project files (`TargetFramework` property: `net8.0` → `net10.0`)
2. Update all 5 package references across affected projects
3. Restore dependencies (`dotnet restore`)
4. Build entire solution to identify all compilation errors
5. Fix all breaking changes in a single pass (reference §Breaking Changes Catalog)
6. Rebuild and verify 0 errors
7. Execute all tests
8. Validate solution-wide functionality

**Atomic Operation**: Steps 1-6 are treated as a single unit of work. Cannot cherry-pick individual project upgrades.

### Dependency-Based Validation Order

While all projects are upgraded atomically, **validation** should follow dependency order to isolate issues efficiently:

1. **Validate Leaf Projects First** (Communication, Domain, Exception)
   - No dependencies = easier to diagnose issues
   - Build independently to confirm project file and package updates successful

2. **Validate Mid-Tier Projects** (Infrastructure, Application)
   - Depend on leaf projects
   - Verify leaf project changes didn't break consumers

3. **Validate Root Project Last** (Api)
   - Depends on all projects
   - Final integration validation point
   - Confirms entire solution works together

**Note**: This validation order is for troubleshooting efficiency during the atomic upgrade, NOT a sequential project-by-project upgrade.

### Risk Management Approach

**Mitigation for Higher Initial Risk**:
- Comprehensive breaking changes catalog prepared in advance (see §Breaking Changes Catalog)
- Clear rollback plan (revert branch to pre-upgrade state)
- All changes in single commit for easy rollback
- No security vulnerabilities to address = lower risk profile
- All SDK-style projects = simpler upgrade process

**Testing Strategy**:
- Build validation after framework/package updates
- Compilation error resolution before testing
- Unit test execution (if tests exist)
- Manual smoke testing of Api endpoints

### Source Control Approach

**Single Commit Strategy** (preferred for All-At-Once):
- All project file updates + package updates + breaking change fixes in one commit
- Commit message: `chore: upgrade solution from .NET 8.0 to .NET 10.0`
- Rationale: Atomic upgrade = atomic commit, easier rollback
- If compilation errors require multiple attempts, intermediate commits allowed but squash before merge

See §Source Control Strategy for detailed branching/merge approach.

## Detailed Dependency Analysis

### Dependency Graph Summary

The solution has a clean, well-structured dependency hierarchy with **3 levels** and **no circular dependencies**:

```
Level 0 (Leaf nodes - 3 projects):
  └─ DematecStock.Communication (0 dependencies, 2 dependants)
  └─ DematecStock.Domain (0 dependencies, 2 dependants)
  └─ DematecStock.Exception (0 dependencies, 3 dependants)

Level 1 (Mid-tier - 2 projects):
  └─ DematecStock.Infrastructure (2 dependencies: Exception, Domain; 1 dependant)
  └─ DematecStock.Application (3 dependencies: Communication, Exception, Domain; 1 dependant)

Level 2 (Root - 1 project):
  └─ DematecStock.Api (4 dependencies: Communication, Exception, Infrastructure, Application; 0 dependants)
```

### Migration Phases (All-At-Once Strategy)

Since we're using the All-At-Once strategy, all projects will be upgraded simultaneously in **Phase 1**. However, understanding the dependency order helps prioritize validation:

**Phase 1: Atomic Upgrade (All Projects)**
- **Group A - Leaf Projects** (no dependencies):
  - `DematecStock.Communication.csproj`
  - `DematecStock.Domain.csproj`
  - `DematecStock.Exception.csproj`

- **Group B - Mid-Tier Projects** (depend on Group A):
  - `DematecStock.Infrastructure.csproj` → depends on Exception, Domain
  - `DematecStock.Application.csproj` → depends on Communication, Exception, Domain

- **Group C - Root Project** (depends on all):
  - `DematecStock.Api.csproj` → depends on Communication, Exception, Infrastructure, Application

**All groups upgraded atomically**, but validation will flow from Group A → B → C.

### Critical Path

The critical path for dependency resolution:
```
Exception/Domain → Infrastructure → Api
                                   ↗
Domain/Communication/Exception → Application ↗
```

**Key Observations**:
- **Exception** project is most depended upon (3 dependants) - changes here ripple through solution
- **Domain** is shared by both Infrastructure and Application
- **Api** consumes all other projects - final validation point
- No bottleneck projects (all dependencies well-distributed)

### Project Groupings

For the All-At-Once strategy, projects are updated simultaneously but can be understood by their role:

| Group | Projects | Role | Validation Priority |
|-------|----------|------|---------------------|
| **Foundation** | Communication, Domain, Exception | Core contracts and DTOs | First (no dependencies) |
| **Business Logic** | Application, Infrastructure | Use cases and data access | Second (after foundation) |
| **Presentation** | Api | HTTP endpoints and DI | Final (after all dependencies) |

## Project-by-Project Plans

### DematecStock.Communication

**Current State**: `net8.0`, ClassLibrary, SDK-style, 11 files, 189 LOC, 0 dependencies, 2 dependants (Api, Application)

**Target State**: `net10.0`

**Migration Steps**:

1. **Prerequisites**: None (leaf project)

2. **Update Target Framework**:
   - File: `src\DematecStock.Communication\DematecStock.Communication.csproj`
   - Change: `<TargetFramework>net8.0</TargetFramework>` → `<TargetFramework>net10.0</TargetFramework>`

3. **Package Updates**: None (no packages referenced)

4. **Expected Breaking Changes**: None
   - No API compatibility issues identified
   - Pure DTO/communication models, no framework-specific APIs

5. **Code Modifications**: None expected
   - Simple response/request DTOs
   - No framework-dependent code

6. **Testing Strategy**:
   - Verify project builds without errors
   - Verify no warnings
   - Consumed by Application and Api - will be validated through those projects

7. **Validation Checklist**:
   - [ ] Project file updated to net10.0
   - [ ] `dotnet restore` succeeds
   - [ ] Project builds: `dotnet build src\DematecStock.Communication\DematecStock.Communication.csproj`
   - [ ] 0 errors, 0 warnings
   - [ ] No breaking changes to public DTOs (verified during Api/Application validation)

---

### DematecStock.Domain

**Current State**: `net8.0`, ClassLibrary, SDK-style, 16 files, 309 LOC, 0 dependencies, 2 dependants (Application, Infrastructure)

**Packages**: DocumentFormat.OpenXml 3.4.1 (✅ compatible with net10.0, no update needed)

**Target State**: `net10.0`

**Migration Steps**:

1. **Prerequisites**: None (leaf project)

2. **Update Target Framework**:
   - File: `src\DematecStock.Domain\DematecStock.Domain.csproj`
   - Change: `<TargetFramework>net8.0</TargetFramework>` → `<TargetFramework>net10.0</TargetFramework>`

3. **Package Updates**: None
   - DocumentFormat.OpenXml 3.4.1: ✅ Compatible with net10.0, no update required

4. **Expected Breaking Changes**: None
   - No API compatibility issues identified
   - Domain models and entities, no framework-specific APIs
   - DocumentFormat.OpenXml is compatible

5. **Code Modifications**: None expected
   - Domain entities, repositories (interfaces), DTOs
   - No framework-dependent code identified

6. **Testing Strategy**:
   - Verify project builds without errors
   - Verify DocumentFormat.OpenXml still functions (if used for Excel export)
   - Consumed by Application and Infrastructure - will be validated through those projects

7. **Validation Checklist**:
   - [ ] Project file updated to net10.0
   - [ ] `dotnet restore` succeeds
   - [ ] Project builds: `dotnet build src\DematecStock.Domain\DematecStock.Domain.csproj`
   - [ ] 0 errors, 0 warnings
   - [ ] DocumentFormat.OpenXml types still resolve correctly
   - [ ] No breaking changes to domain models (verified during Application/Infrastructure validation)

---

### DematecStock.Exception

**Current State**: `net8.0`, ClassLibrary, SDK-style, 4 files, 72 LOC, 0 dependencies, 3 dependants (Api, Application, Infrastructure)

**Target State**: `net10.0`

**Migration Steps**:

1. **Prerequisites**: None (leaf project)

2. **Update Target Framework**:
   - File: `src\DematecStock.Exception\DematecStock.Exception.csproj`
   - Change: `<TargetFramework>net8.0</TargetFramework>` → `<TargetFramework>net10.0</TargetFramework>`

3. **Package Updates**: None (no packages referenced)

4. **Expected Breaking Changes**: None
   - No API compatibility issues identified
   - Custom exception classes deriving from `System.Exception`
   - No framework-specific APIs

5. **Code Modifications**: None expected
   - Simple exception hierarchy
   - File context shows: `DematecStockException` inherits from `System.Exception` (base class, no breaking changes)

6. **Testing Strategy**:
   - Verify project builds without errors
   - Consumed by Api, Application, Infrastructure - will be validated through those projects
   - Exception throwing/catching will be tested through dependent projects

7. **Validation Checklist**:
   - [ ] Project file updated to net10.0
   - [ ] `dotnet restore` succeeds
   - [ ] Project builds: `dotnet build src\DematecStock.Exception\DematecStock.Exception.csproj`
   - [ ] 0 errors, 0 warnings
   - [ ] No breaking changes to exception classes (verified during dependent project validation)

---

### DematecStock.Infrastructure

**Current State**: `net8.0`, ClassLibrary, SDK-style, 8 files, 448 LOC, 2 dependencies (Exception, Domain), 1 dependant (Api)

**Packages**: 
- BCrypt.Net-Next 4.0.3 (✅ compatible)
- Microsoft.EntityFrameworkCore 9.0.11
- Microsoft.EntityFrameworkCore.SqlServer 9.0.11
- Microsoft.Extensions.Configuration.Binder 10.0.1
- System.IdentityModel.Tokens.Jwt 8.15.0 (✅ compatible)

**Target State**: `net10.0`

**Migration Steps**:

1. **Prerequisites**: Exception, Domain projects upgraded to net10.0

2. **Update Target Framework**:
   - File: `src\DematecStock.Infrastructure\DematecStock.Infrastructure.csproj`
   - Change: `<TargetFramework>net8.0</TargetFramework>` → `<TargetFramework>net10.0</TargetFramework>`

3. **Package Updates**:

| Package | Current Version | Target Version | Reason |
|---------|----------------|----------------|--------|
| Microsoft.EntityFrameworkCore | 9.0.11 | 10.0.4 | Framework compatibility (.NET 10 requires EF Core 10) |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.11 | 10.0.4 | Must match EF Core version |
| Microsoft.Extensions.Configuration.Binder | 10.0.1 | 10.0.4 | Patch update for .NET 10 compatibility |

**No update needed**:
- BCrypt.Net-Next 4.0.3: ✅ Compatible
- System.IdentityModel.Tokens.Jwt 8.15.0: ✅ Compatible

4. **Expected Breaking Changes**:

**Category: Configuration Binding** (3 instances)
- **API**: `Microsoft.Extensions.Configuration.ConfigurationBinder.GetValue<T>(IConfiguration, string)`
- **Status**: Binary incompatible
- **Impact**: May require explicit type specification or null handling
- **Files Likely Affected**: Configuration reading code (DI setup, settings classes)
- **Fix**: Review all `IConfiguration.GetValue<T>()` calls, consider using `IOptions<T>` pattern

**Category: JWT Token Generation** (3 instances)
- **API**: `JwtSecurityTokenHandler.CreateToken()`, `JwtSecurityTokenHandler.WriteToken()`, `JwtSecurityTokenHandler` constructor
- **Status**: Binary incompatible
- **Impact**: JWT token creation/serialization may have changed signature or behavior
- **Files Likely Affected**: TokenService.cs, JwtTokenGenerator.cs (or similar)
- **Fix**: Review token generation code, ensure `SecurityTokenDescriptor` and `JwtSecurityToken` usage matches net10.0 API

5. **Code Modifications**:

**High Priority**:
- Review JWT token generation logic (likely in a service/infrastructure layer)
- Validate `TokenValidationParameters` configuration still valid
- Test configuration binding for connection strings and settings

**Areas to Review**:
- Database context registration (EF Core 10 may have DI changes)
- JWT token service implementation
- Configuration access patterns

6. **Testing Strategy**:
   - Build project after updates
   - Verify EF Core migrations still compatible (no schema changes expected)
   - Test JWT token generation end-to-end
   - Validate database connections
   - Test configuration binding for settings

7. **Validation Checklist**:
   - [ ] Project file updated to net10.0
   - [ ] 3 packages updated to version 10.0.4
   - [ ] `dotnet restore` succeeds
   - [ ] Project builds: `dotnet build src\DematecStock.Infrastructure\DematecStock.Infrastructure.csproj`
   - [ ] 0 errors, 0 warnings
   - [ ] JWT token generation code compiles
   - [ ] Configuration binding code compiles
   - [ ] EF Core DbContext registration works
   - [ ] Can connect to database (integration test if available)
   - [ ] JWT tokens can be generated and validated

---

### DematecStock.Application

**Current State**: `net8.0`, ClassLibrary, SDK-style, 16 files, 386 LOC, 3 dependencies (Communication, Exception, Domain), 1 dependant (Api)

**Packages**: 
- AutoMapper 14.0.0 (✅ compatible)
- Microsoft.Extensions.DependencyInjection 9.0.11

**Target State**: `net10.0`

**Migration Steps**:

1. **Prerequisites**: Communication, Exception, Domain projects upgraded to net10.0

2. **Update Target Framework**:
   - File: `src\DematecStock.Application\DematecStock.Application.csproj`
   - Change: `<TargetFramework>net8.0</TargetFramework>` → `<TargetFramework>net10.0</TargetFramework>`

3. **Package Updates**:

| Package | Current Version | Target Version | Reason |
|---------|----------------|----------------|--------|
| Microsoft.Extensions.DependencyInjection | 9.0.11 | 10.0.4 | Framework compatibility (.NET 10 requires matching version) |

**No update needed**:
- AutoMapper 14.0.0: ✅ Compatible with net10.0

4. **Expected Breaking Changes**:

**Category: Configuration Binding** (1 instance)
- **API**: `Microsoft.Extensions.Configuration.ConfigurationBinder.GetValue<T>(IConfiguration, string)` (likely transitive usage)
- **Status**: Binary incompatible
- **Impact**: Minimal (Application layer typically doesn't access configuration directly)
- **Fix**: If used, review and update similar to Infrastructure project

**Note**: Most DI patterns are stable between .NET 9 and 10. ServiceCollection/ServiceProvider usage should be unaffected.

5. **Code Modifications**:

**Low Priority**:
- Verify AutoMapper profiles still compile
- Confirm use case classes build successfully
- No major API changes expected in Application layer

**Areas to Review**:
- Dependency injection registration (if custom IServiceCollection extensions)
- Use case implementations (business logic typically framework-agnostic)

6. **Testing Strategy**:
   - Build project after updates
   - Run unit tests for use cases (if they exist)
   - Verify AutoMapper mappings still function
   - Test through Api project endpoints

7. **Validation Checklist**:
   - [ ] Project file updated to net10.0
   - [ ] 1 package updated to version 10.0.4
   - [ ] `dotnet restore` succeeds
   - [ ] Project builds: `dotnet build src\DematecStock.Application\DematecStock.Application.csproj`
   - [ ] 0 errors, 0 warnings
   - [ ] AutoMapper profiles compile
   - [ ] Use case classes compile
   - [ ] Unit tests pass (if they exist)

---

### DematecStock.Api

**Current State**: `net8.0`, AspNetCore, SDK-style, 7 files, 247 LOC, 4 dependencies (Communication, Exception, Infrastructure, Application), 0 dependants

**Packages**: 
- Microsoft.AspNetCore.Authentication.JwtBearer 8.0.22
- Swashbuckle.AspNetCore 6.6.2 (✅ compatible)

**Target State**: `net10.0`

**Migration Steps**:

1. **Prerequisites**: Communication, Exception, Infrastructure, Application projects upgraded to net10.0

2. **Update Target Framework**:
   - File: `src\DematecStock.Api\DematecStock.Api.csproj`
   - Change: `<TargetFramework>net8.0</TargetFramework>` → `<TargetFramework>net10.0</TargetFramework>`

3. **Package Updates**:

| Package | Current Version | Target Version | Reason |
|---------|----------------|----------------|--------|
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.22 | 10.0.4 | Framework compatibility (.NET 10 requires ASP.NET Core 10) |

**No update needed**:
- Swashbuckle.AspNetCore 6.6.2: ✅ Compatible with net10.0

4. **Expected Breaking Changes**:

**Category: JWT Bearer Authentication Setup** (7 instances - Source Incompatible)

**Affected APIs**:
- `JwtBearerDefaults` type (2 instances)
- `JwtBearerDefaults.AuthenticationScheme` field (2 instances)
- `JwtBearerOptions.TokenValidationParameters` property (1 instance)
- `JwtBearerExtensions` type (1 instance)
- `JwtBearerExtensions.AddJwtBearer()` method (1 instance)

**Status**: Source incompatible (namespace or assembly changes likely)

**Files Likely Affected**:
- `Program.cs` or `Startup.cs` (ASP.NET Core DI configuration)
- Authentication configuration code

**Potential Issues**:
1. **Namespace changes**: `Microsoft.AspNetCore.Authentication.JwtBearer` namespace may have changed
2. **API signature changes**: `AddJwtBearer()` extension method may have new overloads or parameter changes
3. **TokenValidationParameters**: Configuration properties may have changed

**Expected Fixes**:
```csharp
// BEFORE (net8.0):
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters { ... };
    });

// AFTER (net10.0) - likely similar, but may need namespace updates:
using Microsoft.AspNetCore.Authentication.JwtBearer; // Ensure correct namespace

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters { ... };
    });
```

**Category: Service Registration** (1 instance - Binary Incompatible)
- **API**: `ServiceCollectionExtensions` (generic type)
- **Impact**: Minimal - likely internal framework change
- **Fix**: Usually no code changes needed, recompilation sufficient

5. **Code Modifications**:

**High Priority**:
- Review `Program.cs` or `Startup.cs` authentication setup
- Verify `JwtBearerDefaults.AuthenticationScheme` still accessible
- Confirm `AddJwtBearer()` extension method signature matches usage
- Test authentication middleware registration

**Medium Priority**:
- Review controller authorization attributes (should be unaffected)
- Verify Swagger/OpenAPI JWT configuration still works
- Test CORS configuration if used with JWT

**Areas to Review**:
- `builder.Services.AddAuthentication()` calls
- `builder.Services.AddAuthorization()` calls (if used)
- JWT token validation configuration
- Swagger security definitions

6. **Testing Strategy**:
   - Build project after updates
   - Verify application starts without errors
   - Test JWT-protected endpoints:
     - Request token from authentication endpoint
     - Use token to access protected endpoint
     - Verify unauthorized access blocked
   - Test Swagger UI (if JWT configured in Swagger)
   - Smoke test all API endpoints

7. **Validation Checklist**:
   - [ ] Project file updated to net10.0
   - [ ] 1 package updated to version 10.0.4
   - [ ] `dotnet restore` succeeds
   - [ ] Project builds: `dotnet build src\DematecStock.Api\DematecStock.Api.csproj`
   - [ ] 0 errors, 0 warnings
   - [ ] Application starts: `dotnet run --project src\DematecStock.Api\DematecStock.Api.csproj`
   - [ ] Swagger UI loads (if configured)
   - [ ] JWT authentication works:
     - [ ] Can obtain JWT token
     - [ ] Can access protected endpoint with valid token
     - [ ] Unauthorized without token (401)
   - [ ] All controllers respond correctly
   - [ ] No runtime errors in authentication middleware

## Package Update Reference

### Summary Table

| Package | Current Version | Target Version | Projects Affected | Update Reason |
|---------|----------------|----------------|-------------------|---------------|
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.22 | 10.0.4 | 1 (Api) | Framework compatibility - ASP.NET Core 10 required for .NET 10 |
| Microsoft.EntityFrameworkCore | 9.0.11 | 10.0.4 | 1 (Infrastructure) | Framework compatibility - EF Core 10 required for .NET 10 |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.11 | 10.0.4 | 1 (Infrastructure) | Must match EF Core version |
| Microsoft.Extensions.Configuration.Binder | 10.0.1 | 10.0.4 | 1 (Infrastructure) | Patch update for .NET 10 compatibility |
| Microsoft.Extensions.DependencyInjection | 9.0.11 | 10.0.4 | 1 (Application) | Framework compatibility - .NET 10 requires matching version |

### Compatible Packages (No Update Needed)

| Package | Current Version | Projects | Compatibility Notes |
|---------|----------------|----------|---------------------|
| AutoMapper | 14.0.0 | Application | ✅ Compatible with net10.0 |
| BCrypt.Net-Next | 4.0.3 | Infrastructure | ✅ Compatible with net10.0 |
| DocumentFormat.OpenXml | 3.4.1 | Domain | ✅ Compatible with net10.0 |
| Swashbuckle.AspNetCore | 6.6.2 | Api | ✅ Compatible with net10.0 |
| System.IdentityModel.Tokens.Jwt | 8.15.0 | Infrastructure | ✅ Compatible with net10.0 |

### Detailed Package Updates

#### Microsoft.AspNetCore.Authentication.JwtBearer
- **Project**: DematecStock.Api
- **Current**: 8.0.22
- **Target**: 10.0.4
- **Reason**: ASP.NET Core authentication packages must match framework version
- **Breaking Changes**: Source incompatible APIs (see §Breaking Changes Catalog)
- **Impact**: High - JWT authentication configuration requires review
- **Testing**: Validate token generation, authentication middleware, authorization

#### Microsoft.EntityFrameworkCore
- **Project**: DematecStock.Infrastructure
- **Current**: 9.0.11
- **Target**: 10.0.4
- **Reason**: EF Core major version must align with .NET version
- **Breaking Changes**: Minimal for typical usage patterns
- **Impact**: Medium - Database context and migrations
- **Testing**: Verify migrations compatible, database operations work

#### Microsoft.EntityFrameworkCore.SqlServer
- **Project**: DematecStock.Infrastructure
- **Current**: 9.0.11
- **Target**: 10.0.4
- **Reason**: Must match EF Core version
- **Breaking Changes**: None expected (provider update)
- **Impact**: Low - SQL Server provider
- **Testing**: Verify database connections, query execution

#### Microsoft.Extensions.Configuration.Binder
- **Project**: DematecStock.Infrastructure
- **Current**: 10.0.1
- **Target**: 10.0.4
- **Reason**: Patch update for bug fixes and .NET 10 alignment
- **Breaking Changes**: `ConfigurationBinder.GetValue<T>()` binary incompatible
- **Impact**: Medium - Configuration reading
- **Testing**: Verify settings/connection string binding works

#### Microsoft.Extensions.DependencyInjection
- **Project**: DematecStock.Application
- **Current**: 9.0.11
- **Target**: 10.0.4
- **Reason**: DI container must match framework version
- **Breaking Changes**: Minimal (stable API)
- **Impact**: Low - Service registration
- **Testing**: Verify DI container resolves services correctly

### Update Sequence

All packages updated simultaneously as part of the All-At-Once strategy:

1. Update project files to net10.0
2. Update all 5 package references in their respective project files
3. Run `dotnet restore` for entire solution
4. Build solution to identify breaking changes
5. Fix breaking changes
6. Rebuild and verify

### Package Update Commands

```bash
# Infrastructure project
dotnet add src/DematecStock.Infrastructure/DematecStock.Infrastructure.csproj package Microsoft.EntityFrameworkCore --version 10.0.4
dotnet add src/DematecStock.Infrastructure/DematecStock.Infrastructure.csproj package Microsoft.EntityFrameworkCore.SqlServer --version 10.0.4
dotnet add src/DematecStock.Infrastructure/DematecStock.Infrastructure.csproj package Microsoft.Extensions.Configuration.Binder --version 10.0.4

# Application project
dotnet add src/DematecStock.Application/DematecStock.Application.csproj package Microsoft.Extensions.DependencyInjection --version 10.0.4

# Api project
dotnet add src/DematecStock.Api/DematecStock.Api.csproj package Microsoft.AspNetCore.Authentication.JwtBearer --version 10.0.4
```

**Alternative**: Manually edit .csproj files (faster for All-At-Once):
```xml
<!-- Update PackageReference Version attributes -->
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.4" />
```

## Breaking Changes Catalog

### Overview

| Category | Count | Severity | Affected Projects |
|----------|-------|----------|-------------------|
| Binary Incompatible | 8 | 🔴 High | Api (1), Infrastructure (6), Application (1) |
| Source Incompatible | 7 | 🟡 Medium | Api (7) |
| Behavioral Change | 0 | 🟢 Low | None |

### Binary Incompatible APIs (Require Code Changes)

#### 1. Configuration Binding - `ConfigurationBinder.GetValue<T>()`

**Affected Projects**: Infrastructure (3 instances), Application (1 instance)

**API**: `Microsoft.Extensions.Configuration.ConfigurationBinder.GetValue<T>(IConfiguration, string)`

**Issue**: Binary incompatible - method signature or behavior changed

**Typical Usage**:
```csharp
// Current usage (net8.0):
var connectionString = configuration.GetValue<string>("ConnectionStrings:Default");
var maxRetries = configuration.GetValue<int>("Settings:MaxRetries");
```

**Potential Fixes**:
1. **Explicit null handling**:
```csharp
var connectionString = configuration.GetValue<string>("ConnectionStrings:Default", defaultValue: string.Empty);
```

2. **Use IOptions<T> pattern** (recommended):
```csharp
// appsettings.json:
{
  "ConnectionStrings": { "Default": "..." },
  "Settings": { "MaxRetries": 3 }
}

// Configuration class:
public class AppSettings { public int MaxRetries { get; set; } }

// Startup/Program.cs:
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Settings"));

// Usage in service:
public MyService(IOptions<AppSettings> options) {
    var maxRetries = options.Value.MaxRetries;
}
```

3. **Direct indexer access**:
```csharp
var connectionString = configuration["ConnectionStrings:Default"];
```

**Files to Review**: Any file using `IConfiguration.GetValue<T>()` - likely in:
- DI setup/Startup code
- Repository constructors
- Service constructors

---

#### 2. JWT Token Generation - `JwtSecurityTokenHandler`

**Affected Projects**: Infrastructure (4 instances)

**APIs**:
- `System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler` (constructor)
- `JwtSecurityTokenHandler.CreateToken(SecurityTokenDescriptor)`
- `JwtSecurityTokenHandler.WriteToken(SecurityToken)`

**Issue**: Binary incompatible - class or method signatures changed

**Typical Usage**:
```csharp
// Current usage (net8.0):
var tokenHandler = new JwtSecurityTokenHandler();
var tokenDescriptor = new SecurityTokenDescriptor {
    Subject = new ClaimsIdentity(claims),
    Expires = DateTime.UtcNow.AddHours(1),
    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
};
var token = tokenHandler.CreateToken(tokenDescriptor);
var tokenString = tokenHandler.WriteToken(token);
```

**Potential Changes in .NET 10**:
- Constructor parameters may have changed
- `CreateToken()` may return different type
- `WriteToken()` signature may have changed

**Expected Fix** (pending verification):
```csharp
// Likely minimal changes, but verify:
var tokenHandler = new JwtSecurityTokenHandler();
var token = tokenHandler.CreateToken(tokenDescriptor); // May need type cast
var tokenString = tokenHandler.WriteToken(token);
```

**Files to Review**:
- JWT token generation service (likely `TokenService.cs`, `JwtTokenGenerator.cs`, or similar in Infrastructure project)
- Authentication-related services

**Testing Required**:
- Generate token successfully
- Validate token structure (claims, expiration)
- Verify token can be validated by authentication middleware

---

#### 3. Service Collection Extensions - `ServiceCollectionExtensions`

**Affected Projects**: Api (1 instance)

**API**: `Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions`

**Issue**: Binary incompatible - generic type or extension methods changed

**Impact**: Low - likely internal framework change, recompilation usually sufficient

**Typical Usage**: Implicit via `builder.Services.AddXxx()` calls

**Expected Fix**: Usually no code changes, just rebuild

---

### Source Incompatible APIs (Namespace/Assembly Changes)

#### 4. JWT Bearer Authentication Configuration

**Affected Projects**: Api (7 instances total)

**APIs**:
- `Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults` (2 instances)
- `JwtBearerDefaults.AuthenticationScheme` (2 instances)
- `JwtBearerOptions.TokenValidationParameters` (1 instance)
- `Microsoft.Extensions.DependencyInjection.JwtBearerExtensions` (1 instance)
- `JwtBearerExtensions.AddJwtBearer()` (1 instance)

**Issue**: Source incompatible - types moved to different namespace or assembly

**Typical Usage**:
```csharp
// Current usage (net8.0) in Program.cs or Startup.cs:
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"])
            )
        };
    });
```

**Potential Issues**:
1. **Namespace change**: `Microsoft.AspNetCore.Authentication.JwtBearer` may have changed
2. **Assembly reference**: May need to add explicit package reference
3. **API signature**: `AddJwtBearer()` overload may have changed

**Expected Fix**:
```csharp
// Verify namespace still correct:
using Microsoft.AspNetCore.Authentication.JwtBearer;

// If namespace changed, use new namespace (check compilation errors)
// Code structure likely remains similar:
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters { /* same */ };
    });
```

**Files to Review**:
- `Program.cs` (ASP.NET Core 6+ minimal hosting)
- OR `Startup.cs` (if using traditional startup)
- Authentication configuration code

**Testing Required**:
- Application starts without errors
- JWT authentication middleware registered
- Protected endpoints require valid JWT token
- Invalid/missing tokens return 401 Unauthorized

---

### Summary of Required Actions

| Priority | Action | Projects | Expected Effort |
|----------|--------|----------|----------------|
| 🔴 High | Review JWT Bearer authentication setup | Api | Low - likely just namespace fix |
| 🔴 High | Review JWT token generation code | Infrastructure | Low-Medium - verify API usage |
| 🟡 Medium | Review configuration binding calls | Infrastructure, Application | Low - switch to IOptions or add defaults |
| 🟢 Low | Rebuild and verify ServiceCollection usage | Api | Minimal - recompilation likely sufficient |

### Validation Strategy

After applying fixes:
1. **Build Solution**: `dotnet build` - must succeed with 0 errors
2. **Start Api**: Application must start without runtime errors
3. **Test JWT Flow**:
   - Request token from auth endpoint
   - Access protected endpoint with token (success)
   - Access protected endpoint without token (401)
4. **Test Configuration**: Verify settings loaded correctly (connection strings, app settings)
5. **Test Database**: Verify EF Core operations work (if integration tests exist)

### Reference Documentation

- **.NET 10 Breaking Changes**: https://learn.microsoft.com/en-us/dotnet/core/compatibility/10.0
- **ASP.NET Core 10 Breaking Changes**: https://learn.microsoft.com/en-us/aspnet/core/migration/90-to-100
- **EF Core 10 Breaking Changes**: https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-10.0/breaking-changes

## Risk Management

### High-Risk Changes

| Project | Risk Level | Description | Mitigation |
|---------|------------|-------------|------------|
| DematecStock.Api | 🟡 Medium | 7 source incompatible APIs (JWT Bearer authentication) | Pre-identify breaking changes, test authentication flows thoroughly |
| DematecStock.Infrastructure | 🟡 Medium | 6 binary incompatible APIs (JWT token generation, configuration binding) | Review JWT token handler usage, validate configuration binding patterns |
| All Projects | 🟢 Low | Framework version change net8.0 → net10.0 | Standard upgrade path, well-documented |

### Security Vulnerabilities

✅ **No security vulnerabilities identified** in the assessment.

### Technology-Specific Risks

#### IdentityModel & JWT Bearer Authentication
**Affected Projects**: Api, Infrastructure  
**Issue Count**: 4 APIs flagged (26.7% of API issues)  
**Risk Level**: Medium

**Affected APIs**:
- `JwtBearerDefaults.AuthenticationScheme` (source incompatible)
- `JwtBearerOptions.TokenValidationParameters` (source incompatible)
- `JwtBearerExtensions.AddJwtBearer` (source incompatible)
- `JwtSecurityTokenHandler` class and methods (binary incompatible)

**Mitigation**:
- Review Microsoft.AspNetCore.Authentication.JwtBearer 10.0.4 migration guide
- Test authentication flows end-to-end after upgrade
- Validate token generation and validation logic
- Confirm `TokenValidationParameters` configuration still valid

#### Configuration Binding
**Affected Projects**: Infrastructure  
**Issue Count**: 3 APIs (`ConfigurationBinder.GetValue<T>`)  
**Risk Level**: Low-Medium

**Mitigation**:
- Review all `IConfiguration.GetValue<T>()` calls
- Consider migrating to `IOptions<T>` pattern if not already using
- Test configuration loading during startup

### Contingency Plans

#### If Build Fails After Framework Update
**Scenario**: Compilation errors after updating TargetFramework to net10.0

**Actions**:
1. Review errors against §Breaking Changes Catalog
2. Apply fixes incrementally (one API at a time)
3. If blocked, consult .NET 10 breaking changes documentation: https://learn.microsoft.com/en-us/dotnet/core/compatibility/10.0
4. If unresolvable, revert branch and investigate specific issue in isolation

#### If JWT Authentication Breaks
**Scenario**: Token generation or validation fails after package update

**Actions**:
1. Compare `JwtSecurityTokenHandler` usage before/after
2. Review `TokenValidationParameters` configuration
3. Check for namespace changes in Microsoft.AspNetCore.Authentication.JwtBearer 10.0.4
4. Test with debugger to identify exact failure point
5. If needed, temporarily revert to net8.0 compatible packages to isolate issue

#### If Tests Fail
**Scenario**: Existing tests fail after upgrade

**Actions**:
1. Isolate failures by test category (unit vs integration)
2. Review test failures for framework-specific issues vs business logic issues
3. Update test assertions if .NET 10 changes behavior (e.g., exception messages)
4. If widespread failures, validate foundation projects (Communication, Domain, Exception) first

#### If Performance Degrades
**Scenario**: Application slower after upgrade

**Actions**:
1. Profile application to identify bottleneck
2. Review .NET 10 performance changes (usually improvements)
3. Check for unintended configuration changes
4. Measure specific endpoints/operations pre/post upgrade

### Rollback Plan

**Trigger Conditions**:
- Unable to resolve compilation errors within reasonable timeframe
- Critical authentication/authorization failures in production-like environment
- Unacceptable performance degradation
- Test failure rate > 50%

**Rollback Steps**:
1. Revert branch `upgrade-to-NET10` to commit before upgrade started
2. Or: `git reset --hard <pre-upgrade-commit-sha>`
3. Rebuild solution on net8.0
4. Document blocking issues for investigation
5. Plan targeted fixes before retry

## Testing & Validation Strategy

### Multi-Level Testing Approach

#### Level 1: Per-Project Build Validation

After updating all project files and packages (atomic operation), validate each project builds independently:

**Foundation Projects** (validate first):
```bash
dotnet build src/DematecStock.Communication/DematecStock.Communication.csproj
dotnet build src/DematecStock.Domain/DematecStock.Domain.csproj
dotnet build src/DematecStock.Exception/DematecStock.Exception.csproj
```

**Expected Outcome**: 0 errors, 0 warnings

**Business Logic Projects** (validate second):
```bash
dotnet build src/DematecStock.Infrastructure/DematecStock.Infrastructure.csproj
dotnet build src/DematecStock.Application/DematecStock.Application.csproj
```

**Expected Outcome**: 0 errors, 0 warnings  
**If Errors**: Review JWT token generation (Infrastructure) and configuration binding (both)

**Presentation Project** (validate last):
```bash
dotnet build src/DematecStock.Api/DematecStock.Api.csproj
```

**Expected Outcome**: 0 errors, 0 warnings  
**If Errors**: Review JWT Bearer authentication setup in Program.cs

---

#### Level 2: Solution-Wide Build Validation

```bash
dotnet build DematecStock.sln
```

**Expected Outcome**:
- All 6 projects build successfully
- 0 errors
- 0 warnings (ideal, minimal warnings acceptable if unrelated to upgrade)
- No dependency resolution conflicts

**Validation Checklist**:
- [ ] All projects target `net10.0`
- [ ] All package references resolved
- [ ] No transitive dependency conflicts
- [ ] Build output confirms net10.0 assemblies created

---

#### Level 3: Runtime Validation

**Start Application**:
```bash
cd src/DematecStock.Api
dotnet run
```

**Expected Outcome**:
- Application starts without errors
- Kestrel server listening on configured ports
- No exceptions during startup
- Swagger UI loads (if configured)

**Startup Validation Checklist**:
- [ ] Application starts (no crash)
- [ ] No exceptions in console output
- [ ] DI container resolves all services
- [ ] EF Core database context initializes
- [ ] JWT authentication middleware registered
- [ ] Swagger UI accessible (if configured): https://localhost:<port>/swagger

---

#### Level 4: Functional Testing

**JWT Authentication Flow**:

1. **Obtain Token** (if auth endpoint exists):
   ```bash
   curl -X POST https://localhost:<port>/api/auth/login \
     -H "Content-Type: application/json" \
     -d '{"username":"testuser","password":"testpass"}'
   ```
   - [ ] Receives JWT token
   - [ ] Token structure valid (header.payload.signature)
   - [ ] Token contains expected claims

2. **Access Protected Endpoint WITH Token**:
   ```bash
   curl -X GET https://localhost:<port>/api/products \
     -H "Authorization: Bearer <token>"
   ```
   - [ ] Returns 200 OK
   - [ ] Returns expected data

3. **Access Protected Endpoint WITHOUT Token**:
   ```bash
   curl -X GET https://localhost:<port>/api/products
   ```
   - [ ] Returns 401 Unauthorized
   - [ ] Proper error response

**Database Operations**:
- [ ] Can connect to SQL Server
- [ ] Can query entities (GET endpoints)
- [ ] Can create entities (POST endpoints)
- [ ] Can update entities (PUT/PATCH endpoints)
- [ ] Can delete entities (DELETE endpoints)

**Smoke Test All Controllers**:
- [ ] Products endpoints respond
- [ ] Warehouse locations endpoints respond
- [ ] Stock operations endpoints respond
- [ ] All CRUD operations work
- [ ] Validation errors return correctly
- [ ] Exception handling works (custom exception middleware)

---

#### Level 5: Unit/Integration Tests

**If test projects exist**:

```bash
dotnet test
```

**Expected Outcome**:
- All tests pass
- No new test failures introduced by upgrade
- Test coverage maintained

**If Tests Exist, Validate**:
- [ ] Unit tests for Application layer (use cases) pass
- [ ] Integration tests for Infrastructure layer (repositories) pass
- [ ] API integration tests pass (if they exist)
- [ ] No tests skipped unexpectedly

**If Tests Fail**:
1. Isolate failure by test category
2. Determine if failure is framework-related or business logic
3. Update test assertions if .NET 10 changed behavior (e.g., exception messages)
4. Fix any legitimate bugs exposed by upgrade

---

### Phase-Based Testing (All-At-Once Strategy)

Since all projects upgraded atomically, testing is also unified:

**Phase 1: Atomic Upgrade Validation**

| Test Level | Focus | Success Criteria |
|------------|-------|------------------|
| Per-Project Build | Individual project compilation | 6/6 projects build, 0 errors |
| Solution Build | Dependency resolution | Solution builds, no conflicts |
| Runtime Start | Application initialization | Api starts without errors |
| JWT Auth | Authentication flow | Token generation and validation work |
| Database | EF Core operations | CRUD operations succeed |
| Smoke Test | All endpoints | All controllers respond correctly |
| Unit Tests | Business logic | All tests pass (if tests exist) |

**Completion Criteria**: All test levels pass before considering upgrade complete.

---

### Regression Testing Focus Areas

**Configuration**:
- [ ] Connection strings loaded correctly
- [ ] App settings accessible
- [ ] Environment-specific configuration works (Development, Production)

**Authentication/Authorization**:
- [ ] JWT tokens generated correctly
- [ ] Token validation works
- [ ] Authorization policies enforced
- [ ] Role-based access control works (if implemented)

**Entity Framework**:
- [ ] DbContext initialization
- [ ] Database connections
- [ ] Query execution
- [ ] Change tracking
- [ ] Migrations compatible (don't apply, just verify)

**Dependency Injection**:
- [ ] All services resolve
- [ ] Scoped lifetime works (per request)
- [ ] Singleton services maintain state
- [ ] Transient services created correctly

**AutoMapper**:
- [ ] Profiles register correctly
- [ ] Mappings work (DTO ↔ Entity)
- [ ] No unmapped property errors

**OpenXml (DocumentFormat.OpenXml)**:
- [ ] Excel export works (if feature exists)
- [ ] File generation successful

**BCrypt**:
- [ ] Password hashing works
- [ ] Password verification works

---

### Performance Validation

**Basic Performance Checks**:
- [ ] Application startup time reasonable (compare to net8.0 if possible)
- [ ] Response times for endpoints acceptable
- [ ] No obvious memory leaks (monitor during testing)

**Note**: .NET 10 typically includes performance improvements. Regression unlikely but validate if critical.

---

### Testing Tools

**Required**:
- `dotnet` CLI (build, run, test)
- Web browser (Swagger UI)
- HTTP client (curl, Postman, Thunder Client, or browser dev tools)

**Optional**:
- SQL Server Management Studio (database validation)
- Performance profiler (if performance concerns)
- Integration test framework (if tests exist: xUnit, NUnit, MSTest)

---

### Testing Timeline

Given All-At-Once strategy, testing is sequential:

1. **Immediate** (after atomic upgrade): Build validation (5-10 minutes)
2. **Short-term** (after build succeeds): Runtime and functional testing (15-30 minutes)
3. **Medium-term** (after functional tests pass): Full regression testing (30-60 minutes)
4. **Long-term** (before merge): Integration tests and smoke testing in staging environment (if applicable)

**Total Testing Effort**: Low complexity (1-2 hours for comprehensive validation)

## Complexity & Effort Assessment

### Per-Project Complexity

| Project | Complexity | Dependencies | Risk | LOC Impact | Rationale |
|---------|------------|--------------|------|------------|-----------|
| DematecStock.Communication | 🟢 Low | 0 | Low | 0+ | No package updates, no API issues, simple DTO project |
| DematecStock.Domain | 🟢 Low | 0 | Low | 0+ | No package updates, no API issues, domain models only |
| DematecStock.Exception | 🟢 Low | 0 | Low | 0+ | No package updates, no API issues, minimal code (72 LOC) |
| DematecStock.Infrastructure | 🟡 Low-Medium | 2 | Medium | 6+ | 3 package updates, 6 binary incompatible APIs (JWT, config) |
| DematecStock.Application | 🟢 Low | 3 | Low | 1+ | 1 package update, 1 binary incompatible API (config) |
| DematecStock.Api | 🟡 Low-Medium | 4 | Medium | 8+ | 1 package update, 8 API issues (JWT Bearer authentication) |

### Phase Complexity Assessment

Since this is an All-At-Once strategy, there's only one phase:

**Phase 1: Atomic Upgrade**
- **Overall Complexity**: 🟢 Low
- **Total Projects**: 6
- **Total Package Updates**: 5
- **Total API Issues**: 15 (8 binary + 7 source incompatible)
- **Dependency Ordering**: Clear 3-level hierarchy, no cycles
- **Main Challenge**: JWT Bearer authentication APIs (concentrated in Api and Infrastructure)

### Relative Effort by Activity

| Activity | Complexity | Notes |
|----------|------------|-------|
| Project File Updates | 🟢 Minimal | Simple find/replace: `<TargetFramework>net8.0</TargetFramework>` → `<TargetFramework>net10.0</TargetFramework>` in 6 files |
| Package Updates | 🟢 Low | 5 packages, all with known net10.0 versions, no major version jumps |
| Breaking Change Fixes | 🟡 Low-Medium | 15 API issues, concentrated in 2 files (likely JWT and config related) |
| Testing | 🟢 Low | Small codebase (1,651 LOC), well-structured projects |
| Documentation | 🟢 Minimal | Update deployment docs if needed |

### Resource Requirements

**Skills Required**:
- ✅ Familiarity with .NET project file structure (SDK-style)
- ✅ Understanding of NuGet package management
- ⚠️ **Critical**: Experience with ASP.NET Core JWT Bearer authentication
- ✅ Basic understanding of dependency injection and configuration patterns
- ✅ Ability to read compilation errors and breaking change documentation

**Parallel Capacity**:
- **Not applicable** - All-At-Once strategy is a single atomic operation
- One person can complete the entire upgrade
- Alternatively, could split validation: one person validates foundation projects (Communication, Domain, Exception), another validates business logic (Application, Infrastructure, Api)

**Tools Needed**:
- .NET 10 SDK installed ✅ (validated during assessment)
- IDE with .NET 10 support (Visual Studio 2022 17.12+, VS Code, Rider)
- Git for version control ✅ (already configured)

### Estimated Scope

**Total Estimated LOC Changes**: 15+ lines (0.9% of 1,651 LOC codebase)

**Breakdown by Project**:
- Communication: 0+ LOC (framework change only)
- Domain: 0+ LOC (framework change only)
- Exception: 0+ LOC (framework change only)
- Infrastructure: 6+ LOC (JWT token handler, config binding)
- Application: 1+ LOC (config binding)
- Api: 8+ LOC (JWT Bearer authentication setup)

**Note**: These are minimum estimates based on API usage. Actual changes may be higher depending on:
- Conditional compilation directives (unlikely in modern .NET)
- Framework-specific workarounds in current code
- Test code adjustments (not included in LOC estimates above)

## Source Control Strategy

### Branching Strategy

**Current Setup**:
- **Main Branch**: `master`
- **Source Branch**: `master` (starting point for upgrade)
- **Upgrade Branch**: `upgrade-to-NET10` ✅ (already created and checked out)

**Branch Flow**:
```
master (net8.0)
  └─ upgrade-to-NET10 (work branch)
       └─ [atomic upgrade commits]
       └─ merge back to master → (net10.0)
```

**Branch Protection** (recommended):
- Keep `master` stable
- All upgrade work on `upgrade-to-NET10`
- Merge to `master` only after all validation passes

---

### Commit Strategy (All-At-Once Approach)

#### Option 1: Single Atomic Commit (Recommended for All-At-Once)

**Structure**:
```
upgrade-to-NET10
  └─ Commit 1: "chore: upgrade solution from .NET 8.0 to .NET 10.0"
       - All project file updates (6 files)
       - All package reference updates (5 packages)
       - All breaking change fixes
       - Build verification passed
```

**Advantages**:
- ✅ True atomic upgrade (matches All-At-Once strategy)
- ✅ Easy rollback (single commit revert)
- ✅ Clear history (one upgrade = one commit)
- ✅ Fast forward merge possible

**Commit Message Template**:
```
chore: upgrade solution from .NET 8.0 to .NET 10.0

- Update all projects to net10.0 target framework
- Update Microsoft.EntityFrameworkCore 9.0.11 → 10.0.4
- Update Microsoft.EntityFrameworkCore.SqlServer 9.0.11 → 10.0.4
- Update Microsoft.Extensions.Configuration.Binder 10.0.1 → 10.0.4
- Update Microsoft.Extensions.DependencyInjection 9.0.11 → 10.0.4
- Update Microsoft.AspNetCore.Authentication.JwtBearer 8.0.22 → 10.0.4
- Fix JWT Bearer authentication configuration (namespace updates)
- Fix JWT token generation API changes
- Fix configuration binding API changes

Projects upgraded:
- DematecStock.Api
- DematecStock.Application
- DematecStock.Communication
- DematecStock.Domain
- DematecStock.Exception
- DematecStock.Infrastructure

Validation:
- All projects build successfully (0 errors, 0 warnings)
- Application starts without errors
- JWT authentication flow verified
- All API endpoints tested
```

---

#### Option 2: Phased Commits (Alternative if needed)

If compilation errors require iterative debugging, use staged commits:

```
upgrade-to-NET10
  └─ Commit 1: "chore: update project files to net10.0"
  └─ Commit 2: "chore: update NuGet packages for net10.0 compatibility"
  └─ Commit 3: "fix: resolve JWT Bearer authentication breaking changes"
  └─ Commit 4: "fix: resolve configuration binding breaking changes"
  └─ Commit 5: "test: verify all projects build and tests pass"
```

**When to Use**:
- Compilation errors require investigation between steps
- Need to isolate specific breaking change fixes
- Want granular history for troubleshooting

**Before Merge**: Squash commits into single commit (keep clean history)

---

### Commit Frequency

**During Atomic Upgrade**:
- Prefer single commit for entire upgrade
- If blocked by errors, commit working progress to save state
- Intermediate commits allowed but squash before merge

**Checkpoint Commits** (optional):
```bash
# Save progress if debugging complex issue:
git add .
git commit -m "WIP: debugging JWT authentication issue"

# Continue work...

# Squash before merge:
git rebase -i master  # Combine all commits into one
```

---

### Review and Merge Process

#### Pre-Merge Checklist

Before creating pull request or merging `upgrade-to-NET10` → `master`:

**Technical Validation**:
- [ ] All 6 projects build successfully
- [ ] 0 compilation errors
- [ ] 0 warnings (or documented/acceptable warnings)
- [ ] All unit tests pass (if tests exist)
- [ ] Application starts without errors
- [ ] JWT authentication works end-to-end
- [ ] All API endpoints respond correctly
- [ ] Database operations work (EF Core)
- [ ] Configuration loading works

**Code Quality**:
- [ ] No commented-out code left behind
- [ ] No debug/temporary code
- [ ] Code formatting consistent
- [ ] Breaking change fixes are clean and idiomatic

**Documentation**:
- [ ] Commit message(s) clear and descriptive
- [ ] Upgrade plan.md reflects actual changes made
- [ ] Any deviations from plan documented
- [ ] README updated if deployment steps changed

---

#### Pull Request (Recommended)

**Title**: `Upgrade solution from .NET 8.0 to .NET 10.0`

**Description Template**:
```markdown
## Overview
Upgrades entire DematecStock solution from .NET 8.0 to .NET 10.0 (LTS) using All-At-Once strategy.

## Changes
- **Projects**: All 6 projects updated to `net10.0`
- **Packages**: 5 packages updated to version 10.0.4
- **Breaking Changes**: JWT authentication and configuration binding APIs updated

## Testing
- ✅ All projects build (0 errors)
- ✅ Application starts successfully
- ✅ JWT authentication flow verified
- ✅ All API endpoints tested
- ✅ Database operations validated
- ✅ [Unit tests pass] (if applicable)

## Migration Guide
See `.github/upgrades/scenarios/new-dotnet-version_2b6976/plan.md` for detailed plan.

## Rollback Plan
If issues found in production:
- Revert merge commit: `git revert <merge-sha>`
- Or checkout previous release tag

## Reviewers
@team-lead @backend-dev
```

**Review Criteria**:
- Code reviewer validates breaking change fixes
- QA validates functional testing
- Technical lead approves strategy

---

#### Direct Merge (Alternative)

If no PR process or small team:

```bash
# Ensure all changes committed on upgrade-to-NET10
git checkout upgrade-to-NET10
git status  # Should be clean

# Switch to master and merge
git checkout master
git merge upgrade-to-NET10 --no-ff -m "Merge: Upgrade solution from .NET 8.0 to .NET 10.0"

# Push to remote
git push origin master
```

**`--no-ff` flag**: Creates merge commit even for fast-forward, preserves branch history

---

### Tag Release (Recommended)

After successful merge and deployment:

```bash
git checkout master
git tag -a v2.0.0-net10 -m "Release: .NET 10.0 upgrade"
git push origin v2.0.0-net10
```

**Versioning**:
- Major version bump if significant changes (2.0.0)
- Minor version bump if incremental (1.5.0)
- Include .NET version in tag for clarity

---

### Post-Merge Cleanup

```bash
# Optional: Delete upgrade branch after successful merge
git branch -d upgrade-to-NET10         # Delete local
git push origin --delete upgrade-to-NET10  # Delete remote
```

**When to Keep Branch**:
- Keep temporarily if monitoring production for issues
- Delete after stable period (1-2 weeks)

---

### Rollback Scenarios

#### Scenario 1: Rollback Before Merge

**Issue found during development**:
```bash
# Discard all changes and start over:
git checkout master
git branch -D upgrade-to-NET10
git checkout -b upgrade-to-NET10

# Or reset branch to specific commit:
git checkout upgrade-to-NET10
git reset --hard <good-commit-sha>
```

---

#### Scenario 2: Rollback After Merge

**Issue found after merge to master**:

**Option A: Revert merge commit**:
```bash
git checkout master
git revert -m 1 <merge-commit-sha>  # Creates new commit that undoes merge
git push origin master
```

**Option B: Hard reset** (if no one else pulled):
```bash
git checkout master
git reset --hard <commit-before-merge>
git push origin master --force  # ⚠️ Dangerous if others pulled
```

**Option C: Cherry-pick fixes** (if minor issues):
```bash
# Stay on net10.0, fix issues with additional commits
git checkout master
# Fix issue...
git add .
git commit -m "fix: resolve JWT issue found in production"
git push origin master
```

---

### Git Best Practices for This Upgrade

✅ **DO**:
- Commit working state before risky changes
- Write descriptive commit messages
- Test thoroughly before merging
- Use `--no-ff` for merge to preserve history
- Tag releases after successful deployment

❌ **DON'T**:
- Force push to `master` (unless emergency and coordinated)
- Commit broken code to `master`
- Merge without validation
- Leave WIP commits in final history (squash them)
- Skip testing before merge

---

### Coordination with Team

**If multiple developers**:
1. Announce upgrade start in team chat
2. Request other developers hold off on merges to `master` during upgrade
3. Create PR for review before merge
4. Announce when upgrade complete and `master` is stable
5. Team members pull latest `master` and verify on their machines

**If solo developer**:
- Still use branching for safety
- Still test thoroughly before merge
- Still tag releases for easy rollback

## Success Criteria

### Technical Criteria

The migration is considered technically successful when ALL of the following are met:

#### 1. Project Target Frameworks
- [ ] All 6 projects target `net10.0` in their `.csproj` files
- [ ] No projects remain on `net8.0`
- [ ] No multi-targeting (single `<TargetFramework>`, not `<TargetFrameworks>`)

#### 2. Package Updates
- [ ] Microsoft.AspNetCore.Authentication.JwtBearer: 8.0.22 → 10.0.4 ✅
- [ ] Microsoft.EntityFrameworkCore: 9.0.11 → 10.0.4 ✅
- [ ] Microsoft.EntityFrameworkCore.SqlServer: 9.0.11 → 10.0.4 ✅
- [ ] Microsoft.Extensions.Configuration.Binder: 10.0.1 → 10.0.4 ✅
- [ ] Microsoft.Extensions.DependencyInjection: 9.0.11 → 10.0.4 ✅
- [ ] No package version conflicts
- [ ] No transitive dependency warnings

#### 3. Build Success
- [ ] `dotnet restore` completes without errors
- [ ] `dotnet build DematecStock.sln` succeeds
- [ ] All 6 projects build successfully
- [ ] **0 compilation errors**
- [ ] **0 warnings** (or only acceptable/unrelated warnings)
- [ ] Build output confirms net10.0 assemblies:
  - `bin/Debug/net10.0/DematecStock.*.dll`

#### 4. Breaking Changes Resolved
- [ ] JWT Bearer authentication configuration compiles
- [ ] JWT token generation code compiles
- [ ] Configuration binding calls (`GetValue<T>()`) compile
- [ ] No unresolved API incompatibilities
- [ ] All source incompatible API issues fixed

#### 5. Runtime Success
- [ ] Application starts: `dotnet run --project src/DematecStock.Api/DematecStock.Api.csproj`
- [ ] No startup exceptions
- [ ] Kestrel listens on configured port
- [ ] Dependency injection container resolves all services
- [ ] EF Core DbContext initializes successfully
- [ ] JWT authentication middleware registered
- [ ] Swagger UI loads (if configured): `/swagger`

#### 6. Functional Validation
- [ ] **JWT Authentication**:
  - Can generate JWT token
  - Token contains expected claims
  - Protected endpoints accessible with valid token
  - Protected endpoints return 401 without token
- [ ] **Database Operations**:
  - Can connect to SQL Server
  - Can query entities (SELECT)
  - Can insert entities (INSERT)
  - Can update entities (UPDATE)
  - Can delete entities (DELETE)
- [ ] **API Endpoints**:
  - All controllers respond correctly
  - CRUD operations work
  - Validation logic functions
  - Exception handling works (custom exceptions return correct status codes)
- [ ] **Configuration**:
  - Connection strings loaded
  - App settings accessible
  - Environment-specific configuration works

#### 7. Testing (if tests exist)
- [ ] All unit tests pass: `dotnet test`
- [ ] All integration tests pass
- [ ] No tests skipped unexpectedly
- [ ] Test coverage maintained or improved
- [ ] No new test failures introduced

#### 8. No Security Vulnerabilities
- [ ] No new security vulnerabilities introduced
- [ ] All packages free of known vulnerabilities
- [ ] Security best practices maintained

---

### Quality Criteria

#### Code Quality
- [ ] Code changes are clean and idiomatic
- [ ] No commented-out code or debug statements
- [ ] No temporary workarounds left in code
- [ ] Code formatting consistent with project standards
- [ ] No code duplication introduced

#### Documentation
- [ ] Commit messages clear and descriptive
- [ ] `plan.md` reflects actual changes made
- [ ] Any deviations from plan documented
- [ ] README.md updated if needed (deployment instructions, prerequisites)
- [ ] API documentation updated if endpoints changed

#### Performance
- [ ] Application startup time acceptable (similar to or better than net8.0)
- [ ] API response times acceptable (no significant degradation)
- [ ] Memory usage within normal range
- [ ] No obvious performance regressions

---

### Process Criteria

#### All-At-Once Strategy Followed
- [ ] All 6 projects upgraded simultaneously (no intermediate states)
- [ ] All package updates applied atomically
- [ ] Validation followed dependency order (leaf → mid-tier → root)
- [ ] No partial upgrades (e.g., some projects on net8.0, some on net10.0)

#### Source Control
- [ ] All changes committed to `upgrade-to-NET10` branch
- [ ] Commit message(s) follow template
- [ ] Single commit for entire upgrade (or squashed commits)
- [ ] Branch ready for merge to `master`
- [ ] Pre-merge checklist completed

#### Testing & Validation
- [ ] Multi-level testing completed:
  - [ ] Per-project build validation
  - [ ] Solution-wide build validation
  - [ ] Runtime validation
  - [ ] Functional testing
  - [ ] Regression testing
- [ ] Testing timeline followed
- [ ] All test levels passed

---

### Acceptance Criteria (Before Merge to Master)

**Mandatory**:
1. ✅ All technical criteria met (items 1-8 above)
2. ✅ Code quality criteria met
3. ✅ All validation tests passed
4. ✅ No known blocking issues

**Recommended**:
5. ✅ Pull request reviewed and approved (if using PR workflow)
6. ✅ Documentation updated
7. ✅ Team notified of upcoming merge

**Optional**:
8. ✅ Tested in staging environment (if available)
9. ✅ Performance benchmarks validated
10. ✅ Security scan passed

---

### Definition of Done

The upgrade is **DONE** when:

1. ✅ **Code Complete**: All projects on net10.0, all packages updated, all breaking changes fixed
2. ✅ **Build Clean**: Solution builds with 0 errors, 0 warnings
3. ✅ **Tests Pass**: All automated tests pass (if they exist)
4. ✅ **Functionally Validated**: JWT auth works, database works, API endpoints work
5. ✅ **Merged**: Changes merged to `master` branch
6. ✅ **Tagged**: Release tagged (e.g., `v2.0.0-net10`)
7. ✅ **Documented**: Plan.md finalized, commit messages clear
8. ✅ **Deployable**: Ready for deployment to staging/production

---

### Post-Upgrade Validation (Production/Staging)

After deployment to production or staging environment:

**Monitoring** (first 24-48 hours):
- [ ] Application starts successfully in production
- [ ] No exceptions in production logs
- [ ] API endpoints responding correctly
- [ ] Authentication flows working
- [ ] Database operations succeeding
- [ ] Response times acceptable
- [ ] No memory leaks
- [ ] Error rate within normal range

**User Acceptance**:
- [ ] End users can access application
- [ ] No reported functionality regressions
- [ ] Performance acceptable to users

**If Issues Found**:
- Assess severity (critical vs minor)
- Fix forward with hotfix commits
- Or rollback to previous version (see §Source Control Strategy - Rollback Scenarios)

---

### Success Metrics Summary

| Category | Metric | Target |
|----------|--------|--------|
| **Build** | Compilation errors | 0 |
| **Build** | Warnings | 0 (or minimal, documented) |
| **Runtime** | Startup failures | 0 |
| **Functionality** | Critical features broken | 0 |
| **Testing** | Test pass rate | 100% (if tests exist) |
| **Performance** | Response time degradation | < 5% (ideally improved) |
| **Security** | New vulnerabilities | 0 |
| **Code Quality** | Technical debt introduced | 0 |

---

### Celebration 🎉

When all success criteria met:
- Upgrade is complete!
- .NET 10 LTS provides security updates until 2027
- Solution benefits from latest platform improvements
- Team gained valuable upgrade experience

**Next Steps**:
- Monitor production for 1-2 weeks
- Update internal documentation
- Share learnings with team
- Plan future upgrades (.NET 11+ when available)
