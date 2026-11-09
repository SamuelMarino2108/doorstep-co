# Doorstep CO

[![CI](https://github.com/SamuelMarino2108/doorstep-co/actions/workflows/ci.yml/badge.svg)](https://github.com/SamuelMarino2108/doorstep-co/actions/workflows/ci.yml)

A .NET library and HTTP API that parses, normalizes and validates free-text Colombian addresses.

> **Status:** work in progress. The project is in early development and the API is not stable yet.

## Getting started

Prerequisites: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

From the repository root:

```bash
dotnet build
dotnet test
dotnet run --project src/DoorstepCO.Api
```

Once the API is running:

- Health check: http://localhost:5132/health
- Swagger UI: http://localhost:5132/swagger

## License

[MIT](LICENSE)
