namespace RestaurantReservation.Db.Entities;

public class Table
{
    public int TableId { get; set; }
    public int Capacity { get; set; }

    public int RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();
}