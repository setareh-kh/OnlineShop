using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public enum UserType
    {
        Admin = 0,
        User = 1
    }
    public class User : ISqlEntity
    {

        public int Id { get; set; }
        [MaxLength(250)]
        public string? Name { get; set; }
        [Required, MaxLength(250)]
        public required string Mobile { get; set; }
        public required UserType Type { get; set; }
        [MaxLength(250)] public string VerifyCode { get; set; } = null!;
        public DateTime ExpireCode { get; set; }
        public DateTime? UpdatedCode { get; set; }
        public bool? Active { get; set; }
        public bool IsVerified { get; set; } = false;
        [Required]
        public required DateTime CreateAt { get; set; }
        public ICollection<Token> Tokens { get; set; } = new List<Token>();

    }
}
