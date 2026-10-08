using Microsoft.EntityFrameworkCore;
using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.Db.Repositories;

public sealed class ReservationRepository
{
    private readonly RestaurantReservationDbContext _context;

    public ReservationRepository(RestaurantReservationDbContext context)
    {
        _context = context;
    }

    public async Task<Reservation> CreateAsync(Reservation reservation)
    {
        await _context.Reservations.AddAsync(reservation);
        await _context.SaveChangesAsync();
        return reservation;
    }

    public async Task<List<Reservation>> ListAsync()
    {
        return await _context.Reservations
            .AsNoTracking()
            .Include(reservation => reservation.Restaurant)
            .Include(reservation => reservation.Customer)
            .Include(reservation => reservation.Table)
            .OrderBy(reservation => reservation.Date)
            .ToListAsync();
    }

    public async Task<Reservation?> GetByIdAsync(int reservationId)
    {
        return await _context.Reservations
            .AsNoTracking()
            .Include(reservation => reservation.Restaurant)
            .Include(reservation => reservation.Customer)
            .Include(reservation => reservation.Table)
            .FirstOrDefaultAsync(reservation => reservation.ReservationId == reservationId);
    }

    public async Task<Reservation?> UpdateAsync(Reservation reservation)
    {
        var current = await _context.Reservations.FindAsync(reservation.ReservationId);
        if (current is null) return null;

        current.Date = reservation.Date;
        current.PartySize = reservation.PartySize;
        current.RestaurantId = reservation.RestaurantId;
        current.CustomerId = reservation.CustomerId;
        current.TableId = reservation.TableId;

        await _context.SaveChangesAsync();
        return current;
    }

    public async Task<bool> DeleteAsync(int reservationId)
    {
        var entity = await _context.Reservations.FindAsync(reservationId);
        if (entity is null) return false;

        _context.Reservations.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Reservation>> GetReservationsByCustomerAsync(int customerId)
    {
        return await _context.Reservations
            .AsNoTracking()
            .Include(r => r.Restaurant)
            .Include(r => r.Table)
            .Include(r => r.Customer)
            .Where(r => r.CustomerId == customerId)
            .OrderBy(r => r.Date)
            .ToListAsync();
    }
}