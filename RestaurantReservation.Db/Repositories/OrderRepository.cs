using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Db.Entities;
using RestaurantReservation;

namespace RestaurantReservation.Db.Repositories;

public sealed class OrderRepository
{
    private readonly RestaurantReservationDbContext _context;

    public OrderRepository(RestaurantReservationDbContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> UpdateAsync(Order order)
    {
        var current = await _context.Orders.FindAsync(order.OrderId);
        if (current is null) return null;

        current.Date = order.Date;
        current.TotalAmount = order.TotalAmount;
        current.EmployeeId = order.EmployeeId;
        current.ReservationId = order.ReservationId;

        await _context.SaveChangesAsync();
        return current;
    }

    public async Task<bool> DeleteAsync(int orderId)
    {
        var entity = await _context.Orders.FindAsync(orderId);
        if (entity is null) return false;

        _context.Orders.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<OrderWithMenuItemsDto>> ListOrdersAndMenuItemsAsync(int reservationId)
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .Include(order => order.Employee)
            .Include(order => order.OrderItems)
            .ThenInclude(orderItem => orderItem.Item)
            .Where(order => order.ReservationId == reservationId)
            .OrderBy(order => order.Date)
            .ToListAsync();

        return orders
            .Select(order => new OrderWithMenuItemsDto(
                order.OrderId,
                order.Date,
                order.TotalAmount,
                $"{order.Employee.FirstName} {order.Employee.LastName}",
                order.OrderItems
                    .Select(orderItem => new OrderMenuItemDto(
                        orderItem.ItemId,
                        orderItem.Item.Name,
                        orderItem.Quantity,
                        orderItem.Item.Price))
                    .ToList()))
            .ToList();
    }

    public async Task<List<MenuItem>> ListOrderedMenuItemsAsync(int reservationId)
    {
        var orderedMenuItems = await _context.Orders
            .AsNoTracking()
            .Where(order => order.ReservationId == reservationId)
            .Include(order => order.OrderItems)
            .ThenInclude(orderItem => orderItem.Item)
            .SelectMany(order => order.OrderItems)
            .Select(orderItem => orderItem.Item)
            .ToListAsync();

        return orderedMenuItems
            .GroupBy(menuItem => menuItem.ItemId)
            .Select(group => group.First())
            .OrderBy(menuItem => menuItem.Name)
            .ToList();
    }

    public async Task<decimal> CalculateAverageOrderAmountAsync(int employeeId)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(order => order.EmployeeId == employeeId)
            .Select(order => (decimal?)order.TotalAmount)
            .AverageAsync() ?? 0m;
    }
}
