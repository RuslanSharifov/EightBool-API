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
    {
        try
        {
            return Ok(await _orderService.GetBySessionAsync(sessionId));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "HallAdmin,SuperAdmin")]
    public async Task<IActionResult> Add(OrderRequest request)
    {
        try
        {
            return Ok(await _orderService.AddAsync(request));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("id")]
    [Authorize(Roles = "HallAdmin,SuperAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _orderService.DeleteAsync(id);
            return Ok(new { message = "Sifariş silindi." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}