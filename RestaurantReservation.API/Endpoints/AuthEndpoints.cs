using RestaurantReservation.API.Contracts;

namespace RestaurantReservation.API.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api");

        api.MapPost("/auth", (AuthRequest? request, JwtTokenService tokenService) =>
        {
            if (request is null)
                return Results.BadRequest("Request body is required.");

            var username = request.Username?.Trim();
            if (string.IsNullOrWhiteSpace(username))
                return Results.BadRequest("Username is required.");

            var token = tokenService.CreateToken(username);
            return Results.Ok(new { token });
        });

        return app;
    }
}