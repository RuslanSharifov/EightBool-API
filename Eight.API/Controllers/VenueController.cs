using Eight.Application.DTOs.Venue;
using Eight.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eight.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VenueController : ControllerBase
{
    private readonly IVenueService _venueService;

    public VenueController(IVenueService venueService)
    {
        _venueService = venueService;
    }

    [HttpGet]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetAll()
        => Ok(await _venueService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(await _venueService.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Create(VenueRequest request)
        => Ok(await _venueService.CreateAsync(request));

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Update(Guid id, VenueRequest request)
        => Ok(await _venueService.UpdateAsync(id, request));

    [HttpPatch("{id}/active")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> SetActive(Guid id, [FromQuery] bool isActive)
    {
        await _venueService.SetActiveAsync(id, isActive);
        return NoContent();
    }
}