using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Repositories;

public sealed class MenuItemRepository
{
    private readonly RestaurantReservationDbContext _context;

    public MenuItemRepository(RestaurantReservationDbContext context)
    {
        _context = context;
    }

    public async Task<MenuItem> CreateAsync(MenuItem menuItem)
    {
        await _context.MenuItems.AddAsync(menuItem);
        await _context.SaveChangesAsync();
        return menuItem;
    }

    public async Task<MenuItem?> UpdateAsync(MenuItem menuItem)
    {
        var current = await _context.MenuItems.FindAsync(menuItem.ItemId);
        if (current is null) return null;

        current.Name = menuItem.Name;
        current.Description = menuItem.Description;
        current.Price = menuItem.Price;
        current.RestaurantId = menuItem.RestaurantId;

        await _context.SaveChangesAsync();
        return current;
    }

    public async Task<bool> DeleteAsync(int itemId)
    {
        var entity = await _context.MenuItems.FindAsync(itemId);
        if (entity is null) return false;

        _context.MenuItems.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
