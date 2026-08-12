using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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

builder.Services.AddScoped<EmployeeRepository>();
builder.Services.AddScoped<ReservationRepository>();
builder.Services.AddScoped<OrderRepository>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

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

api.MapPost("/auth", (AuthRequest? request) =>
{
    if (request is null)
        return Results.BadRequest("Request body is required.");

    var username = request.Username.Trim();
    if (string.IsNullOrWhiteSpace(username))
        return Results.BadRequest("Username is required.");

    return Results.Ok(CreateJwtToken(username, jwtIssuer, jwtSecretKey));
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

static string CreateJwtToken(string username, string issuer, string secretKey)
{
    var signingCredentials = new SigningCredentials(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var token = new JwtSecurityToken(
        issuer: issuer,
        claims: claims,
        expires: DateTime.UtcNow.AddDays(1),
        signingCredentials: signingCredentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
}

record AuthRequest(string Username);