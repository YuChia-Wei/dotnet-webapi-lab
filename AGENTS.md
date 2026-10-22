# AGENTS Instructions for dotnet-webapi-lab

## Guiding Principles
This repository is guided by Domain-Driven Design (DDD), Clean Architecture, and Hexagonal Architecture. Project and folder naming conventions must align with the terminology found in these design and architectural patterns.

## Repository Layout
- The solution file (`dotnet-lab.slnx`) is located in the root directory.
- The **src** folder contains all C# projects.
- Deployment related files live under **deploy/** (Docker and Helm charts).
- Authentication samples are in **keycloak/**.
- CI configuration resides in **.github/workflows/**.

## Project Structure and Layers
The solution follows the principles of Clean Architecture and Domain-Driven Design. The project structure reflects this with a clear separation of concerns into the following layers:

- **ArchPrecept packages**: Domain building blocks come from `ArchPrecept.Contracts.Domain`; repository/query/event-dispatch contracts come from `ArchPrecept.Contracts.Application`. Do not recreate local copies of these contracts.
- **`dotnetLab.Domain`**: This is the core of the application. It contains the domain entities, value objects, aggregates, and domain events that model the business domain. It depends only on `ArchPrecept.Contracts.Domain`.
- **`dotnetLab.Application`**: This layer holds the application logic. It orchestrates the domain objects to perform specific use cases. It contains application services, commands, queries, and Data Transfer Objects (DTOs). It depends on the `Domain` layer.
- **`dotnetLab.Infrastructure`**: Implements interfaces defined in the `Application` and `Domain` layers. It deals with external concerns like file systems, external APIs, or message queues.
- **`dotnetLab.Persistence.*`**: This layer is responsible for data persistence. It contains implementations of the repository interfaces defined in the `Domain` layer, typically using an ORM like Entity Framework Core.
- **`dotnetLab.WebApi`**: The presentation layer. It exposes the application's functionality as a RESTful API. It contains controllers that handle HTTP requests, delegate work to the `Application` layer, and return responses.
- **`dotnetLab.GrpcService`**: An alternative presentation layer that exposes functionality via gRPC services.
- **`dotnetLab.CrossCutting.Observability`**: Contains cross-cutting concerns like logging, metrics, and tracing that can be applied across all layers.
- **`ArchPrecept.Analyzers.Core`**: Version 0.0.3-preview is referenced centrally by `src/Directory.Build.targets` for every production project. `src/archprecept.repository.xml` retains the existing architecture profiles and opts into `ddd`. DBA1001-DBA1017 are errors; DBA1018/DBA1019 are warnings. The package-owned domain-event port needs no suppression. Treat DBA0002 not-evaluated as a coverage limit, not a pass.
- **`dotnetLab.Tests`**: Domain, application and in-process HTTP regression tests. All C# projects, including tests, stay under `src/`.

## Development Guidelines
- Maintain the layer boundaries: inner layers must not depend on outer layers.
- Keep the project naming and organization consistent when adding new projects.
- Conform to the coding style enforced by `.editorconfig`. Run `dotnet format` before committing.
- Ensure the solution builds: `dotnet build dotnet-lab.slnx -c Release`.
- After a successful build, unit tests must be run: `dotnet test`.

### Coding Rules
- Asynchronous methods must be suffixed with `Async` and expose an overload that accepts a `CancellationToken`. Framework overrides retain their required names (for example gRPC `SayHello`) and pass the framework cancellation token.
- Public classes and methods must include XML documentation comments written in Traditional Chinese (Taiwan usage) or English.

### Web API Rules
- Types representing API input should be named with the `Request` suffix.
- Types representing API output should be named with the `ViewModel` suffix.
- Every Web API endpoint must specify `ProducesResponseType<ApiResponse<TResponse>>`.
