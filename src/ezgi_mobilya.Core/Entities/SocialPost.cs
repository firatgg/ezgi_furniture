using System;

namespace ezgi_mobilya.Core.Entities
{
    public class SocialPost : BaseEntity
    {
        public string Caption { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string Platforms { get; set; } = null!; // E.g., "Instagram,Facebook"
        public bool IsPosted { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SocialPostId { get; set; } // ID returned from Meta/Instagram API
    }
}
