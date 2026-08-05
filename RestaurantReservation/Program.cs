using RestaurantReservation;
using RestaurantReservation.Db;
using RestaurantReservation.Db.Entities;

await using var context = new RestaurantReservationDbContext();
var service = new RestaurantReservationService(context);

await service.EnsureDatabaseAsync();

var demoRestaurant = await service.CreateRestaurantAsync(new Restaurant
{
	Name = "Demo Harbor",
	Address = "1 Demo Street",
	PhoneNumber = "555-2001",
	OpeningHours = "10:00-22:00"
});

demoRestaurant.Name = "Demo Harbor Updated";
demoRestaurant.Address = "2 Demo Avenue";
await service.UpdateRestaurantAsync(demoRestaurant);

var demoCustomer = await service.CreateCustomerAsync(new Customer
{
	FirstName = "Test",
	LastName = "Customer",
	Email = "test.customer@example.com",
	PhoneNumber = "555-2002"
});

demoCustomer.PhoneNumber = "555-2999";
await service.UpdateCustomerAsync(demoCustomer);

var demoEmployee = await service.CreateEmployeeAsync(new Employee
{
	FirstName = "Test",
	LastName = "Manager",
	Position = "Manager",
	RestaurantId = demoRestaurant.RestaurantId
});

demoEmployee.LastName = "Manager Updated";
await service.UpdateEmployeeAsync(demoEmployee);

var demoTable = await service.CreateTableAsync(new Table
{
	Capacity = 4,
	RestaurantId = demoRestaurant.RestaurantId
});

demoTable.Capacity = 6;
await service.UpdateTableAsync(demoTable);

var demoMenuItem = await service.CreateMenuItemAsync(new MenuItem
{
	Name = "Demo Burger",
	Description = "A sample burger for console testing",
	Price = 15.25m,
	RestaurantId = demoRestaurant.RestaurantId
});

demoMenuItem.Price = 16.75m;
await service.UpdateMenuItemAsync(demoMenuItem);

var demoReservation = await service.CreateReservationAsync(new Reservation
{
	Date = DateTime.Now.AddDays(7),
	PartySize = 4,
	RestaurantId = demoRestaurant.RestaurantId,
	CustomerId = demoCustomer.CustomerId,
	TableId = demoTable.TableId
});

demoReservation.PartySize = 5;
await service.UpdateReservationAsync(demoReservation);

var demoOrder = await service.CreateOrderAsync(new Order
{
	Date = DateTime.Now.AddDays(7).AddMinutes(15),
	TotalAmount = 16.75m,
	EmployeeId = demoEmployee.EmployeeId,
	ReservationId = demoReservation.ReservationId
});

demoOrder.TotalAmount = 18.00m;
await service.UpdateOrderAsync(demoOrder);

var demoOrderItem = await service.CreateOrderItemAsync(new OrderItem
{
	Quantity = 1,
	ItemId = demoMenuItem.ItemId,
	OrderId = demoOrder.OrderId
});

demoOrderItem.Quantity = 2;
await service.UpdateOrderItemAsync(demoOrderItem);

var managers = await service.ListManagersAsync();
Console.WriteLine("Managers:");
foreach (var manager in managers)
{
	Console.WriteLine($"- {manager.FirstName} {manager.LastName} ({manager.Position})");
}

var customerReservations = await service.GetReservationsByCustomerAsync(demoCustomer.CustomerId);
Console.WriteLine($"Reservations for customer {demoCustomer.CustomerId}:");
foreach (var reservation in customerReservations)
{
	Console.WriteLine($"- Reservation {reservation.ReservationId} on {reservation.Date:u} for {reservation.PartySize} guests at {reservation.Restaurant.Name}");
}

var reservationOrders = await service.ListOrdersAndMenuItemsAsync(demoReservation.ReservationId);
Console.WriteLine($"Orders and menu items for reservation {demoReservation.ReservationId}:");
foreach (var order in reservationOrders)
{
	Console.WriteLine($"- Order {order.OrderId} by {order.EmployeeName} for {order.TotalAmount:C} at {order.Date:u}");
	foreach (var menuItem in order.MenuItems)
	{
		Console.WriteLine($"  - {menuItem.Quantity} x {menuItem.Name} ({menuItem.UnitPrice:C})");
	}
}

var orderedMenuItems = await service.ListOrderedMenuItemsAsync(demoReservation.ReservationId);
Console.WriteLine($"Ordered menu items for reservation {demoReservation.ReservationId}:");
foreach (var menuItem in orderedMenuItems)
{
	Console.WriteLine($"- {menuItem.Name} ({menuItem.Price:C})");
}

var averageOrderAmount = await service.CalculateAverageOrderAmountAsync(demoEmployee.EmployeeId);
Console.WriteLine($"Average order amount for employee {demoEmployee.EmployeeId}: {averageOrderAmount:C}");

var reservationViewRows = await service.ListReservationsWithCustomerAndRestaurantAsync();
Console.WriteLine("Reservations from view:");
foreach (var reservation in reservationViewRows)
{
    Console.WriteLine($"- Reservation {reservation.ReservationId} for {reservation.CustomerFirstName} {reservation.CustomerLastName} at {reservation.RestaurantName}");
}

await service.DeleteOrderItemAsync(demoOrderItem.OrderItemId);
await service.DeleteOrderAsync(demoOrder.OrderId);
await service.DeleteReservationAsync(demoReservation.ReservationId);
await service.DeleteMenuItemAsync(demoMenuItem.ItemId);
await service.DeleteTableAsync(demoTable.TableId);
await service.DeleteEmployeeAsync(demoEmployee.EmployeeId);
await service.DeleteCustomerAsync(demoCustomer.CustomerId);
await service.DeleteRestaurantAsync(demoRestaurant.RestaurantId);

Console.WriteLine("Demo completed.");