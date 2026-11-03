using AutoMapper;
using OnlineShop.Models;
using OnlineShop.Repositories;
namespace OnlineShop.Services.Services;

public class ImageService(IImageRepository imageRepository) :BaseService<Image>(imageRepository), IImageService
{
      public async Task<Image?> SaveAsync(IFormFile file, string itemType, int itemId, int priority = 0)
    {
        if (file == null || file.Length == 0)
            return null;

        var image = await imageRepository.SaveAsync(file, itemType, itemId, priority);
        if (image == null)
            return null;
        return image;
    }

    public async Task<Image?> GetAsync(string itemType, int itemId)
    {
        var image = await imageRepository.GetAsync(itemType, itemId);
        if (image == null)
            return null;
        return image;
    }

    public async Task<List<Image>> GetImagesAsync(string itemType, int itemId)
    {
         return await imageRepository.GetImagesAsync(itemType, itemId);
         
    }

    public async Task<bool> DeleteAsync(string itemType,int itemId)
    {
        return await imageRepository.DeleteAsync(itemType,itemId);
    }
}