using System;
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
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IRepository<Category>> _mockCategoryRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockCategoryRepository = new Mock<IRepository<Category>>();
            _mockMapper = new Mock<IMapper>();
            _productService = new ProductService(
                _mockProductRepository.Object,
                _mockCategoryRepository.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 10.99m },
                new Product { Id = 2, Name = "Product 2", Price = 20.99m }
            };

            var productDtos = new List<ProductDto>
            {
                new ProductDto { Id = 1, Name = "Product 1", Price = 10.99m },
                new ProductDto { Id = 2, Name = "Product 2", Price = 20.99m }
            };

            _mockProductRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(products);

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<ProductDto>>(products))
                .Returns(productDtos);

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.First().Id);
            Assert.Equal("Product 1", result.First().Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_WithValidId_ShouldReturnProduct()
        {
            // Arrange
            int productId = 1;
            var product = new Product { Id = productId, Name = "Test Product", Price = 15.99m };
            var productDto = new ProductDto { Id = productId, Name = "Test Product", Price = 15.99m };

            _mockProductRepository.Setup(repo => repo.GetProductWithCategoryAsync(productId))
                .ReturnsAsync(product);

            _mockMapper.Setup(mapper => mapper.Map<ProductDto>(product))
                .Returns(productDto);

            // Act
            var result = await _productService.GetProductByIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
            Assert.Equal("Test Product", result.Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            int productId = 999;

            _mockProductRepository.Setup(repo => repo.GetProductWithCategoryAsync(productId))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _productService.GetProductByIdAsync(productId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldCreateAndReturnProduct()
        {
            // Arrange
            var productDto = new ProductDto
            {
                Name = "New Product",
                Description = "Description",
                Price = 29.99m,
                Stock = 10,
                CategoryId = 1
            };

            var product = new Product
            {
                Name = "New Product",
                Description = "Description",
                Price = 29.99m,
                Stock = 10,
                CategoryId = 1
            };

            var createdProduct = new Product
            {
                Id = 1,
                Name = "New Product",
                Description = "Description",
                Price = 29.99m,
                Stock = 10,
                CategoryId = 1
            };

            var createdProductDto = new ProductDto
            {
                Id = 1,
                Name = "New Product",
                Description = "Description",
                Price = 29.99m,
                Stock = 10,
                CategoryId = 1
            };

            _mockMapper.Setup(mapper => mapper.Map<Product>(productDto))
                .Returns(product);

            _mockProductRepository.Setup(repo => repo.AddAsync(product))
                .ReturnsAsync(createdProduct);

            _mockMapper.Setup(mapper => mapper.Map<ProductDto>(createdProduct))
                .Returns(createdProductDto);

            // Act
            var result = await _productService.CreateProductAsync(productDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("New Product", result.Name);
            Assert.Equal(29.99m, result.Price);
        }

        [Fact]
        public async Task UpdateProductAsync_WithValidProduct_ShouldUpdateProduct()
        {
            // Arrange
            var productDto = new ProductDto
            {
                Id = 1,
                Name = "Updated Product",
                Description = "Updated Description",
                Price = 39.99m,
                Stock = 15,
                CategoryId = 2
            };

            var existingProduct = new Product
            {
                Id = 1,
                Name = "Original Product",
                Description = "Original Description",
                Price = 29.99m,
                Stock = 10,
                CategoryId = 1
            };

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productDto.Id))
                .ReturnsAsync(existingProduct);

            // Act
            await _productService.UpdateProductAsync(productDto);

            // Assert
            _mockMapper.Verify(mapper => mapper.Map(productDto, existingProduct), Times.Once);
            _mockProductRepository.Verify(repo => repo.UpdateAsync(existingProduct), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_WithValidId_ShouldDeleteProduct()
        {
            // Arrange
            int productId = 1;
            var product = new Product { Id = productId, Name = "Test Product" };

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync(product);

            // Act
            await _productService.DeleteProductAsync(productId);

            // Assert
            _mockProductRepository.Verify(repo => repo.DeleteAsync(product), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_WithInvalidId_ShouldNotCallDelete()
        {
            // Arrange
            int productId = 999;

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync((Product)null);

            // Act
            await _productService.DeleteProductAsync(productId);

            // Assert
            _mockProductRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task GetFeaturedProductsAsync_ShouldReturnSpecifiedNumberOfProducts()
        {
            // Arrange
            int count = 3;
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 10.99m },
                new Product { Id = 2, Name = "Product 2", Price = 20.99m },
                new Product { Id = 3, Name = "Product 3", Price = 30.99m },
                new Product { Id = 4, Name = "Product 4", Price = 40.99m },
                new Product { Id = 5, Name = "Product 5", Price = 50.99m }
            };

            var productDtos = new List<ProductDto>
            {
                new ProductDto { Id = 5, Name = "Product 5", Price = 50.99m },
                new ProductDto { Id = 4, Name = "Product 4", Price = 40.99m },
                new ProductDto { Id = 3, Name = "Product 3", Price = 30.99m }
            };

            _mockProductRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(products);

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<Product>>()))
                .Returns(productDtos);

            // Act
            var result = await _productService.GetFeaturedProductsAsync(count);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }
    }
} 