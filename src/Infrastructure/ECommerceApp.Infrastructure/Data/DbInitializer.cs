using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ECommerceApp.Core.Entities;
using ECommerceApp.Infrastructure.Identity;

namespace ECommerceApp.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            try
            {
                // Ensure database is created and apply migrations
                context.Database.Migrate();
                
                // Seed roles
                await SeedRolesAsync(roleManager);
                
                // Seed admin user
                await SeedAdminUserAsync(userManager);
                
                // Seed categories
                await SeedCategoriesAsync(context);
                
                // Seed products
                await SeedProductsAsync(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
        }

        private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            const string adminEmail = "admin@example.com";
            const string adminPassword = "Admin123!";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        private static async Task SeedCategoriesAsync(ApplicationDbContext context)
        {
            if (!context.Categories.Any())
            {
                var categories = new[]
                {
                    new Category { Name = "Electronics", Description = "Electronic devices and gadgets" },
                    new Category { Name = "Clothing", Description = "Apparel and fashion items" },
                    new Category { Name = "Books", Description = "Books and publications" },
                    new Category { Name = "Home & Garden", Description = "Home decor and gardening products" }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedProductsAsync(ApplicationDbContext context)
        {
            if (!context.Products.Any())
            {
                var electronicsCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Electronics");
                var clothingCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Clothing");
                var booksCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Books");

                if (electronicsCategory != null && clothingCategory != null && booksCategory != null)
                {
                    var products = new[]
                    {
                        new Product 
                        { 
                            Name = "Smartphone", 
                            Description = "Latest smartphone with high-end features", 
                            Price = 799.99m, 
                            Stock = 50, 
                            ImageUrl = "/uploads/smartphone.jpg", 
                            CategoryId = electronicsCategory.Id 
                        },
                        new Product 
                        { 
                            Name = "Laptop", 
                            Description = "Powerful laptop for work and gaming", 
                            Price = 1299.99m, 
                            Stock = 30, 
                            ImageUrl = "/uploads/laptop.jpg", 
                            CategoryId = electronicsCategory.Id 
                        },
                        new Product 
                        { 
                            Name = "T-Shirt", 
                            Description = "Comfortable cotton t-shirt", 
                            Price = 19.99m, 
                            Stock = 100, 
                            ImageUrl = "/uploads/tshirt.jpg", 
                            CategoryId = clothingCategory.Id 
                        },
                        new Product 
                        { 
                            Name = "Jeans", 
                            Description = "Classic blue jeans", 
                            Price = 49.99m, 
                            Stock = 75, 
                            ImageUrl = "/uploads/jeans.jpg", 
                            CategoryId = clothingCategory.Id 
                        },
                        new Product 
                        { 
                            Name = "Programming Book", 
                            Description = "Learn programming with this comprehensive guide", 
                            Price = 39.99m, 
                            Stock = 40, 
                            ImageUrl = "/uploads/programming_book.jpg", 
                            CategoryId = booksCategory.Id 
                        }
                    };

                    await context.Products.AddRangeAsync(products);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
} 