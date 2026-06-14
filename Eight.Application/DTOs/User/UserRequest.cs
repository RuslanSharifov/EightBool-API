using Eight.Domain.Entities;
using Eight.Domain.Enums;

namespace Eight.Application.DTOs.User;

public record UserRequest(string Name, string Email, string Password, UserRole Role, Guid? VenueId);