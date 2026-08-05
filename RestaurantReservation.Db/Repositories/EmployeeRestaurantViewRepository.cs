using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Db.Views;

namespace RestaurantReservation.Db.Repositories;

public sealed class EmployeeRestaurantViewRepository
{
    private readonly RestaurantReservationDbContext _context;

    public EmployeeRestaurantViewRepository(RestaurantReservationDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmployeeRestaurantView>> ListEmployeesWithRestaurantAsync()
    {
        return await _context.EmployeeRestaurantViews
            .AsNoTracking()
            .OrderBy(e => e.RestaurantName)
            .ThenBy(e => e.EmployeeLastName)
            .ToListAsync();
    }
}
