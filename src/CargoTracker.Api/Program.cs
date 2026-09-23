using CargoTracker.Application.Abstractions;
using CargoTracker.Application.Services;
using CargoTracker.Infrastructure.Persistence;
using CargoTracker.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IShipmentRepository, EfShipmentRepository>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();
builder.Services.AddDbContext<CargoTrackerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CargoTrackerDb")));
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
