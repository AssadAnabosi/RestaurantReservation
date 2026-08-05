using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Repositories;

public sealed class OrderItemRepository
{
    private readonly RestaurantReservationDbContext _context;

    public OrderItemRepository(RestaurantReservationDbContext context)
    {
        _context = context;
    }

    public async Task<OrderItem> CreateAsync(OrderItem orderItem)
    {
        await _context.OrderItems.AddAsync(orderItem);
        await _context.SaveChangesAsync();
        return orderItem;
    }

    public async Task<OrderItem?> UpdateAsync(OrderItem orderItem)
    {
        var current = await _context.OrderItems.FindAsync(orderItem.OrderItemId);
        if (current is null) return null;

        current.Quantity = orderItem.Quantity;
        current.ItemId = orderItem.ItemId;
        current.OrderId = orderItem.OrderId;

        await _context.SaveChangesAsync();
        return current;
    }

    public async Task<bool> DeleteAsync(int orderItemId)
    {
        var entity = await _context.OrderItems.FindAsync(orderItemId);
        if (entity is null) return false;

        _context.OrderItems.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
