# 🏍️ MotoAdvisor

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-19-61DAFB?logo=react)](https://react.dev/)
[![TypeScript](https://img.shields.io/badge/TypeScript-6.0-3178C6?logo=typescript)](https://www.typescriptlang.org/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker)](https://docs.docker.com/compose/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

A full-stack motorcycle catalog and recommendation platform. Browse and filter motorcycles by brand, category and price, write reviews, save favorites, and get AI-powered recommendations in natural language — powered by Google Gemini embeddings and RAG.

---

## 📸 Screenshots

> _Add screenshots here once the app is deployed._

| Catalog | AI Recommendations | Admin Panel |
|---|---|---|
| _(screenshot)_ | _(screenshot)_ | _(screenshot)_ |

---

## ✨ Features

- **Motorcycle catalog** — browse the full fleet with photos, specs (engine, horsepower, license category), brand and category
- **Advanced filtering** — filter by brand, category, price range; full-text keyword search
- **AI recommendations** — describe what you're looking for in plain Romanian and get a ranked shortlist with a natural-language explanation (RAG + Gemini)
- **Reviews & ratings** — authenticated users can post, edit and delete reviews; admins can moderate all
- **Favorites** — save motorcycles to a personal list
- **Admin dashboard** — full CRUD for motorcycles, brands and categories; role-protected endpoints
- **Photo management** — drop images into a watched folder on the server and they appear live without a restart
- **JWT authentication** — register/login with secure token auth; role-based access (`User` / `Admin`)

---

## 🛠️ Tech Stack

### Backend
| Layer | Technology |
|---|---|
| Runtime | ASP.NET Core 8.0 (minimal hosting model) |
| ORM | Entity Framework Core 8 + Npgsql |
| Auth | ASP.NET Core Identity + JWT Bearer |
| Logging | Serilog (console + rolling file) |
| Database | PostgreSQL 16 |
| API docs | Swagger / OpenAPI (Swashbuckle) |

### Frontend
| | Technology |
|---|---|
| Framework | React 19 + TypeScript 6 |
| Build tool | Vite 8 |
| Routing | React Router v7 |
| HTTP client | Axios |

### AI
| | |
|---|---|
| Embeddings | `gemini-embedding-001` (Google Gemini) |
| Generation | `gemini-2.0-flash` |
| Strategy | RAG — cosine similarity over in-memory embedding index, top-5 results fed to generative model |

---

## 🏗️ Architecture

The solution follows **Clean Architecture** across three projects:

```
MotoAdvisor/
├── MotoAdvisor.Core/            # Domain — no external dependencies
│   ├── Entities/                # Motorcycle, Brand, Category, Review, …
│   ├── DTOs/                    # Request/response shapes
│   └── Interfaces/              # IMotorcycleService, IRagService, …
│
├── MotoAdvisor.Infrastructure/  # Data access & services
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── DbSeeder.cs
│   ├── Repositories/            # EF Core implementations of IRepository<T>
│   └── Services/                # AuthService, RagService, MotorcycleService, …
│
└── MotoAdvisor.API/             # HTTP layer
    ├── Controllers/             # Thin — delegate everything to services
    └── Services/                # PhotoSyncService, PhotoWatcherService
```

**Request flow:**

```
HTTP request
  → Controller (validates, extracts claims)
    → IXxxService (business logic, defined in Core)
      → IXxxRepository (EF Core query, defined in Infrastructure)
        → PostgreSQL
```

**RAG flow:**

```
Startup: all motorcycles → Gemini embeddings → in-memory Dictionary<id, float[]>

POST /api/recommend { query }
  → embed query with Gemini
  → cosine similarity against all cached embeddings (budget boost/penalty applied)
  → top-5 IDs → DB fetch for full data
  → build context string → Gemini generative model
  → { aiResponse, motorcycles[] }
```

---

## 🚀 Running Locally

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Node.js 22+](https://nodejs.org/)
- [PostgreSQL 16](https://www.postgresql.org/) running locally (or via Docker)
- A [Google AI Studio](https://aistudio.google.com/) API key for Gemini

### 1. Clone and configure

```bash
git clone <repo-url>
cd MotoAdvisor
```

Edit `MotoAdvisor.API/appsettings.Development.json` and fill in your local DB connection string and Gemini key:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=motoadvisor;Username=<user>;Password=<pass>"
  },
  "Gemini": { "ApiKey": "<your-key>" }
}
```

### 2. Run the API

```bash
cd MotoAdvisor.API
dotnet run
# API available at https://localhost:5000
# Swagger UI at https://localhost:5000/swagger
```

EF Core migrations and the default seed data (brands, categories, an admin user) run automatically on first startup.

### 3. Run the frontend

```bash
cd frontend
npm install
npm run dev
# Dev server at http://localhost:5173
# Vite proxies /api/* and /images/* to http://localhost:5000
```

---

## 🐳 Running with Docker Compose

```bash
# 1. Copy and fill in the env file
cp .env.example .env
#    Set DB_PASSWORD and JWT_KEY (min 32 chars)
#    Optionally set GEMINI_API_KEY

# 2. Create the external network (once per machine)
docker network create studlab_net

# 3. Start everything
docker compose up -d --build

# 4. Follow logs
docker compose logs -f api
```

Services started:

| Container | Role | Internal port |
|---|---|---|
| `student-glosper-db` | PostgreSQL 16 | 5432 |
| `student-glosper-api` | ASP.NET Core API | 8080 |
| `student-glosper-frontend` | nginx serving React SPA | 80 |

The API and frontend use `expose:` (not `ports:`), so they are only reachable inside the `studlab_net` Docker network — your VPS reverse proxy handles external traffic.

### Adding photos

Drop image folders onto the server at `/home/glosper/photos/`:

```
/home/glosper/photos/
└── Ducati Panigale V4S/
    ├── panigale-v4s_1.jpg
    └── panigale-v4s_2.jpg
```

The folder name must match a motorcycle name in the database (case-insensitive, spaces and hyphens normalised). `PhotoWatcherService` picks up new files live; `PhotoSyncService` reconciles on every startup.

---

## 📡 API Endpoints

### Auth
| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/auth/register` | — | Register a new user |
| `POST` | `/api/auth/login` | — | Login, returns JWT |

### Motorcycles
| Method | Path | Auth | Description |
|---|---|---|---|
| `GET` | `/api/motorcycles` | — | List all; filter via `?brandId=&categoryId=&minPrice=&maxPrice=` |
| `GET` | `/api/motorcycles/{id}` | — | Get by ID |
| `GET` | `/api/motorcycles/search?q=` | — | Keyword search |
| `POST` | `/api/motorcycles` | Admin | Create |
| `PUT` | `/api/motorcycles/{id}` | Admin | Update |
| `DELETE` | `/api/motorcycles/{id}` | Admin | Delete |

### Brands & Categories
| Method | Path | Auth |
|---|---|---|
| `GET` | `/api/brands` | — |
| `POST/PUT/DELETE` | `/api/brands/{id}` | Admin |
| `GET` | `/api/categories` | — |
| `POST/PUT/DELETE` | `/api/categories/{id}` | Admin |

### Reviews
| Method | Path | Auth |
|---|---|---|
| `GET` | `/api/motorcycles/{id}/reviews` | — |
| `POST` | `/api/motorcycles/{id}/reviews` | User |
| `PUT` | `/api/reviews/{id}` | Owner |
| `DELETE` | `/api/reviews/{id}` | Owner or Admin |

### Favorites
| Method | Path | Auth |
|---|---|---|
| `GET` | `/api/favorites` | User |
| `POST` | `/api/favorites/{motorcycleId}` | User |
| `DELETE` | `/api/favorites/{motorcycleId}` | User |

### AI Recommendations
| Method | Path | Auth | Body |
|---|---|---|---|
| `POST` | `/api/recommend` | User | `{ "query": "vreau o motocicleta sport sub 10000 euro" }` |

---

## 🤖 RAG Recommendation System

MotoAdvisor uses a **Retrieval-Augmented Generation** pipeline entirely in Romanian:

1. **Index (startup)** — every motorcycle in the database is described as a natural-language string (name, brand, category, year, price, horsepower, engine, license category, description) and embedded with `gemini-embedding-001`. Vectors are stored in memory.

2. **Retrieve** — the user's query is embedded and compared to all cached vectors using cosine similarity. If the query mentions a budget (e.g. _"sub 8000 euro"_), in-budget motorcycles get a 15 % score boost and over-budget ones a 15 % penalty. The top 5 results are returned.

3. **Generate** — the top-5 motorcycle summaries are passed as context to `gemini-2.0-flash` with a Romanian-language system prompt. The model explains why each model fits the query and flags budget mismatches.

The service handles Gemini rate-limit (HTTP 429) responses gracefully — the ranked list is still returned with a localised fallback message instead of the AI explanation.

---

## 🚢 VPS Deployment (student-dev.ro)

The project targets the `studlab_net` shared Docker network used by student-dev.ro. The VPS Nginx reverse proxy routes `motoadvisor.student-dev.ro` to the frontend container on port 80; the frontend's own nginx proxies `/api/*` and `/images/*` inward to the API container.

```
Internet
  → VPS nginx (HTTPS, motoadvisor.student-dev.ro)
    → student-glosper-frontend :80  (nginx, serves React SPA)
      → /api/*    proxy_pass → student-glosper-api :8080
      → /images/* proxy_pass → student-glosper-api :8080
```

**First deploy:**

```bash
ssh user@student-dev.ro
cd ~/MotoAdvisor
cp .env.example .env && nano .env      # fill in secrets
mkdir -p ~/photos
docker compose up -d --build
```

**Subsequent deploys:**

```bash
git pull
docker compose up -d --build --no-deps api   # rebuild only changed service
```

**Useful commands:**

```bash
docker compose ps                      # check container status
docker compose logs -f api             # tail API logs
docker compose exec db psql -U motoadvisor motoadvisor   # DB shell
docker compose down                    # stop (data volumes preserved)
docker compose down -v                 # stop + wipe volumes (⚠️ destructive)
```

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).
