using DiscountSystem.API.Services;
using DiscountSystem.Core.Interfaces;
using DiscountSystem.Data.Context;
using DiscountSystem.Data.Repositories;
using DiscountSystem.Services;
using DiscountSystem.Services.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

// Configure Entity Framework with PostgreSQL
builder.Services.AddDbContext<DiscountDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
});

// Register repositories
builder.Services.AddScoped<IDiscountCodeRepository, DiscountCodeRepository>();

// Register business services (from our extension method)
builder.Services.AddDiscountServices();
builder.Services.AddScoped<ICodeGenerator, CodeGenerator>();

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline
app.MapGrpcService<GrpcDiscountService>();
app.MapGrpcReflectionService();

// Add a simple HTTP endpoint for health checks
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DiscountDbContext>();
    
    // For development - ensure database exists
    if (app.Environment.IsDevelopment())
    {
        await dbContext.Database.EnsureCreatedAsync();
    }
}

app.Run();