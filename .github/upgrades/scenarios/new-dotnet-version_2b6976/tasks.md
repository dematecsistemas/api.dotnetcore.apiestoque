# DematecStock .NET 10 Upgrade Tasks

## Overview

This document tracks the execution of the DematecStock solution upgrade from .NET 8.0 to .NET 10.0. All 6 projects will be upgraded simultaneously in a single atomic operation, followed by validation.

**Progress**: 0/4 tasks complete (0%) ![0%](https://progress-bar.xyz/0)

---

## Tasks

### [ ] TASK-001: Verify prerequisites
**References**: Plan §Phase 0

- [ ] (1) Verify .NET 10 SDK installed per Plan §Prerequisites
- [ ] (2) SDK version meets .NET 10 requirements (**Verify**)

---

### [ ] TASK-002: Atomic framework and dependency upgrade
**References**: Plan §Phase 1, Plan §Package Update Reference, Plan §Breaking Changes Catalog

- [ ] (1) Update TargetFramework to net10.0 in all 6 project files per Plan §Phase 1 (Communication, Domain, Exception, Infrastructure, Application, Api)
- [ ] (2) All project files updated to net10.0 (**Verify**)
- [ ] (3) Update 5 package references per Plan §Package Update Reference (EF Core 9.0.11→10.0.4, EF Core SqlServer 9.0.11→10.0.4, Configuration.Binder 10.0.1→10.0.4, DependencyInjection 9.0.11→10.0.4, Authentication.JwtBearer 8.0.22→10.0.4)
- [ ] (4) All package references updated (**Verify**)
- [ ] (5) Restore all dependencies
- [ ] (6) All dependencies restored successfully (**Verify**)
- [ ] (7) Build solution and fix all compilation errors per Plan §Breaking Changes Catalog (focus: JWT Bearer authentication namespace/API changes in Api project, JWT token generation API changes in Infrastructure project, configuration binding API changes)
- [ ] (8) Solution builds with 0 errors (**Verify**)
- [ ] (9) Start application to verify runtime initialization
- [ ] (10) Application starts without exceptions (**Verify**)

---

### [ ] TASK-003: Validate JWT authentication and API functionality
**References**: Plan §Testing Strategy Level 4

- [ ] (1) Execute JWT authentication test sequence using HTTP client per Plan §Testing Strategy Level 4 (obtain token via POST to auth endpoint, access protected endpoint with Authorization header containing token, attempt access to protected endpoint without token)
- [ ] (2) JWT authentication flow validates correctly (token obtained successfully, protected endpoint accessible with valid token, unauthorized access returns 401) (**Verify**)
- [ ] (3) Execute sample HTTP requests to representative API endpoints per Plan §Testing Strategy Level 4 (test basic CRUD operations to verify controllers respond correctly)
- [ ] (4) API endpoints respond with expected status codes (**Verify**)

---

### [ ] TASK-004: Final commit
**References**: Plan §Source Control Strategy

- [ ] (1) Commit all changes with message: "chore: upgrade solution from .NET 8.0 to .NET 10.0"

---