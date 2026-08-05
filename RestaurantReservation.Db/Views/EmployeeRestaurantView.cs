namespace RestaurantReservation.Db.Views;

public sealed class EmployeeRestaurantView
{
    public int EmployeeId { get; set; }
    public string EmployeeFirstName { get; set; } = string.Empty;
    public string EmployeeLastName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public int RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public string RestaurantAddress { get; set; } = string.Empty;
    public string? RestaurantPhoneNumber { get; set; }
    public string? RestaurantOpeningHours { get; set; }
}