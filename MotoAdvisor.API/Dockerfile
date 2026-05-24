# ── Stage 1: build ───────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first for better layer caching
COPY MotoAdvisor.Core/MotoAdvisor.Core.csproj                         MotoAdvisor.Core/
COPY MotoAdvisor.Infrastructure/MotoAdvisor.Infrastructure.csproj      MotoAdvisor.Infrastructure/
COPY MotoAdvisor.API/MotoAdvisor.API.csproj                            MotoAdvisor.API/

RUN dotnet restore MotoAdvisor.API/MotoAdvisor.API.csproj

# Copy full source after restore (deps cached above)
COPY MotoAdvisor.Core/           MotoAdvisor.Core/
COPY MotoAdvisor.Infrastructure/  MotoAdvisor.Infrastructure/
COPY MotoAdvisor.API/            MotoAdvisor.API/

RUN dotnet publish MotoAdvisor.API/MotoAdvisor.API.csproj \
    -c Release -o /app/publish --no-restore

# ── Stage 2: runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

# curl is needed for the docker-compose healthcheck
RUN apk add --no-cache curl

# Pre-create directories the app writes to at runtime
RUN mkdir -p wwwroot/photos photos-source logs

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "MotoAdvisor.API.dll"]
