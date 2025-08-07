# AGENTS Instructions for dotnet-webapi-lab

## Guiding Principles
This repository is guided by Domain-Driven Design (DDD), Clean Architecture, and Hexagonal Architecture. Project and folder naming conventions must align with the terminology found in these design and architectural patterns.

## Repository Layout
- The **src** folder contains the solution (`dotnet-lab.sln`) and all C# projects.
  - Projects are organized by layers: `dotnetLab.BuildingBlocks.Domain`, `dotnetLab.Domain`, `dotnetLab.Application`, `dotnetLab.Infrastructure`, `dotnetLab.Persistence.*`, `dotnetLab.WebApi`, `dotnetLab.GrpcService`, etc.
- Deployment related files live under **deploy/** (Docker and Helm charts).
- Authentication samples are in **keycloak/**.
- CI configuration resides in **.github/workflows/**.

## Development Guidelines
- Maintain the layer boundaries: inner layers must not depend on outer layers.
- Keep the project naming and organization consistent when adding new projects.
- Conform to the coding style enforced by `.editorconfig`. Run `dotnet format` before committing.
- Ensure the solution builds: `dotnet build dotnet-lab.sln -c Release`.
- After a successful build, unit tests must be run: `dotnet test`.

### Coding Rules
- Asynchronous methods must be suffixed with `Async` and expose an overload that accepts a `CancellationToken`.
- Public classes and methods must include XML documentation comments written in Traditional Chinese (Taiwan usage) or English.

### Web API Rules
- Types representing API input should be named with the `Request` suffix.
- Types representing API output should be named with the `ViewModel` suffix.
- Every Web API endpoint must specify `ProducesResponseType<ApiResponse<TResponse>>`.