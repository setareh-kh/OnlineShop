using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Dtos.Requests;
using OnlineShop.Models;
using OnlineShop.Services;
using OnlineShop.Setting;
using OnlineShop.Validators;

namespace OnlineShop.Controllers;

[Authorize(AllowedTypes = [1])]
[ApiController]
[Route(ApiRoutes.Website.Checkout)]
public class CheckoutController(
    ICheckoutService checkoutService) : ControllerBase
{
    [HttpGet("Preview")]
    public async Task<IActionResult> Preview()
    {
        var user = HttpContext.Items["user"] as User;

        if (user == null)
            return Unauthorized();

        var result = await checkoutService.PreviewAsync(user.Id);

        return Ok(result);
    }
    [HttpPost("Create")]
public async Task<IActionResult> Create(
    [FromBody] CheckoutRequest request,
    [FromHeader(Name = "Idempotency-Key"), Required]
    string idempotencyKey)
{
    var user = HttpContext.Items["user"] as User;

    if (user == null)
        return Unauthorized();

    try
    {
        var result = await checkoutService.CreateOrderAsync(
            user.Id,
            request,
            idempotencyKey);

        return Ok(result);
    }
    catch (ArgumentException ex)
    {
        return BadRequest(new { message = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return Conflict(new { message = ex.Message });
    }
}
}