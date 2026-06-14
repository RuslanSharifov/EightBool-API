using Eight.Application.DTOs.User;
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
            .Select(x => ToResponse(x))
            .ToListAsync();

    public async Task<UserResponse> CreateAsync(UserRequest request)
    {
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
        new(x.Id, x.Name, x.Email, x.Role, x.IsActive);
}