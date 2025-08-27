# Payment Gateway BFF Service

A Backend-for-Frontend service built with .NET 8 that provides a unified API for payment operations while interfacing with third-party payment providers.

## Features

- **Authentication API**: User login with JWT token generation
- **Payment Processing**: Secure payment creation and processing
- **API Versioning**: Support for multiple API versions (v1, v2)
- **Resiliency**: Circuit breaker, retry, and timeout policies
- **Observability**: Health checks, structured logging, metrics
- **Performance**: Optimized for high throughput (1000+ RPS)

## Quick Start

### Prerequisites

- .NET 8 SDK
- Docker (optional, for containerized deployment)
- k6 (for load testing)

### Running Locally

1. **Clone and restore packages:**

```bash
git clone <repository-url>
cd BackendForFrontend
dotnet restore
```

