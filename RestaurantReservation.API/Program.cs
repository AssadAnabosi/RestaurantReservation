using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using RestaurantReservation.Db;
using RestaurantReservation.Db.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<RestaurantReservationDbContext>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddScoped<EmployeeRepository>();
builder.Services.AddScoped<ReservationRepository>();
builder.Services.AddScoped<OrderRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok("Ok"))
    .WithName("GetHealth");

app.MapGet("/api/employees/managers", ([FromServices] EmployeeRepository repo) =>
{
    return repo.ListManagersAsync();
}).WithName("ListManagers");

app.MapGet("/api/reservations/customer/{customerId}", ([FromServices] ReservationRepository repo, int customerId) =>
{
    return repo.GetReservationsByCustomerAsync(customerId);
}).WithName("GetReservationsByCustomer");

app.MapGet("/api/reservations/{reservationId}/orders", ([FromServices] OrderRepository repo, int reservationId) =>
{
    return repo.ListOrdersAndMenuItemsAsync(reservationId);
}).WithName("ListOrdersAndMenuItems");

app.MapGet("/api/reservations/{reservationId}/menu-items", ([FromServices] OrderRepository repo, int reservationId) =>
{
    return repo.ListOrderedMenuItemsAsync(reservationId);
}).WithName("ListOrderedMenuItems");

app.Run();