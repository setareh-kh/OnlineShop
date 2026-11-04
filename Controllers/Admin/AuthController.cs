using Microsoft.AspNetCore.Mvc;
using OnlineShop.Dtos.Requests;
using OnlineShop.Models;
using OnlineShop.Services;
using OnlineShop.Setting;
using OnlineShop.Validators;

namespace OnlineShop.Controllers.Admin;


[ApiController]
[Route(ApiRoutes.Admin.Auth)]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] MobileRequest mobileRequest)
    {
        return Ok(await authService.Login(mobileRequest, UserType.Admin));
    }
    [HttpPost("Register")]
    public async Task<IActionResult> RegisterAsync([FromBody] MobileRequest mobileRequest)
    {
        return Ok(await authService.RegisterMobileAsync(mobileRequest, UserType.Admin));

    }
    [Authorize(AllowedTypes = [0])]
    [HttpPost("Verify")]
    public async Task<IActionResult> VerifyAsync([FromBody] VerifyCodeRequest codeRequest)
    {
        return Ok(await authService.CodeVerificatinAsync(codeRequest));

    }

}