using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DeviceDetectorNET;
using DeviceDetectorNET.Cache;
using Microsoft.IdentityModel.Tokens;
using OnlineShop.Dtos.Objects;
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Extentions;
using OnlineShop.Models;
using OnlineShop.Repositories;

namespace OnlineShop.Services.Services;

public class AuthService(
    AppSettings appSettings,
    IUserRepository userRepository,
    ITokenRepository tokenRepository,
    IHttpContextAccessor httpContextAccessor
) : IAuthService
{
    private const int LimitTokenDays = 15;
    private string GenerateCode()
    {
        return RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
    }
    public async Task<StandardResponse> RegisterMobileAsync(MobileRequest mobileRequest, UserType userType)
    {
        var exUser = await userRepository.FindAsync(x => x.Mobile == mobileRequest.Mobile);
        if (exUser != null)
            return new StandardResponse
            {
                Success = false,
                Message = "MobileNumber is Exsits.",
                Object = false
            };
        var now = DateTime.UtcNow;
        var newUser = new User
        {
            Mobile = mobileRequest.Mobile,
            Type = userType,
            VerifyCode = GenerateCode(),
            ExpireCode = now.AddMinutes(10),
            UpdatedCode = now,
            CreateAt = now,
        };
        var userId = await userRepository.InsertAsync(newUser);
        /*BackgroundJob.Enqueue("critical", 
            () => MobileService.sendVerificationMobileAsync(newUser.Mobile, newUser.VerifyCode));
        BackgroundJob.Schedule(
            "critical",
            () => MobileService.sendVerificationMobileAsync(newUser.Mobile, newUser.VerifyCode),
            TimeSpan.FromDays(2)
        );*/
        return new StandardResponse
        {
            Success = true,
            Message = "user is created",
            Object = new
            {
                UserId = userId,
                Mobile = newUser.Mobile,
                VerifyCode = newUser.VerifyCode
            }

        };

    }
    public async Task<StandardResponse> Login(MobileRequest mobileRequest, UserType userType = UserType.User)
    {
        var user = await userRepository.FindAsync(x => x.Mobile == mobileRequest.Mobile);
        if (user == null)
            return await RegisterMobileAsync(mobileRequest, userType);
        if (!user!.IsVerified)
            return await CodeVerificatinAsync(new VerifyCodeRequest { UserId = user.Id, VerifyCode = "" });
        return new StandardResponse
        {
            Success = true,
            Message = "Login is successful.",
            Object = new { UserId = user.Id }
        };
    }
    public async Task<StandardResponse> CodeVerificatinAsync(VerifyCodeRequest codeRequest)
    {
        var user = await userRepository.GetByIdAsync(codeRequest.UserId);
        if (user == null)
            return new StandardResponse
            {
                Success = false,
                Message = "user not Found.",
                Object = false
            };
        if (user.ExpireCode <= DateTime.UtcNow)
            return new StandardResponse
            {
                Success = false,
                Message = "verifycode Expiered.",
                Object = false
            };

        if (user.VerifyCode != codeRequest.VerifyCode)
            return new StandardResponse
            {
                Success = false,
                Message = "Invalid verifycode",
                Object = false
            };

        var token = await GenerateJwtToken(user);
        user.IsVerified = true;
        user.Active = true;
        await userRepository.UpdateByIdAsync(user.Id, user);
        return new StandardResponse
        {
            Success = true,
            Message = "",
            Object = new
            {
                Id = user.Id,
                Mobile = user.Mobile,
                Token = token
            }
        };
    }
    private async Task<string> GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(appSettings.Secret!);
        var expire = DateTime.Now.AddDays(LimitTokenDays);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim("id", user.Id.ToString()),
                new Claim("mobile", user.Mobile ?? ""),
                new Claim("type", user.Type.ToString() ?? "")
            ]),
            Expires = expire,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        string userAgent = (httpContextAccessor.HttpContext?.Request.Headers.UserAgent ?? "")!;
        var dd = new DeviceDetector(userAgent);
        dd.SetCache(new DictionaryCache());
        dd.Parse();
        var os = dd.GetOs()?.Match;
        var clientMatch = dd.GetBrowserClient()?.Match;

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);
        var t = new Token
        {
            UserId = user.Id,
            Hash = jwt,
            CreatedAt = DateTime.Now,
            IpAddress = httpContextAccessor.HttpContext?.GetRealClientIpAddress(),
            ExpiredAt = expire,
            Os = $"{os?.Name ?? "os"} {os?.Version ?? ""}",
            Browser = clientMatch?.Name ?? "browser",
            Active = true
        };

        await tokenRepository.InsertAsync(t);
        await tokenRepository.SaveChangesAsync();
        return jwt;
    }
}