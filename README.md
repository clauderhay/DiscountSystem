# Discount Code Generation System

A high-performance discount code generation and validation system built with .NET 8 and gRPC.

## Features
- Generate 1-2000 unique discount codes per request
- Thread-safe concurrent request handling
- Memory caching for improved performance
- Bulk operations optimized for large code generation
- gRPC API for high-performance communication
- Comprehensive client with interactive UI
- Production-ready with proper error handling and logging

### Additional Features

- **Stress Testing**: Test concurrent request handling
- **Performance Metrics**: Measure response times and throughput

## Architecture

The solution follows Clean Architecture principles:

DiscountSystem/
- |---Core/ # Domain models and interface
- |---Services/ # Business logic implementation
- |---Data/ # Data access layer with EF Core
- |---API/ # gRPC API endpoints
- |---Client/ # Interactive UI

## Technology Stack

- **.NET 8**: Latest LTS version
- **gRPC**: High-performance RPC framework
- **PostgreSQL**: Primary database (configurable)
- **Entity Framework Core 8**: ORM with code-first migrations
- **Docker**: Containerized database for easy setup
- **Memory Cache**: Built-in ASP.NET Core caching

## Prerequisites

- .NET 8 SDK
- Docker (for PostgresSQL)
- A gRPC client (like Postman, grpcurl, or the provided client)

## Quick Start

### 1. Clone and Setup Database

Clone the repository

        git clone <https://github.com/clauderhay/DiscountSystem.git>

        cd DiscountSystem

Start PostgresSQL with Docker

        docker-compose up -d

### 2. Run Database Migrations

        cd src/DiscountSystem.API

        dotnet ef database update

### 3. Start the API

        cd src/DiscountSystem.API

        dotnet run

### 4. Run the Client

In a new terminal

        cd src/DiscountSystem.Client

        dotnet run

### API Endpoints

#### gRPC Service: Discount
##### GenerateCodes

- Request: count (uint32) — Number of codes to generate (1–2000)
- Response: result (bool) - Success/failure

##### UseCode

- Request: code (string) - 8-character discount code
- Response: result (uint32) - 0 for success, 1 for failure

### Stress Test Results

Example stress test with 2 concurrent requests generating 100 codes each:

=== Stress Test Results ===
- Total time: 155ms
- Successful requests: 2
- Failed requests: 0
- Average time per request: 77ms
- Codes generated per second: 1290

## Technical Implementation

### Code Generation
- Cryptographically secure random generation
- 8-character alphanumeric codes
- Pre-validation for uniqueness

### Concurrency
- SemaphoreSlim for thread-safe generation
- Prevents race conditions

### Performance
- Bulk database operations
- Memory caching for used codes
- Optimized indexes

## View DB Data from Postgres (using docker)

    docker exec -it discount_postgres psql -U discount_user -d discount_db -c "SELECT * FROM \"DiscountCodes\" ORDER BY \"CreatedAt\" DESC;"
