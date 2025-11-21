# Delivery Backend API (Ocean Professional)

A .NET 8 Web API for a delivery application. Provides endpoints for:
- User registration and login (placeholder authentication)
- Order placement and management
- Order tracking

Runs on port 3001 and exposes OpenAPI docs at /docs.

## Quick start

- Requirements: .NET 8 SDK
- From the delivery_backend directory:

```bash
dotnet restore
dotnet run
```

Service will listen on:
- http://localhost:3001
- Swagger UI: http://localhost:3001/docs
- OpenAPI JSON: http://localhost:3001/openapi.json

## Health

- GET / => { status: "healthy", ... }

## Endpoints

- Users
  - POST /api/users/register
    - body: { name, email, password }
    - 201 Created => user profile
  - POST /api/users/login
    - body: { email, password }
    - 200 OK => user profile
  - GET /api/users/{id}
    - 200 OK => user profile

- Orders
  - POST /api/orders
    - body: { userId, origin, destination, package: { description, weightKg, lengthCm, widthCm, heightCm } }
    - 201 Created => order
  - GET /api/orders/{id}
    - 200 OK => order
  - GET /api/orders/user/{userId}
    - 200 OK => order[]
  - PATCH /api/orders/{id}/status
    - body: { status: Created|Assigned|InTransit|Delivered|Cancelled }
    - 200 OK => order

- Tracking
  - GET /api/tracking/{orderId}
    - 200 OK => trackingEvent[]

## Architecture

- Controllers: API entrypoints with OpenAPI metadata
- Services: Business logic
- Repositories: In-memory storage via interfaces for future DB swap
- Models/DTOs: Entities and transport models
- Middleware: Global error handler

## Notes
- Authentication is placeholder only and not suitable for production.
- No environment variables required at this stage.
- Style: Ocean Professional (primary #2563EB, secondary #F59E0B)

