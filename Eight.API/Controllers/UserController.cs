using Eight.Application.DTOs.User;
using Eight.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace Eight.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin")]
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
    public async Task<IActionResult> GetAll()
        => Ok(await _userService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(await _userService.GetByIdAsync(id));

    [HttpPut("{id}/role")]
    public async Task<IActionResult> ChangeRole(Guid id, [FromBody] int newRole)
    {
        await _userService.UpdateRol(id, newRole);
        return Ok(new { message = "Rol uğurla yeniləndi." });
    }

    [HttpPost]
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

    [HttpPatch("{id}/active")]
    public async Task<IActionResult> SetActive(Guid id, [FromQuery] bool isActive)
    {
        await _userService.SetActiveAsync(id, isActive);
        return NoContent();
    }
}