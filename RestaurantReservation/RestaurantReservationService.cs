using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Db;
using RestaurantReservation.Db.Entities;

namespace RestaurantReservation;

public sealed class RestaurantReservationService
{
    private readonly RestaurantReservationDbContext _context;

    public RestaurantReservationService(RestaurantReservationDbContext context)
    {
        _context = context;
    }

    public Task EnsureDatabaseAsync() => _context.Database.MigrateAsync();

    public Task<Customer> CreateCustomerAsync(Customer customer) => CreateAsync(_context.Customers, customer);

    public Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant) => CreateAsync(_context.Restaurants, restaurant);

    public Task<Employee> CreateEmployeeAsync(Employee employee) => CreateAsync(_context.Employees, employee);

    public Task<Table> CreateTableAsync(Table table) => CreateAsync(_context.Tables, table);

    public Task<Reservation> CreateReservationAsync(Reservation reservation) => CreateAsync(_context.Reservations, reservation);

    public Task<Order> CreateOrderAsync(Order order) => CreateAsync(_context.Orders, order);

    public Task<MenuItem> CreateMenuItemAsync(MenuItem menuItem) => CreateAsync(_context.MenuItems, menuItem);

    public Task<OrderItem> CreateOrderItemAsync(OrderItem orderItem) => CreateAsync(_context.OrderItems, orderItem);

    public Task<Customer?> UpdateCustomerAsync(Customer customer) => UpdateAsync<Customer>(customer.CustomerId, current =>
    {
        current.FirstName = customer.FirstName;
        current.LastName = customer.LastName;
        current.Email = customer.Email;
        current.PhoneNumber = customer.PhoneNumber;
    });

    public Task<Restaurant?> UpdateRestaurantAsync(Restaurant restaurant) => UpdateAsync<Restaurant>(restaurant.RestaurantId, current =>
    {
        current.Name = restaurant.Name;
        current.Address = restaurant.Address;
        current.PhoneNumber = restaurant.PhoneNumber;
        current.OpeningHours = restaurant.OpeningHours;
    });

    public Task<Employee?> UpdateEmployeeAsync(Employee employee) => UpdateAsync<Employee>(employee.EmployeeId, current =>
    {
        current.FirstName = employee.FirstName;
        current.LastName = employee.LastName;
        current.Position = employee.Position;
        current.RestaurantId = employee.RestaurantId;
    });

    public Task<Table?> UpdateTableAsync(Table table) => UpdateAsync<Table>(table.TableId, current =>
    {
        current.Capacity = table.Capacity;
        current.RestaurantId = table.RestaurantId;
    });

    public Task<Reservation?> UpdateReservationAsync(Reservation reservation) => UpdateAsync<Reservation>(reservation.ReservationId, current =>
    {
        current.Date = reservation.Date;
        current.PartySize = reservation.PartySize;
        current.RestaurantId = reservation.RestaurantId;
        current.CustomerId = reservation.CustomerId;
        current.TableId = reservation.TableId;
    });

    public Task<Order?> UpdateOrderAsync(Order order) => UpdateAsync<Order>(order.OrderId, current =>
    {
        current.Date = order.Date;
        current.TotalAmount = order.TotalAmount;
        current.EmployeeId = order.EmployeeId;
        current.ReservationId = order.ReservationId;
    });

    public Task<MenuItem?> UpdateMenuItemAsync(MenuItem menuItem) => UpdateAsync<MenuItem>(menuItem.ItemId, current =>
    {
        current.Name = menuItem.Name;
        current.Description = menuItem.Description;
        current.Price = menuItem.Price;
        current.RestaurantId = menuItem.RestaurantId;
    });

    public Task<OrderItem?> UpdateOrderItemAsync(OrderItem orderItem) => UpdateAsync<OrderItem>(orderItem.OrderItemId, current =>
    {
        current.Quantity = orderItem.Quantity;
        current.ItemId = orderItem.ItemId;
        current.OrderId = orderItem.OrderId;
    });

    public Task<bool> DeleteCustomerAsync(int customerId) => DeleteAsync<Customer>(customerId);

    public Task<bool> DeleteRestaurantAsync(int restaurantId) => DeleteAsync<Restaurant>(restaurantId);

    public Task<bool> DeleteEmployeeAsync(int employeeId) => DeleteAsync<Employee>(employeeId);

    public Task<bool> DeleteTableAsync(int tableId) => DeleteAsync<Table>(tableId);

    public Task<bool> DeleteReservationAsync(int reservationId) => DeleteAsync<Reservation>(reservationId);

    public Task<bool> DeleteOrderAsync(int orderId) => DeleteAsync<Order>(orderId);

    public Task<bool> DeleteMenuItemAsync(int itemId) => DeleteAsync<MenuItem>(itemId);

    public Task<bool> DeleteOrderItemAsync(int orderItemId) => DeleteAsync<OrderItem>(orderItemId);

    public async Task<List<Employee>> ListManagersAsync()
    {
        return await _context.Employees
            .AsNoTracking()
            .Where(employee => employee.Position == "Manager")
            .OrderBy(employee => employee.LastName)
            .ThenBy(employee => employee.FirstName)
            .ToListAsync();
    }

    public async Task<List<Reservation>> GetReservationsByCustomerAsync(int customerId)
    {
        return await _context.Reservations
            .AsNoTracking()
            .Include(reservation => reservation.Restaurant)
            .Include(reservation => reservation.Table)
            .Include(reservation => reservation.Customer)
            .Where(reservation => reservation.CustomerId == customerId)
            .OrderBy(reservation => reservation.Date)
            .ToListAsync();
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

    private async Task<T> CreateAsync<T>(DbSet<T> set, T entity) where T : class
    {
        await set.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    private async Task<T?> UpdateAsync<T>(int id, Action<T> updateAction) where T : class
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity is null)
        {
            return null;
        }

        updateAction(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    private async Task<bool> DeleteAsync<T>(int id) where T : class
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity is null)
        {
            return false;
        }

        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}

public sealed record OrderMenuItemDto(int ItemId, string Name, int Quantity, decimal UnitPrice);

public sealed record OrderWithMenuItemsDto(
    int OrderId,
    DateTime Date,
    decimal TotalAmount,
    string EmployeeName,
    List<OrderMenuItemDto> MenuItems);