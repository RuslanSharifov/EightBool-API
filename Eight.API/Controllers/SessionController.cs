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
    {
        try
        {
            var result = await _sessionService.GetActiveByVenueAsync(venueId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Xəta baş verdi: " + ex.Message });
        }
    }

    [HttpGet("table/{tableId}/active")]
    public async Task<IActionResult> GetActiveByTable(Guid tableId)
    {
        try
        {
            var result = await _sessionService.GetActiveByTableAsync(tableId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("venue/{venueId}/history")]
    public async Task<IActionResult> GetHistory(Guid venueId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        try
        {
            var result = await _sessionService.GetHistoryByVenueAsync(venueId, from, to);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("open")]
    [Authorize(Roles = "HallAdmin,SuperAdmin")]
    public async Task<IActionResult> Open(SessionRequest request)
    {
        try
        {
            return Ok(await _sessionService.OpenAsync(request));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/close")]
    [Authorize(Roles = "HallAdmin,SuperAdmin,Admin")]
    public async Task<IActionResult> Close(Guid id)
    {
        try
        {
            return Ok(await _sessionService.CloseAsync(id));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/customer-count")]
    [Authorize(Roles = "HallAdmin,SuperAdmin")]
    public async Task<IActionResult> UpdateCustomerCount(Guid id, [FromQuery] int count)
    {
        try
        {
            var result = await _sessionService.UpdateCustomerCountAsync(id, count);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}