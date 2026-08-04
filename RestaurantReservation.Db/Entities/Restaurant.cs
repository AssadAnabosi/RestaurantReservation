namespace RestaurantReservation.Db.Entities;

public class Restaurant
{
    public int RestaurantId { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? OpeningHours { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();
    public virtual ICollection<Table> Tables { get; set; } = new HashSet<Table>();
    public virtual ICollection<MenuItem> MenuItems { get; set; } = new HashSet<MenuItem>();
}