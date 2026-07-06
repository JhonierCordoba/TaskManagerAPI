# Task Manager API

A RESTful API for managing projects and tasks, built with .NET 9 and Clean Architecture principles.

## Tech Stack

- **Framework:** .NET 9 / ASP.NET Core
- **Architecture:** Clean Architecture + CQRS with MediatR
- **ORM:** Entity Framework Core 9 (writes) + Dapper (reads)
- **Database:** SQL Server
- **Auth:** JWT Bearer tokens
- **Validation:** FluentValidation with pipeline behavior
- **Docs:** Scalar UI (OpenAPI)

## Architecture

The solution is divided into 4 layers with strict dependency rules — dependencies only point inward:

```
TaskManager.API           → Entry point, controllers, middleware
TaskManager.Application   → Commands, queries, handlers, validators
TaskManager.Infrastructure → EF Core, Dapper, repositories, JWT
TaskManager.Domain        → Entities, interfaces, enums
```

### CQRS Pattern

All operations are split into two paths:

- **Commands** — write operations (create, update). Handled by EF Core + Unit of Work.
- **Queries** — read operations (list, get by id). Handled by Dapper for optimal performance.

MediatR routes each request to its corresponding handler automatically.

## Getting Started

### Prerequisites

- .NET 9 SDK
- SQL Server (local or Docker)

### Setup

1. Clone the repository

```bash
git clone https://github.com/your-username/TaskManagerAPI.git
cd TaskManagerAPI
```

2. Configure the connection string and JWT settings in `TaskManager.API/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TaskManagerDB;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "your-secret-key-minimum-32-characters",
    "Issuer": "TaskManagerAPI",
    "Audience": "TaskManagerClient"
  }
}
```

3. Apply migrations

```bash
dotnet ef database update --project TaskManager.Infrastructure --startup-project TaskManager.API
```

4. Run the API

```bash
dotnet run --project TaskManager.API
```

5. Open the Scalar UI at `https://localhost:{port}/scalar/v1`

## API Endpoints

### Auth
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/auth/register` | Register a new user | No |
| POST | `/api/auth/login` | Login and get JWT token | No |

### Projects
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/projects` | List all user projects | Yes |
| GET | `/api/projects/{id}` | Get project by id | Yes |
| POST | `/api/projects` | Create a new project | Yes |
| PUT | `/api/projects/{id}` | Update a project | Yes |

### Tasks
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/tasks/project/{projectId}` | List tasks by project | Yes |
| POST | `/api/tasks` | Create a new task | Yes |
| PATCH | `/api/tasks/{id}/status` | Update task status | Yes |

### Query filters for tasks

```
GET /api/tasks/project/{projectId}?status=Pending&priority=High
```

Available values:
- `status`: `Pending`, `InProgress`, `Completed`
- `priority`: `Low`, `Medium`, `High`

## Key Concepts

**ValidationBehavior** — A MediatR pipeline behavior that runs FluentValidation automatically before every command reaches its handler. No validation code needed in handlers.

**Global Exception Middleware** — Catches all unhandled exceptions and maps them to the correct HTTP status code (`404`, `403`, `400`, `409`, `500`) with a consistent JSON response format.

**Unit of Work** — Wraps EF Core's `SaveChangesAsync` so all write operations in a command either commit together or roll back entirely.

**Private setters on entities** — Domain entities can only be modified through explicit methods (`Update()`, `ChangeStatus()`), preventing invalid state from outside the domain layer.

## Project Structure

```
src/
├── TaskManager.API/
│   ├── Controllers/
│   ├── Extensions/
│   └── Middleware/
├── TaskManager.Application/
│   ├── Common/
│   │   ├── Behaviors/
│   │   ├── Exceptions/
│   │   └── Interfaces/
│   ├── DTOs/
│   └── Features/
│       ├── Auth/
│       ├── Projects/
│       └── Tasks/
├── TaskManager.Domain/
│   ├── Entities/
│   ├── Enums/
│   └── Interfaces/
└── TaskManager.Infrastructure/
    ├── Persistence/
    │   ├── Configurations/
    │   └── Repositories/
    └── Services/
```