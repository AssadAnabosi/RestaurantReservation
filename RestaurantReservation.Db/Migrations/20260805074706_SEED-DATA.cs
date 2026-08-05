using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RestaurantReservation.Db.Migrations
{
    /// <inheritdoc />
    public partial class SEEDDATA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "customers",
                columns: new[] { "customer_id", "email", "first_name", "last_name", "phone_number" },
                values: new object[,]
                {
                    { 1, "alice.johnson@example.com", "Alice", "Johnson", "555-1001" },
                    { 2, "bob.martinez@example.com", "Bob", "Martinez", "555-1002" },
                    { 3, "carol.nguyen@example.com", "Carol", "Nguyen", "555-1003" },
                    { 4, "david.smith@example.com", "David", "Smith", "555-1004" },
                    { 5, "eve.brown@example.com", "Eve", "Brown", "555-1005" }
                });

            migrationBuilder.InsertData(
                table: "restaurants",
                columns: new[] { "restaurant_id", "address", "name", "opening_hours", "phone_number" },
                values: new object[,]
                {
                    { 1, "12 Pier Rd", "Harbor Dine", "9:00-22:00", "555-0101" },
                    { 2, "34 Green St", "Garden Grill", "8:00-21:00", "555-0102" },
                    { 3, "78 Sunset Blvd", "Sunset Bistro", "11:00-23:00", "555-0103" },
                    { 4, "101 Oak Ave", "Oak & Vine", "10:00-22:30", "555-0104" },
                    { 5, "202 Maple Dr", "Maple Table", "7:30-20:30", "555-0105" }
                });

            migrationBuilder.InsertData(
                table: "employees",
                columns: new[] { "employee_id", "first_name", "last_name", "position", "restaurant_id" },
                values: new object[,]
                {
                    { 1, "Liam", "Cooper", "Manager", 1 },
                    { 2, "Mia", "Lopez", "Host", 2 },
                    { 3, "Noah", "Kim", "Chef", 3 },
                    { 4, "Olivia", "Davis", "Server", 4 },
                    { 5, "Ethan", "Wilson", "Bartender", 5 }
                });

            migrationBuilder.InsertData(
                table: "menu_items",
                columns: new[] { "item_id", "description", "name", "price", "restaurant_id" },
                values: new object[,]
                {
                    { 1, "Classic pizza with tomato and mozzarella", "Margherita Pizza", 12.50m, 1 },
                    { 2, "Romaine, parmesan, croutons", "Caesar Salad", 8.25m, 2 },
                    { 3, "Served with lemon butter", "Grilled Salmon", 18.00m, 3 },
                    { 4, "12oz steak with herbs", "Ribeye Steak", 24.75m, 4 },
                    { 5, "Three pancakes with syrup", "Pancake Stack", 7.00m, 5 }
                });

            migrationBuilder.InsertData(
                table: "tables",
                columns: new[] { "table_id", "capacity", "restaurant_id" },
                values: new object[,]
                {
                    { 1, 2, 1 },
                    { 2, 4, 2 },
                    { 3, 6, 3 },
                    { 4, 4, 4 },
                    { 5, 8, 5 }
                });

            migrationBuilder.InsertData(
                table: "reservations",
                columns: new[] { "reservation_id", "customer_id", "date", "party_size", "restaurant_id", "table_id" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 8, 10, 19, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, 1 },
                    { 2, 2, new DateTime(2026, 8, 11, 18, 30, 0, 0, DateTimeKind.Unspecified), 4, 2, 2 },
                    { 3, 3, new DateTime(2026, 8, 12, 20, 0, 0, 0, DateTimeKind.Unspecified), 6, 3, 3 },
                    { 4, 4, new DateTime(2026, 8, 13, 19, 30, 0, 0, DateTimeKind.Unspecified), 4, 4, 4 },
                    { 5, 5, new DateTime(2026, 8, 14, 9, 0, 0, 0, DateTimeKind.Unspecified), 5, 5, 5 }
                });

            migrationBuilder.InsertData(
                table: "orders",
                columns: new[] { "order_id", "date", "employee_id", "reservation_id", "total_amount" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 8, 10, 19, 15, 0, 0, DateTimeKind.Unspecified), 1, 1, 25.00m },
                    { 2, new DateTime(2026, 8, 11, 18, 45, 0, 0, DateTimeKind.Unspecified), 2, 2, 45.50m },
                    { 3, new DateTime(2026, 8, 12, 20, 20, 0, 0, DateTimeKind.Unspecified), 3, 3, 72.00m },
                    { 4, new DateTime(2026, 8, 13, 19, 50, 0, 0, DateTimeKind.Unspecified), 4, 4, 39.75m },
                    { 5, new DateTime(2026, 8, 14, 9, 30, 0, 0, DateTimeKind.Unspecified), 5, 5, 18.00m }
                });

            migrationBuilder.InsertData(
                table: "order_items",
                columns: new[] { "order_item_id", "item_id", "order_id", "quantity" },
                values: new object[,]
                {
                    { 1, 1, 1, 1 },
                    { 2, 2, 2, 2 },
                    { 3, 3, 3, 1 },
                    { 4, 4, 4, 1 },
                    { 5, 5, 5, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "order_items",
                keyColumn: "order_item_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "order_items",
                keyColumn: "order_item_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "order_items",
                keyColumn: "order_item_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "order_items",
                keyColumn: "order_item_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "order_items",
                keyColumn: "order_item_id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "menu_items",
                keyColumn: "item_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "menu_items",
                keyColumn: "item_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "menu_items",
                keyColumn: "item_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "menu_items",
                keyColumn: "item_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "menu_items",
                keyColumn: "item_id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "orders",
                keyColumn: "order_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "orders",
                keyColumn: "order_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "orders",
                keyColumn: "order_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "orders",
                keyColumn: "order_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "orders",
                keyColumn: "order_id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "employees",
                keyColumn: "employee_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "employees",
                keyColumn: "employee_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "employees",
                keyColumn: "employee_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "employees",
                keyColumn: "employee_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "employees",
                keyColumn: "employee_id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "reservations",
                keyColumn: "reservation_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "reservations",
                keyColumn: "reservation_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "reservations",
                keyColumn: "reservation_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "reservations",
                keyColumn: "reservation_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "reservations",
                keyColumn: "reservation_id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "customers",
                keyColumn: "customer_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "customers",
                keyColumn: "customer_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "customers",
                keyColumn: "customer_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "customers",
                keyColumn: "customer_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "customers",
                keyColumn: "customer_id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "tables",
                keyColumn: "table_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "tables",
                keyColumn: "table_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "tables",
                keyColumn: "table_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "tables",
                keyColumn: "table_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "tables",
                keyColumn: "table_id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "restaurants",
                keyColumn: "restaurant_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "restaurants",
                keyColumn: "restaurant_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "restaurants",
                keyColumn: "restaurant_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "restaurants",
                keyColumn: "restaurant_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "restaurants",
                keyColumn: "restaurant_id",
                keyValue: 5);
        }
    }
}
