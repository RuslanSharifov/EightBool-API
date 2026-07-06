using Eight.Application.DTOs.Venue;
using Eight.Application.Interfaces;
using Eight.Domain.Entities;
using Eight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eight.Infrastructure.Services;

public class VenueService : IVenueService
{
    private readonly AppDbContext _db;

    public VenueService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<VenueResponse>> GetAllAsync()
    {
        return await _db.Venues
            .Select(x => ToResponse(x))
            .ToListAsync();
    }

    public async Task<VenueResponse> GetByIdAsync(Guid id)
    {
        var venue = await _db.Venues.FindAsync(id)
            ?? throw new Exception("Venue tapılmadı.");
        return ToResponse(venue);
    }

    public async Task<VenueResponse> CreateAsync(VenueRequest request)
    {
        var venue = new Venue
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Address = request.Address,
            OpenTime = request.OpenTime,
            CloseTime = request.CloseTime,
            ServiceChargeEnabled = request.ServiceChargeEnabled,
            ServiceChargePercent = request.ServiceChargePercent,
            AdminId = request.AdminId
        };

        var user = await _db.Users.FindAsync(venue.AdminId);
        if (user == null)
            throw new Exception("İsdifadəçi tapılmadı");

        user.VenueId = venue.Id;


        _db.Venues.Add(venue);
        await _db.SaveChangesAsync();
        return ToResponse(venue);
    }

    public async Task<VenueResponse> UpdateAsync(Guid id, VenueRequest request)
    {
        var venue = await _db.Venues.FindAsync(id)
            ?? throw new Exception("Venue tapılmadı.");

        venue.Name = request.Name;
        venue.Address = request.Address;
        venue.OpenTime = request.OpenTime;
        venue.CloseTime = request.CloseTime;
        venue.ServiceChargeEnabled = request.ServiceChargeEnabled;
        venue.ServiceChargePercent = request.ServiceChargePercent;

        await _db.SaveChangesAsync();
        return ToResponse(venue);
    }

    public async Task SetActiveAsync(Guid id, bool isActive)
    {
        var venue = await _db.Venues.FindAsync(id)
            ?? throw new Exception("Venue tapılmadı.");
        venue.IsActive = isActive;
        await _db.SaveChangesAsync();
    }

    private static VenueResponse ToResponse(Venue x) => new(
        x.Id, 
        x.Name, 
        x.AdminId,
        x.Address, 
        x.OpenTime, 
        x.CloseTime,
        x.IsActive, 
        x.ServiceChargeEnabled, 
        x.ServiceChargePercent);
}