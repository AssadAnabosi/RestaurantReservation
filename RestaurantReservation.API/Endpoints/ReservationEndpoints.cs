using Microsoft.AspNetCore.Mvc;
using RestaurantReservation.API.Contracts;
using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Repositories;

namespace RestaurantReservation.API.Endpoints;

public static class ReservationEndpoints
{
    public static IEndpointRouteBuilder MapReservationEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api");
        var reservations = api.MapGroup("/reservations");

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

        reservations.MapGet("/{reservationId:int}/orders",
                ([FromServices] OrderRepository repo, int reservationId) => repo.ListOrdersAndMenuItemsAsync(reservationId))
            .RequireAuthorization()
            .WithName("ListOrdersAndMenuItems");

        reservations.MapGet("/{reservationId:int}/menu-items",
                ([FromServices] OrderRepository repo, int reservationId) => repo.ListOrderedMenuItemsAsync(reservationId))
            .RequireAuthorization()
            .WithName("ListOrderedMenuItems");

        return app;
    }
}