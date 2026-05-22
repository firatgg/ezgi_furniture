using ezgi_mobilya.Service.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ezgi_mobilya.Service.Services
{
    public interface ISocialMediaService
    {
        Task<List<SocialPostDto>> GetAllPostsAsync();
        Task<SocialPostDto> CreateAndPublishPostAsync(SocialPostDto dto);
    }
}
