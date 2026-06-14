using System.Globalization;

namespace Eight.Application.DTOs.Auth;

public record LoginResponse
(
    string Token,
    string Role,
    string Name
);
