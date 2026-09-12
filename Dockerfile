# ---- Build stage: full SDK, used only to compile ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore on its own layer so it only re-runs when the csproj changes
COPY CoffeeShopApi.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# ---- Runtime stage: slim ASP.NET runtime, no SDK ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Non-root user provided by the image; .NET 8+ images listen on 8080
USER $APP_UID
EXPOSE 8080

ENTRYPOINT ["dotnet", "CoffeeShopApi.dll"]
