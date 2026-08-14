using ezgi_mobilya.Core.Entities;

namespace ezgi_mobilya.Service.Services
{
    public interface IEmailService
    {
        bool IsConfigured { get; }
        Task SendContactNotificationAsync(ContactMessage message, CancellationToken cancellationToken = default);
    }
}
