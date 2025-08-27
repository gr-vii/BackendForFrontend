# Payment Gateway BFF (Backend-for-Frontend)

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)](https://github.com/your-org/payment-gateway-bff)

A high-performance Backend-for-Frontend service built with .NET 8 that provides a unified, resilient API for payment operations while interfacing with third-party payment providers. Designed to handle 1000+ requests per second with enterprise-grade reliability patterns.

## 🚀 Features

### Core Functionality
- **🔐 Authentication API**: Secure user login with JWT token generation
- **💳 Payment Processing**: Real-time payment creation and processing
- **📊 API Versioning**: Support for multiple API versions (v1, v2) with deprecation handling
- **🛡️ Security**: Input validation, secure headers, and correlation tracking

### Reliability & Performance
- **🔄 Resiliency Patterns**: Circuit breaker, retry policies, and timeout handling using Polly
- **📈 High Performance**: Optimized for 1000+ RPS with minimal latency
- **🏥 Health Checks**: Comprehensive health monitoring for dependencies
- **📝 Observability**: Structured logging with Serilog and correlation IDs

### Developer Experience
- **📖 OpenAPI/Swagger**: Interactive API documentation
- **🧪 Testing**: Unit tests, integration tests, and benchmarks
- **🚀 Load Testing**: K6 scripts for performance validation
- **🐳 Docker Ready**: Containerization support

## 🏗️ Architecture

### Clean Architecture Layers

```
┌─────────────────────────────────────┐
│            WebApi Layer             │  ← Controllers, Middleware, Configuration
├─────────────────────────────────────┤
│          Application Layer          │  ← Commands, Queries, Handlers, Validators
├─────────────────────────────────────┤
│        Infrastructure Layer        │  ← External Services, HTTP Clients, Resilience
└─────────────────────────────────────┘
```

### Request Flow

```
Client Request → Controller → MediatR → Command Handler → Provider Client → External API
                     ↓              ↓            ↓              ↓
                Validation    → Business Logic → Resilience → Response Mapping
```

## 🛠️ Technology Stack

- **Framework**: .NET 8 (ASP.NET Core)
- **Architecture**: Clean Architecture with CQRS
- **Mediator**: MediatR for command/query handling
- **Validation**: FluentValidation
- **Resilience**: Polly (Circuit Breaker, Retry, Timeout)
- **Logging**: Serilog with structured logging
- **Testing**: xUnit, Moq, BenchmarkDotNet
- **Load Testing**: K6
- **Documentation**: Swagger/OpenAPI

## 🚀 Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Git](https://git-scm.com/)
- [Docker](https://www.docker.com/) (optional)
- [K6](https://k6.io/) (for load testing)

### Local Development

1. **Clone the repository:**
```bash
git clone https://github.com/your-org/payment-gateway-bff.git
cd payment-gateway-bff
```

2. **Restore dependencies:**
```bash
dotnet restore
```

3. **Start the mock provider (in a separate terminal):**
```bash
cd tests/MockProvider
dotnet run
```

4. **Run the main application:**
```bash
cd src/BackendForFrontend.WebApi
dotnet run
```

5. **Access the application:**
- API: `https://localhost:5000`
- Swagger UI: `https://localhost:5000/swagger`
- Health Checks: `https://localhost:5000/health/ready`

## 📚 API Documentation

### Authentication Endpoint

**POST** `/v1/auth/login`

```json
// Request
{
  "username": "test@example.com",
  "password": "password123"
}

// Response
{
  "isSuccess": true,
  "jwt": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-12-31T23:59:59Z"
}
```

### Payment Endpoint

**POST** `/v1/payments`

```json
// Request
{
  "amount": 100.50,
  "currency": "USD",
  "destinationAccount": "ACC123456789"
}

// Response
{
  "isSuccess": true,
  "paymentId": "PAY_AB12CD34",
  "providerReference": "REF_567890",
  "processedAt": "2024-12-31T12:00:00Z"
}
```

### Headers

- `Authorization: Bearer <jwt-token>` (for payments)
- `X-Correlation-ID: <uuid>` (optional, auto-generated if not provided)
- `X-Api-Version: 1.0` (optional, defaults to latest)

## 🧪 Testing

### Run Unit Tests
```bash
dotnet test tests/BackendForFrontend.UnitTests/
```

### Run Integration Tests
```bash
# Ensure mock provider is running
dotnet test tests/BackendForFrontend.IntegrationTests/
```

### Run Benchmarks
```bash
dotnet run --project tests/BackendForFrontend.Benchmarks/ -c Release
```

### Load Testing with K6
```bash
# Start the application and mock provider first
k6 run scripts/k6-load-test.js
```

Expected performance targets:
- **Throughput**: 1000+ RPS
- **Latency P95**: < 150ms
- **Error Rate**: < 1%

## 🏥 Health Checks

The application provides comprehensive health check endpoints:

- **Liveness**: `/health/live` - Basic application health
- **Readiness**: `/health/ready` - Application + dependencies health

Health check response:
```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "provider",
      "status": "Healthy",
      "description": "Provider is responding",
      "duration": 45.2
    }
  ],
  "totalDuration": 45.2
}
```

## 🐳 Docker Deployment

### Build Image
```bash
docker build -t payment-gateway-bff .
```

### Run Container
```bash
docker run -d -p 8080:8080 \
  --name payment-gateway-bff \
  -e ASPNETCORE_ENVIRONMENT=Production \
  payment-gateway-bff
```

### Docker Compose
```yaml
version: '3.8'
services:
  bff:
    build: .
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    depends_on:
      - mock-provider
  
  mock-provider:
    build: ./tests/MockProvider
    ports:
      - "5001:5001"
```

## ⚙️ Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment (Development/Production) | Development |
| `ASPNETCORE_URLS` | Application URLs | `https://localhost:5000` |
| `PROVIDER_BASE_URL` | Payment provider base URL | `http://localhost:5001` |
| `CIRCUIT_BREAKER_THRESHOLD` | Circuit breaker failure threshold | 5 |
| `RETRY_COUNT` | Number of retry attempts | 3 |

### appsettings.json Structure
```json
{
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "logs/app-.log" } }
    ]
  },
  "HealthChecks": {
    "Provider": {
      "Timeout": "00:00:02"
    }
  }
}
```

## 📊 Monitoring & Observability

### Structured Logging
All requests include correlation IDs for distributed tracing:
```json
{
  "timestamp": "2024-12-31T12:00:00Z",
  "level": "Information",
  "message": "Processing payment",
  "correlationId": "550e8400-e29b-41d4-a716-446655440000",
  "amount": 100.50,
  "currency": "USD"
}
```

### Metrics
- Request duration
- Request count by endpoint
- Error rates
- Circuit breaker states
- Health check status

## 🔧 Development

### Project Structure
```
├── src/
│   ├── BackendForFrontend.Application/     # Business logic, commands, handlers
│   ├── BackendForFrontend.Infrastructure/  # External dependencies, HTTP clients
│   └── BackendForFrontend.WebApi/          # API controllers, middleware
├── tests/
│   ├── BackendForFrontend.UnitTests/       # Unit tests
│   ├── BackendForFrontend.IntegrationTests/ # Integration tests
│   ├── BackendForFrontend.Benchmarks/      # Performance benchmarks
│   └── MockProvider/                       # Test payment provider
└── scripts/
    └── k6-load-test.js                     # Load testing script
```

### Coding Standards
- Clean Architecture principles
- SOLID design principles
- Async/await for all I/O operations
- Comprehensive error handling
- Input validation with FluentValidation
- Unit test coverage > 80%

### Adding New Features
1. Create command/query in Application layer
2. Implement handler with business logic
3. Add validation rules
4. Create/update controller endpoint
5. Add unit and integration tests
6. Update API documentation

## 🚨 Troubleshooting

### Common Issues

**Q: "Unable to find package" errors during restore**
```bash
# Clear NuGet cache and restore
dotnet nuget locals all --clear
dotnet restore --force
```

**Q: Mock provider not responding**
```bash
# Check if mock provider is running
curl http://localhost:5001/health
# Or restart it
cd tests/MockProvider && dotnet run
```

**Q: High latency in load tests**
```bash
# Check circuit breaker status in logs
# Verify mock provider performance
# Review connection pool settings
```

### Debug Mode
Set environment variable for detailed logging:
```bash
export ASPNETCORE_ENVIRONMENT=Development
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Code Quality Requirements
- All tests must pass
- Code coverage > 80%
- No breaking changes to existing APIs
- Follow existing coding conventions
- Update documentation for new features

## 📋 Roadmap

- [ ] **v2.1**: Add OAuth2 support
- [ ] **v2.2**: Implement payment webhooks
- [ ] **v2.3**: Add caching layer (Redis)
- [ ] **v2.4**: Implement rate limiting
- [ ] **v3.0**: Add GraphQL support
- [ ] **v3.1**: Microservices decomposition

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙋‍♂️ Support

- **Documentation**: Check this README and inline code comments
- **Issues**: Open a GitHub issue for bugs or feature requests
- **Discussions**: Use GitHub Discussions for questions
- **Email**: payments-team@company.com

---

**Built with ❤️ by the Payment Team**
