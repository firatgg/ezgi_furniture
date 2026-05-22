using ezgi_mobilya.Service.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ezgi_mobilya.Service.Services
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllAsync();
        Task<List<ProductDto>> GetActiveProductsAsync();
        Task<List<ProductDto>> GetProductsWithCategoryAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> AddAsync(ProductDto dto);
        Task UpdateAsync(ProductDto dto);
        Task DeleteAsync(int id);
    }
}
