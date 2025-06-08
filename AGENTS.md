# AGENTS Instructions for dotnet-webapi-lab

## Repository Layout
- The **src** folder contains the solution (`dotnet-lab.sln`) and all C# projects.
  - Projects follow Clean Architecture / Hexagonal layers and are prefixed with numbers (e.g. `dotnetLab.SharedKernel`, `dotnetLab.Domains`, `dotnetLab.UseCases`, `dotnetLab.Persistence.*`, `dotnetLab.WebApi`, `dotnetLab.GrpcService`, etc.).
- Deployment related files live under **deploy/** (Docker and Helm charts).
- Authentication samples are in **keycloak/**.
- CI configuration resides in **.github/workflows/**.

## Development Guidelines
- Maintain the layer boundaries: inner layers (lower numbers) must not depend on outer layers.
- Keep folder naming and numbering consistent when adding new projects.
- Conform to the coding style enforced by `.editorconfig`. Run `dotnet format` before committing.
- Ensure the solution builds: `dotnet build src/dotnet-lab.sln -c Release`.
- When test projects exist, run `dotnet test` as part of verification.

### Coding Rules
- Asynchronous methods must be suffixed with `Async` and expose an overload that accepts a `CancellationToken`.
- Public classes and methods must include XML documentation comments written in Traditional Chinese (Taiwan usage) or English.

### Web API Rules
- Types representing API input should be named with the `Request` suffix.
- Types representing API output should be named with the `Response` suffix.
- Every Web API endpoint must specify `ProducesResponseType<ApiResponse<TResponse>>`.

