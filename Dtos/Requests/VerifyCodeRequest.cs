using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Dtos.Requests
{
    public class VerifyCodeRequest
    {
        public int UserId { get; set; }
        [MaxLength(250)] 
        public string VerifyCode { get; set; } = null!;
    }
}