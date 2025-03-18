using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerceApp.Core.DTOs;

namespace ECommerceApp.Core.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto);
        Task UpdateCategoryAsync(CategoryDto categoryDto);
        Task DeleteCategoryAsync(int id);
    }
} 