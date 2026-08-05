using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantReservation.Db.Migrations
{
    /// <inheritdoc />
    public partial class SP_CustomerReservationPartySizeGreaterThan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var storedProcedure = """
                                  CREATE OR ALTER PROCEDURE dbo.sp_get_customers_with_reservation_party_size_greater_than
                                      @party_size int
                                  AS
                                  BEGIN
                                      SET NOCOUNT ON;

                                      SELECT DISTINCT c.customer_id,
                                                      c.first_name,
                                                      c.last_name,
                                                      c.email,
                                                      c.phone_number
                                      FROM customers AS c
                                      INNER JOIN reservations AS r ON r.customer_id = c.customer_id
                                      WHERE r.party_size > @party_size
                                      ORDER BY c.last_name, c.first_name;
                                  END;
                                  """;

            migrationBuilder.Sql(storedProcedure);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.sp_get_customers_with_reservation_party_size_greater_than;");
        }
    }
}
