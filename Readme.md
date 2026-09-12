# CoffeeShopApi

REST API built with ASP.NET Core 10, Entity Framework Core and PostgreSQL, running on Docker.

## Requirements

- Docker with Docker Compose
- .NET SDK 10.0+ and the EF Core CLI, to run migrations from your machine:

```bash
dotnet tool install --global dotnet-ef
```

## Setup

### 1. Environment variables

```bash
cp .env.example .env
```

Fill in `.env`. The same file configures the API container, the Postgres container and the `dotnet ef` CLI:

```dotenv
DB_HOST=localhost
DB_PORT=5433
DB_DATABASE=coffeeshop
DB_USERNAME=postgres
DB_PASSWORD=secret
```

`DB_HOST` and `DB_PORT` are what **your machine** uses to reach the database, through the port forwarded by Docker. The API container ignores them: `compose.yaml` overrides them with `db:5432`, the database's address inside the Docker network.

Optional overrides for the ports published on your machine:

| Variable | Default | |
|---|---|---|
| `APP_PORT` | `5063` | API |
| `FORWARD_DB_PORT` | `5433` | Postgres (5432 is left free for a local Postgres) |

### 2. Start the containers

```bash
docker compose up -d --build
```

This starts two services:

| Service | Image | Reachable from your machine at |
|---|---|---|
| `api` | built from `Dockerfile` | http://localhost:5063 |
| `db` | `postgres:18-alpine` | `localhost:5433` |

The database named in `DB_DATABASE` is created on first start and stored in the `pgdata` volume. The API waits until Postgres passes its healthcheck before starting.

### 3. Apply migrations

Migrations are not applied automatically. Run them from your machine against the container:

```bash
dotnet ef database update
```

### 4. Check it works

```bash
curl http://localhost:5063/api/products
```

## API documentation

When `ASPNETCORE_ENVIRONMENT=Development`:

| URL | |
|---|---|
| http://localhost:5063/scalar | Scalar UI — browse and call endpoints |
| http://localhost:5063/openapi/v1.json | OpenAPI spec |

Both are disabled outside Development.

## Commands

### Docker

```bash
docker compose up -d --build      # start (rebuild the image after code changes)
docker compose ps                 # container status
docker compose logs -f api        # follow API logs
docker compose restart api        # restart the API only
docker compose down               # stop and remove containers, keep data
docker compose down -v            # also delete the database volume (wipes all data)

docker compose exec db sh -c 'psql -U "$POSTGRES_USER" -d "$POSTGRES_DB"'   # open psql
```

### EF Core

```bash
dotnet ef migrations add <Name>   # create a migration from model changes
dotnet ef migrations remove       # undo the last unapplied migration
dotnet ef database update         # apply pending migrations
dotnet ef migrations list         # show applied/pending
```

## Running without Docker

For hot reload with `dotnet watch`, you can run the API directly on your machine. It still needs a Postgres server: either the `db` container (`docker compose up -d db`, keeping `DB_PORT=5433`) or a local install (`DB_PORT=5432`, with the database created beforehand using `createdb <DB_DATABASE>`).

```bash
dotnet restore
dotnet ef database update
dotnet watch                      # or: dotnet run
```

API available at **http://localhost:5063**. If the `api` container is running, stop it first (`docker compose stop api`) because both use port 5063.
