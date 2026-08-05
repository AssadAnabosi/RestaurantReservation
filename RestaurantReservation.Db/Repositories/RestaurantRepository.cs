using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Repositories;

public sealed class RestaurantRepository
{
    private readonly RestaurantReservationDbContext _context;

    public RestaurantRepository(RestaurantReservationDbContext context)
    {
        _context = context;
    }

    public async Task<Restaurant> CreateAsync(Restaurant restaurant)
    {
        await _context.Restaurants.AddAsync(restaurant);
        await _context.SaveChangesAsync();
        return restaurant;
    }

    public async Task<Restaurant?> UpdateAsync(Restaurant restaurant)
    {
        var current = await _context.Restaurants.FindAsync(restaurant.RestaurantId);
        if (current is null) return null;

        current.Name = restaurant.Name;
        current.Address = restaurant.Address;
        current.PhoneNumber = restaurant.PhoneNumber;
        current.OpeningHours = restaurant.OpeningHours;

        await _context.SaveChangesAsync();
        return current;
    }

    public async Task<bool> DeleteAsync(int restaurantId)
    {
        var entity = await _context.Restaurants.FindAsync(restaurantId);
        if (entity is null) return false;

        _context.Restaurants.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
