using Eight.Application.DTOs.Enum;
using Eight.Application.DTOs.Table;
using Eight.Application.Interfaces;
using Eight.Domain.Entities;
using Eight.Domain.Enums;
using Eight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

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

    public Task<List<EnumResponse>> GetTableTypes()
    {
        var types = Enum.GetValues<TableType>()
            .Select(x => new EnumResponse
            {
                Value = (int)x,
                Name = x.ToString()
            })
            .ToList();

        return Task.FromResult(types);
    }

    public Task<List<EnumResponse>> GetTableStatus()
    {
        var status = Enum.GetValues<TableStatus>()
            .Select(x => new EnumResponse
            {
                Value = (int)x,
                Name = x.ToString()
            })
            .ToList();

        return Task.FromResult(status);
    }

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


        if (request.Name.Length < 3)
            throw new Exception("Masa adı minimum 3 simvoldan ibarət olmalıdır.");
        table.Name = request.Name;
        table.Type = request.Type;
        table.PricePerHour = Math.Abs(request.PricePerHour);
        await _db.SaveChangesAsync();
        return ToResponse(table);
    }

    public async Task SetStatusAsync(Guid id, TableStatus status)
    {
        var table = await _db.Tables.FindAsync(id)
            ?? throw new Exception("Masa tapılmadı.");
        if (table.Status != TableStatus.Occupied|| table.Status != TableStatus.Available)
            throw new Exception("Bu masa hal-hazırda məşğuldur (aktiv oyun var). ");

        table.Status = status;
        await _db.SaveChangesAsync();
    }

    private static TableResponse ToResponse(Table x) =>
        new(x.Id, x.Name, x.Type, x.Status, x.PricePerHour, x.IsActive);

    public async Task DeleteAsync(Guid id)
    {
        var table = await _db.Tables.FindAsync(id)
            ?? throw new Exception("Masa tapılmadı");

        if (table.Status == TableStatus.Occupied)
            throw new Exception("Bu masa hal-hazırda məşğuldur (aktiv oyun var) və silinə bilməz.");

        bool hasActiveSession = await _db.Sessions
            .AnyAsync(s => s.TableId == id && s.Status == SessionStatus.Open);

        if (hasActiveSession)
            throw new Exception("Masanın aktiv hesabat sessiyası mövcuddur. Öncə sessiyanı bağlayın.");

        _db.Tables.Remove(table);
        await _db.SaveChangesAsync();
    }


}