using Microsoft.AspNetCore.Mvc;
using RestaurantReservation.API.Contracts;
using RestaurantReservation.API.Services;

namespace RestaurantReservation.API.Endpoints;

public static class ReservationEndpoints
{
    public static IEndpointRouteBuilder MapReservationEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api");
        var reservations = api.MapGroup("/reservations");

        reservations.MapGet("", ([FromServices] IReservationService service) => service.ListAsync())
            .RequireAuthorization()
            .WithName("ListReservations");

        reservations.MapGet("/{reservationId:int}", ([FromServices] IReservationService service, int reservationId) =>
                service.GetByIdAsync(reservationId))
            .RequireAuthorization()
            .WithName("GetReservationById");

        reservations.MapPost("", async ([FromServices] IReservationService service, ReservationRequest? request) =>
            {
                if (request is null)
                    return Results.BadRequest("Request body is required.");

                if (request.PartySize <= 0)
                    return Results.BadRequest("Party size must be greater than zero.");

                var created = await service.CreateAsync(request);

                return Results.Created($"/api/reservations/{created.ReservationId}", created);
            })
            .RequireAuthorization()
            .WithName("CreateReservation");

        reservations.MapPut("/{reservationId:int}", async ([FromServices] IReservationService service, int reservationId,
                ReservationRequest? request) =>
            {
                if (request is null)
                    return Results.BadRequest("Request body is required.");

                if (request.PartySize <= 0)
                    return Results.BadRequest("Party size must be greater than zero.");

                var updated = await service.UpdateAsync(reservationId, request);

                return updated is null ? Results.NotFound() : Results.Ok(updated);
            })
            .RequireAuthorization()
            .WithName("UpdateReservation");

        reservations.MapDelete("/{reservationId:int}", async ([FromServices] IReservationService service, int reservationId) =>
            {
                var deleted = await service.DeleteAsync(reservationId);
                return deleted ? Results.NoContent() : Results.NotFound();
            })
            .RequireAuthorization()
            .WithName("DeleteReservation");

        reservations.MapGet("/customer/{customerId:int}",
                ([FromServices] IReservationService service, int customerId) => service.GetReservationsByCustomerAsync(customerId))
            .RequireAuthorization()
            .WithName("GetReservationsByCustomer");

        reservations.MapGet("/{reservationId:int}/orders",
                ([FromServices] IReservationService service, int reservationId) => service.ListOrdersAndMenuItemsAsync(reservationId))
            .RequireAuthorization()
            .WithName("ListOrdersAndMenuItems");

        reservations.MapGet("/{reservationId:int}/menu-items",
                ([FromServices] IReservationService service, int reservationId) => service.ListOrderedMenuItemsAsync(reservationId))
            .RequireAuthorization()
            .WithName("ListOrderedMenuItems");

        return app;
    }
}