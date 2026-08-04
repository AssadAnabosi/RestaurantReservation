namespace RestaurantReservation.Db.Entities;

public class Employee
{
    public int EmployeeId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Position { get; set; }

    public int RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
}