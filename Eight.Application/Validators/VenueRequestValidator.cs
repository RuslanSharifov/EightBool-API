using Eight.Application.DTOs.Venue;
using FluentValidation;

namespace Eight.Application.Validators;

public class VenueRequestValidator : AbstractValidator<VenueRequest>
{
    public VenueRequestValidator()
    {
        RuleFor(x => x.Name).MinimumLength(3).WithMessage("Ad minimum 3 simvol olmalıdır.");
        RuleFor(x => x.Address).MinimumLength(5).WithMessage("Ünvan minimum 5 simvol olmalıdır.");
        RuleFor(x => x.CloseTime).GreaterThan(x => x.OpenTime).WithMessage("Bağlanış saatı açılışdan gec olmalıdır.");
        RuleFor(x => x.ServiceChargePercent).GreaterThan(0).When(x => x.ServiceChargeEnabled).WithMessage("Servis haqqı 0-dan böyük olmalıdır.");
        RuleFor(x => x.AdminId).NotEmpty().WithMessage("Admin seçilməlidir.");
    }
}