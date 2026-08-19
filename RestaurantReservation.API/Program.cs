using System.Text.Json.Serialization;
using RestaurantReservation.API;
using RestaurantReservation.API.Endpoints;
using RestaurantReservation.API.Services;
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
builder.Services.AddSingleton<JwtTokenService>();

builder.Services.AddScoped<EmployeeRepository>();
builder.Services.AddScoped<ReservationRepository>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IReservationService, ReservationService>();

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

app.MapHealthEndpoint();
app.MapAuthEndpoints();
app.MapEmployeeEndpoints();
app.MapReservationEndpoints();

app.Run();