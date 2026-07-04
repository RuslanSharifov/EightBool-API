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

    [HttpPost]
    public async Task<IActionResult> Create(UserRequest request)
    {
        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
        return Ok(await _userService.CreateAsync(request));
    }

    [HttpPatch("{id}/active")]
    public async Task<IActionResult> SetActive(Guid id, [FromQuery] bool isActive)
    {
        await _userService.SetActiveAsync(id, isActive);
        return NoContent();
    }
}