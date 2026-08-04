using System.ComponentModel.DataAnnotations;

namespace RestaurantReservation.Db;

public class MenuItem
{
    public int ItemId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }

    public int RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
}