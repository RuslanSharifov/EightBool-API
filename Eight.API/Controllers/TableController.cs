using Eight.Application.DTOs.Table;
using Eight.Application.Interfaces;
using Eight.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eight.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TableController : ControllerBase
{
    private readonly ITableService _tableService;

    public TableController(ITableService tableService) => _tableService = tableService;

    [HttpGet("venue/{venueId}")]
    public async Task<IActionResult> GetByVenue(Guid venueId)
        => Ok(await _tableService.GetByVenueAsync(venueId));

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Create(TableRequest request)
        => Ok(await _tableService.CreateAsync(request));

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Update(Guid id, TableRequest request)
        => Ok(await _tableService.UpdateAsync(id, request));

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "SuperAdmin,Admin,HallAdmin")]
    public async Task<IActionResult> SetStatus(Guid id, [FromQuery] TableStatus status)
    {
        await _tableService.SetStatusAsync(id, status);
        return NoContent();
    }

    [HttpDelete("id")]
    [Authorize(Roles = "SuperAdmin,Admin,HallAdmin")]
    public async Task<IActionResult> DeleteTableAsync(Guid id)
    {
        try
        {
            await _tableService.DeleteAsync(id);
            return Ok(new { message = "Masa ugurla slinid." });
        }catch (Exception ex) 
        {
            return BadRequest(new {message = ex});
        }
    }
}