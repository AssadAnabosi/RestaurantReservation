namespace RestaurantReservation.API;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; init; } = "RestaurantReservation.API";
    public required string SecretKey { get; init; }
    public int ExpiryDays { get; init; } = 1;
}