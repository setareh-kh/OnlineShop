using AutoMapper;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
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
            // دریافت محصولات همراه Category و Pagination
            var res = await productRepository.Filter(filterDto);

            var products = res.Items ?? new List<Product>();

            // دریافت شناسه محصولات همین صفحه
            var productIds = products.Select(p => p.Id).ToList();
            // دریافت تصویر با الویت صفر محصولات همین صفحه
            var images = await imageService.GetMainProductImagesAsync(productIds);
            // تبدیل محصولات به Response
            var result = mapper.Map<List<ProductResponse>>(products);
            // ااتصال تصاویر به محصولات
            foreach (var product in result)
            {
                var image = images.FirstOrDefault(x => x.ItemId == product.Id);

                if (image != null)
                    product.Images = new List<ImageResponse> { mapper.Map<ImageResponse>(image) };
            }

            // بازگرداندن نتیجه همراه اطلاعات Pagination
            return new PaginateResponse<ProductResponse>()
            {
                Page = res.Page,
                Pager = res.Pager,
                Total = res.Total,
                Pages = res.Pages,
                Items = result
            };
        }
        public async Task<PaginateResponse<ProductResponse>> Filter2(ProductFilterRequest filterDto)
        {
            // دریافت محصولات همراه Category و Pagination
            var res = await productRepository.Filter(filterDto);

            var products = res.Items ?? new List<Product>();

            // دریافت شناسه محصولات همین صفحه
            var productIds = products.Select(p => p.Id).ToList();

            // دریافت تصاویر محصولات همین صفحه
            var images = await imageService.WhereAsync(x =>
                x.ItemType == "Product" &&
                productIds.Contains(x.ItemId));

            // گروه بندی تصاویر بر اساس شناسه محصول
            var imagesLookup = images.ToLookup(x => x.ItemId);

            // تبدیل محصولات به Response
            var result = mapper.Map<List<ProductResponse>>(products);

            // اضافه کردن تصاویر به هر محصول
            foreach (var product in result)
            {
                product.Images = mapper.Map<List<ImageResponse>>(
                    imagesLookup[product.Id].OrderBy(x => x.Priority));
            }

            // بازگرداندن نتیجه همراه اطلاعات Pagination
            return new PaginateResponse<ProductResponse>()
            {
                Page = res.Page,
                Pager = res.Pager,
                Total = res.Total,
                Pages = res.Pages,
                Items = result
            };
        }

        /*public async Task<PaginateResponse<ProductResponse>> Filter(ProductFilterRequest filterDto)
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
        }*/
        public async Task<List<ProductResponse>> GetAllWithImages()
        {
            // دریافت محصولات از BaseService
            var products = await base.GetAll();

            if (products == null)
                return new List<ProductResponse>();

            var productList = products.ToList();

            if (productList.Count == 0)
                return new List<ProductResponse>();

            // دریافت شناسه تمام محصولات
            var productIds = productList.Select(x => x.Id).ToList();

            // دریافت تصاویر همه محصولات با یک Query
            var images = await imageService.WhereAsync(x =>
                x.ItemType == "Product" &&
                productIds.Contains(x.ItemId));

            // تبدیل محصولات به Response
            var result = mapper.Map<List<ProductResponse>>(productList);

            // گروه‌بندی تصاویر بر اساس ItemId
            var imagesLookup = images.ToLookup(x => x.ItemId);

            foreach (var product in result)
            {
                product.Images = mapper.Map<List<ImageResponse>>(
                    imagesLookup[product.Id]
                        .OrderBy(x => x.Priority)
                );
            }

            return result;
        }
        public async Task<List<ProductResponse>> GetAllWithDetailsAsync()
        {
            var products = await productRepository.GetAllWithCategoryAsync();

            if (products.Count == 0)
                return new List<ProductResponse>();
            var productIds = products.Select(p => p.Id).ToList();
            var images = await imageService.GetMainProductImagesAsync(productIds);
            var result = mapper.Map<List<ProductResponse>>(products);

            foreach (var product in result)
            {
                var image = images.FirstOrDefault(x => x.ItemId == product.Id);

                if (image != null)
                    product.Images = new List<ImageResponse> { mapper.Map<ImageResponse>(image) };
            }

            return result;
        }
        public async Task<ProductResponse?> GetWithDetailsAsync(int id)
        {
            // دریافت محصول به همراه دسته‌بندی
            var product = await productRepository.GetWithCategoryAsync(id);

            if (product == null)
                return null;

            // دریافت تصاویر محصول
            var images = await imageService.WhereAsync(x =>
                x.ItemType == "Product" &&
                x.ItemId == id);

            // تبدیل Product به ProductResponse
            var result = mapper.Map<ProductResponse>(product);

            // تبدیل تصاویر و مرتب‌سازی بر اساس Priority
            result.Images = mapper.Map<List<ImageResponse>>(
                images.OrderBy(x => x.Priority));

            return result;
        }


    }
}