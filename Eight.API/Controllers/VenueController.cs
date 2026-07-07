using Eight.Application.DTOs.Venue;
using Eight.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Eight.API.Controllers;

//[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VenueController : ControllerBase
{
    private readonly IVenueService _venueService;
    private readonly IValidator<VenueRequest> _validator;

    public VenueController(IVenueService venueService, IValidator<VenueRequest> validator)
    {
        _venueService = venueService;
        _validator = validator;
    }

    [HttpGet]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetAll()
        => Ok(await _venueService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(await _venueService.GetByIdAsync(id));

    [Authorize(Roles = "SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(VenueRequest request)
    {
        Console.WriteLine(request);

        var validation = await _validator.ValidateAsync(request);

        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
        try
        {
            return Ok(await _venueService.CreateAsync(request));
        }
        catch (Exception ex)
        {
            return BadRequest(new[] { ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> DeleteById(Guid id)
    {
        try
        {
            await _venueService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new[] { ex.Message });
        }
    }


    [HttpPatch("{venueId}/admin")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> ChangeAdmin(Guid venueId, [FromQuery] Guid newAdminId)
    {
        try
        {
            await _venueService.ChangeAdminAsync(venueId, newAdminId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new[] { ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Update(Guid id, VenueRequest request)
    {
        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
        try
        {
            return Ok(await _venueService.UpdateAsync(id, request));
        }
        catch (Exception ex)
        {
            return BadRequest(new[] { ex.Message });
        }
    }

    [HttpPatch("{id}/active")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> SetActive(Guid id, [FromQuery] bool isActive)
    {
        await _venueService.SetActiveAsync(id, isActive);
        return NoContent();
    }
}