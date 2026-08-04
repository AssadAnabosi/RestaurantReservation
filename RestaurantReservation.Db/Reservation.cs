namespace RestaurantReservation.Db;

public class Reservation
{
    public int ReservationId { get; set; }
    public DateTime Date { get; set; }
    public int PartySize { get; set; }

    public int RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;

    public int CustomerId { get; set; }
    public virtual Customer Customer { get; set; } = null!;

    public int TableId { get; set; }
    public virtual Table Table { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
}