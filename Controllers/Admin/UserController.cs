using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Services;
using OnlineShop.Setting;
using OnlineShop.Validators;

namespace OnlineShop.Controllers.Admin;

//[Authorize(AllowedTypes = [0])]
[ApiController]
[Route(ApiRoutes.Admin.User)]
public class UserController(IUserService userService, IMapper mapper) : ControllerBase
{

    [HttpGet("Search")]
    public async Task<IActionResult> Filter([FromQuery] UserFilterRequest userFilter)
    {
        var res = await userService.Filter(userFilter);
        List<AdminUserResponse> resLst = mapper.Map<List<AdminUserResponse>>(res.Items);
        return Ok(new
        {
            Page = res.Page,
            Pager = res.Pager,
            Pages = res.Pages,
            Total = res.Total,
            Items = resLst
        });

    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await userService.GetByIdAsync(id);
        if (!user.Success) return NotFound();
        user.Object = mapper.Map<AdminUserResponse>(user.Object);
        return Ok(user);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> ActiveEdit(int id, [FromQuery] bool Active)
    {
        var result = await userService.ChangeActive(id,Active);

        if (!result)
            return NotFound();
        return Ok("Edit is successful.");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await userService.DeleteAsync(id);

        if (!result)
            return NotFound();

        return Ok("Delete is successful.");
    }


}
