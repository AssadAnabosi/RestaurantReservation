using RestaurantReservation.API.Contracts;

namespace RestaurantReservation.API.Services;

public interface IEmployeeService
{
    Task<List<EmployeeDto>> ListManagersAsync();
    Task<decimal> CalculateAverageOrderAmountAsync(int employeeId);
}
