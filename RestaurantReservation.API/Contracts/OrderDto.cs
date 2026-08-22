namespace RestaurantReservation.API.Contracts;

public sealed record OrderDto(
    int OrderId,
    DateTime Date,
    decimal TotalAmount,
    string EmployeeName,
    IReadOnlyList<OrderMenuItemDto> MenuItems);

public sealed record OrderMenuItemDto(
    int ItemId,
    string Name,
    int Quantity,
    decimal UnitPrice);
