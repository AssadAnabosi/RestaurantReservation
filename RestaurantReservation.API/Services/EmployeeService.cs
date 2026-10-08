using RestaurantReservation.API.Contracts;
using RestaurantReservation.Db.Repositories;

namespace RestaurantReservation.API.Services;

public sealed class EmployeeService : IEmployeeService
{
    private readonly EmployeeRepository _employeeRepository;
    private readonly OrderRepository _orderRepository;

    public EmployeeService(EmployeeRepository employeeRepository, OrderRepository orderRepository)
    {
        _employeeRepository = employeeRepository;
        _orderRepository = orderRepository;
    }

    public async Task<List<EmployeeDto>> ListManagersAsync() =>
        (await _employeeRepository.ListManagersAsync())
        .Select(employee => new EmployeeDto(
            employee.EmployeeId,
            employee.FirstName,
            employee.LastName,
            employee.Position,
            employee.RestaurantId))
        .ToList();

    public Task<decimal> CalculateAverageOrderAmountAsync(int employeeId) =>
        _orderRepository.CalculateAverageOrderAmountAsync(employeeId);
}
