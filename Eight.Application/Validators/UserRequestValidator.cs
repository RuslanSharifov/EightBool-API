using Eight.Application.DTOs.User;
using FluentValidation;



namespace Eight.Application.Validators;


public class UserRequestValidator : AbstractValidator<UserRequest>
{
    public UserRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Ad boş ola bilməz.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email düzgün deyil.");
        RuleFor(x => x.Password).MinimumLength(6).WithMessage("Şifrə minimum 6 simvol olmalıdır.");
    }
}
