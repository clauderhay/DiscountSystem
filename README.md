# Discount Code Generation System

A high-performance discount code generation and validation system built with .NET 8 and gRPC.

## Features
- Generate 1-2000 unique discount codes per request
- Thread-safe concurrent request handling
- Memory caching for improved performance
- Bulk operations optimized for large code generation
- gRPC API for high-performance communication

## Architecture

The solution follows Clean Architecture principles:

DiscountSystem/
- |---Core/ # Domain models and interface
- |---Services/ # Business logic implementation
- |---Data/ # Data access layer with EF Core
- |---API/ # gRPC API endpoints
- |---Client/ # Interactive UI

## Prerequisites

- .NET 8 SDK
- Docker (for PostgresSQL)
- A gRPC client (like Postman, grpcurl, or the provided client)

## Getting Started

### 1. Start PostgresSQL Database

`docker-compose up -d`

### 2. Run Database Migrations

`cd DiscountSystem.API`
`dotnet ef database update`

### 3. Run the API

`dotnet run --project DiscountSystem.API`

### API Endpoints

#### gRPC Service: Discount
##### GenerateCodes

- Request: count (uint32) — Number of codes to generate (1–2000)
- Response: result (bool) - Success/failure

##### UseCode

- Request: code (string) - 8-character discount code
- Response: result (uint32) - 0 for success, 1 for failure

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

# Discount System Client

A gRPC client for testing the Discount Code Generation System.

## Features

- **Generate Codes**: Create 1-2000 unique discount codes
- **Use Codes**: Validate and mark codes as used

### Additional Features

- **Stress Testing**: Test concurrent request handling
- **Performance Metrics**: Measure response times and throughput


## Usage

1. Ensure the API is running on `http://localhost:5000`
2. Run the client: `dotnet run`
3. Follow the interactive menu

### Stress Test Results

Example stress test with 2 concurrent requests generating 100 codes each:

=== Stress Test Results ===
- Total time: 155ms
- Successful requests: 2
- Failed requests: 0
- Average time per request: 77ms
- Codes generated per second: 1290

## View DB Data from Postgres (using docker)

`docker exec -it discount_postgres psql -U discount_user -d discount_db -c "SELECT * FROM \"DiscountCodes\" ORDER BY \"CreatedAt\" DESC;"`