using Microsoft.AspNetCore.Mvc;
using OnlineShop.Services;
using OnlineShop.Setting;


namespace OnlineShop.Controllers.Admin;

//[Authorize(AllowedTypes = [0])]
[ApiController]
[Route(ApiRoutes.Admin.Cache)]
public class CacheController(IRedisService redisService) : ControllerBase
{
    [HttpDelete("{key}")]
    public async Task<IActionResult> RemoveAsync(string key)
    {
        if (!await redisService.KeyExistsAsync(key)) return Ok("key isn't Exists.");
        return await redisService.DeleteKeyAsync(key) ?
            Ok("The remove was successful.") : BadRequest("The remove wasn't successful.");
    }

    [HttpDelete("Empty")]
    public async Task<IActionResult> EmptyAsync()
    {
        var keys = (await redisService.GetKeysByPrefixAsync()).ToArray();
        await redisService.DeleteKeysAsync(keys);
        return Ok("cache is already empty");
    }
    [HttpGet("AllKeys")]
    public async Task<IActionResult> GetAllKeys()
    {
        return Ok(await redisService.GetKeysByPrefixAsync());

    }
    [HttpGet("KeyValues")]
    public async Task<IActionResult> KeyValues(string pattern)
    {
        return Ok(redisService.KeyValues(pattern));

    }

}