using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantReservation.Db.Migrations
{
    /// <inheritdoc />
    public partial class CalculateTotalRevenueFunction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var function = """
                           CREATE OR ALTER FUNCTION dbo.fn_calculate_total_revenue_by_restaurant
                           (
                               @restaurant_id int
                           )
                           RETURNS decimal(18,2)
                           AS
                           BEGIN
                               DECLARE @total_revenue decimal(18,2);

                               SELECT @total_revenue = COALESCE(SUM(o.total_amount), 0)
                               FROM orders AS o
                               INNER JOIN reservations AS r ON r.reservation_id = o.reservation_id
                               WHERE r.restaurant_id = @restaurant_id;

                               RETURN COALESCE(@total_revenue, 0);
                           END;
                           """;
            
            migrationBuilder.Sql(function);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS dbo.fn_calculate_total_revenue_by_restaurant;");
        }
    }
}
