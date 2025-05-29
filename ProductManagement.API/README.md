# ProductManagement.API

This is the backend API for the Product Management application built with ASP.NET Core 8.0.

## Prerequisites

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- MySQL Server (version 8.0 recommended)
- Visual Studio, Visual Studio Code, or any preferred IDE with C# support

## Database Setup

1. Install and configure MySQL Server
2. Create a database named `productManagement`
3. Update the connection string in `appsettings.json` if needed:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=productManagement;User=root;Password=1234"
}
```

## Installation

1. Clone the repository
2. Navigate to the API project directory:

```bash
cd ProductManagement.API
```

3. Restore NuGet packages:

```bash
dotnet restore
```

4. Apply database migrations:

```bash
dotnet ef database update
```

## Running the Application

Start the API server:

```bash
dotnet run
```

The API will be available at:

- API: https://localhost:7134 (HTTPS) or http://localhost:5134 (HTTP)
- Swagger UI: https://localhost:7134/swagger (HTTPS) or http://localhost:5134/swagger (HTTP)

## Development

### Adding a New Migration

When you make changes to the data models, create a new migration:

```bash
dotnet ef migrations add [MigrationName]
```

### Updating the Database

Apply pending migrations to the database:

```bash
dotnet ef database update
```

## Key Dependencies

- Entity Framework Core (MySQL Provider)
- JWT Authentication
- Swagger/OpenAPI
- Serilog for logging
- BCrypt for password hashing

## Configuration

The application uses the following configuration sections:

- `ConnectionStrings`: Database connection
- `Jwt`: Authentication settings
- `Logging`: Log configuration

For development-specific settings, modify `appsettings.Development.json`.
