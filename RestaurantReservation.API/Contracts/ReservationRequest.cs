namespace RestaurantReservation.API.Contracts;

public sealed record ReservationRequest(DateTime Date, int PartySize, int RestaurantId, int CustomerId, int TableId);