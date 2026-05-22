using FluentValidation;
using ezgi_mobilya.Service.DTOs;

namespace ezgi_mobilya.Service.Validations
{
    public class ContactMessageDtoValidator : AbstractValidator<ContactMessageDto>
    {
        public ContactMessageDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Ad Soyad alanı boş geçilemez.")
                .MaximumLength(100).WithMessage("Ad Soyad alanı en fazla 100 karakter olabilir.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta alanı boş geçilemez.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
                .MaximumLength(100).WithMessage("E-posta alanı en fazla 100 karakter olabilir.");

            RuleFor(x => x.Subject)
                .MaximumLength(150).WithMessage("Konu alanı en fazla 150 karakter olabilir.");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Mesaj alanı boş geçilemez.")
                .MaximumLength(2000).WithMessage("Mesaj alanı en fazla 2000 karakter olabilir.");
        }
    }
}
