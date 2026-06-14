using Eight.Application.DTOs.User;
using Eight.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eight.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService) => _userService = userService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _userService.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create(UserRequest request)
        => Ok(await _userService.CreateAsync(request));

    [HttpPatch("{id}/active")]
    public async Task<IActionResult> SetActive(Guid id, [FromQuery] bool isActive)
    {
        await _userService.SetActiveAsync(id, isActive);
        return NoContent();
    }
}