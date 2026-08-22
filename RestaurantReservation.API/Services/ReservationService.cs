using RestaurantReservation.API.Contracts;
using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Repositories;

namespace RestaurantReservation.API.Services;

public sealed class ReservationService : IReservationService
{
    private readonly ReservationRepository _reservationRepository;
    private readonly OrderRepository _orderRepository;

    public ReservationService(ReservationRepository reservationRepository, OrderRepository orderRepository)
    {
        _reservationRepository = reservationRepository;
        _orderRepository = orderRepository;
    }

    public async Task<List<ReservationDto>> ListAsync() =>
        (await _reservationRepository.ListAsync()).Select(MapReservation).ToList();

    public async Task<ReservationDto?> GetByIdAsync(int reservationId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        return reservation is null ? null : MapReservation(reservation);
    }

    public async Task<ReservationDto> CreateAsync(ReservationRequest request)
    {
        var reservation = await _reservationRepository.CreateAsync(new Reservation
        {
            Date = request.Date,
            PartySize = request.PartySize,
            RestaurantId = request.RestaurantId,
            CustomerId = request.CustomerId,
            TableId = request.TableId
        });
        return MapReservation(reservation);
    }

    public async Task<ReservationDto?> UpdateAsync(int reservationId, ReservationRequest request)
    {
        var reservation = await _reservationRepository.UpdateAsync(new Reservation
        {
            ReservationId = reservationId,
            Date = request.Date,
            PartySize = request.PartySize,
            RestaurantId = request.RestaurantId,
            CustomerId = request.CustomerId,
            TableId = request.TableId
        });
        return reservation is null ? null : MapReservation(reservation);
    }

    public Task<bool> DeleteAsync(int reservationId) => _reservationRepository.DeleteAsync(reservationId);

    public async Task<List<ReservationDto>> GetReservationsByCustomerAsync(int customerId) =>
        (await _reservationRepository.GetReservationsByCustomerAsync(customerId)).Select(MapReservation).ToList();

    public async Task<List<OrderDto>> ListOrdersAndMenuItemsAsync(int reservationId) =>
        (await _orderRepository.ListOrdersAndMenuItemsAsync(reservationId))
        .Select(order => new OrderDto(
            order.OrderId,
            order.Date,
            order.TotalAmount,
            order.EmployeeName,
            order.MenuItems
                .Select(item => new RestaurantReservation.API.Contracts.OrderMenuItemDto(
                    item.ItemId, item.Name, item.Quantity, item.UnitPrice))
                .ToList()))
        .ToList();

    public async Task<List<MenuItemDto>> ListOrderedMenuItemsAsync(int reservationId) =>
        (await _orderRepository.ListOrderedMenuItemsAsync(reservationId))
        .Select(item => new MenuItemDto(item.ItemId, item.Name, item.Description, item.Price, item.RestaurantId))
        .ToList();

    private static ReservationDto MapReservation(Reservation reservation) => new(
        reservation.ReservationId,
        reservation.Date,
        reservation.PartySize,
        reservation.RestaurantId,
        reservation.CustomerId,
        reservation.TableId,
        reservation.Restaurant?.Name,
        reservation.Customer is null ? null : $"{reservation.Customer.FirstName} {reservation.Customer.LastName}",
        reservation.Table?.Capacity);
}
