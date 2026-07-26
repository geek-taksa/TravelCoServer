# TravelCo

Final project for the Server-Side Internet Information Systems course — a "countries of the world" web application. Users browse and search countries, keep personal Visited / Want-to-visit lists, share trips, play timed quizzes, and manage their profile. Includes an admin dashboard.

## Features
- User registration, login, logout, and profile with preferences (continents, spoken languages + levels)
- Countries: browse, search, filter (region / language / currency), sort, and full detail pages
- Country data imported server-side from the [countries.dev](https://countries.dev) API and stored in the DB
- Country records CRUD (admin)
- Personal lists: Visited and Want-to-visit (add / remove / move)
- Community sharing: post, edit, and delete trip thoughts/recommendations/reviews
- Two time-limited quizzes with server-side scoring and points
- Admin: user management (lock/unlock, block sharing) and usage statistics

## Tech stack
- **Client:** HTML, CSS, JavaScript (no framework)
- **Server:** ASP.NET Core Web API (.NET 6)
- **Database:** Microsoft SQL Server (access via ADO.NET + stored procedures)
- **Auth:** JWT (bearer tokens); passwords stored salted + hashed (PBKDF2)

## Architecture
Three-tier separation:
- **Controllers** — HTTP layer (no SQL, no business rules)
- **Services** — business logic
- **Repositories** — ADO.NET data access; all SQL lives in stored procedures

## Project structure
## Getting started

### Prerequisites
- .NET 6 SDK and Visual Studio 2022
- Access to a Microsoft SQL Server instance

### 1. Database
1. Run `Database/TravelCo_Schema.sql` to create the tables.
2. Run `Database/StoredProcedures.sql` to create the stored procedures.
3. (Optional) Seed the two quizzes.
4. Populate countries by calling the admin import endpoint `POST /api/countries/import` (or from Swagger), which pulls all countries from countries.dev.

### 2. Server
1. Create `appsettings.json` in the server root (it is git-ignored). Use this template and fill in your connection string:
```json
   {
     "Logging": { "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" } },
     "AllowedHosts": "*",
     "ConnectionStrings": {
       "myProjDB": "Data Source=YOUR_SERVER;Initial Catalog=YOUR_DB;User ID=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True"
     },
     "Jwt": {
       "Key": "a-long-random-secret-at-least-32-characters",
       "Issuer": "TravelCoServer",
       "Audience": "TravelCoClient"
     }
   }
```
2. Open the solution in Visual Studio and run (IIS Express / Kestrel). Swagger opens at `/swagger`.

### 3. Client
1. In `client/js/config.js`, set `BASE_URL` to your running server's URL, e.g. `https://localhost:7000`.
2. Serve the `client` folder over HTTP (e.g. via IIS Express / Live Server) — not by opening the file directly, because it uses ES modules.

### Making an admin
New users are created with the `user` role. To grant admin rights, update the row in SQL:
```sql
UPDATE TravelCo_Users SET Role = 'admin' WHERE Email = 'your-email';
```
Then log in again to get a token carrying the admin role.

## Deployment
The client talks to the server through a single `BASE_URL` in `client/js/config.js`:
- **Local development:** `BASE_URL = "https://localhost:7000"` (the running backend)
- **Deployment:** `BASE_URL` = the deployed backend's base URL (e.g. `https://<host>/<path>/tar1`)

## Author
Maria Dotsenko
