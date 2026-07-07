using Eight.Application.DTOs.Venue;
using Eight.Application.Interfaces;
using Eight.Domain.Entities;
using Eight.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;

namespace Eight.Infrastructure.Services;

public class VenueService : IVenueService
{
    private readonly AppDbContext _db;

    public VenueService(AppDbContext db)
    {
        _db = db;
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

        if (request.AdminId == Guid.Empty)
            throw new Exception("Admin seçilməyib.");

        var admin = await _db.Users.FindAsync(request.AdminId)
            ?? throw new Exception("Admin tapılmadı.");

        if (admin.Role != UserRole.Admin)
            throw new Exception("Seçilən istifadəçi Admin rolunda deyil.");

        if (admin.VenueId != null)
            throw new Exception($"'{admin.Name}' artıq başqa obyektə təyin edilib.");

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

        admin.VenueId = venue.Id;
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

    public async Task DeleteAsync(Guid id)
    {
        var venue = await (_db.Venues.FindAsync(id))
            ?? throw new Exception("Obyekt tapılmaı. ");
        
        if (venue.AdminId != null)
        {
            var admin = await _db.Users.FindAsync(venue.AdminId);
            if (admin != null)
            {
                admin.VenueId = null;
            }
        }   

        _db.Venues.Remove(venue);
        await _db.SaveChangesAsync();
    }

    public async Task SetActiveAsync(Guid id, bool isActive)
    {
        var venue = await _db.Venues.FindAsync(id)
            ?? throw new Exception("Venue tapılmadı.");
        venue.IsActive = isActive;
        await _db.SaveChangesAsync();
    }



    public async Task ChangeAdminAsync(Guid VenueId, Guid NewAdminId)
    {
        var venue = await _db.Venues.FindAsync(VenueId)
            ?? throw new Exception("Obyekt tapılmadı.");

        var oldAdmin = await _db.Users.FindAsync(venue.AdminId);
        var NewAdmin = await _db.Users.FindAsync(NewAdminId)
            ?? throw new Exception("Təyin olunan admin tapılmadı. ");
        if (NewAdmin.Role != UserRole.Admin)
            throw new Exception("Uygnsuz Admin Rolu");


        if (NewAdmin.VenueId != null)
        {
            var oldVenue = await _db.Venues.FindAsync(NewAdmin.VenueId);
            throw new Exception($"@{NewAdmin.Name} Admin olaraq '{oldVenue?.Name}' obyektində fəaliyyət göstərir.");
        }


        var newAdmin = await _db.Users.FindAsync(NewAdminId)
            ?? throw new Exception("Dəyişiklik uğursuz oldu, admin tapılmadı. ");

        if (oldAdmin != null)
            oldAdmin.VenueId = null;
        venue.AdminId = NewAdminId;
        NewAdmin.VenueId = venue.Id;
        await _db.SaveChangesAsync();
    }
}