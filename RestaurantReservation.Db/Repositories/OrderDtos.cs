namespace RestaurantReservation;

public sealed record OrderMenuItemDto(int ItemId, string Name, int Quantity, decimal UnitPrice);

public sealed record OrderWithMenuItemsDto(
    int OrderId,
    DateTime Date,
    decimal TotalAmount,
    string EmployeeName,
    List<OrderMenuItemDto> MenuItems);
