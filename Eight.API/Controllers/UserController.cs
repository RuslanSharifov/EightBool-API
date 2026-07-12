using Microsoft.AspNetCore.Authorization;
using Eight.Application.Interfaces;
using Eight.Application.DTOs.User;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Net.WebSockets;

namespace Eight.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IValidator<UserRequest> _validator;

    public UserController(IUserService userService, IValidator<UserRequest> userValidator)
    {
        _userService = userService;
        _validator = userValidator;
    }

    [HttpGet]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetAll()
        => Ok(await _userService.GetAllAsync());

    [HttpGet("available-admins")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetAvailableAdmins()
        => Ok(await _userService.GetAvailableAdminsAsync());

    [HttpGet("venue/{venueId}/staff")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetStaffByVenue(Guid venueId)
    {
        var staff = await _userService.GetStaffByVenueAsync(venueId);
        return Ok(staff);
    }

    [HttpGet("Profile/{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(await _userService.GetByIdAsync(id));

    [HttpPut("{id}/role")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> ChangeRole(Guid id, [FromBody] int newRole)
    {
        await _userService.UpdateRol(id, newRole);
        return Ok(new { message = "Rol uğurla yeniləndi." });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Create(UserRequest request)
    {
        var validation = await _validator.ValidateAsync(request);

        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        try
        {
            return Ok(await _userService.CreateAsync(request));
        }
        catch (Exception ex)
        {
            return BadRequest(new[] { ex.Message });
        }
    }

    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> DeleteUserById(Guid id)
    {
        await _userService.DeleteAsync(id);
        return Ok(new { message = "İstifadəçi silindi." });
    }

    [HttpPatch("{id}/active")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> SetActive(Guid id, [FromQuery] bool isActive)
    {
        await _userService.SetActiveAsync(id, isActive);
        return NoContent();
    }

    [HttpPatch("{id}/role")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromQuery] int role)
    {
        try
        {
            await _userService.UpdateRoleAsync(id, role);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new[] { ex.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetUserVenue(Guid id)
    {
        try
        {
            var venue = await _userService.GetUserVenue(id);
            return Ok(venue);
        }
        catch (Exception ex)
        {
            return BadRequest(new[] { ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequest request)
    {
        try
        {
            await _userService.UpdateAsync(id, request);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new[] { ex.Message });
        }
    }
}