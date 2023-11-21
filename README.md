# ASP.NET Core E-Commerce Web App
A Generic ASP.NET Core E-Commerce Web App.

## Geting Started
[here](https://learn.microsoft.com/en-us/aspnet/core/getting-started/)

## Common VS Code settings
### VSCode disable auto solution creation
* auto .sln creation by vscode on run can be disabled by setting `"dotnet.automaticallyCreateSolutionInWorkspace": false` in VS Code user settings

## Common Core CLI
### Create Multi Project Solution Core CLI
1. create a solution `dotnet new sln -o <SOLUTION_NAME>`
2. go into folder `cd <SOLUTION_NAME>`
3. create proj `dotnet new mvc -o <SOLUTION_NAME>.<SOLUTION_PROJECT_NAME>`
4. add to solution with `dotnet sln add <SOLUTION_PROJECT_NAME>/<SOLUTION_PROJECT_NAME>.csproj --in-root`

### Postgres
* create db for user postgres with `CREATE DATABASE ecommerce OWNER postgres;`

### Run
* run with hot reload enabled `dotnet watch`
* run a profile from [launchSettings.json](ECommerce/ECommerce.WebApp/Properties/launchSettings.json) with `dotnet run --launch-profile "<PROFILE_NAME>"` e.g. `dotnet run --launch-profile "http"`