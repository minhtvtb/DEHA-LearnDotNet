using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ECommerceApp.Core.Entities;
using ECommerceApp.Core.Interfaces;
using ECommerceApp.Infrastructure.Data;

namespace ECommerceApp.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, IRepository<Category>
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Override to include products count
        public override async Task<IReadOnlyList<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.Products)
                .ToListAsync();
        }

        // Override to include related products
        public override async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
} 