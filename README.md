# Discount Code Generation System

A high-performance discount code generation and validation system built with .NET 8 and gRPC.

## Features
- Generate 1-2000 unique discount codes per request
- Thread-safe concurrent request handling
- Memory caching for improved performance
- Bulk operations optimized for large code generation
- gRPC API for high-performance communication

## Architecture
(TBD)

## Getting Started

(Setup instructions will be added once implementation is complete)

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

--- test commit for user change ---