# CRN Technical Assessment - RESTful Backend API

## Overview

This project is a RESTful Backend API developed as part of the CRN Technical Assessment.

The API provides Product management functionality using CRUD operations and follows a layered architecture with separation of responsibilities.

### Main Features

- Product CRUD operations
- Item management with Product relationship
- JWT Authentication
- Refresh Token Rotation
- Role-Based Authorization (RBAC)
- FluentValidation
- API Versioning
- Pagination
- Repository and Service Layer
- Entity Framework Core
- SQL Server
- Structured Logging
- Global Exception Handling
- Security Headers
- CORS
- Response Compression
- Swagger / OpenAPI documentation
- Unit Tests
- API Integration Tests
- Infrastructure Tests
- Docker and Docker Compose support

## Technology Stack

- C#
- ASP.NET Core Web API
- .NET 10
- Entity Framework Core
- SQL Server
- JWT Bearer Authentication
- FluentValidation
- Swagger / OpenAPI
- xUnit
- Moq
- WebApplicationFactory
- Docker
- Docker Compose



## Architecture

The project follows a layered architecture to separate business logic, data access, API handling, and domain models.

```text
SDTechnicalAssessment
│
├── Application
│   ├── DTOs
│   ├── Interfaces
│   ├── Services
│   ├── Validators
│   └── Mapping
│
├── Domain
│   ├── Entities
│   ├── Enums
│   ├── Events
│   └── Exceptions
│
├── Infrastructure
│   ├── Data
│   │   ├── Configurations
│   │   └── Repositories
│   ├── Identity
│   ├── Logging
│   └── Security
│
├── Controllers
├── Middleware
├── Program.cs
├── appsettings.json
├── Dockerfile
└── README.md
```

### Layer Responsibilities

**API Layer**

Contains controllers, middleware, and application startup configuration. It handles HTTP requests and responses.

**Application Layer**

Contains DTOs, interfaces, validators, and service classes. It contains the application's business logic and coordinates operations between the API and Infrastructure layers.

**Domain Layer**

Contains the core entities used by the application, such as `Product`, `Item`, `User`, and `RefreshToken`.

**Infrastructure Layer**

Handles database access, repositories, Entity Framework Core configuration, refresh-token hashing, and other external infrastructure concerns.

### Request Flow

```text
Client
  ↓
Controller
  ↓
Application Service
  ↓
Repository
  ↓
Entity Framework Core
  ↓
SQL Server
```

Authentication requests follow:

```text
Client
  ↓
AuthController
  ↓
AuthService
  ↓
UserRepository
  ↓
SQL Server

AuthService
  ↓
JWT Access Token
  +
Refresh Token
```

This separation improves maintainability, testability, and separation of concerns.


## API Endpoints

### Authentication

| Method | Endpoint            | Description                              | Authorization |
| ------ | ------------------- | ---------------------------------------- | ------------- |
| POST   | `/api/Auth/login`   | Login and generate access/refresh tokens | Public        |
| POST   | `/api/Auth/refresh` | Generate new access/refresh tokens       | Public        |

### Products

The Product APIs are versioned using API version `1.0`.

| Method | Endpoint                | Description            | Authorization |
| ------ | ----------------------- | ---------------------- | ------------- |
| GET    | `/api/v1/Products`      | Get paginated products | User/Admin    |
| GET    | `/api/v1/Products/{id}` | Get product by ID      | User/Admin    |
| POST   | `/api/v1/Products`      | Create a product       | Admin         |
| PUT    | `/api/v1/Products/{id}` | Update a product       | Admin         |
| DELETE | `/api/v1/Products/{id}` | Delete a product       | Admin         |

### Pagination

The Product list API supports pagination using query parameters:

```text
GET /api/v1/Products?pageNumber=1&pageSize=10
```

Example response structure:

```json
{
  "data": [],
  "pageNumber": 1,
  "pageSize": 10,
  "totalRecords": 0,
  "totalPages": 0
}
```

## Authentication Flow

1. Call the login endpoint with valid username and password.
2. The API validates the user's credentials.
3. The API returns a short-lived JWT access token and a refresh token.
4. Use the access token in the HTTP `Authorization` header.
5. When the access token expires, send the refresh token to the refresh endpoint.
6. The old refresh token is revoked.
7. A new access token and refresh token are generated.
8. A revoked refresh token cannot be reused.

### Authorization Header

For protected Product APIs, send the JWT access token as:

```text
Authorization: Bearer <access-token>
```

### Role-Based Authorization

The application supports two roles:

* **Admin** — Can perform Product CRUD operations.
* **User** — Can view Products but cannot create, update, or delete Products.

Unauthorized requests return `401 Unauthorized`.

Authenticated users without the required role receive `403 Forbidden`.

## Swagger

Swagger/OpenAPI is available during development.

For local execution, open:

```text
https://localhost:<port>/swagger
```

When running the API through Docker with the configured port mapping, Swagger can be accessed at:

```text
http://localhost:8081/swagger
```

The Swagger UI supports JWT Bearer authentication through the **Authorize** button.
## Database

The application uses Microsoft SQL Server with Entity Framework Core.

### Database Tables

#### Product

* `Id` - Primary Key
* `ProductName` - Product name
* `CreatedBy` - User who created the product
* `CreatedOn` - Creation timestamp
* `ModifiedBy` - User who last modified the product
* `ModifiedOn` - Last modification timestamp

#### Item

* `Id` - Primary Key
* `ProductId` - Foreign Key to Product
* `Quantity` - Item quantity

#### User

* `Id` - Primary Key
* `Username` - Unique username
* `Password` - Hashed password
* `Role` - User role

#### RefreshToken

* `Id` - Primary Key
* `UserId` - Foreign Key to User
* `Token` - SHA-256 hashed refresh token
* `CreatedOn` - Token creation time
* `ExpiresOn` - Token expiry time
* `RevokedOn` - Token revocation time

## Local Setup

### Prerequisites

Install the following before running the application locally:

* Visual Studio
* .NET 10 SDK
* SQL Server LocalDB
* Git

### Database Configuration

The local application uses SQL Server LocalDB.

The connection string is configured in `appsettings.json`:

```text
Server=(localdb)\MSSQLLocalDB;
Database=CRNTechnicalAssessmentDb;
Trusted_Connection=True;
TrustServerCertificate=True;
```

### Apply Database Migrations

Open Package Manager Console and run:

```powershell
Update-Database
```

This creates/updates the required database tables using Entity Framework Core migrations.

### Run the Application

Run the application from Visual Studio or using:

```powershell
dotnet run
```

Swagger UI will be available at:

```text
https://localhost:<port>/swagger
```

## Docker Setup

The project includes Docker support using `Dockerfile` and `docker-compose.yml`.

### Docker Services

Docker Compose starts two services:

1. **SQL Server**
2. **ASP.NET Core API**

The API connects to the SQL Server container using the Docker service name:

```text
Server=sqlserver
```

### Build and Start Containers

From the solution folder containing `docker-compose.yml`, run:

```powershell
docker compose up --build
```

### Check Running Containers

```powershell
docker ps
```

Expected containers include:

```text
crn-api
crn-sqlserver
```

### Docker Swagger

With the configured Docker port mapping, Swagger UI is available at:

```text
http://localhost:8081/swagger
```

### Stop Containers

Press `Ctrl+C` in the terminal running Docker Compose, or use:

```powershell
docker compose down
```

The SQL Server data is stored in the Docker volume:

```text
sqlserver-data
```

## Environment Configuration

For local development, database and JWT configuration are stored in `appsettings.json`.

For production deployments, sensitive values such as:

* JWT signing key
* SQL Server password
* Database connection string

should be supplied through environment variables, secret stores, or deployment configuration rather than committed to source control.

## Testing

The solution contains separate test projects for different application layers.

```text
tests
├── SDTechnicalAssessment.Application.Tests
├── SDTechnicalAssessment.API.Tests
└── SDTechnicalAssessment.Infrastructure.Tests
```

### Application Unit Tests

Application service logic is tested using:

* xUnit
* Moq

These tests verify product creation, retrieval, update, deletion, pagination, and not-found scenarios.

### API Integration Tests

API integration tests use:

* xUnit
* WebApplicationFactory
* HttpClient

These tests verify:

* Login
* JWT authentication
* Refresh token flow
* Refresh token rotation
* Unauthorized requests
* Product CRUD endpoints
* Role-based authorization
* Validation responses

### Infrastructure Tests

Infrastructure tests verify repository behavior and refresh-token hashing.

### Run Tests

From Visual Studio:

```text
Test → Test Explorer → Run All Tests
```

Or from the solution directory:

```powershell
dotnet test
```

## Security

The application implements the following security measures:

* JWT Bearer authentication
* Short-lived access tokens
* Refresh token rotation
* SHA-256 hashing for stored refresh tokens
* Password hashing using ASP.NET Core `PasswordHasher`
* Role-Based Access Control
* FluentValidation for input validation
* Global exception handling
* Security response headers
* CORS configuration
* HTTPS redirection
* JWT issuer and audience validation
* JWT lifetime validation
* Zero clock skew for token validation

### Security Headers

The API adds security headers including:

* `X-Content-Type-Options`
* `X-Frame-Options`
* `Referrer-Policy`
* `Permissions-Policy`

## Error Handling

Unhandled exceptions are captured by global exception middleware.

The API returns a consistent JSON response for unexpected server errors instead of exposing internal exception details.

Example:

```json
{
  "statusCode": 500,
  "message": "An unexpected error occurred."
}
```

## Performance Considerations

The API includes several performance improvements:

* Asynchronous database operations
* `AsNoTracking()` for read-only queries
* Pagination for Product listing
* Database indexes for frequently queried fields
* Response compression
* Repository and service separation

## Future Improvements

The following improvements can be considered for a production deployment:

* Move secrets to a secure secret-management system
* Add centralized logging such as Serilog with an external log store
* Add database health checks
* Add Docker health checks and startup retry logic
* Add automated CI/CD using GitHub Actions
* Add additional API and repository test coverage
* Add production-specific configuration
* Add rate limiting
