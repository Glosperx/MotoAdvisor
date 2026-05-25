# MotoAdvisor

A motorcycle catalog and recommendation app built with ASP.NET Core and React. Users can browse motorcycles, filter by brand/category/price, leave reviews, save favorites, and get AI-powered recommendations by describing what they want in plain language.

Live at: https://motoadvisor.student-dev.ro

---

## Tech stack

**Backend:** ASP.NET Core 8, Entity Framework Core 8, PostgreSQL 16, ASP.NET Identity, JWT auth, Serilog

**Frontend:** React 19, TypeScript, Vite, React Router v7, Axios

**AI:** Google Gemini (`gemini-embedding-001` for embeddings, `gemini-2.0-flash` for text generation)

---

## Features

- Browse motorcycles with photos, specs (engine, horsepower, license category)
- Filter by brand, category, price range; keyword search
- AI recommendations — describe what you want in Romanian, get a ranked list with an explanation
- Reviews (post/edit/delete your own; admins can delete any)
- Favorites list per user
- Admin panel — full CRUD for motorcycles, brands, categories
- Photo sync — drop images into a folder on the server, they appear without a restart

---

## Project structure

Clean Architecture split across three projects:

```
MotoAdvisor.Core/           domain entities, DTOs, interfaces
MotoAdvisor.Infrastructure/ EF Core, repositories, services (including RagService)
MotoAdvisor.API/            controllers, hosted services, program entry point
```

---

## Running locally

**Prerequisites:** .NET 8 SDK, Node 22+, PostgreSQL running locally, Gemini API key

**API:**
```bash
# fill in connection string and Gemini key in appsettings.Development.json first
cd MotoAdvisor.API
dotnet run
# runs on https://localhost:5000, Swagger at /swagger
```

Migrations and seed data run automatically on startup.

**Frontend:**
```bash
cd frontend
npm install
npm run dev
# runs on http://localhost:5173
# /api/* and /images/* are proxied to localhost:5000 via Vite config
```

---

## Running with Docker

```bash
cp .env.example .env
# set DB_PASSWORD, JWT_KEY (min 32 chars), GEMINI_API_KEY

docker network create studlab_net   # only needed once
docker compose up -d --build
```

Containers: `student-glosper-db` (Postgres), `student-glosper-api` (API on :8080), `student-glosper-frontend` (nginx on :80). None expose ports to the host directly — traffic comes through the VPS reverse proxy via the shared `studlab_net` network.

**Adding photos:** put folders under `/home/glosper/photos/` on the server. Folder name must match a motorcycle name in the DB (case-insensitive). The watcher picks them up live.

```
/home/glosper/photos/
  Ducati Panigale V4S/
    panigale1.jpg
    panigale2.jpg
```

---

## API overview

| Method | Path | Auth |
|--------|------|------|
| POST | /api/auth/register | — |
| POST | /api/auth/login | — |
| GET | /api/motorcycles | — |
| GET | /api/motorcycles/search?q= | — |
| POST/PUT/DELETE | /api/motorcycles/{id} | Admin |
| GET | /api/brands | — |
| POST/PUT/DELETE | /api/brands/{id} | Admin |
| GET | /api/categories | — |
| POST/PUT/DELETE | /api/categories/{id} | Admin |
| GET/POST/DELETE | /api/favorites | User |
| GET | /api/motorcycles/{id}/reviews | — |
| POST | /api/motorcycles/{id}/reviews | User |
| PUT/DELETE | /api/reviews/{id} | Owner / Admin |
| POST | /api/recommend | User |

---

## How the AI recommendations work

On startup, every motorcycle is converted to a text description and embedded using Gemini. The vectors are kept in memory.

When a user sends a query to `POST /api/recommend`:
1. The query is embedded with the same model
2. Cosine similarity is computed against all cached vectors
3. If the query mentions a budget, in-budget bikes get a small score boost
4. Top 5 results are fetched from the DB
5. They're passed as context to `gemini-2.0-flash`, which writes a short explanation in Romanian

If Gemini rate-limits the request, the ranked list is still returned with a fallback message instead of the AI text.

---

## Deployment

The app runs on student-dev.ro under a shared Docker network (`studlab_net`). The server's nginx handles HTTPS and routes the domain to the frontend container; the frontend's nginx then proxies `/api/` and `/images/` to the API internally.

```bash
# first deploy
ssh user@student-dev.ro
git clone <repo> && cd MotoAdvisor
cp .env.example .env && nano .env
mkdir -p ~/photos
docker compose up -d --build

# update after changes
git pull
docker compose up -d --build --no-deps api
```

---

## License

MIT
