using FluentValidation;
using ezgi_mobilya.Service.DTOs;

namespace ezgi_mobilya.Service.Validations
{
    public class ProductDtoValidator : AbstractValidator<ProductDto>
    {
        public ProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} alanı boş geçilemez.")
                .MaximumLength(150).WithMessage("{PropertyName} alanı en fazla 150 karakter olabilir.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("{PropertyName} alanı 0'dan büyük olmalıdır.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("{PropertyName} alanı geçerli bir kategori olmalıdır.");
        }
    }
}
