# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Mirra Orchestrator is an Azure Functions (isolated worker, .NET 9) app that generates AI-tuned content (text + images) and publishes it to customer-owned WordPress blogs on a schedule. A single Timer trigger drives everything; there are no HTTP endpoints.

## Build / Run

- Build: `dotnet build --configuration Release`
- Run locally (Azure Functions host required): `func start` from the project root after `dotnet build` — `local.settings.json` only sets the runtime to `dotnet-isolated` and dev storage; real secrets come from User Secrets (`<UserSecretsId>cc1d121f-d65c-4b27-88a5-966badb71755`) or environment variables.
- No test project exists — there is nothing to `dotnet test`.
- CI/CD: push to `main` triggers `.github/workflows/main_mirra-orchestrator.yml`, which builds and deploys to the `mirra-orchestrator` Azure Function App via OIDC login.

## Required configuration keys

Loaded in `Program.cs` via `local.settings.json` → User Secrets → environment variables:

- `ConnectionStrings:DefaultConnection` — SQL Server (the context is registered with `UseSqlServer` despite MySQL also being referenced in the csproj).
- `AI:AzureOpenAI:Endpoint` / `AI:AzureOpenAI:ApiKey` — used for `gpt-4o` chat completion through Semantic Kernel.
- `AI:OpenAI:ApiKey` — used for `gpt-image-1-mini` image generation (both via Semantic Kernel `AddOpenAITextToImage` and directly via `OpenAIIntegration`).

## Architecture

The pipeline is a linear chain, and the layering is strict — understanding it requires tracing one scheduled run end-to-end.

### Trigger → orchestration flow

1. `Functions/SchedulerFunctions.cs` — single `TimerTrigger("*/15 * * * *")` calls `ISchedulingService.runAllScheduledPosts()`. There are no HTTP functions.
2. `Service/SchedulingService.cs` — loads every `Scheduling` row, filters by status (`ACTIVE`) and by a custom cron match in `ShouldExecuteNow` (uses `NCrontab`, compares hour/day/day-of-week ignoring minutes — the 15-min timer acts as the real minute-level granularity).
3. `Service/OrchestrationService.cs` — dispatches on `Platform.Id` (currently only `WORDPRESS = 1`, see `Enums/Platform.cs`). For WordPress it: resolves the per-customer config, fetches recent posts to pass as "avoid-duplication" context, generates the blog post, publishes it, generates a summary, and persists a `Content` row.
4. `Service/ContentGenerationService.cs` — the actual generation pipeline:
   - `PromptFormatterService` substitutes `{{VariableName}}` tokens in the platform's stored `Prompt` / `SystemPrompt` / `SummaryPrompt` using the `Parameters` row and summaries of previous posts.
   - `ModelCommunicationService` calls Azure OpenAI chat (via Semantic Kernel `IChatCompletionService`) for text and OpenAI image API (via `OpenAIIntegration`) for images.
   - Images are embedded by scanning the model output for the custom markup `[IMG: description &&& caption]`, generating each image, uploading it via `IImageRepository.SaveImage` (implemented by `WordpressIntegration`, which posts to `/wp/v2/media`), and replacing the markup with a `<figure>` block in `ModelResponseFormatter`.
   - The final response is split on `---` into title/content by `ModelResponseFormatter.GetWordpressBlogPostFromModelResponse` — the model is expected to output `<title>---<content>`.
5. `Integration/WordpressIntegration.cs` — publishes via REST to `{customerConfig.Url}/wp/v2/posts` using HTTP Basic auth pulled from `CustomerPlatformTableRow`. **Note**: `_configuration` is a mutable public property set per call from `OrchestrationService.setWordpressAccessConfigurations`, so this service is not safe to reuse across concurrent customers in the same scope.

### Data layer

- `Repository/DatabaseContext.cs` — EF Core context with `DbSet`s for scheduling, customers, platforms, contents, parameters, customer-platform configs, and scheduling status.
- `Repository/DbEntities/*TableRow.cs` — persistence entities, separate from `Model/*.cs` domain objects.
- `Repository/Mapper/*Profile.cs` — AutoMapper profiles (auto-registered via `AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())`) that project `TableRow` → domain model. `SchedulingRepository` uses `ProjectTo<Scheduling>` so the model shape drives the generated SQL — adding a field to a domain model may silently break the query if no mapping exists.
- `Repository/Repositories/DefaultRepository.cs` is a shared base that holds `DatabaseContext` + `IMapper`.

### DI composition

All registrations live in `Program.cs`. Semantic Kernel is built once as a singleton and `IChatCompletionService` / `ITextToImageService` are resolved from it. Everything else (services, repositories, integrations, `IRestClient`) is `Scoped` — matching the per-invocation lifetime of Azure Functions.

## Conventions worth knowing

- **Prompt variables** use `{{Name}}` with whitespace tolerated (`\{\{\s*(.*?)\s*\}\}`). The full set lives in `PromptFormatterService.ReplacePromptVariables` — missing values are substituted with the literal string `(not informed)`.
- **Image markup in model output**: `[IMG: <description> &&& <caption>]`. The description becomes the image-gen prompt; the caption is HTML-encoded into a `<figcaption>`. Surrounding `*` or `_` on the caption are stripped.
- **Title/content separator**: the model must return `<title>---<content>`. The first `---` wins.
- **New platform support** requires: a new `Enums/Platform` entry, a new case in `OrchestrationService.PostContent`, and (usually) a new `IImageRepository`-capable integration since images are uploaded back to the publishing platform.
- **Namespace**: root namespace is `Mirra_Orchestrator` (underscore), matching `<RootNamespace>` in the csproj — the folder has a space but the namespace does not.
- `NoWarn` in the csproj suppresses `SKEXP0010`, `SKEXP0001`, `OPENAI001` (Semantic Kernel / OpenAI experimental-API warnings) — don't "fix" these by adding pragmas in source.
