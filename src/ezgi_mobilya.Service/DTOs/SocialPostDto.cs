using System;

namespace ezgi_mobilya.Service.DTOs
{
    public class SocialPostDto
    {
        public int Id { get; set; }
        public string Caption { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string Platforms { get; set; } = null!;
        public bool IsPosted { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SocialPostId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
