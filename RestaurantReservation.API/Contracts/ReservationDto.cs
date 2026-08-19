namespace RestaurantReservation.API.Contracts;

public sealed record ReservationDto(
    int ReservationId,
    DateTime Date,
    int PartySize,
    int RestaurantId,
    int CustomerId,
    int TableId,
    string? RestaurantName,
    string? CustomerName,
    int? TableCapacity);
