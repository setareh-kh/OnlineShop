using Microsoft.AspNetCore.Mvc;
using OnlineShop.Services;
using OnlineShop.Setting;
using OnlineShop.Validators;

namespace OnlineShop.Controllers.Admin;

[Authorize(AllowedTypes = [0])]
[ApiController]
[Route(ApiRoutes.Admin.Cache)]
public class CacheController(IRedisService redisService) : ControllerBase
{
    [HttpDelete("{key}")]
    public async Task<IActionResult> RemoveAsync(string key)
    {
        if (!await redisService.KeyExistsAsync(key)) return Ok("key isn't Exists.");
        return await redisService.DeleteKeyAsync(key)? 
            Ok("The remove was successful."):BadRequest("The remove wasn't successful.");
    }

    [HttpDelete("Prefix/{prefix}")]
    public async Task<IActionResult> DeleteByPrefixAsync(string prefix)
    {
        var keys = (await redisService.GetKeysByPrefixAsync(prefix)).ToArray();
        await redisService.DeleteKeysAsync(keys);
        return Ok("The remove was successful.");
    }

}