using AutoMapper;
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

    //var response = mapper.Map<List<ProductResponse>>(products);

    await redisService.SetValueAsync("all_products_Website",products,TimeSpan.FromMinutes(10));

    return Ok(products);
}

    [HttpGet("Filter")]
    public async Task<IActionResult> Filter([FromQuery] ProductFilterRequest productFilter)
    {
        var result = await productService.Filter(productFilter);
        return Ok(result);
    }


}