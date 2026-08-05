using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Repositories;

public sealed class TableRepository
{
    private readonly RestaurantReservationDbContext _context;

    public TableRepository(RestaurantReservationDbContext context)
    {
        _context = context;
    }

    public async Task<Table> CreateAsync(Table table)
    {
        await _context.Tables.AddAsync(table);
        await _context.SaveChangesAsync();
        return table;
    }

    public async Task<Table?> UpdateAsync(Table table)
    {
        var current = await _context.Tables.FindAsync(table.TableId);
        if (current is null) return null;

        current.Capacity = table.Capacity;
        current.RestaurantId = table.RestaurantId;

        await _context.SaveChangesAsync();
        return current;
    }

    public async Task<bool> DeleteAsync(int tableId)
    {
        var entity = await _context.Tables.FindAsync(tableId);
        if (entity is null) return false;

        _context.Tables.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
