using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerceApp.Core.Entities;

namespace ECommerceApp.Core.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<Product?> GetProductWithCategoryAsync(int id);
        Task<Product?> GetProductWithReviewsAsync(int id);
    }
} 