# Doorstep CO

[![CI](https://github.com/SamuelMarino2108/doorstep-co/actions/workflows/ci.yml/badge.svg)](https://github.com/SamuelMarino2108/doorstep-co/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/DoorstepCO.Core.svg)](https://www.nuget.org/packages/DoorstepCO.Core)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

A .NET library and HTTP API that parses, normalizes and validates free-text Colombian addresses, so bad
addresses are caught before a shipment is dispatched.

```
"cll 59 a bis sur # 80-12 apto 302"  →  "CL 59A BIS SUR 80 12 AP 302"
```

Every address comes back with its structured components, a confidence score and a list of detected issues.
Parsing is deterministic and rule-based: no AI, no external services, no dependencies.

## What it understands

| Feature | Examples |
|---|---|
| Road types and their variants | calle, cll, cl, carrera, cra, kr, diagonal, dg, transversal, tv, avenida, av |
| Compound avenues | `avenida calle 26` → `AC 26`, `av. carrera 68` → `AK 68` |
| Named avenues | `av boyacá # 80-12` → `AK 72 80 12` |
| Letters, BIS and quadrants | `59 a bis`, `68B`, `sur`, `este` |
| Plate separators | `#`, `no.`, `nro`, `numero` |
| Complements | apartamento, torre, interior, bloque, casa, manzana, lote, conjunto |
| Rural addresses | vereda, finca, km... flagged as non-standard |

## Library

```bash
dotnet add package DoorstepCO.Core
```

```csharp
using DoorstepCO.Core;

var result = AddressValidator.Validate("cll 59 a bis sur # 80-12 apto 302 frente al parque");

result.Normalized;  // "CL 59A BIS SUR 80 12 AP 302"
result.Confidence;  // 0.7
result.Issues;      // [UnrecognizedText: "frente al parque"]
result.Address;     // RoadType, Road, Cross, PlateNumber and Complements
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
| `NamedRoad` | The road is a name that could not be matched to its number, e.g. `av jimenez` | 0.2 |

## HTTP API

### Run it

With the .NET 8 SDK:

```bash
dotnet run --project src/DoorstepCO.Api
```

The API listens on http://localhost:5132. With Docker:

```bash
docker build --tag doorstep-co-api .
docker run --rm -p 8080:8080 doorstep-co-api
```

The API listens on http://localhost:8080.

| Path | Description |
|---|---|
| `/` | Web UI: normalize one address or upload a file |
| `POST /normalize` | Normalize a single address |
| `POST /normalize/batch` | Normalize a `.csv` or `.xlsx` file |
| `/health` | Health check |
| `/swagger` | Swagger UI (Development environment) |

### `POST /normalize`

```bash
curl -X POST http://localhost:5132/normalize \
  -H "Content-Type: application/json" \
  -d '{"address": "cll 59 a bis sur # 80-12 apto 302 frente al parque"}'
```

```json
{
  "address": {
    "roadType": "Calle",
    "road": { "number": 59, "letter": "A", "isBis": true, "bisLetter": null, "quadrant": "Sur", "name": null },
    "cross": { "number": 80, "letter": null, "isBis": false, "bisLetter": null, "quadrant": null, "name": null },
    "plateNumber": 12,
    "complements": [{ "type": "Apartamento", "value": "302" }]
  },
  "normalized": "CL 59A BIS SUR 80 12 AP 302",
  "confidence": 0.7,
  "issues": [
    {
      "code": "UnrecognizedText",
      "message": "Text after the address was not recognized and was ignored. \"frente al parque\""
    }
  ]
}
```

When the address cannot be parsed, `address` and `normalized` are `null` and `confidence` is `0`.

### `POST /normalize/batch`

Upload a `.csv` or `.xlsx` file (up to 10 MB) as the `file` field of a multipart form. The file needs a
`DIRECCION` column (case and accents are ignored). The response is the same file, named `<name>-normalizada`,
with a `NORMALIZADA` column appended; the cell is left empty when the address cannot be parsed. CSV files may be
separated by commas, semicolons or tabs.

```bash
curl -F "file=@samples/direcciones.csv" http://localhost:5132/normalize/batch -o direcciones-normalizada.csv
```

```
PEDIDO;DIRECCION;CIUDAD;NORMALIZADA
1001;cll 59 a bis sur # 80-12 apto 302;Bogotá;CL 59A BIS SUR 80 12 AP 302
1002;Cra 7 No. 10-20 Torre 2 Apto 501;Bogotá;CRA 7 10 20 TO 2 AP 501
1003;Av. Calle 26 # 68B-31;Bogotá;AC 26 68B 31
```

Errors are returned as `400 Bad Request` problem details with a `code`: `EmptyFile`, `MissingAddressColumn`,
`UnreadableFile`, `UnsupportedFileType` or `FileTooLarge`.

## Development

```bash
dotnet build
dotnet test
dotnet format --verify-no-changes
```

Pushing a `v*` tag (for example `v0.1.0`) publishes `DoorstepCO.Core` to NuGet with that version.

## License

[MIT](LICENSE)
