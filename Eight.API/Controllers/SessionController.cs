using Eight.Application.Interfaces;
using Eight.Application.DTOs.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eight.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SessionController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public SessionController(ISessionService sessionService) => _sessionService = sessionService;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(await _sessionService.GetByIdAsync(id));

    [HttpGet("venue/{venueId}/active")]
    public async Task<IActionResult> GetActive(Guid venueId)
        => Ok(await _sessionService.GetActiveByVenueAsync(venueId));

    [HttpPost("open")]
    [Authorize(Roles = "HallAdmin,SuperAdmin")]
    public async Task<IActionResult> Open(SessionRequest request)
        => Ok(await _sessionService.OpenAsync(request));

    [HttpPatch("{id}/close")]
    [Authorize(Roles = "HallAdmin,SuperAdmin")]
    public async Task<IActionResult> Close(Guid id)
        => Ok(await _sessionService.CloseAsync(id));
}