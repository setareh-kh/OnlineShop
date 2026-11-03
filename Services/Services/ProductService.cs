using AutoMapper;
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;
using OnlineShop.Repositories;

namespace OnlineShop.Services.Services
{
    public class ProductService(IProductRepository productRepository
    , IMapper mapper, ICategoryRepository categoryRepository
    , IImageService imageService) : BaseService<Product>(productRepository), IProductService
    {
        public async Task<Product?> AddAsync(AddProductRequest dto)
        {

            var category = await categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return null;
            var product = mapper.Map<Product>(dto);
            product.CreateAt = DateTime.UtcNow;
            product.CreateAt = DateTime.UtcNow;
            product.Available = product.Quantity > 0;
            int productId = await productRepository.InsertAsync(product);
            if (dto.Images != null && dto.Images.Count > 0)
            {
                int priority = 0;
                foreach (var image in dto.Images)
                {
                    await imageService.SaveAsync(image, "Product", productId, priority);
                    priority++;
                }
            }
            //  تبدیل Entity به Response DTO
            //return mapper.Map<AdminProductResponseDto>(product);
            return product;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var product = await productRepository.GetByIdAsync(id);
            if (product == null)
                return false;
            productRepository.Delete(product);
            await productRepository.SaveChangesAsync();
            await imageService.DeleteAsync("Product", id);
            return true;
        }
        public async Task<bool> UpdateAsync(int id, UpdateProductRequest updateProduct)
        {

            var product = await productRepository.GetByIdAsync(id);
            if (product == null)
                return false;
            var category = await categoryRepository.GetByIdAsync(updateProduct.CategoryId);
            if (category == null)
                return false;
            mapper.Map(updateProduct, product);
            product.Available = product.Quantity > 0;
            product.UpdateDate = DateTime.UtcNow;
            var result = await productRepository.UpdateByIdAsync(id, product);
            if (!result)
                return false;
            if (updateProduct.Images == null || updateProduct.Images.Count == 0)
                return true;
            var images = await imageService.WhereAsync(x => x.ItemType == "Product" && x.ItemId == id);
            var maxPriority = images.Max(x => x.Priority);
            var priority = maxPriority + 1;
            // if add==false ok remove all of images of product
            if (!updateProduct.AddImage)
            {
                await imageService.DeleteAsync("Product", product.Id);
                priority = 0;
            }
            //  اضافه کردن تصاویر جدید با مقدار تنظیم شده اولویت در هر دو صورت
            foreach (var image in updateProduct.Images)
            {
                await imageService.SaveAsync(image, "Product", product.Id, priority);
                priority++;
            }
            return true;
        }

        public async Task<PaginateResponse<ProductResponse>> Filter(ProductFilterRequest filterDto)
        {
            var res = await productRepository.Filter(filterDto);
            return new PaginateResponse<ProductResponse>()
            {
                Page = res.Page,
                Pager = res.Pager,
                Total = res.Total,
                Pages = res.Pages,
                Items = res.Items?.Select(mapper.Map<ProductResponse>).ToList()
            };
        }

    }
}