namespace RestaurantReservation.Db.Entities;

public class OrderItem
{
    public int OrderItemId { get; set; }
    public int Quantity { get; set; }

    public int ItemId { get; set; }
    public virtual MenuItem Item { get; set; } = null!;

    public int OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;
}