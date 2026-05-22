using ezgi_mobilya.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ezgi_mobilya.Core.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<List<Product>> GetProductsWithCategoryAsync();
    }
}
