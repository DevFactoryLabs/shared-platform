# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build shared-platform.slnx

# Test (all)
dotnet test shared-platform.slnx

# Test (single project)
dotnet test libs/Handlers/Shared.Handlers.Tests/Shared.Handlers.Tests.csproj

# Pack (NuGet packages)
dotnet pack shared-platform.slnx --configuration Release

# Code coverage report
./coverage.sh  # generates coverage-report/index.html
```

## Architecture

This is a **monorepo of shared libraries** for distributed, event-driven microservices targeting **.NET 10**. Libraries are published to GitHub Packages as NuGet packages, versioned via git tags (`v*.*.*`).

### Domain Layout (`libs/`)

| Domain | Purpose |
|--------|---------|
| `Core` | Base types: `AggregateRoot`, `IDomainEvent`, `IIntegrationEvent`, `IdGenerator` |
| `Handlers` | CQRS: `ICommand`, `IQuery<T>`, `ICommandHandler`, `IQueryHandler` |
| `Results` | Result pattern: `Result`, `Result<T>`, `Error`; HTTP mapping in `.Extensions` |
| `Publishing` | Domain event dispatcher — collects from EF `ChangeTracker`, dispatches via MediatR |
| `Outbox` | Transactional outbox pattern using EF Core + Dapper + PostgreSQL + RabbitMQ |
| `Inbox` | Idempotent event consumer base using Dapper + RabbitMQ |
| `Messaging` | `IMessageBus` abstraction + RabbitMQ implementation |
| `Correlation` | `ICorrelationContext` (AsyncLocal) + OpenTelemetry log processor |
| `Caching` | `ICacheService` abstraction + Redis implementation |
| `Identity` | JWT/claims helpers, `IdentityUser`, Keycloak config, `SharedRoleNames` |
| `Validations` | FluentValidation integration; CPF/CNPJ validators (`.Cpf()`, `.Cnpj()`) |
| `Queries` | Pagination: `BaseQuery`, `IPagedList<T>`, `PagedList<T>` |

### Key Patterns

**Aggregate / Domain Events**
- Entities extend `AggregateRoot`, call `RaiseEvent(IDomainEvent)` to queue events
- `Publishing` reads events from EF `ChangeTracker` and dispatches handlers in the same request

**CQRS (Handlers)**
- Commands return `Task<Result>` or `Task<Result<T>>`
- Queries return `Task<Result<T>>`
- Register via `services.AddHandlersFromAssembly(assembly)`

**Result Pattern**
- Use `Result` / `Result<T>` for business logic errors instead of exceptions
- `Results.Extensions` maps results to HTTP responses (200/400/404)

**Outbox Pattern**
- Implement `IOutboxDbContext` (adds `OutboxMessages` DbSet)
- Call `IOutboxPublisher.PublishAsync()` inside the same transaction as domain changes
- Background service polls and publishes to RabbitMQ with Polly retry

**Inbox Pattern**
- Consumers extend `InboxEventConsumer<TEvent, TDbConnectionFactory>`
- Idempotency enforced by message ID check before processing

### Conventions

- Private fields: `_camelCase`; interfaces: `IPrefix`
- Nullable enabled project-wide; use explicit null handling
- All libraries are packable (`IsPackable=true`); test projects are not (`IsPackable=false`)
- Default package version is `0.0.0`; CI overrides from git tag during release

### CI/CD

- **PR Validation**: restore → build (Release) → test with coverage → report
- **Release**: triggered by `v*.*.*` tag → build → test → pack → push to GitHub Packages
