using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ECommerceApp.Core.DTOs;
using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Interfaces;
using ECommerceApp.Infrastructure.Services;
using Moq;
using Xunit;

namespace ECommerceApp.UnitTests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<IRepository<Category>> _mockCategoryRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _mockCategoryRepository = new Mock<IRepository<Category>>();
            _mockMapper = new Mock<IMapper>();
            _categoryService = new CategoryService(
                _mockCategoryRepository.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ShouldReturnAllCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1", Description = "Description 1" },
                new Category { Id = 2, Name = "Category 2", Description = "Description 2" }
            };

            var categoryDtos = new List<CategoryDto>
            {
                new CategoryDto { Id = 1, Name = "Category 1", Description = "Description 1" },
                new CategoryDto { Id = 2, Name = "Category 2", Description = "Description 2" }
            };

            _mockCategoryRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(categories);

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<CategoryDto>>(categories))
                .Returns(categoryDtos);

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.First().Id);
            Assert.Equal("Category 1", result.First().Name);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_WithValidId_ShouldReturnCategory()
        {
            // Arrange
            int categoryId = 1;
            var category = new Category { Id = categoryId, Name = "Test Category", Description = "Test Description" };
            var categoryDto = new CategoryDto { Id = categoryId, Name = "Test Category", Description = "Test Description" };

            _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId))
                .ReturnsAsync(category);

            _mockMapper.Setup(mapper => mapper.Map<CategoryDto>(category))
                .Returns(categoryDto);

            // Act
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryId, result.Id);
            Assert.Equal("Test Category", result.Name);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            int categoryId = 999;

            _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId))
                .ReturnsAsync((Category)null);

            // Act
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateCategoryAsync_ShouldCreateAndReturnCategory()
        {
            // Arrange
            var categoryDto = new CategoryDto
            {
                Name = "New Category",
                Description = "New Description"
            };

            var category = new Category
            {
                Name = "New Category",
                Description = "New Description"
            };

            var createdCategory = new Category
            {
                Id = 1,
                Name = "New Category",
                Description = "New Description"
            };

            var createdCategoryDto = new CategoryDto
            {
                Id = 1,
                Name = "New Category",
                Description = "New Description"
            };

            _mockMapper.Setup(mapper => mapper.Map<Category>(categoryDto))
                .Returns(category);

            _mockCategoryRepository.Setup(repo => repo.AddAsync(category))
                .ReturnsAsync(createdCategory);

            _mockMapper.Setup(mapper => mapper.Map<CategoryDto>(createdCategory))
                .Returns(createdCategoryDto);

            // Act
            var result = await _categoryService.CreateCategoryAsync(categoryDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("New Category", result.Name);
        }

        [Fact]
        public async Task UpdateCategoryAsync_WithValidCategory_ShouldUpdateCategory()
        {
            // Arrange
            var categoryDto = new CategoryDto
            {
                Id = 1,
                Name = "Updated Category",
                Description = "Updated Description"
            };

            var existingCategory = new Category
            {
                Id = 1,
                Name = "Original Category",
                Description = "Original Description"
            };

            _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryDto.Id))
                .ReturnsAsync(existingCategory);

            // Act
            await _categoryService.UpdateCategoryAsync(categoryDto);

            // Assert
            _mockMapper.Verify(mapper => mapper.Map(categoryDto, existingCategory), Times.Once);
            _mockCategoryRepository.Verify(repo => repo.UpdateAsync(existingCategory), Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryAsync_WithValidId_ShouldDeleteCategory()
        {
            // Arrange
            int categoryId = 1;
            var category = new Category { Id = categoryId, Name = "Test Category" };

            _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId))
                .ReturnsAsync(category);

            // Act
            await _categoryService.DeleteCategoryAsync(categoryId);

            // Assert
            _mockCategoryRepository.Verify(repo => repo.DeleteAsync(category), Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryAsync_WithInvalidId_ShouldNotCallDelete()
        {
            // Arrange
            int categoryId = 999;

            _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId))
                .ReturnsAsync((Category)null);

            // Act
            await _categoryService.DeleteCategoryAsync(categoryId);

            // Assert
            _mockCategoryRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Category>()), Times.Never);
        }
    }
} 