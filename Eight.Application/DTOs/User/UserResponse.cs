using Eight.Domain.Entities;
using Eight.Domain.Enums;

namespace Eight.Application.DTOs.User;

public record UserResponse(Guid Id, string Name, string Email, UserRole Role, bool IsActive);