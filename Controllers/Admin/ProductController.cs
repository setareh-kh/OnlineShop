using Microsoft.AspNetCore.Mvc;
using OnlineShop.Dtos.Requests;
using OnlineShop.Models;
using OnlineShop.Services;
using OnlineShop.Setting;

namespace OnlineShop.Controllers.Admin;

[ApiController]
[Route(ApiRoutes.Admin.Product)]
public class ProductController(IProductService productService
            , IRedisService redisService) : ControllerBase
{
    [HttpGet("All")]
    public async Task<IActionResult> GetAll()
    {
        if (redisService.KeyExists($"all_products"))
            return Ok(await redisService.GetValueAsync<List<Product>>($"all_products"));
        var products = (await productService.GetAll())!.ToList();
        await redisService.SetValueAsync($"all_products", products!, TimeSpan.FromMinutes(10));
        return Ok(products);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (redisService.KeyExists($"product_{id}"))
            return Ok(await redisService.GetValueAsync<Product>($"product_{id}"));
        var product = await productService.GetAsync(id);
        if (product == null)
            return NotFound();
        await redisService.SetValueAsync($"product_{id}", product, TimeSpan.FromMinutes(10));
        return Ok(product);
    }
    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromForm] AddProductRequest addProduct)
    {
        var product = await productService.AddAsync(addProduct);
        if (product == null)
            return BadRequest("product's category does not exists.");
        await redisService.SetValueAsync($"product_{product!.Id}", product, TimeSpan.FromMinutes(10));
        redisService.DeleteKey($"all_products");
        return Ok(product);
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await productService.DeleteAsync(id);
        if (!result)
            return NotFound();
        if (redisService.KeyExists($"product_{id}"))
            redisService.DeleteKey($"product_{id}");
        redisService.DeleteKey($"all_products");
        return Ok("The remove was successful.");
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromForm] UpdateProductRequest updateProduct, int id)
    {
        if (redisService.KeyExists($"product_{id}"))
            redisService.DeleteKey($"product_{id}");
        var result = await productService.UpdateAsync(id, updateProduct);
        if (!result)
            return NotFound();
        var product = await productService.FindAsync(p => p.Id == id);
        await redisService.SetValueAsync($"product_{id}", product!, TimeSpan.FromMinutes(10));
        redisService.DeleteKey($"all_products");
        return Ok("The update was successful.");
    }
    [HttpGet("Filter")]
    public async Task<IActionResult> Filter([FromQuery] ProductFilterRequest productFilter)
    {
        var result = await productService.Filter(productFilter);
            return Ok(result);
    }

    
}