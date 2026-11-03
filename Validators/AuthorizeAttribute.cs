using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OnlineShop.Models;

namespace OnlineShop.Validators
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public int[] AllowedTypes { get; set; } = [];
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.Items["user"] as User;
            if (user == null)
            {
                // not login
                context.Result = new JsonResult(new { Success = false, Message = "Unauthorized" })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }

            if (user is { Active: false })
            {
                context.Result = new JsonResult(new { Success = false, Message = "AccessDenied" })
                { StatusCode = StatusCodes.Status403Forbidden };
            }

            if (user == null || AllowedTypes .Length == 0) return;

            if (!AllowedTypes .Contains((int)user.Type!))
            {
                context.Result = new JsonResult(new { Success = false, Message = "AccessDenied" })
                { StatusCode = StatusCodes.Status403Forbidden };
            }

        }
    }
}
