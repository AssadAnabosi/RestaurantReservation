namespace RestaurantReservation.Db;

public class Customer
{
    public int CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();
}