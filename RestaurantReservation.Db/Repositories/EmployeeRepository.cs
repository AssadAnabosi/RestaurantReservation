using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Repositories;

public sealed class EmployeeRepository
{
    private readonly RestaurantReservationDbContext _context;

    public EmployeeRepository(RestaurantReservationDbContext context)
    {
        _context = context;
    }

    public async Task<Employee> CreateAsync(Employee employee)
    {
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee?> UpdateAsync(Employee employee)
    {
        var current = await _context.Employees.FindAsync(employee.EmployeeId);
        if (current is null) return null;

        current.FirstName = employee.FirstName;
        current.LastName = employee.LastName;
        current.Position = employee.Position;
        current.RestaurantId = employee.RestaurantId;

        await _context.SaveChangesAsync();
        return current;
    }

    public async Task<bool> DeleteAsync(int employeeId)
    {
        var entity = await _context.Employees.FindAsync(employeeId);
        if (entity is null) return false;

        _context.Employees.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Employee>> ListManagersAsync()
    {
        return await _context.Employees
            .AsNoTracking()
            .Where(e => e.Position == "Manager")
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync();
    }
}
