using ezgi_mobilya.Core.Entities;
using ezgi_mobilya.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ezgi_mobilya.Data.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Product>> GetProductsWithCategoryAsync()
        {
            return await _context.Products.Include(x => x.Category).ToListAsync();
        }
    }
}
