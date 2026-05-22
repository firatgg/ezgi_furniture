using System;

namespace ezgi_mobilya.Core.Entities
{
    public class ContactMessage : BaseEntity
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Subject { get; set; }
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; } = false;
    }
}
