using Microsoft.AspNetCore.Http.HttpResults;
using RestaurantReservation.API.Contracts;

namespace RestaurantReservation.API.Endpoints;

public static class AuthEndpoints
{
    public record AuthResponse(string Token);

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api");

        api.MapPost("/auth",
            Results<
                    Ok<AuthResponse>,
                    BadRequest<string>>
                (AuthRequest? request, JwtTokenService tokenService) =>
            {
                if (request is null)
                    return TypedResults.BadRequest("Request body is required.");

                var username = request.Username?.Trim();
                if (string.IsNullOrWhiteSpace(username))
                    return TypedResults.BadRequest("Username is required.");

                var token = tokenService.CreateToken(username);
                return TypedResults.Ok(new AuthResponse(token));
            });

        return app;
    }
}