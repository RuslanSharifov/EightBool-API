using Eight.Application.DTOs.Session;
using Eight.Application.Interfaces;
using Eight.Domain.Entities;
using Eight.Domain.Enums;
using Eight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eight.Infrastructure.Services;

public class SessionService : ISessionService
{
    private readonly AppDbContext _db;

    public SessionService(AppDbContext db) => _db = db;

    public async Task<SessionResponse> OpenAsync(SessionRequest request)
    {
        var table = await _db.Tables.FindAsync(request.TableId)
            ?? throw new Exception("Masa tapılmadı.");

        if (table.Status == TableStatus.Occupied)
            throw new Exception("Masa doludur.");

        var session = new Session
        {
            Id = Guid.NewGuid(),
            TableId = request.TableId,
            CustomerCount = request.CustomerCount,
            OpenedAt = DateTime.UtcNow,
            Status = SessionStatus.Open
        };

        table.Status = TableStatus.Occupied;
        _db.Sessions.Add(session);
        await _db.SaveChangesAsync();
        return ToResponse(session, table.Name);
    }

    public async Task<SessionResponse> CloseAsync(Guid id)
    {
        var session = await _db.Sessions
            .Include(x => x.Table)
            .Include(x => x.Orders)
            .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new Exception("Sessiya tapılmadı.");

        session.ClosedAt = DateTime.UtcNow;
        session.Status = SessionStatus.Closed;

        var hours = (decimal)(session.ClosedAt.Value - session.OpenedAt).TotalHours;
        var tableAmount = Math.Ceiling(hours * session.Table.PricePerHour * 100) / 100;
        var ordersAmount = session.Orders.Sum(x => x.Quantity * x.UnitPrice);

        var venue = await _db.Venues.FindAsync(session.Table.VenueId);
        var serviceCharge = 0m;
        if (venue?.ServiceChargeEnabled == true)
            serviceCharge = (tableAmount + ordersAmount) * venue.ServiceChargePercent / 100;

        session.TotalAmount = tableAmount + ordersAmount + serviceCharge;
        session.Table.Status = TableStatus.Available;

        await _db.SaveChangesAsync();
        return ToResponse(session, session.Table.Name);
    }

    public async Task<SessionResponse> GetByIdAsync(Guid id)
    {
        var session = await _db.Sessions
            .Include(x => x.Table)
            .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new Exception("Sessiya tapılmadı.");
        return ToResponse(session, session.Table.Name);
    }

    public async Task<List<SessionResponse>> GetActiveByVenueAsync(Guid venueId)
        => await _db.Sessions
            .Include(x => x.Table)
            .Where(x => x.Table.VenueId == venueId && x.Status == SessionStatus.Open)
            .Select(x => ToResponse(x, x.Table.Name))
            .ToListAsync();

    private static SessionResponse ToResponse(Session x, string tableName) => new(
        x.Id, x.TableId, tableName, x.CustomerCount,
        x.OpenedAt, x.ClosedAt, x.TotalAmount, x.Status.ToString());
}