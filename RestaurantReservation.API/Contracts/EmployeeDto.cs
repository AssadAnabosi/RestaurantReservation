namespace RestaurantReservation.API.Contracts;

public sealed record EmployeeDto(
    int EmployeeId,
    string FirstName,
    string LastName,
    string Position,
    int RestaurantId);
