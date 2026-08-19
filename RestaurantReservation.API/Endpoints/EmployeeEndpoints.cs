using Microsoft.AspNetCore.Mvc;
using RestaurantReservation.API.Services;

namespace RestaurantReservation.API.Endpoints;

public static class EmployeeEndpoints
{
    public static IEndpointRouteBuilder MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api");

        api.MapGet("/employees/managers", ([FromServices] IEmployeeService service) => service.ListManagersAsync())
            .RequireAuthorization()
            .WithName("ListManagers");

        api.MapGet("/employees/{employeeId:int}/average-order-amount",
            ([FromServices] IEmployeeService service, int employeeId) => service.CalculateAverageOrderAmountAsync(employeeId))
            .RequireAuthorization()
            .WithName("CalculateAverageOrderAmount");

        return app;
    }
}