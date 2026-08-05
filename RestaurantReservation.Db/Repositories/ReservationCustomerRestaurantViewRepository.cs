using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Db.Views;

namespace RestaurantReservation.Db.Repositories;

public sealed class ReservationCustomerRestaurantViewRepository
{
    private readonly RestaurantReservationDbContext _context;

    public ReservationCustomerRestaurantViewRepository(RestaurantReservationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReservationCustomerRestaurantView>> ListReservationsWithCustomerAndRestaurantAsync()
    {
        return await _context.ReservationCustomerRestaurantViews
            .AsNoTracking()
            .OrderBy(r => r.ReservationDate)
            .ToListAsync();
    }
}
