namespace RestaurantReservation.API.Contracts;

public sealed record MenuItemDto(
    int ItemId,
    string Name,
    string Description,
    decimal Price,
    int RestaurantId);
