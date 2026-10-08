using RestaurantReservation.API.Contracts;

namespace RestaurantReservation.API.Services;

public interface IReservationService
{
    Task<List<ReservationDto>> ListAsync();
    Task<ReservationDto?> GetByIdAsync(int reservationId);
    Task<ReservationDto> CreateAsync(ReservationRequest request);
    Task<ReservationDto?> UpdateAsync(int reservationId, ReservationRequest request);
    Task<bool> DeleteAsync(int reservationId);
    Task<List<ReservationDto>> GetReservationsByCustomerAsync(int customerId);
    Task<List<OrderDto>> ListOrdersAndMenuItemsAsync(int reservationId);
    Task<List<MenuItemDto>> ListOrderedMenuItemsAsync(int reservationId);
}
