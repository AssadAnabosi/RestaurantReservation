using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Views;

namespace RestaurantReservation.Db;

public class RestaurantReservationDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<ReservationCustomerRestaurantView> ReservationCustomerRestaurantViews { get; set; }

    public DbSet<EmployeeRestaurantView> EmployeeRestaurantViews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var connectionString = configuration.GetConnectionString("SQLSERVER_CONNECTIONSTRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'SQLSERVER_CONNECTIONSTRING' was not found in appsettings.json.");
        }

        optionsBuilder
            .UseSqlServer(connectionString)
            .UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MenuItem>().HasKey(p => p.ItemId);

        modelBuilder.Entity<OrderItem>()
            .HasOne<Order>(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Order>()
            .HasOne<Reservation>(o => o.Reservation)
            .WithMany(r => r.Orders)
            .HasForeignKey(o => o.ReservationId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Reservation>()
            .HasOne<Table>(r => r.Table)
            .WithMany(t => t.Reservations)
            .HasForeignKey(r => r.TableId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ReservationCustomerRestaurantView>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vw_reservations_with_customer_restaurant");
        });

        modelBuilder.Entity<EmployeeRestaurantView>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vw_employees_with_restaurant");
        });

        // Seed data: 5 records per table
        modelBuilder.Entity<Restaurant>().HasData(
            new Restaurant
            {
                RestaurantId = 1, Name = "Harbor Dine", Address = "12 Pier Rd", PhoneNumber = "555-0101",
                OpeningHours = "9:00-22:00"
            },
            new Restaurant
            {
                RestaurantId = 2, Name = "Garden Grill", Address = "34 Green St", PhoneNumber = "555-0102",
                OpeningHours = "8:00-21:00"
            },
            new Restaurant
            {
                RestaurantId = 3, Name = "Sunset Bistro", Address = "78 Sunset Blvd", PhoneNumber = "555-0103",
                OpeningHours = "11:00-23:00"
            },
            new Restaurant
            {
                RestaurantId = 4, Name = "Oak & Vine", Address = "101 Oak Ave", PhoneNumber = "555-0104",
                OpeningHours = "10:00-22:30"
            },
            new Restaurant
            {
                RestaurantId = 5, Name = "Maple Table", Address = "202 Maple Dr", PhoneNumber = "555-0105",
                OpeningHours = "7:30-20:30"
            }
        );

        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                CustomerId = 1, FirstName = "Alice", LastName = "Johnson", Email = "alice.johnson@example.com",
                PhoneNumber = "555-1001"
            },
            new Customer
            {
                CustomerId = 2, FirstName = "Bob", LastName = "Martinez", Email = "bob.martinez@example.com",
                PhoneNumber = "555-1002"
            },
            new Customer
            {
                CustomerId = 3, FirstName = "Carol", LastName = "Nguyen", Email = "carol.nguyen@example.com",
                PhoneNumber = "555-1003"
            },
            new Customer
            {
                CustomerId = 4, FirstName = "David", LastName = "Smith", Email = "david.smith@example.com",
                PhoneNumber = "555-1004"
            },
            new Customer
            {
                CustomerId = 5, FirstName = "Eve", LastName = "Brown", Email = "eve.brown@example.com",
                PhoneNumber = "555-1005"
            }
        );

        modelBuilder.Entity<Employee>().HasData(
            new Employee
                { EmployeeId = 1, FirstName = "Liam", LastName = "Cooper", Position = "Manager", RestaurantId = 1 },
            new Employee { EmployeeId = 2, FirstName = "Mia", LastName = "Lopez", Position = "Host", RestaurantId = 2 },
            new Employee { EmployeeId = 3, FirstName = "Noah", LastName = "Kim", Position = "Chef", RestaurantId = 3 },
            new Employee
                { EmployeeId = 4, FirstName = "Olivia", LastName = "Davis", Position = "Server", RestaurantId = 4 },
            new Employee
                { EmployeeId = 5, FirstName = "Ethan", LastName = "Wilson", Position = "Bartender", RestaurantId = 5 }
        );

        modelBuilder.Entity<Table>().HasData(
            new Table { TableId = 1, Capacity = 2, RestaurantId = 1 },
            new Table { TableId = 2, Capacity = 4, RestaurantId = 2 },
            new Table { TableId = 3, Capacity = 6, RestaurantId = 3 },
            new Table { TableId = 4, Capacity = 4, RestaurantId = 4 },
            new Table { TableId = 5, Capacity = 8, RestaurantId = 5 }
        );

        modelBuilder.Entity<MenuItem>().HasData(
            new MenuItem
            {
                ItemId = 1, Name = "Margherita Pizza", Description = "Classic pizza with tomato and mozzarella",
                Price = 12.50m, RestaurantId = 1
            },
            new MenuItem
            {
                ItemId = 2, Name = "Caesar Salad", Description = "Romaine, parmesan, croutons", Price = 8.25m,
                RestaurantId = 2
            },
            new MenuItem
            {
                ItemId = 3, Name = "Grilled Salmon", Description = "Served with lemon butter", Price = 18.00m,
                RestaurantId = 3
            },
            new MenuItem
            {
                ItemId = 4, Name = "Ribeye Steak", Description = "12oz steak with herbs", Price = 24.75m,
                RestaurantId = 4
            },
            new MenuItem
            {
                ItemId = 5, Name = "Pancake Stack", Description = "Three pancakes with syrup", Price = 7.00m,
                RestaurantId = 5
            }
        );

        modelBuilder.Entity<Reservation>().HasData(
            new Reservation
            {
                ReservationId = 1, Date = new DateTime(2026, 8, 10, 19, 0, 0), PartySize = 2, RestaurantId = 1,
                CustomerId = 1, TableId = 1
            },
            new Reservation
            {
                ReservationId = 2, Date = new DateTime(2026, 8, 11, 18, 30, 0), PartySize = 4, RestaurantId = 2,
                CustomerId = 2, TableId = 2
            },
            new Reservation
            {
                ReservationId = 3, Date = new DateTime(2026, 8, 12, 20, 0, 0), PartySize = 6, RestaurantId = 3,
                CustomerId = 3, TableId = 3
            },
            new Reservation
            {
                ReservationId = 4, Date = new DateTime(2026, 8, 13, 19, 30, 0), PartySize = 4, RestaurantId = 4,
                CustomerId = 4, TableId = 4
            },
            new Reservation
            {
                ReservationId = 5, Date = new DateTime(2026, 8, 14, 9, 0, 0), PartySize = 5, RestaurantId = 5,
                CustomerId = 5, TableId = 5
            }
        );

        modelBuilder.Entity<Order>().HasData(
            new Order
            {
                OrderId = 1, Date = new DateTime(2026, 8, 10, 19, 15, 0), TotalAmount = 25.00m, EmployeeId = 1,
                ReservationId = 1
            },
            new Order
            {
                OrderId = 2, Date = new DateTime(2026, 8, 11, 18, 45, 0), TotalAmount = 45.50m, EmployeeId = 2,
                ReservationId = 2
            },
            new Order
            {
                OrderId = 3, Date = new DateTime(2026, 8, 12, 20, 20, 0), TotalAmount = 72.00m, EmployeeId = 3,
                ReservationId = 3
            },
            new Order
            {
                OrderId = 4, Date = new DateTime(2026, 8, 13, 19, 50, 0), TotalAmount = 39.75m, EmployeeId = 4,
                ReservationId = 4
            },
            new Order
            {
                OrderId = 5, Date = new DateTime(2026, 8, 14, 9, 30, 0), TotalAmount = 18.00m, EmployeeId = 5,
                ReservationId = 5
            }
        );

        modelBuilder.Entity<OrderItem>().HasData(
            new OrderItem { OrderItemId = 1, Quantity = 1, ItemId = 1, OrderId = 1 },
            new OrderItem { OrderItemId = 2, Quantity = 2, ItemId = 2, OrderId = 2 },
            new OrderItem { OrderItemId = 3, Quantity = 1, ItemId = 3, OrderId = 3 },
            new OrderItem { OrderItemId = 4, Quantity = 1, ItemId = 4, OrderId = 4 },
            new OrderItem { OrderItemId = 5, Quantity = 3, ItemId = 5, OrderId = 5 }
        );
    }
}