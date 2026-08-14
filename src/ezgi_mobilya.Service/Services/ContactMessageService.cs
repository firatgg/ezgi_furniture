using AutoMapper;
using ezgi_mobilya.Core.Entities;
using ezgi_mobilya.Core.Repositories;
using ezgi_mobilya.Core.UnitOfWorks;
using ezgi_mobilya.Service.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ezgi_mobilya.Service.Services
{
    public class ContactMessageService : IContactMessageService
    {
        private readonly IGenericRepository<ContactMessage> _contactMessageRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly ILogger<ContactMessageService> _logger;

        public ContactMessageService(
            IGenericRepository<ContactMessage> contactMessageRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            ILogger<ContactMessageService> logger)
        {
            _contactMessageRepository = contactMessageRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<List<ContactMessageDto>> GetAllAsync()
        {
            var messages = await _contactMessageRepository.GetAll().ToListAsync();
            return _mapper.Map<List<ContactMessageDto>>(messages);
        }

        public async Task<ContactMessageDto?> GetByIdAsync(int id)
        {
            var message = await _contactMessageRepository.GetByIdAsync(id);
            return _mapper.Map<ContactMessageDto>(message);
        }

        public async Task<ContactMessageDto> AddAsync(ContactMessageDto dto)
        {
            var message = _mapper.Map<ContactMessage>(dto);
            message.IsRead = false;
            message.CreatedDate = DateTime.UtcNow;
            message.UpdatedDate = null;

            if (!_emailService.IsConfigured)
            {
                _logger.LogError("SMTP ayarları eksik olduğu için iletişim formu e-postası gönderilemedi.");
                throw new InvalidOperationException("SMTP ayarları yapılandırılmamış.");
            }

            await _emailService.SendContactNotificationAsync(message);

            await _contactMessageRepository.AddAsync(message);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Yeni iletişim mesajı kaydedildi ve e-posta gönderildi. Email: {Email}", message.Email);
            return _mapper.Map<ContactMessageDto>(message);
        }

        public async Task MarkAsReadAsync(int id)
        {
            var message = await _contactMessageRepository.GetByIdAsync(id);
            if (message != null)
            {
                message.IsRead = true;
                _contactMessageRepository.Update(message);
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var message = await _contactMessageRepository.GetByIdAsync(id);
            if (message != null)
            {
                _contactMessageRepository.Remove(message);
                await _unitOfWork.CommitAsync();
            }
        }
    }
}
