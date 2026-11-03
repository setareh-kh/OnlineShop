using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Services;
using OnlineShop.Setting;

namespace OnlineShop.Controllers;

[ApiController]
[Route(ApiRoutes.Website.Category)]
public class CategoryController(ICategoryService categoryService, IMapper mapper) : ControllerBase
{

    [HttpGet("All")]
    public async Task<IActionResult> GetAll()
    {
        var res = await categoryService.GetAlldAsync();
        if (!res.Success) return NotFound();
        res.Object = mapper.Map<List<CategoryResponse>>(res.Object);
        return Ok(res);

    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var res = await categoryService.GetByIdAsync(id);
        if (!res.Success) return NotFound();
        res.Object = mapper.Map<CategoryResponse>(res.Object);
        return Ok(res);
    }

    [HttpPost("Search")]
    public async Task<IActionResult> Search([FromQuery] BaseFilterRequest filterRequest)
    {
        var res = await categoryService.Filter(filterRequest);
        List<CategoryResponse> resLst = mapper.Map<List<CategoryResponse>>(res.Items);
        return Ok(new
        {
            Page = res.Page,
            Pager = res.Pager,
            Pages = res.Pages,
            Total = res.Total,
            Items = resLst

        });
    }

}