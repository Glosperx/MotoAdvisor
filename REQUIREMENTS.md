# MotoAdvisor - DAW Project Requirements

## Tech Stack
- Backend: .NET 8, ASP.NET Core Web API, Entity Framework Core 8
- Auth: ASP.NET Core Identity + JWT
- DB: PostgreSQL (running in Podman on localhost:5432, db=motoadvisor, user=postgres, pass=postgres)
- Frontend: React 18 + TypeScript, React Router, Axios
- Extra: Google Gemini API for RAG (text-embedding-004 + cosine similarity)
- Architecture: Classic (Controllers + Services + Repositories)

## Entities (8 total)
- ApplicationUser (extends IdentityUser)
- AspNetRoles (Identity)
- Brand (Id, Name, Country, LogoUrl)
- Category (Id, Name, Description)
- Motorcycle (Id, Name, Year, Price, Engine, Power, BrandId, CategoryId, Description, EmbeddingVector)
- MotorcycleImage (Id, MotorcycleId, ImageUrl, IsMain)
- Review (Id, MotorcycleId, UserId, Rating, Content, CreatedAt)
- UserFavorite (UserId, MotorcycleId) -- Many-to-Many

## Relations
- Brand -> Motorcycle (One-to-Many)
- Category -> Motorcycle (One-to-Many)
- Motorcycle -> MotorcycleImage (One-to-Many)
- Motorcycle -> Review (One-to-Many)
- User <-> Motorcycle via UserFavorite (Many-to-Many)

## API Controllers (minimum 5)
- AuthController: POST /api/auth/register, POST /api/auth/login
- MotorcyclesController: full CRUD + GET /api/motorcycles/search?q=
- BrandsController: full CRUD
- CategoriesController: full CRUD
- ReviewsController: CRUD + GET /api/motorcycles/{id}/reviews
- FavoritesController: GET/POST/DELETE /api/favorites
- RecommendController: POST /api/recommend (RAG endpoint)

## Auth Requirements
- JWT Bearer tokens
- Roles: Admin, User
- Admin-only: POST/PUT/DELETE motorcycles, brands, categories
- User: add reviews, favorites

## Architecture
- MotoAdvisor.Core: Entities, DTOs, Interfaces
- MotoAdvisor.Infrastructure: DbContext, Repositories, Services
- MotoAdvisor.API: Controllers, Program.cs, Middleware

## Frontend Pages (5 required)
1. Home - catalog motociclete cu filtre (brand, category, price)
2. MotorcycleDetail - galerie, specs, reviews, buton favorite
3. Login / Register
4. Profile - favoritele userului
5. Admin Dashboard - CRUD motorcycles/brands/categories
6. Search - rezultate cautare naturala (RAG)

## Extra Feature
RAG system: la POST /api/recommend, query-ul e embeds cu Gemini text-embedding-004,
cosine similarity in-memory cu toate motorcycle embeddings, returneaza top 5 + raspuns generat.
