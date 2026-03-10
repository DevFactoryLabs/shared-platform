# Shared Libraries

Cross-cutting libraries used across bounded contexts (establishments, orders, products, notifications).

| Feature | Description |
|---------|-------------|
| [Caching](libs/Caching/README.md) | Cache abstraction and Redis implementation |
| [Contracts](libs/Contracts/README.md) | Cross-context integration events |
| [Core](libs/Core/README.md) | Domain base classes, events, identification |
| [Correlation](libs/Correlation/README.md) | Correlation ID for distributed tracing |
| [ExternalClients](libs/ExternalClients/README.md) | HTTP client abstractions and implementations |
| [Handlers](libs/Handlers/README.md) | CQRS handler interfaces and base classes |
| [Identity](libs/Identity/README.md) | JWT, claims, and authorization helpers |
| [Inbox](libs/Inbox/README.md) | Idempotent event consumption |
| [Messaging](libs/Messaging/README.md) | Message bus abstraction (RabbitMQ) |
| [Outbox](libs/Outbox/README.md) | Reliable event publishing |
| [Publishing](libs/Publishing/README.md) | Domain event publishing |
| [Queries](libs/Queries/README.md) | Pagination and query models |
| [Results](libs/Results/README.md) | Result pattern for error handling |
| [Validations](libs/Validations/README.md) | FluentValidation extensions and validators |

## Development

### Build

```bash
dotnet build shared-platform.slnx
```

### Test

```bash
dotnet test shared-platform.slnx
```

### Code Coverage

```bash
chmod +x coverage.sh
./coverage.sh
# Opens coverage-report/index.html with the full report
```

### Publishing a New Version

Versioning follows [Semantic Versioning](https://semver.org/) (major.minor.patch) via git tags:

```bash
git tag v1.2.3
git push origin v1.2.3
```

This triggers the release workflow, which builds, tests, and publishes all packages to GitHub Packages automatically.
