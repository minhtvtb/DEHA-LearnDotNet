using System;
using System.Linq;
using System.Threading.Tasks;
using ECommerceApp.Core.Entities;
using ECommerceApp.Infrastructure.Data;
using ECommerceApp.Infrastructure.Repositories;
using ECommerceApp.IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ECommerceApp.IntegrationTests.Repositories
{
    public class ProductRepositoryTests : DatabaseTestBase
    {
        private readonly ProductRepository _productRepository;

        public ProductRepositoryTests()
        {
            _productRepository = new ProductRepository(DbContext);
        }

        protected override void SeedDatabase()
        {
            // Add test categories
            var electronics = new Category { Name = "Electronics", Description = "Electronic devices" };
            var clothing = new Category { Name = "Clothing", Description = "Apparel items" };
            DbContext.Categories.AddRange(electronics, clothing);
            DbContext.SaveChanges();

            // Add test products
            DbContext.Products.AddRange(
                new Product
                {
                    Name = "Laptop",
                    Description = "High-performance laptop",
                    Price = 1299.99m,
                    Stock = 10,
                    ImageUrl = "/images/laptop.jpg",
                    CategoryId = electronics.Id
                },
                new Product
                {
                    Name = "Smartphone",
                    Description = "Latest smartphone model",
                    Price = 799.99m,
                    Stock = 15,
                    ImageUrl = "/images/smartphone.jpg",
                    CategoryId = electronics.Id
                },
                new Product
                {
                    Name = "T-Shirt",
                    Description = "Cotton t-shirt",
                    Price = 19.99m,
                    Stock = 50,
                    ImageUrl = "/images/tshirt.jpg",
                    CategoryId = clothing.Id
                }
            );
            DbContext.SaveChanges();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            // Act
            var products = await _productRepository.GetAllAsync();

            // Assert
            Assert.Equal(3, products.Count());
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnProduct()
        {
            // Arrange
            var expectedProduct = await DbContext.Products.FirstOrDefaultAsync();
            Assert.NotNull(expectedProduct); // Ensure we have a product to test with

            // Act
            var product = await _productRepository.GetByIdAsync(expectedProduct.Id);

            // Assert
            Assert.NotNull(product);
            Assert.Equal(expectedProduct.Id, product.Id);
            Assert.Equal(expectedProduct.Name, product.Name);
        }

        [Fact]
        public async Task GetProductWithCategoryAsync_ShouldReturnProductWithCategory()
        {
            // Arrange
            var expectedProduct = await DbContext.Products.FirstOrDefaultAsync();
            Assert.NotNull(expectedProduct); // Ensure we have a product to test with

            // Act
            var product = await _productRepository.GetProductWithCategoryAsync(expectedProduct.Id);

            // Assert
            Assert.NotNull(product);
            Assert.Equal(expectedProduct.Id, product.Id);
            Assert.NotNull(product.Category);
            Assert.Equal(expectedProduct.CategoryId, product.Category.Id);
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_ShouldReturnProductsInCategory()
        {
            // Arrange
            var electronics = await DbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Electronics");
            Assert.NotNull(electronics); // Ensure we have the category to test with

            // Act
            var products = await _productRepository.GetProductsByCategoryAsync(electronics.Id);

            // Assert
            Assert.Equal(2, products.Count());
            Assert.All(products, p => Assert.Equal(electronics.Id, p.CategoryId));
        }

        [Fact]
        public async Task AddAsync_ShouldAddNewProduct()
        {
            // Arrange
            var category = await DbContext.Categories.FirstOrDefaultAsync();
            Assert.NotNull(category); // Ensure we have a category to use

            var newProduct = new Product
            {
                Name = "New Test Product",
                Description = "Test description",
                Price = 99.99m,
                Stock = 5,
                ImageUrl = "/images/test.jpg",
                CategoryId = category.Id
            };

            // Act
            var addedProduct = await _productRepository.AddAsync(newProduct);

            // Assert
            Assert.NotNull(addedProduct);
            Assert.NotEqual(0, addedProduct.Id); // ID should be assigned
            Assert.Equal("New Test Product", addedProduct.Name);

            // Verify the product is in the database
            var dbProduct = await DbContext.Products.FindAsync(addedProduct.Id);
            Assert.NotNull(dbProduct);
            Assert.Equal(newProduct.Name, dbProduct.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct()
        {
            // Arrange
            var productToUpdate = await DbContext.Products.FirstOrDefaultAsync();
            Assert.NotNull(productToUpdate); // Ensure we have a product to update

            productToUpdate.Name = "Updated Product Name";
            productToUpdate.Price = 149.99m;
            productToUpdate.Stock = 20;

            // Act
            await _productRepository.UpdateAsync(productToUpdate);

            // Assert - Fetch the product from the DB again to verify it was updated
            var updatedProduct = await DbContext.Products.FindAsync(productToUpdate.Id);
            Assert.NotNull(updatedProduct);
            Assert.Equal("Updated Product Name", updatedProduct.Name);
            Assert.Equal(149.99m, updatedProduct.Price);
            Assert.Equal(20, updatedProduct.Stock);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveProduct()
        {
            // Arrange
            var productToDelete = await DbContext.Products.FirstOrDefaultAsync();
            Assert.NotNull(productToDelete); // Ensure we have a product to delete
            var productId = productToDelete.Id;

            // Act
            await _productRepository.DeleteAsync(productToDelete);

            // Assert
            var deletedProduct = await DbContext.Products.FindAsync(productId);
            Assert.Null(deletedProduct); // The product should no longer exist
        }
    }
} 