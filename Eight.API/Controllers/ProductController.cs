using Eight.Application.DTOs.Product;
using Eight.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eight.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService) => _productService = productService;

    [HttpGet("venue/{venueId}")]
    public async Task<IActionResult> GetByVenue(Guid venueId)
        => Ok(await _productService.GetByVenueAsync(venueId));

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Create(ProductRequest request)
        => Ok(await _productService.CreateAsync(request));

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Update(Guid id, ProductRequest request)
        => Ok(await _productService.UpdateAsync(id, request));

    [HttpPatch("{id}/active")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> SetActive(Guid id, [FromQuery] bool isActive)
    {
        await _productService.SetActiveAsync(id, isActive);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }
}