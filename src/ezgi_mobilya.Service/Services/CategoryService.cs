using AutoMapper;
using ezgi_mobilya.Core.Entities;
using ezgi_mobilya.Core.Repositories;
using ezgi_mobilya.Core.UnitOfWorks;
using ezgi_mobilya.Service.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ezgi_mobilya.Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IGenericRepository<Category> categoryRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAll().ToListAsync();
            return _mapper.Map<List<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> AddAsync(CategoryDto dto)
        {
            var category = _mapper.Map<Category>(dto);
            await _categoryRepository.AddAsync(category);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task UpdateAsync(CategoryDto dto)
        {
            var category = _mapper.Map<Category>(dto);
            _categoryRepository.Update(category);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category != null)
            {
                _categoryRepository.Remove(category);
                await _unitOfWork.CommitAsync();
            }
        }
    }
}
