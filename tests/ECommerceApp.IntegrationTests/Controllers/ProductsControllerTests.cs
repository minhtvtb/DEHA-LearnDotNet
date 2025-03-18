using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ECommerceApp.Core.DTOs;
using ECommerceApp.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace ECommerceApp.IntegrationTests.Controllers
{
    public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public ProductsControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetProducts_ReturnsSuccess()
        {
            // Act
            var response = await _client.GetAsync("/Products");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8", 
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task GetProductDetails_WithValidId_ReturnsSuccess()
        {
            // Arrange
            // Get a valid ID from the seeded data
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var product = await dbContext.Products.FirstOrDefaultAsync();
            Assert.NotNull(product); // Ensure we have a product

            // Act
            var response = await _client.GetAsync($"/Products/Details/{product.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8", 
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task GetProductDetails_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/Products/Details/999999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    // Custom WebApplicationFactory that configures services for testing
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Find the DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });

                // Build the service provider
                var sp = services.BuildServiceProvider();

                // Create and seed the database
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                    // Ensure database is created
                    db.Database.EnsureCreated();

                    // Add test data
                    SeedTestData(db);
                }
            });
        }

        private void SeedTestData(ApplicationDbContext db)
        {
            // Clear existing data
            db.Products.RemoveRange(db.Products);
            db.Categories.RemoveRange(db.Categories);
            db.SaveChanges();

            // Add categories
            var electronics = new Core.Entities.Category { Name = "Electronics", Description = "Electronic devices" };
            var clothing = new Core.Entities.Category { Name = "Clothing", Description = "Apparel items" };
            db.Categories.AddRange(electronics, clothing);
            db.SaveChanges();

            // Add products
            db.Products.AddRange(
                new Core.Entities.Product
                {
                    Name = "Test Laptop",
                    Description = "Test laptop description",
                    Price = 1299.99m,
                    Stock = 10,
                    ImageUrl = "/images/test-laptop.jpg",
                    CategoryId = electronics.Id
                },
                new Core.Entities.Product
                {
                    Name = "Test T-Shirt",
                    Description = "Test t-shirt description",
                    Price = 19.99m,
                    Stock = 50,
                    ImageUrl = "/images/test-tshirt.jpg",
                    CategoryId = clothing.Id
                }
            );
            db.SaveChanges();
        }
    }
} 