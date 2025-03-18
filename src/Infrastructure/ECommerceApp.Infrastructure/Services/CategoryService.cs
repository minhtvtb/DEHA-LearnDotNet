using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ECommerceApp.Core.DTOs;
using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Interfaces;

namespace ECommerceApp.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(
            IRepository<Category> categoryRepository,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var categoryDtos = categories.Select(c => 
            {
                var dto = _mapper.Map<CategoryDto>(c);
                dto.ProductCount = c.Products.Count;
                return dto;
            });
            
            return categoryDtos;
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;
            
            var categoryDto = _mapper.Map<CategoryDto>(category);
            categoryDto.ProductCount = category.Products.Count;
            return categoryDto;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            var result = await _categoryRepository.AddAsync(category);
            return _mapper.Map<CategoryDto>(result);
        }

        public async Task UpdateCategoryAsync(CategoryDto categoryDto)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryDto.Id);
            if (category != null)
            {
                _mapper.Map(categoryDto, category);
                await _categoryRepository.UpdateAsync(category);
            }
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category != null)
            {
                await _categoryRepository.DeleteAsync(category);
            }
        }
    }
} 