# AspNetCore.Sample
This sample code shows how to develop an Azure web app with ASP.NET Core 2.1

## Tips
- Use `Logger` in `Common` project for trace. May use ILogger to route to Application Insights in the future.
- Use `AutoMapper` for object-object mapping.
- When creating a new project, use `StyleCop` and `FxCop`.
  - StyleCop: Provides developers an effective way to follow the coding standard which makes your codebase looks like it's been written by a single person.
  - FxCop: Checks your code for security, performance, and design issues.
  - How to use them?
    - Install their Nuget Packages: `StyleCop.Analyzers` and `Microsoft.CodeAnalysis.RxCopAnalyzers`.
    - Include `Shared\StyleCop\stylecop.ruleset` in your project file. Please refer to existing projects.
    - You can customize your rules either in stylecop.ruleset or in Visual Studio.
- When you troubleshoot an issue or performance tuning, you can add or update configuration items in `Application settings` on Azure portal to override the same settings in appsettings.json in the code.
- You can query `Application Insights` to assist troubleshoot.
