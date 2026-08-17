using Microsoft.AspNetCore.Mvc;
using RestaurantReservation.Db.Repositories;

namespace RestaurantReservation.API.Endpoints;

public static class EmployeeEndpoints
{
    public static IEndpointRouteBuilder MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api");

        api.MapGet("/employees/managers", ([FromServices] EmployeeRepository repo) => repo.ListManagersAsync())
            .RequireAuthorization()
            .WithName("ListManagers");

        api.MapGet("/employees/{employeeId:int}/average-order-amount",
                ([FromServices] OrderRepository repo, int employeeId) => repo.CalculateAverageOrderAmountAsync(employeeId))
            .RequireAuthorization()
            .WithName("CalculateAverageOrderAmount");

        return app;
    }
}