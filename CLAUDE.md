# CLAUDE.md

Guidance for Claude Code when working in this repository.

## Project

**Doorstep CO** (repo: `doorstep-co`) — a .NET library and HTTP API that parses, normalizes and validates
free-text Colombian addresses (e.g. `"cll 59 a bis sur # 80-12 apto 302"` → `"CL 59A BIS SUR 80 12 AP 302"`),
returning structured components, a confidence score and a list of detected issues.

Goal: catch bad addresses before a shipment is dispatched, reducing failed deliveries in last-mile logistics.

## Tech stack

- .NET 8 (C# 12), nullable reference types enabled, implicit usings enabled
- `DoorstepCO.Core`: class library, published as a NuGet package, **zero external dependencies**
- `DoorstepCO.Api`: ASP.NET Core Minimal API, Swagger/OpenAPI
- Tests: xUnit + FluentAssertions
- CI: GitHub Actions (restore, build, test on every push and PR)
- Docker for the API

## Repository layout

```
src/
  DoorstepCO.Core/
    Parsing/         tokenizer and parser
    Normalization/   abbreviations, synonyms, common typos
    Validation/      issue detection and confidence scoring
    Formatting/      standard output (DIAN-style abbreviations: CL, CRA, DG, TV, AC, AK, AV)
  DoorstepCO.Api/
tests/
  DoorstepCO.Core.Tests/
  DoorstepCO.Api.Tests/
samples/             example CSV files (fake data only)
```

Do not add new projects or top-level folders without asking first.

## Language

- Everything in English: code, identifiers, comments, commit messages, README, docs, test names.
- Exception: domain input data (address strings and Spanish keywords such as "calle", "carrera", "apto") stays in Spanish.

## Architecture principles

- Keep it simple. No over-engineering: no MediatR, no CQRS, no repository pattern, no unnecessary interfaces or layers.
- Core contains all business logic; the API is a thin layer that only maps HTTP to Core calls.
- Prefer small, pure, testable functions and immutable types (`record`).
- Parsing is deterministic and rule-based. No AI or external services in Core.
- Public API of Core must be small and documented with XML doc comments.

## Coding conventions

- Follow `.editorconfig`; code must be clean under `dotnet format`.
- PascalCase for types and members, camelCase for locals and parameters, `_camelCase` for private fields.
- File-scoped namespaces, one type per file.
- No magic strings: keywords, abbreviations and synonyms live in dedicated lookup classes.
- Treat warnings as errors.
- No commented-out code, no TODOs left without an issue reference.

## Testing

- **TDD**: write or update the failing test first, then the implementation.
- Test names: `MethodName_Scenario_ExpectedResult`.
- Use `[Theory]` with `[InlineData]` or a data source for address variations.
- Every bug fix includes a regression test.
- Test data must be **fake**. Never commit real customer addresses or personal data.

## Workflow

1. For any non-trivial task, propose a short plan first and wait for approval.
2. Work in small steps. After each logical change:
   - `dotnet build` (no warnings)
   - `dotnet test` (all green)
   - commit
3. Never commit code that does not build or has failing tests.
4. Work on a feature branch per phase (`feat/basic-parser`, `feat/complements`, ...); never commit directly to `main`.

## Commits

- Conventional Commits, in English, imperative mood, lowercase, no trailing period:
  - `feat: parse road type and plate number`
  - `test: add cases for bis and quadrant modifiers`
  - `fix: handle "no." as plate separator`
  - `refactor: extract abbreviation lookup`
  - `docs: add batch endpoint example to readme`
  - `chore: configure github actions ci`
- One logical change per commit. No "WIP", "misc" or "updates" commits.
- Subject line under 72 characters; add a body only when the "why" is not obvious.
- Do not mix formatting changes with logic changes in the same commit.
- The repository owner is the sole author of every commit. Never add `Co-Authored-By` trailers or any
  other AI attribution (e.g. "Generated with Claude Code") to commit messages or pull request descriptions.

## Safety

- Never delete files or folders without asking explicitly first.
- Never force-push, rewrite published history or delete branches.
- Do not add NuGet packages without asking.

## Roadmap

1. Setup: solution, projects, `.editorconfig`, CI
2. Basic parser: road type, road number, cross number, plate number
3. Modifiers: letters, BIS, Sur/Este, compound avenues, keyword variants
4. Complements: apartment, tower, interior, block, house, manzana, lote, conjunto
5. Validation: issues list and confidence score; rural addresses flagged as non-standard
6. API: `POST /normalize`, `POST /normalize/batch` (CSV in → CSV out), Swagger, Dockerfile
7. Release: NuGet package, README with examples and badges