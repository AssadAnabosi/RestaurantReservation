using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantReservation.Db.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeRestaurantView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var view = """
                       CREATE OR ALTER VIEW dbo.vw_employees_with_restaurant AS
                       SELECT
                           e.employee_id AS employee_id,
                           e.first_name AS employee_first_name,
                           e.last_name AS employee_last_name,
                           e.position AS position,
                           e.restaurant_id AS restaurant_id,
                           rest.name AS restaurant_name,
                           rest.address AS restaurant_address,
                           rest.phone_number AS restaurant_phone_number,
                           rest.opening_hours AS restaurant_opening_hours
                       FROM employees AS e
                       INNER JOIN restaurants AS rest ON rest.restaurant_id = e.restaurant_id;
                       """;
            migrationBuilder.Sql(view);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS dbo.vw_employees_with_restaurant;");
        }
    }
}
