using Microsoft.AspNetCore.Http.HttpResults;
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

        reservations.MapGet("/{reservationId:int}",
                async Task<Results<
                        Ok<ReservationDto>,
                        NotFound>>
                    ([FromServices] IReservationService service, int reservationId) =>
                {
                    var res = await service.GetByIdAsync(reservationId);
                    return res is null ? TypedResults.NotFound() : TypedResults.Ok(res);
                })
            .RequireAuthorization()
            .WithName("GetReservationById");

        reservations.MapPost("",
                async Task<Results<
                        Created<ReservationDto>,
                        BadRequest<string>>>
                    ([FromServices] IReservationService service, ReservationRequest? request) =>
                {
                    var created = await service.CreateAsync(request!);

                    return TypedResults.Created($"/api/reservations/{created.ReservationId}", created);
                })
            .RequireAuthorization()
            .WithName("CreateReservation")
            .AddEndpointFilter(async (context, next) =>
            {
                var errors = new Dictionary<string, string[]>();
                var requestBody = context.GetArgument<ReservationRequest>(1) ?? null;
                if (requestBody is null)
                {
                    return Results.BadRequest("Request body is required.");
                }

                if (requestBody.PartySize <= 0)
                    errors.Add(nameof(ReservationRequest.PartySize), ["Party size must be greater than zero."]);

                if (errors.Count > 0)
                    return Results.ValidationProblem(errors);

                return await next(context);
            });

        reservations.MapPut("/{reservationId:int}",
                async Task<Results<
                    Ok<ReservationDto>,
                    BadRequest<string>,
                    NotFound>>
                ([FromServices] IReservationService service, int reservationId,
                    ReservationRequest? request) =>
                {
                    if (request is null)
                        return TypedResults.BadRequest("Request body is required.");

                    if (request.PartySize <= 0)
                        return TypedResults.BadRequest("Party size must be greater than zero.");

                    var updated = await service.UpdateAsync(reservationId, request);

                    return updated is null ? TypedResults.NotFound() : TypedResults.Ok(updated);
                })
            .RequireAuthorization()
            .WithName("UpdateReservation");

        reservations.MapDelete("/{reservationId:int}",
                async Task<Results<
                        NoContent,
                        NotFound>>
                    ([FromServices] IReservationService service, int reservationId) =>
                {
                    var deleted = await service.DeleteAsync(reservationId);
                    return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
                })
            .RequireAuthorization()
            .WithName("DeleteReservation");

        reservations.MapGet("/customer/{customerId:int}",
                ([FromServices] IReservationService service, int customerId) =>
                    service.GetReservationsByCustomerAsync(customerId))
            .RequireAuthorization()
            .WithName("GetReservationsByCustomer");

        reservations.MapGet("/{reservationId:int}/orders",
                ([FromServices] IReservationService service, int reservationId) =>
                    service.ListOrdersAndMenuItemsAsync(reservationId))
            .RequireAuthorization()
            .WithName("ListOrdersAndMenuItems");

        reservations.MapGet("/{reservationId:int}/menu-items",
                ([FromServices] IReservationService service, int reservationId) =>
                    service.ListOrderedMenuItemsAsync(reservationId))
            .RequireAuthorization()
            .WithName("ListOrderedMenuItems");

        return app;
    }
}