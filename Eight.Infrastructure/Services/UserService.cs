using Eight.Application.DTOs.User;
using Eight.Application.DTOs.Venue;
using Eight.Application.Interfaces;
using Eight.Domain.Entities;
using Eight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eight.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db) => _db = db;


    public async Task<List<UserResponse>> GetAllAsync()
        => await _db.Users
            .Where(x => x.Role != UserRole.SuperAdmin)
            .Select(x => ToResponse(x))
            .ToListAsync();
    public async Task<UserResponse> GetByIdAsync(Guid id)
    {
        var user = await _db.Users.FindAsync(id)
            ?? throw new Exception("İstifadəçi tapılmadı.");

        var UserFront = new User()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            Role = user.Role
        };
        return ToResponse(UserFront);
    }

    public async Task<UserResponse> CreateAsync(UserRequest request)
    {
        if (await _db.Users.AnyAsync(x => x.Email == request.Email))
            throw new Exception("Bu email artıq mövcuddur.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return ToResponse(user);
    }
    public async Task SetActiveAsync(Guid id, bool isActive)
    {
        var user = await _db.Users.FindAsync(id)
            ?? throw new Exception("İstifadəçi tapılmadı.");
        user.IsActive = isActive;
        await _db.SaveChangesAsync();
    }

    private static UserResponse ToResponse(User x) =>
        new(x.Id, x.VenueId!, x.Name, x.Email, x.Role, x.IsActive);
    private static VenueResponse ToResponse(Venue x) =>
    new(
        x.Id,
        x.Name,
        x.AdminId,
        x.Address,
        x.OpenTime,
        x.CloseTime,
        x.IsActive,
        x.ServiceChargeEnabled,
        x.ServiceChargePercent
    );

    public async Task UpdateRol(Guid id, int rol)
    {
        var user = await _db.Users.FindAsync(id)
            ?? throw new Exception("İstifadəçi tapılmadı.");
        if (!Enum.IsDefined(typeof(UserRole), rol))
        {
            throw new Exception("Rol tipi xetası.");
        }
        user.Role = (UserRole)rol;
        await _db.SaveChangesAsync();
    }

    public async Task UpdateRoleAsync(Guid id, int role)
    {
        var user = await _db.Users.FindAsync(id)
            ?? throw new Exception("İstifadəçi tapılmadı.");
        if (!Enum.IsDefined(typeof(UserRole), role))
            throw new Exception("Rol tipi xətası.");
        user.Role = (UserRole)role;
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, UserUpdateRequest request)
    {
        var user = await _db.Users.FindAsync(id)
            ?? throw new Exception("İstifadəçi tapılmadı.");

        if (user.Email != request.Email)
        {
            if (await _db.Users.AnyAsync(x => x.Id != id && x.Email == request.Email))
                throw new Exception("Bu email artıq mövcuddur.");

            user.Email = request.Email;
        }

        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Trim().Length < 3)
            throw new Exception("Ad minimum 3 simvol olmalıdır.");

        if (!Enum.IsDefined(typeof(UserRole), request.Role))
            throw new Exception("Yanlış rol seçilib.");

        if (user.Role != request.Role)
            user.Role = request.Role;

        if (!string.IsNullOrWhiteSpace(request.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        user.Name = request.Name.Trim();

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _db.Users.FindAsync(id)
            ?? throw new Exception("İstifadəçi tapılmadı. ");
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
    }

    public async Task<VenueResponse?> GetUserVenue(Guid id)
    {
        var user = await _db.Users.FindAsync(id)
            ?? throw new Exception("İstifadəçi tapılmadı.");

        if (user.VenueId == null)
            return null;

        var venue = await _db.Venues.FindAsync(user.VenueId)
            ?? throw new Exception("Məkan tapılmadı.");

        return ToResponse(venue);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _db.Users.FindAsync(id);

        if (user == null)
            throw new Exception("İstifadəçi tapılmadı");

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
    }

}