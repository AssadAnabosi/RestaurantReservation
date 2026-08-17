using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using RestaurantReservation.API;
using RestaurantReservation.Db;
using RestaurantReservation.Db.Entities;
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
var reservations = api.MapGroup("/reservations");

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

reservations.MapGet("", ([FromServices] ReservationRepository repo) => repo.ListAsync())
    .RequireAuthorization()
    .WithName("ListReservations");

reservations.MapGet("/{reservationId:int}", ([FromServices] ReservationRepository repo, int reservationId) =>
    repo.GetByIdAsync(reservationId))
    .RequireAuthorization()
    .WithName("GetReservationById");

reservations.MapPost("", async ([FromServices] ReservationRepository repo, ReservationRequest? request) =>
    {
        if (request is null)
            return Results.BadRequest("Request body is required.");

        if (request.PartySize <= 0)
            return Results.BadRequest("Party size must be greater than zero.");

        var created = await repo.CreateAsync(new Reservation
        {
            Date = request.Date,
            PartySize = request.PartySize,
            RestaurantId = request.RestaurantId,
            CustomerId = request.CustomerId,
            TableId = request.TableId
        });

        return Results.Created($"/api/reservations/{created.ReservationId}", created);
    })
    .RequireAuthorization()
    .WithName("CreateReservation");

reservations.MapPut("/{reservationId:int}", async ([FromServices] ReservationRepository repo, int reservationId,
        ReservationRequest? request) =>
    {
        if (request is null)
            return Results.BadRequest("Request body is required.");

        if (request.PartySize <= 0)
            return Results.BadRequest("Party size must be greater than zero.");

        var updated = await repo.UpdateAsync(new Reservation
        {
            ReservationId = reservationId,
            Date = request.Date,
            PartySize = request.PartySize,
            RestaurantId = request.RestaurantId,
            CustomerId = request.CustomerId,
            TableId = request.TableId
        });

        return updated is null ? Results.NotFound() : Results.Ok(updated);
    })
    .RequireAuthorization()
    .WithName("UpdateReservation");

reservations.MapDelete("/{reservationId:int}", async ([FromServices] ReservationRepository repo, int reservationId) =>
    {
        var deleted = await repo.DeleteAsync(reservationId);
        return deleted ? Results.NoContent() : Results.NotFound();
    })
    .RequireAuthorization()
    .WithName("DeleteReservation");

reservations.MapGet("/customer/{customerId:int}",
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

record ReservationRequest(DateTime Date, int PartySize, int RestaurantId, int CustomerId, int TableId);

record AuthRequest(string Username);