using Microsoft.AspNetCore.Mvc;
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;
using OnlineShop.Services;
using OnlineShop.Setting;

namespace OnlineShop.Controllers.Admin;
//[Authorize(Type = new int[] { 0 })]
[ApiController]
[Route(ApiRoutes.Admin.Category)]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{

    [HttpGet("All")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await categoryService.GetAlldAsync());

    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        return Ok(await categoryService.GetByIdAsync(id));
    }

    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromBody] AddCategoryRequest addCategory)
    {
        var result = await categoryService.AddAsync(addCategory);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest dto)
    {
        var result = await categoryService.UpdateAsync(id, dto);
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await categoryService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}