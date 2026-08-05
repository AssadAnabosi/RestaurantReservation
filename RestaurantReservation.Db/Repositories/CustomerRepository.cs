using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Repositories;

public sealed class CustomerRepository
{
    private readonly RestaurantReservationDbContext _context;

    public CustomerRepository(RestaurantReservationDbContext context)
    {
        _context = context;
    }

    public async Task<Customer> CreateAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<Customer?> UpdateAsync(Customer customer)
    {
        var current = await _context.Customers.FindAsync(customer.CustomerId);
        if (current is null) return null;

        current.FirstName = customer.FirstName;
        current.LastName = customer.LastName;
        current.Email = customer.Email;
        current.PhoneNumber = customer.PhoneNumber;

        await _context.SaveChangesAsync();
        return current;
    }

    public async Task<bool> DeleteAsync(int customerId)
    {
        var entity = await _context.Customers.FindAsync(customerId);
        if (entity is null) return false;

        _context.Customers.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Customer>> GetCustomersWithReservationPartySizeGreaterThanAsync(int partySize)
    {
        return await _context.Customers
            .FromSql($"EXEC dbo.sp_get_customers_with_reservation_party_size_greater_than {partySize}")
            .AsNoTracking()
            .ToListAsync();
    }
}
