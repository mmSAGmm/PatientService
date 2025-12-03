# Patient Service API

A RESTful API service for managing patient information built with .NET 8.0. This service provides CRUD operations for patient data with advanced search capabilities, observability features, and containerization support.

## Features

- **Patient Management**: Full CRUD operations (Create, Read, Update, Delete) for patient records
- **Advanced Search**: Pattern-based patient search functionality
- **API Documentation**: Swagger/OpenAPI integration for interactive API exploration
- **Observability**: OpenTelemetry integration with Prometheus metrics and distributed tracing
- **Health Checks**: Actuator endpoints for application health and metrics
- **Validation**: FluentValidation for request validation
- **Docker Support**: Containerized deployment with Docker and Docker Compose
- **MySQL Database**: Persistent storage using MySQL

## Technology Stack

- **.NET 8.0** - Web API framework
- **MySQL** - Database
- **AutoMapper** - Object-to-object mapping
- **FluentValidation** - Request validation
- **OpenTelemetry** - Observability (tracing, metrics, logging)
- **Swashbuckle (Swagger)** - API documentation
- **Steeltoe Management** - Health checks and actuator endpoints
- **Docker** - Containerization

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) and Docker Compose (optional, for containerized deployment)
- MySQL (if running without Docker)

## Getting Started

### Option 1: Using Docker Compose (Recommended)

The easiest way to run the application is using Docker Compose, which sets up both the API and MySQL database:

```bash
docker-compose up -d
```

This will:
- Build and start the Patient Service API on port `8080`
- Start a MySQL database on port `3306`
- Initialize the database schema automatically

The API will be available at: `http://localhost:8080`

### Option 2: Local Development

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd test_agsr
   ```

2. **Set up MySQL database**
   
   Make sure MySQL is running and create a database. You can use the initialization script:
   ```bash
   mysql -u root -p < my-init-scripts/init.sql
   ```

3. **Configure the connection string**
   
   Update `src/PatientService/appsettings.json` with your MySQL connection string:
   ```json
   {
     "MySql:CONNECTIONSTRING": "Server=localhost;Port=3306;Database=test;Uid=root;Pwd=yourpassword;"
   }
   ```

4. **Restore dependencies**
   ```bash
   dotnet restore
   ```

5. **Build the solution**
   ```bash
   dotnet build
   ```

6. **Run the application**
   ```bash
   dotnet run --project src/PatientService/PatientService.csproj
   ```

The API will start on the port configured in `launchSettings.json` (typically `http://localhost:5000` or `https://localhost:5001`).

## Configuration

### Environment Variables

You can override configuration using environment variables:

- `ASPNETCORE_URLS` - The URL where the API listens (default: `http://+:8080` in Docker)
- `MySql__CONNECTIONSTRING` - MySQL connection string

### Docker Compose Environment

In `docker-compose.yml`, the MySQL connection string is set via environment variable:
```yaml
MySql__CONNECTIONSTRING: "Server=db;Port=3306;Database=test;Uid=root;Pwd=example;"
```

## API Endpoints

### Base URL
- Local: `http://localhost:8080` (Docker) or `http://localhost:5000` (local)
- Base path: `/api/v1/patient`

### Endpoints

#### 1. Create Patient
- **POST** `/api/v1/patient`
- **Request Body**: Patient information (use, family, given, gender, birthDate, active)
- **Response**: `201 Created` with created patient data

#### 2. Get Patient by ID
- **GET** `/api/v1/patient/{id}`
- **Parameters**: `id` (Guid)
- **Response**: `200 OK` with patient data or `204 No Content` if not found

#### 3. Search Patients
- **GET** `/api/v1/patient/search?pattern={pattern}&pattern={pattern}...`
- **Parameters**: `pattern` (string array) - Search patterns
- **Response**: `200 OK` with matching patients

#### 4. Update Patient
- **PUT** `/api/v1/patient`
- **Request Body**: Updated patient information
- **Response**: `200 OK` on success

#### 5. Delete Patient
- **DELETE** `/api/v1/patient/{id}`
- **Parameters**: `id` (Guid)
- **Response**: `200 OK` on success

## API Documentation

Once the application is running, you can access:

- **Swagger UI**: `http://localhost:8080/swagger` (or your configured port)
- **Prometheus Metrics**: `http://localhost:8080/metrics`
- **Health/Actuator Endpoints**: Configured via Steeltoe Management

## Project Structure

```
test_agsr/
├── src/
│   ├── PatientService/          # Main API application
│   ├── Patient.Domain/          # Domain layer with business logic
│   ├── Patient.DomainModels/    # Domain models
│   ├── DbDataAcess/             # Data access layer
│   └── GenerateApp/             # Utility application
├── Patient.Domain.Tests/        # Unit tests
├── my-init-scripts/             # Database initialization scripts
│   └── init.sql
├── postman/                     # Postman collection for API testing
│   └── PatientCollection.postman_collection.json
├── docker-compose.yml           # Docker Compose configuration
├── Dockerfile                   # Docker image definition
└── PatientService.sln          # Solution file
```

## Testing

### Using Postman

A Postman collection is available in `postman/PatientCollection.postman_collection.json` for testing the API endpoints.

### Running Tests

```bash
dotnet test
```

## Development

### Building

```bash
dotnet build PatientService.sln
```

### Running in Development Mode

```bash
cd src/PatientService
dotnet watch run
```

The `watch` command enables hot reload for faster development.

## Docker

### Building the Docker Image

```bash
docker build -t patient-service .
```

### Running with Docker

```bash
docker run -p 8080:8080 \
  -e MySql__CONNECTIONSTRING="Server=host.docker.internal;Port=3306;Database=test;Uid=root;Pwd=example;" \
  patient-service
```

## Observability

The application includes OpenTelemetry instrumentation for:

- **Tracing**: Distributed tracing for ASP.NET Core and HTTP client calls
- **Metrics**: Prometheus metrics export endpoint
- **Logging**: Structured logging support

Access metrics at: `http://localhost:8080/metrics`

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines on contributing to this project.

## Support

For issues and questions, please open an issue in the repository.
