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

## Usage

```csharp
using DoorstepCO.Core;

var result = AddressValidator.Validate("cll 59 a bis sur # 80-12 apto 302 frente al parque");

result.Normalized;  // "CL 59A BIS SUR 80 12 AP 302"
result.Confidence;  // 0.7
result.Issues;      // [UnrecognizedText: "frente al parque"]
```

`Confidence` goes from 0.0 (unusable) to 1.0 (no issues found). Each issue lowers it:

| Issue | Meaning | Penalty |
|---|---|---|
| `Empty` | The input is blank | 1.0 |
| `Unparseable` | The input is not an urban address | 1.0 |
| `RuralAddress` | Rural keywords such as vereda, finca or km (non-standard) | 0.8 |
| `ZeroNumber` | The road, crossing road or plate number is 0 | 0.4 |
| `UnrecognizedText` | Trailing text was ignored | 0.3 |
| `PlateNumberOutOfRange` | The plate number is greater than 150 | 0.3 |

## License

[MIT](LICENSE)
