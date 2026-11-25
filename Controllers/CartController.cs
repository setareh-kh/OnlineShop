
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;
using OnlineShop.Services;
using OnlineShop.Setting;
using OnlineShop.Validators;

namespace OnlineShop.Controllers;

[Authorize(AllowedTypes = [1])]
[ApiController]
[Route(ApiRoutes.Website.Cart)]
public class CartController(ICartService cartService) : ControllerBase
{
    [HttpPost("Additems")]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemRequest request)
    {
        // دریافت کاربر جاری از JwtMiddleware
        var user = HttpContext.Items["user"] as User;

        if (user == null)
            return Unauthorized(new
            {
                Success = false,
                Message = "Unauthorized"
            });

        var result = await cartService.AddItemAsync(user.Id, request);

        if (!result)
            return BadRequest(new
            {
                Success = false,
                Message = "Product is unavailable or requested quantity exceeds stock."
            });

        return Ok(new
        {
            Success = true,
            Message = "Product added to cart successfully."
        });
    }
    [HttpGet("Showitems")]
    public async Task<IActionResult> GetCart()
    {
        var user = HttpContext.Items["user"] as User;

        if (user == null) return Unauthorized();

        return Ok(await cartService.GetCartAsync(user.Id));
    }
    [HttpPut("items/{id:int}")]
    public async Task<IActionResult> UpdateItem(int id,[FromBody] UpdateCartItemRequest request)
    {
        var user = HttpContext.Items["user"] as User;

        if (user == null)
            return Unauthorized();

        var result = await cartService.UpdateItemAsync(user.Id, id, request);

        if (!result)
            return BadRequest(new
            {
                Success = false,
                Message = "Unable to update cart item."
            });

        return Ok(new
        {
            Success = true,
            Message = "Cart item updated successfully."
        });
    }

    [HttpDelete("items/{id:int}")]
    public async Task<IActionResult> RemoveItem(int id)
    {
        var user = HttpContext.Items["user"] as User;

        if (user == null)
            return Unauthorized();

        var result = await cartService.RemoveItemAsync(user.Id, id);

        if (!result)
            return BadRequest(new
            {
                Success = false,
                Message = "Unable to remove cart item."
            });

        return Ok(new
        {
            Success = true,
            Message = "Cart item removed successfully."
        });
    }
}
