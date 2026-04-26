# EF Core migrations

- Install the `dotnet-ef` tool: `dotnet tool restore` in the repository root (see [`.config/dotnet-tools.json`](../../.config/dotnet-tools.json) and the main [README.md](../../README.md)).
- Apply: from the project directory, `dotnet ef database update` (requires a valid `ConnectionStrings:DefaultConnection` and the `Microsoft.EntityFrameworkCore.Design` package in the csproj).
- This repo may not include `DatabaseContextModelSnapshot.cs` if migrations were authored by hand. If you add more migrations with the CLI, run `dotnet ef migrations add <Name>` once the project builds; the tool will create or update the snapshot as needed.
