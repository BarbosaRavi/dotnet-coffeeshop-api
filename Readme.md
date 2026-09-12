# CoffeeShopApi

REST API built with ASP.NET Core 10, Entity Framework Core and PostgreSQL.

## Requirements

- .NET SDK 10.0+
- PostgreSQL 14+
- EF Core CLI:

```bash
dotnet tool install --global dotnet-ef
```

## Setup

### 1. Environment variables

```bash
cp .env.example .env
```

Edit `.env` with your local values

### 2. Create the database

EF Core creates tables, not the database itself:

```bash
createdb table_name
```

### 3. Install dependencies and apply migrations

```bash
dotnet restore
dotnet ef database update
```

### 4. Run

```bash
dotnet run
```

API available at **http://localhost:5063**

For hot reload during development:

```bash
dotnet watch
```

## API documentation

With the app running in Development mode:

| URL | |
|---|---|
| http://localhost:5063/scalar | Scalar UI — browse and call endpoints |
| http://localhost:5063/openapi/v1.json | OpenAPI spec |

Both are disabled outside Development.

## Commands

```bash
dotnet build                      # compile
dotnet run                        # run
dotnet watch                      # run with hot reload
dotnet format                     # format code

dotnet ef migrations add <Name>   # create a migration from model changes
dotnet ef migrations remove       # undo the last unapplied migration
dotnet ef database update         # apply pending migrations
dotnet ef migrations list         # show applied/pending
```