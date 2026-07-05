using Eight.Application.DTOs.User;
using FluentValidation;

namespace Eight.Application.Validators;

public class UserUpdateRequestValidator : AbstractValidator<UserUpdateRequest>
{
    public UserUpdateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Ad boş ola bilməz.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email düzgün deyil.");
        RuleFor(x => x.Password).MinimumLength(6).When(x => !string.IsNullOrEmpty(x.Password))
            .WithMessage("Şifrə minimum 6 simvol olmalıdır.");
    }
}