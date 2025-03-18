using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerceApp.Core.DTOs;

namespace ECommerceApp.Core.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
        Task<ProductDto> CreateProductAsync(ProductDto productDto);
        Task UpdateProductAsync(ProductDto productDto);
        Task DeleteProductAsync(int id);
        Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync(int count);
    }
} 