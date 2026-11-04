using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Services;
using OnlineShop.Setting;
using OnlineShop.Validators;

namespace OnlineShop.Controllers.Profile;


[ApiController]
[Route(ApiRoutes.Profile.MyProfile)]
public class MyProfileController(IUserService userService, IImageService imageService) : ControllerBase
{

    [HttpPut("Edit/{id:int}")]
    public async Task<IActionResult> Edit([FromForm] UpdateUserRequest updateUser, int id)
    {
        var result = await userService.UpdateAsync(id, updateUser);
        if (!result)
            return NotFound();
        object ob = "";
        if (!string.IsNullOrWhiteSpace(updateUser.Name)) ob = updateUser.Name;
        if (updateUser.Image?.Length > 0)
            ob = (await imageService.GetAsync("User", id))!.Url;
        if (!string.IsNullOrWhiteSpace(updateUser.Name) && (updateUser.Image?.Length > 0))
            ob = new
            {
                Url = (await imageService.GetAsync("User", id))!.Url,
                Name = updateUser.Name
            };
        return Ok(new StandardResponse
        {
            Success = true,
            Message = "Update is successful.",
            Object = ob
        });

    }


}
