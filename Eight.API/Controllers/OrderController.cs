using Eight.Application.DTOs.Order;
using Eight.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eight.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService) => _orderService = orderService;

    [HttpGet("session/{sessionId}")]
    public async Task<IActionResult> GetBySession(Guid sessionId)
        => Ok(await _orderService.GetBySessionAsync(sessionId));

    [HttpPost]
    [Authorize(Roles = "HallAdmin,SuperAdmin")]
    public async Task<IActionResult> Add(OrderRequest request)
        => Ok(await _orderService.AddAsync(request));

    [HttpDelete("{id}")]
    [Authorize(Roles = "HallAdmin,SuperAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _orderService.DeleteAsync(id);
        return NoContent();
    }
}