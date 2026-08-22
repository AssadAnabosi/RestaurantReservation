namespace RestaurantReservation.API.Endpoints;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health", () => Results.Ok("Ok"))
            .WithName("GetHealth");

        return app;
    }
}