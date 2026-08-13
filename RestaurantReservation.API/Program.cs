using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using RestaurantReservation.API;
using RestaurantReservation.Db;
using RestaurantReservation.Db.Repositories;

var builder = WebApplication.CreateBuilder(args);

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtIssuer = jwtSection["Issuer"] ?? "RestaurantReservation.API";
var jwtSecretKey = jwtSection["SecretKey"]
                   ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<RestaurantReservationDbContext>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddSingleton<JwtTokenService>();

builder.Services.AddScoped<EmployeeRepository>();
builder.Services.AddScoped<ReservationRepository>();
builder.Services.AddScoped<OrderRepository>();

builder.Services
    .AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .ValidateDataAnnotations()
    .Validate(o => !string.IsNullOrWhiteSpace(o.SecretKey), "Jwt:SecretKey is not configured.")
    .ValidateOnStart();

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok("Ok"))
    .WithName("GetHealth");

var api = app.MapGroup("/api");

api.MapPost("/auth", (AuthRequest? request, JwtTokenService tokenService) =>
{
    if (request is null)
        return Results.BadRequest("Request body is required.");

    var username = request.Username?.Trim();
    if (string.IsNullOrWhiteSpace(username))
        return Results.BadRequest("Username is required.");

    var token = tokenService.CreateToken(request.Username!);
    return Results.Ok(new { token });
});

api.MapGet("/employees/managers", ([FromServices] EmployeeRepository repo) => repo.ListManagersAsync())
    .RequireAuthorization()
    .WithName("ListManagers");

api.MapGet("/reservations/customer/{customerId}",
        ([FromServices] ReservationRepository repo, int customerId) => repo.GetReservationsByCustomerAsync(customerId))
    .RequireAuthorization()
    .WithName("GetReservationsByCustomer");

api.MapGet("/reservations/{reservationId}/orders",
        ([FromServices] OrderRepository repo, int reservationId) => repo.ListOrdersAndMenuItemsAsync(reservationId))
    .RequireAuthorization()
    .WithName("ListOrdersAndMenuItems");

api.MapGet("/reservations/{reservationId}/menu-items",
        ([FromServices] OrderRepository repo, int reservationId) => repo.ListOrderedMenuItemsAsync(reservationId))
    .RequireAuthorization()
    .WithName("ListOrderedMenuItems");

api.MapGet("/employees/{employeeId}/average-order-amount",
        ([FromServices] OrderRepository repo, int employeeId) => repo.CalculateAverageOrderAmountAsync(employeeId))
    .RequireAuthorization()
    .WithName("CalculateAverageOrderAmount");

app.Run();

record AuthRequest(string Username);