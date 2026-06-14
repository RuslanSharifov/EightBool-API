using Eight.Application.DTOs.Table;
using Eight.Application.Interfaces;
using Eight.Domain.Entities;
using Eight.Domain.Enums;
using Eight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eight.Infrastructure.Services;

public class TableService : ITableService
{
    private readonly AppDbContext _db;

    public TableService(AppDbContext db) => _db = db;

    public async Task<List<TableResponse>> GetByVenueAsync(Guid venueId)
        => await _db.Tables
            .Where(x => x.VenueId == venueId)
            .Select(x => ToResponse(x))
            .ToListAsync();

    public async Task<TableResponse> CreateAsync(TableRequest request)
    {
        var table = new Table
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Type = request.Type,
            PricePerHour = request.PricePerHour,
            VenueId = request.VenueId,
            Status = TableStatus.Available
        };
        _db.Tables.Add(table);
        await _db.SaveChangesAsync();
        return ToResponse(table);
    }

    public async Task<TableResponse> UpdateAsync(Guid id, TableRequest request)
    {
        var table = await _db.Tables.FindAsync(id)
            ?? throw new Exception("Masa tapılmadı.");
        table.Name = request.Name;
        table.Type = request.Type;
        table.PricePerHour = request.PricePerHour;
        await _db.SaveChangesAsync();
        return ToResponse(table);
    }

    public async Task SetStatusAsync(Guid id, TableStatus status)
    {
        var table = await _db.Tables.FindAsync(id)
            ?? throw new Exception("Masa tapılmadı.");
        table.Status = status;
        await _db.SaveChangesAsync();
    }

    private static TableResponse ToResponse(Table x) =>
        new(x.Id, x.Name, x.Type, x.Status, x.PricePerHour, x.IsActive);
}