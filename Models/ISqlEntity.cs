using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models;

public interface ISqlEntity
{
    [Key] public int Id { get; set; }
}