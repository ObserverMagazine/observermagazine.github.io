# Observer Magazine

A free, open-source Blazor WebAssembly application deployed to GitHub Pages.

Built with **.NET 11** (STS) and **C# 15**.

## Features

- **Blog** — Markdown posts with YAML front matter, compiled to static JSON and HTML at build time
- **RSS Feed** — Auto-generated `feed.xml` with full post content
- **Responsive Tables** — Sortable, filterable data tables that work on mobile
- **Master-Detail Flow** — Click a list item to view full details, with local CRUD
- **Text-to-Speech** — MP3 narration generated in CI with KittenTTS
- **Structured Logging and Telemetry** — `ILogger` plus an app-lifetime `IHostedService`
- **Full Test Suite** — bUnit component tests and xUnit v3 integration tests on Microsoft.Testing.Platform

## Requirements

- .NET SDK 11.0.100 or later (pinned in `global.json`)

## Quick Start

```bash
dotnet restore
dotnet run --project tools/ObserverMagazine.ContentProcessor -- --content-dir content/blog --output-dir src/ObserverMagazine.Web/wwwroot --authors-dir content/authors
dotnet run --project src/ObserverMagazine.Web
dotnet test
```

`dotnet run` serves the app with the Blazor WebAssembly Gateway, which replaces the old DevServer from .NET 11 onwards.

## Technology

| Component | Technology | License |
|-----------|------------|---------|
| Framework | .NET 11 / Blazor WebAssembly | MIT |
| Dev server | Microsoft.AspNetCore.Components.Gateway | MIT |
| Markdown | Markdig 1.6.0 | BSD-2-Clause |
| YAML | YamlDotNet 18.1.0 | MIT |
| Testing | xUnit v3 4.0.3 + bUnit 2.15.2 | Apache-2.0 / MIT |
| TTS | KittenTTS 0.8.1 (Python 3.12) | Apache-2.0 |

All dependencies are free for any use — no payment required, ever.

## License

AGPLv3
