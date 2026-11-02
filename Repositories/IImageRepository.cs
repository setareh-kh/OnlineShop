
using OnlineShop.Models;

namespace OnlineShop.Repositories
{
    public interface IImageRepository : IBaseRepository<Image>
    {
        Task<Image> SaveAsync(IFormFile file, string itemType, int itemId, int priority = 0);
        Task<Image?> GetAsync(string itemType, int itemId);
        Task<List<Image>> GetImagesAsync(string itemType, int itemId);
        Task<bool> DeleteAsync(string itemType, int itemid);
        //Task<bool> UpdateAsync(IFormFile file, string itemType, int itemId, int priority = 0, bool isAdded = true);

    }
}