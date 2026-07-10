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
        if (request.CustomerCount <= 0)
            throw new Exception("Müştəri sayı ən azı 1 olmalıdır.");

        var table = await _db.Tables.FindAsync(request.TableId)
            ?? throw new Exception("Masa tapılmadı.");

        if (!table.IsActive)
            throw new Exception("Bu masa deaktivdir, sessiya açıla bilməz.");

        if (table.Status == TableStatus.Occupied)
            throw new Exception("Masa artıq doludur.");

        if (table.Status == TableStatus.UnderRepair)
            throw new Exception("Masa təmirdədir, sessiya açıla bilməz.");

        if (table.Status == TableStatus.OutOfOrder)
            throw new Exception("Masa sıradan çıxıb, sessiya açıla bilməz.");

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

        if (session.Status == SessionStatus.Closed)
            throw new Exception("Bu sessiya artıq bağlanıb.");

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

    public async Task<SessionResponse?> GetActiveByTableAsync(Guid tableId)
    {
        var table = await _db.Tables.FindAsync(tableId)
            ?? throw new Exception("Masa tapılmadı.");

        var session = await _db.Sessions
            .Include(x => x.Table)
            .Where(x => x.TableId == tableId && x.Status == SessionStatus.Open)
            .FirstOrDefaultAsync();

        // Aktiv sessiya yoxdursa xəta yox, sadəcə null qaytarılır — bu normal haldır (masa boşdur).
        return session is null ? null : ToResponse(session, table.Name);
    }

    public async Task<List<SessionResponse>> GetActiveByVenueAsync(Guid venueId)
        => await _db.Sessions
            .Include(x => x.Table)
            .Where(x => x.Table.VenueId == venueId && x.Status == SessionStatus.Open)
            .Select(x => ToResponse(x, x.Table.Name))
            .ToListAsync();

    public async Task<List<SessionResponse>> GetHistoryByVenueAsync(Guid venueId, DateTime? from, DateTime? to)
    {
        var query = _db.Sessions
            .Include(x => x.Table)
            .Where(x => x.Table.VenueId == venueId && x.Status == SessionStatus.Closed);

        if (from.HasValue)
            query = query.Where(x => x.OpenedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.OpenedAt <= to.Value);

        return await query
            .OrderByDescending(x => x.OpenedAt)
            .Select(x => ToResponse(x, x.Table.Name))
            .ToListAsync();
    }

    public async Task<SessionResponse> UpdateCustomerCountAsync(Guid sessionId, int customerCount)
    {
        if (customerCount <= 0)
            throw new Exception("Müştəri sayı ən azı 1 olmalıdır.");

        var session = await _db.Sessions
            .Include(x => x.Table)
            .FirstOrDefaultAsync(x => x.Id == sessionId)
            ?? throw new Exception("Sessiya tapılmadı.");

        if (session.Status == SessionStatus.Closed)
            throw new Exception("Bağlı sessiyada müştəri sayı dəyişdirilə bilməz.");

        session.CustomerCount = customerCount;
        await _db.SaveChangesAsync();
        return ToResponse(session, session.Table.Name);
    }

    private static SessionResponse ToResponse(Session x, string tableName) => new(
        x.Id, x.TableId, tableName, x.CustomerCount,
        x.OpenedAt, x.ClosedAt, x.TotalAmount, x.Status.ToString());
}