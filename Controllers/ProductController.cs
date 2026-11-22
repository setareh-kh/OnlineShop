using Microsoft.AspNetCore.Mvc;
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Services;
using OnlineShop.Setting;

namespace OnlineShop.Controllers;

[ApiController]
[Route(ApiRoutes.Website.Product)]
public class ProductController(IProductService productService
            , IRedisService redisService) : ControllerBase
{
    [HttpGet("All")]
    public async Task<IActionResult> GetAll()
    {
        if (redisService.KeyExists("all_products_Website"))
        {
            var cachedProducts =
                await redisService.GetValueAsync<List<ProductResponse>>("all_products_Website");

            if (cachedProducts is not null)
                return Ok(cachedProducts);
        }

        var products = await productService.GetAllWithDetailsAsync();

        await redisService.SetValueAsync("all_products_Website", products, TimeSpan.FromMinutes(10));

        return Ok(products);
    }

    [HttpGet("Filter")]
    public async Task<IActionResult> Filter([FromQuery] ProductFilterRequest productFilter)
    {
        var result = await productService.Filter(productFilter);
        return Ok(result);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (redisService.KeyExists($"product_{id}"))
        {
            var cachedProduct = await redisService.GetValueAsync<ProductResponse>($"product_{id}");
            if (cachedProduct is not null) return Ok(cachedProduct);
        }

        var result = await productService.GetWithDetailsAsync(id);
        if (result == null)
            return NotFound();
        await redisService.SetValueAsync($"product_{id}", result, TimeSpan.FromMinutes(10));
        return Ok(result);
    }

}