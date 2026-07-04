using Eight.Domain.Entities;

namespace Eight.Application.DTOs.User;

public record UserRequest(
    string Name, 
    string Email, 
    string Password, 
    UserRole Role
);