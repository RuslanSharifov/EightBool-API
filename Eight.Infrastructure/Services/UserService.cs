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
        new(x.Id, x.Name, x.Email, x.Role, x.IsActive);

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
}