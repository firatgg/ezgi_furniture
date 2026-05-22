using ezgi_mobilya.Service.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ezgi_mobilya.Service.Services
{
    public interface IContactMessageService
    {
        Task<List<ContactMessageDto>> GetAllAsync();
        Task<ContactMessageDto?> GetByIdAsync(int id);
        Task<ContactMessageDto> AddAsync(ContactMessageDto dto);
        Task MarkAsReadAsync(int id);
        Task DeleteAsync(int id);
    }
}
