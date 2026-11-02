using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;

namespace OnlineShop.Repositories.Repositories
{
    public class ImageRepository (SqlContext context):BaseRepository<Image>(context), IImageRepository
    {
        private async Task<string> StoreToFileAsync(string uniqStart, string lastPath, IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName);
            var randomName = Path.GetRandomFileName();
            var fileName = $"{uniqStart}_{randomName}{ext}";
            var storeDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Assets", lastPath);
            if (!Directory.Exists(storeDirectory))
                Directory.CreateDirectory(storeDirectory);
            var storeFile = Path.Combine(storeDirectory, fileName);
            if (File.Exists(storeFile))
                File.Delete(storeFile);
            using (var fs = File.Create(storeFile))
            {
                await file.CopyToAsync(fs);
            }
            return fileName;
        }
        public async Task<Image> SaveAsync(IFormFile file, string itemType, int itemId, int priority)
        {
            string nameStart = itemId.ToString();
            var name = await StoreToFileAsync(nameStart, $"{itemType}s", file);
            var image = new Image
            {
                Name = name,
                ItemType = itemType,
                ItemId = itemId,
                Url = Path.Combine($"\\Assets" , $"{itemType}s",$"{name}"),
                Priority = priority
            };
            await SqlContext.Images.AddAsync(image);
            await SqlContext.SaveChangesAsync();
            return image;
        }
        public async Task<Image?> GetAsync(string itemType, int itemId)
        {
            return await SqlContext.Images.FirstOrDefaultAsync(image => image.ItemType == itemType && image.ItemId == itemId);
        }
        public async Task<List<Image>> GetImagesAsync(string itemType, int itemId)
        {
            return await SqlContext.Images.Where(x => x.ItemType == itemType && x.ItemId == itemId).ToListAsync();
        }
        public async Task<bool> DeleteAsync(string itemType, int itemid)
        {
            var imgs = await SqlContext.Images.Where(x => x.ItemType == itemType && x.ItemId == itemid).ToListAsync();
            if (!imgs.Any()) return false;
            foreach (var image in imgs)
            {
                var dltFile = $"{Directory.GetCurrentDirectory()}\\Assets\\{itemType}s\\{image.Name}";
                //Console.WriteLine(Directory.GetCurrentDirectory());
                //Console.WriteLine(image.Url);
                //Console.WriteLine(dltFile);
                File.Delete(dltFile);
                SqlContext.Images.Remove(image);
            }
            await SqlContext.SaveChangesAsync();
            return true;
        }
        /*public async Task<bool> UpdateAsync(IFormFile file, string itemType, int itemId, int priority, bool isAdded=true)
        {
            if (isAdded)
            {
                await SaveAsync(file, itemType, itemId);
                return true;
            }
            else
            {
                var result = await DeleteAsync(itemType, itemId);
                if (result == true)
                {
                    await SaveAsync(file, itemType, itemId);
                    return true;
                }
                else return false;      
            }

        }*/
    }
}