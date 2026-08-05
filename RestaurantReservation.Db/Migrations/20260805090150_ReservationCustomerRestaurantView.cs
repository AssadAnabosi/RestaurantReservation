using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantReservation.Db.Migrations
{
    /// <inheritdoc />
    public partial class ReservationCustomerRestaurantView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var view = """
                       CREATE OR ALTER VIEW dbo.vw_reservations_with_customer_restaurant AS
                       SELECT
                           r.reservation_id AS reservation_id,
                           r.date AS reservation_date,
                           r.party_size AS party_size,
                           r.customer_id AS customer_id,
                           c.first_name AS customer_first_name,
                           c.last_name AS customer_last_name,
                           c.email AS customer_email,
                           c.phone_number AS customer_phone_number,
                           r.restaurant_id AS restaurant_id,
                           rest.name AS restaurant_name,
                           rest.address AS restaurant_address,
                           rest.phone_number AS restaurant_phone_number,
                           rest.opening_hours AS restaurant_opening_hours
                       FROM reservations AS r
                       INNER JOIN customers AS c ON c.customer_id = r.customer_id
                       INNER JOIN restaurants AS rest ON rest.restaurant_id = r.restaurant_id;
                       """;
            migrationBuilder.Sql(view);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS dbo.vw_reservations_with_customer_restaurant;");
        }
    }
}
