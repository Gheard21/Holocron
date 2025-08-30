# Holocron

Full‑stack app with a .NET 9 Web API and a Vite + Vue 3 client. The API can serve the built client from `wwwroot` for a single‑process deploy, or you can run both in dev mode.

## Prerequisites

-   .NET 9 SDK
-   Node.js 18+ and npm
-   SQLite (bundled with .NET provider; no separate service needed)

## Quick start (serve built client from API)

1. Clone and open
    - git clone git@github.com:Gheard21/Holocron.git
    - cd Holocron
2. Build the client
    - cd App/client
    - npm ci
    - npm run build
3. Run the API
    - cd ../Api
    - dotnet run

API will start on:

-   HTTP: http://localhost:5275
-   HTTPS: https://localhost:7052

The API serves static files from `wwwroot`, so the built client is available at the root path.

## Dev workflow (hot reload)

-   Terminal A (client):
    -   cd App/client
    -   npm ci
    -   npm run dev
    -   Vite dev server: http://localhost:3000
-   Terminal B (API):
    -   cd App/Api
    -   dotnet watch

Tip: During dev, use the Vite server directly. The API will still be available on 5275/7052 for API calls.

## API endpoints (summary)

Base URL: `/api`

Likes

-   GET `/likes/{name}` → returns count (anonymous)
-   GET `/likes/{name}/me` → returns bool if current user liked (auth)
-   POST `/likes/{name}` → like (auth)
-   DELETE `/likes/{name}` → unlike (auth)

Comments

-   GET `/comments/{name}` → list comments for name (anonymous)
-   GET `/comments/{name}/count` → count comments (anonymous)
-   GET `/comments/{name}/me` → has current user commented (auth)
-   POST `/comments` → create comment (auth)

Request: POST `/comments`

```
{
	"name": "Luke Skywalker",
	"dateWatched": "2025-08-29T00:00:00Z",
	"rating": 8,
	"review": "Great!"
}
```

Validation:

-   name: required, <= 100 chars
-   dateWatched: required, not in future
-   rating: 1–10
-   review: required, <= 2000 chars

## Authentication

JWT (Auth0) is configured in `Program.cs`:

-   Authority: https://dev-z9o0q5dh.eu.auth0.com/
-   Audience: https://holocrononline.org

Anonymous endpoints are noted above; others require a Bearer token.

## Database

SQLite file is at `App/Api/Data/holocron.db`. On first run in Development, migrations are applied automatically.

Run EF Core tooling (optional):

-   dotnet tool install --global dotnet-ef
-   cd App/Api
-   dotnet ef migrations add SomeChange
-   dotnet ef database update

Tenancy: `TenantId` is auto‑populated on insert for tenant‑scoped entities using the current user id.

## Testing

-   dotnet test

## Troubleshooting

-   Port in use: adjust ports in `App/Api/Properties/launchSettings.json` or Vite `vite.config.mts`.
-   Missing client assets when running API: ensure `npm run build` in `App/client` before `dotnet run`.
