using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Db;
using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Views;
using RestaurantReservation.Db.Repositories;

namespace RestaurantReservation;

public sealed class RestaurantReservationService
{
    private readonly RestaurantReservationDbContext _context;
    private readonly CustomerRepository _customerRepository;
    private readonly RestaurantRepository _restaurantRepository;
    private readonly EmployeeRepository _employeeRepository;
    private readonly TableRepository _tableRepository;
    private readonly ReservationRepository _reservationRepository;
    private readonly OrderRepository _orderRepository;
    private readonly MenuItemRepository _menuItemRepository;
    private readonly OrderItemRepository _orderItemRepository;
    private readonly ReservationCustomerRestaurantViewRepository _reservationCustomerRestaurantViewRepository;
    private readonly EmployeeRestaurantViewRepository _employeeRestaurantViewRepository;

    public RestaurantReservationService(RestaurantReservationDbContext context)
    {
        _context = context;
        _customerRepository = new CustomerRepository(context);
        _restaurantRepository = new RestaurantRepository(context);
        _employeeRepository = new EmployeeRepository(context);
        _tableRepository = new TableRepository(context);
        _reservationRepository = new ReservationRepository(context);
        _orderRepository = new OrderRepository(context);
        _menuItemRepository = new MenuItemRepository(context);
        _orderItemRepository = new OrderItemRepository(context);
        _reservationCustomerRestaurantViewRepository = new ReservationCustomerRestaurantViewRepository(context);
        _employeeRestaurantViewRepository = new EmployeeRestaurantViewRepository(context);
    }

    public Task EnsureDatabaseAsync() => _context.Database.MigrateAsync();

    public Task<Customer> CreateCustomerAsync(Customer customer) => _customerRepository.CreateAsync(customer);

    public Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant) =>
        _restaurantRepository.CreateAsync(restaurant);

    public Task<Employee> CreateEmployeeAsync(Employee employee) => _employeeRepository.CreateAsync(employee);

    public Task<Table> CreateTableAsync(Table table) => _tableRepository.CreateAsync(table);

    public Task<Reservation> CreateReservationAsync(Reservation reservation) =>
        _reservationRepository.CreateAsync(reservation);

    public Task<Order> CreateOrderAsync(Order order) => _orderRepository.CreateAsync(order);

    public Task<MenuItem> CreateMenuItemAsync(MenuItem menuItem) => _menuItemRepository.CreateAsync(menuItem);

    public Task<OrderItem> CreateOrderItemAsync(OrderItem orderItem) => _orderItemRepository.CreateAsync(orderItem);

    public Task<Customer?> UpdateCustomerAsync(Customer customer) => _customerRepository.UpdateAsync(customer);

    public Task<Restaurant?> UpdateRestaurantAsync(Restaurant restaurant) =>
        _restaurantRepository.UpdateAsync(restaurant);

    public Task<Employee?> UpdateEmployeeAsync(Employee employee) => _employeeRepository.UpdateAsync(employee);

    public Task<Table?> UpdateTableAsync(Table table) => _tableRepository.UpdateAsync(table);

    public Task<Reservation?> UpdateReservationAsync(Reservation reservation) =>
        _reservationRepository.UpdateAsync(reservation);

    public Task<Order?> UpdateOrderAsync(Order order) => _orderRepository.UpdateAsync(order);

    public Task<MenuItem?> UpdateMenuItemAsync(MenuItem menuItem) => _menuItemRepository.UpdateAsync(menuItem);

    public Task<OrderItem?> UpdateOrderItemAsync(OrderItem orderItem) => _orderItemRepository.UpdateAsync(orderItem);

    public Task<bool> DeleteCustomerAsync(int customerId) => _customerRepository.DeleteAsync(customerId);

    public Task<bool> DeleteRestaurantAsync(int restaurantId) => _restaurantRepository.DeleteAsync(restaurantId);

    public Task<bool> DeleteEmployeeAsync(int employeeId) => _employeeRepository.DeleteAsync(employeeId);

    public Task<bool> DeleteTableAsync(int tableId) => _tableRepository.DeleteAsync(tableId);

    public Task<bool> DeleteReservationAsync(int reservationId) => _reservationRepository.DeleteAsync(reservationId);

    public Task<bool> DeleteOrderAsync(int orderId) => _orderRepository.DeleteAsync(orderId);

    public Task<bool> DeleteMenuItemAsync(int itemId) => _menuItemRepository.DeleteAsync(itemId);

    public Task<bool> DeleteOrderItemAsync(int orderItemId) => _orderItemRepository.DeleteAsync(orderItemId);

    public Task<List<Employee>> ListManagersAsync() => _employeeRepository.ListManagersAsync();

    public Task<List<Reservation>> GetReservationsByCustomerAsync(int customerId) =>
        _reservationRepository.GetReservationsByCustomerAsync(customerId);

    public Task<List<OrderWithMenuItemsDto>> ListOrdersAndMenuItemsAsync(int reservationId) =>
        _orderRepository.ListOrdersAndMenuItemsAsync(reservationId);

    public Task<List<MenuItem>> ListOrderedMenuItemsAsync(int reservationId) =>
        _orderRepository.ListOrderedMenuItemsAsync(reservationId);

    public Task<decimal> CalculateAverageOrderAmountAsync(int employeeId) =>
        _orderRepository.CalculateAverageOrderAmountAsync(employeeId);

    public Task<List<ReservationCustomerRestaurantView>> ListReservationsWithCustomerAndRestaurantAsync() =>
        _reservationCustomerRestaurantViewRepository.ListReservationsWithCustomerAndRestaurantAsync();

    public Task<List<EmployeeRestaurantView>> ListEmployeesWithRestaurantAsync() =>
        _employeeRestaurantViewRepository.ListEmployeesWithRestaurantAsync();

    public Task<List<Customer>> GetCustomersWithReservationPartySizeGreaterThanAsync(int partySize)
        => _customerRepository.GetCustomersWithReservationPartySizeGreaterThanAsync(partySize);
}