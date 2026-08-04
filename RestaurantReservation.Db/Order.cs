namespace RestaurantReservation.Db;

public class Order
{
    public int OrderId { get; set; }
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }

    public int EmployeeId { get; set; }
    public virtual Employee Employee { get; set; } = null!;

    public int ReservationId { get; set; }
    public virtual Reservation Reservation { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
}