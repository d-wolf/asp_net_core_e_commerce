# ASP.NET Core MVC WebApp 
A Generic ASP.NET Core E-Commerce Web App.

## Geting Started
[here](https://learn.microsoft.com/en-us/aspnet/core/getting-started/)

### Build
* run with hot reload `dotnet watch`
* run a profile from [launchSettings.json](ECommerce/ECommerce.WebApp/Properties/launchSettings.json) with `dotnet run --launch-profile "<PROFILE_NAME>"` e.g. `dotnet run --launch-profile "http"`

## DB
* uses postgresql db
* [install](https://www.postgresql.org/)
* DB name: `ecommerce`
* DB user: `postgres`
* DB user pw: `postgres`
* create db `CREATE DATABASE ecommerce OWNER postgres;`

## EF Core Migration
To make migrations work in multi project solution go through [this guide](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/projects?tabs=dotnet-core-cli) first.

1. add migration `dotnet ef migrations add <MIGRATION_NAME> -o Data/Migrations --project ../ECommerce.DataAccess`
2. create schema from migration `dotnet ef database update`