# ASP.NET Core E-Commerce Web App
A Generic ASP.NET Core E-Commerce Web App.

## Geting Started
[here](https://learn.microsoft.com/en-us/aspnet/core/getting-started/)

## Build
* to build the solution run `dotnet build`

## Multi Project Solution
In VSCode .sln will be created automatically on run. It can be disabled by setting `"dotnet.automaticallyCreateSolutionInWorkspace": false` in user settings.

1. To create a Solution manually run `dotnet new sln -o <SOLUTION_NAME>`
2. go to `cd <SOLUTION_NAME>`
3. create sub project `dotnet new <PROJECT_TYPE> -o <SOLUTION_NAME>.<_PROJECT_NAME>`
   1. "PROJECT_TYPE" can be `mvc` or `classlib`
4. add the new project to a solution with `dotnet sln add <SOLUTION_PROJECT_NAME>/<SOLUTION_PROJECT_NAME>.csproj --in-root`
5. to add references between projects run `dotnet add reference ../<SOLUTION_PROJECT_NAME>` from taget project folder

## Add Areas
* `dotnet tool install -g dotnet-aspnet-codegenerator`
* add `export PATH="$PATH":"$HOME/.dotnet/tools"` to your `.zrsh`
* `dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design --version <VERSION>`
* `dotnet-aspnet-codegenerator area <AREA_NAME>`
