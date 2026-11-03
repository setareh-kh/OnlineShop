
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;

namespace OnlineShop.Services;

public interface IAuthService
{
  Task<StandardResponse> RegisterMobileAsync(MobileRequest mobileRequest, UserType userType);
  Task<StandardResponse> Login(MobileRequest mobileRequest, UserType userType = UserType.User);
  Task<StandardResponse> CodeVerificatinAsync(VerifyCodeRequest codeRequest);
  


}