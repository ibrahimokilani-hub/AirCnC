# Hotel Booking System

A hotel booking REST API built with **.NET 10** and **Clean Architecture**. Guests search hotels, book rooms and manage their bookings; admins manage cities, hotels and rooms.

> 🚧 Work in progress: an internship final project, built increment by increment.

## Table of Contents

- [About the Project](#about-the-project)
- [Built With](#built-with)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [Project Structure](#project-structure)
- [Running the Tests](#running-the-tests)
- [Roadmap](#roadmap)

## About the Project

### Features

- **Clean Architecture**: dependencies point inward only (Api → Infrastructure → Core → Contracts)
- **CQRS**: every use case is a command or a query with its own handler
- **Result pattern**: expected failures (not found, conflict, validation) are returned, not thrown
- **One response shape**: `{ "data": … }` for success, `{ "status", "message" }` for errors
- **Validation** with FluentValidation, returned as `400` with per-field `errors`
- **Auditing and soft delete**: created/updated by and at are set automatically; deleted rows are hidden, not removed
- **Structured logging** with Serilog
- **Global exception handling**: unexpected errors become a logged `500` with the same error shape

### Response format

```jsonc
// success
{ "data": { "id": 1, "name": "Jenin", "country": "Palestine" } }

// failure
{ "status": 404, "message": "The city with ID '99' was not found." }
```

## Built With

| Area | Technology |
|---|---|
| Framework | .NET 10, ASP.NET Core (controllers) |
| Database | SQL Server 2022 (Docker), EF Core 10 |
| Validation | FluentValidation |
| Logging | Serilog |
| API docs | Swagger (Swashbuckle) |
| Tests | xUnit, NSubstitute, NetArchTest |

## Getting Started

### Prerequisites

- [.NET SDK 10.0.400+](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Installation

1. Create a `.env` file in the repository root with the SQL Server password:

   ```
   MSSQL_SA_PASSWORD=<a strong password>
   ```

2. Start SQL Server (it listens on port `1434`):

   ```bash
   docker compose up -d
   ```

3. Store the connection string in user-secrets, for both the API and the migrations tool:

   ```bash
   dotnet user-secrets set "ConnectionStrings:HotelBookingDb" "Server=localhost,1434;Database=HotelBookingSystem;User Id=sa;Password=<password>;TrustServerCertificate=True" --project src/HotelBooking.Api
   dotnet user-secrets set "ConnectionStrings:HotelBookingDb" "Server=localhost,1434;Database=HotelBookingSystem;User Id=sa;Password=<password>;TrustServerCertificate=True" --project src/HotelBooking.Infrastructure
   ```

4. Create the database:

   ```bash
   dotnet ef database update --project src/HotelBooking.Infrastructure --startup-project src/HotelBooking.Infrastructure
   ```

5. Run the API:

   ```bash
   dotnet run --project src/HotelBooking.Api
   ```

   Swagger: http://localhost:5209/swagger

## API Endpoints

Base URL: `/api/v1`

### Cities (admin)

| Method | Route | Description | Success |
|---|---|---|---|
| `POST` | `/admin/cities` | Create a city | `201` |
| `GET` | `/admin/cities/{id}` | Get a city by id | `200` |
| `PUT` | `/admin/cities/{id}` | Update a city's name, country and post office | `204` |
| `DELETE` | `/admin/cities/{id}` | Delete a city (soft delete) | `204` |
| `GET` | `/admin/cities` | 🚧 Paged, searchable, sortable list | `200` |

Common errors: `400` invalid input · `404` not found · `409` duplicate city.

## Project Structure

```
src/
  HotelBooking.Api             → controllers, middleware, Swagger, startup
  HotelBooking.Core            → Domain (entities, rules) + Application (commands, queries, handlers)
  HotelBooking.Infrastructure  → EF Core, SQL Server, migrations, auditing interceptor
  HotelBooking.Contracts       → request and response DTOs, no dependencies
tests/
  HotelBooking.Tests           → Unit/, Integration/, Architecture/
docs/                          → build guide and reference (architecture, database, decisions)
```

## Running the Tests

```bash
dotnet test
```

## Roadmap

- [x] Foundation: solution, Result/Error, EF Core, create and get a city
- [x] Response envelope, logging, global exception handling
- [x] Auditing and soft delete; update and delete a city
- [ ] Admin grid for cities (paging, search, sort)
- [ ] Authentication: Identity, JWT, roles, refresh tokens
- [ ] Hotels, room types, rooms, amenities
- [ ] Search and hotel page
- [ ] Cart, checkout, bookings
- [ ] Permissions, caching, home page, reviews
- [ ] Images (Cloudinary), confirmation email, PDF invoice
- [ ] Docker image and CI
