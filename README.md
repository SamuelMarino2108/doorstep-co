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

## Live demo

**[doorstep-co.onrender.com](https://doorstep-co.onrender.com/)**: web UI to normalize one address or upload a `.csv`/`.xlsx` file.
To explore and try the API, open **[Swagger UI](https://doorstep-co.onrender.com/swagger)** or use the
*Ver la API* button in the web UI.

> [!NOTE]
> The demo runs on Render's free plan. The instance goes to sleep after 15 minutes without traffic, so the
> first request can take up to a minute while it wakes up. After that, responses are immediate.

## What it understands

| Feature | Examples |
|---|---|
| Road types and their variants | calle, cll, cl, carrera, cra, kr, k, diagonal, dg, transversal, tv, avenida, av, autopista, circular, circunvalar |
| Misspelled road types | `carrerra 7 # 10-20` → `CRA 7 10 20`, flagged `CorrectedTypo` |
| Compound avenues | `avenida calle 26` → `AC 26`, `av. carrera 68` → `AK 68` |
| Named avenues | `av boyacá # 80-12` → `AK 72 80 12` |
| Letters and BIS | `59 a bis`, `68B`, `59-a`, two letters as in Medellín: `44AE`, `33AA` |
| Ordinal road numbers | `calle 1ra`, `calle 2da`, `calle primera` |
| Quadrants | `sur` always qualifies the calle and `este` the carrera; `norte` is implicit and dropped |
| Plate separators | `#`, `no.`, `nro`, `numero` |
| Complements | apartamento, torre, interior, bloque, casa, manzana, lote, oficina, local, piso, bodega, consultorio, etapa, sector |
| Named complements | conjunto, edificio, barrio, urbanización: `edificio el roble` → `ED EL ROBLE` |
| Trailing city or department | `cra 7 # 10-20, Bogotá D.C.` → `CRA 7 10 20` |
| Complement-only addresses | `mz 4 casa 12` → `MZ 4 CA 12`, flagged as non-standard |
| Rural addresses | vereda, finca, km... flagged as non-standard |

Single `S` and `E` letters after a number are read as Sur and Este, following the nomenclature of Bogotá and
Medellín. Cali's `Norte` and `Oeste` sectors are not supported yet.

## Library

```bash
dotnet add package DoorstepCO.Core
```

```csharp
using DoorstepCO.Core;

var result = AddressValidator.Validate("cll 59 a bis sur # 80-12 apto 302 frente al parque");

result.Normalized;  // "CL 59A BIS SUR 80 12 AP 302 FRENTE AL PARQUE"
result.Confidence;  // 0.7
result.Issues;      // [UnrecognizedText: "frente al parque"]
result.Address;     // RoadType, Road, Cross, PlateNumber, Complements and Remarks
```

`Confidence` goes from 0.0 (unusable) to 1.0 (no issues found). Each issue lowers it:

| Issue | Meaning | Penalty |
|---|---|---|
| `Empty` | The input is blank | 1.0 |
| `Unparseable` | The input is not an urban address | 1.0 |
| `RuralAddress` | Rural keywords such as vereda, finca or km (non-standard) | 0.8 |
| `NonStandardAddress` | No road, only complements, e.g. `mz 4 casa 12` | 0.8 |
| `ZeroNumber` | The road, crossing road or plate number is 0 | 0.4 |
| `UnrecognizedText` | Trailing text was not recognized; it is kept at the end of `Normalized` | 0.3 |
| `IncompleteComplement` | A complement has no value, e.g. `... apto`; the text is kept at the end of `Normalized` | 0.3 |
| `RoadNumberOutOfRange` | The road or crossing road number is greater than 300 | 0.3 |
| `PlateNumberOutOfRange` | The plate number is greater than 250 | 0.3 |
| `NamedRoad` | The road is a name that could not be matched to its number, e.g. `av jimenez` | 0.2 |
| `CorrectedTypo` | The road type was misspelled and corrected, e.g. `carrerra` | 0.1 |

Plate numbers are written with at least two digits: `calle 10 # 4-5` → `CL 10 4 05`.

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
| `/swagger` | Swagger UI to explore and try the API |

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
    "road": { "number": 59, "letter": "A", "isBis": true, "bisLetter": null, "quadrant": "Sur", "name": null, "secondLetter": null },
    "cross": { "number": 80, "letter": null, "isBis": false, "bisLetter": null, "quadrant": null, "name": null, "secondLetter": null },
    "plateNumber": 12,
    "complements": [{ "type": "Apartamento", "value": "302" }],
    "remarks": "FRENTE AL PARQUE"
  },
  "normalized": "CL 59A BIS SUR 80 12 AP 302 FRENTE AL PARQUE",
  "confidence": 0.7,
  "issues": [
    {
      "code": "UnrecognizedText",
      "message": "Text after the address was not recognized and was kept as remarks. \"frente al parque\""
    }
  ]
}
```

When the address cannot be parsed, `address` and `normalized` are `null` and `confidence` is `0`. When it is
made only of complements, `address` is `null` and `normalized` holds the complements (`"MZ 4 CA 12"`).

### `POST /normalize/batch`

Upload a `.csv` or `.xlsx` file (up to 10 MB) as the `file` field of a multipart form. The file needs a
`DIRECCION` column (case and accents are ignored). The response is the same file, named `<name>-normalizada`,
with three columns appended:

| Column | Content |
|---|---|
| `NORMALIZADA` | The normalized address; empty when nothing could be recognized |
| `CONFIANZA` | The confidence score, from `0.00` to `1.00` |
| `PROBLEMAS` | The issue messages, separated by `; `; empty when there are none |

Rows with a blank address keep the three cells empty. CSV files may be separated by commas, semicolons or tabs.

```bash
curl -F "file=@samples/direcciones.csv" http://localhost:5132/normalize/batch -o direcciones-normalizada.csv
```

```
PEDIDO;DIRECCION;CIUDAD;NORMALIZADA;CONFIANZA;PROBLEMAS
1001;cll 59 a bis sur # 80-12 apto 302;Bogotá;CL 59A BIS SUR 80 12 AP 302;1.00;
1006;cl 10 # 5-51 frente al parque;Barranquilla;CL 10 5 51 FRENTE AL PARQUE;0.70;"Text after the address was not recognized and was kept as remarks. ""frente al parque"""
1007;vereda el salitre finca la esperanza;Chía;;0.00;"The address looks rural and does not follow the standard urban nomenclature.; The address could not be recognized as an urban address."
1008;cra 15 # 0-45;Bucaramanga;CRA 15 0 45;0.60;The road, crossing road or plate number is 0.
1010;mz 4 casa 12;Cartagena;MZ 4 CA 12;0.20;The address has no road, only complements, and does not follow the standard urban nomenclature.
```

Errors are returned as `400 Bad Request` problem details with a `code`: `EmptyFile`, `MissingAddressColumn`,
`UnreadableFile`, `UnsupportedFileType` or `FileTooLarge`.

## Development

```bash
dotnet build
dotnet test
dotnet format --verify-no-changes
```

Pushing a `v*` tag (for example `v0.2.0`) publishes `DoorstepCO.Core` to NuGet with that version.

## Behavior changes in 0.2.0

The public API only gains members (`Address.Remarks`, `RoadNumber.SecondLetter` and new enum values), so code
written for 0.1.0 still compiles. Some results change:

- `Address.Remarks` holds unrecognized trailing text, and `AddressFormatter.Format` appends it, so
  `Normalized` keeps it instead of dropping it.
- `ValidationResult.Normalized` is not `null` for complement-only addresses even though `Address` is.
- Plate numbers are formatted with at least two digits (`4 05` instead of `4 5`).
- The batch output has `CONFIANZA` and `PROBLEMAS` columns after `NORMALIZADA`.

## License

[MIT](LICENSE)
