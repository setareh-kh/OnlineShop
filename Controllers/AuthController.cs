using Microsoft.AspNetCore.Mvc;
using OnlineShop.Dtos.Requests;
using OnlineShop.Services;
using OnlineShop.Setting;
using OnlineShop.Validators;

namespace OnlineShop.Controllers;
[ApiController]
[Route(ApiRoutes.Website.Auth)]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] MobileRequest mobileRequest)
    {
        return Ok(await authService.Login(mobileRequest));
    }
    [HttpPost("Register")]
    public async Task<IActionResult> RegisterAsync([FromBody] MobileRequest mobileRequest)
    {
        return Ok(await authService.RegisterMobileAsync(mobileRequest,Models.UserType.User));
    }
    
    [HttpPost("Verify")]
    public async Task<IActionResult> VerifyAsync([FromBody] VerifyCodeRequest codeRequest)
    {
        return Ok(await authService.CodeVerificatinAsync(codeRequest));

    }

}