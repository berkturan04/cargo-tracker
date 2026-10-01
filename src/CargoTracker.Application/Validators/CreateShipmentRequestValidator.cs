using CargoTracker.Application.DTOs;
using FluentValidation;

namespace CargoTracker.Application.Validators;

public class CreateShipmentRequestValidator : AbstractValidator<CreateShipmentRequest>
{
    public CreateShipmentRequestValidator()
    {
        RuleFor(x => x.ReceiverName)
            .NotEmpty().WithMessage("Alıcı adı boş olamaz.")
            .MaximumLength(200);

        RuleFor(x => x.OriginCity)
            .NotEmpty().WithMessage("Çıkış şehri boş olamaz.")
            .MaximumLength(100);

        RuleFor(x => x.DestinationCity)
            .NotEmpty().WithMessage("Varış şehri boş olamaz.")
            .MaximumLength(100);

        RuleFor(x => x.WeightKg)
            .GreaterThan(0).WithMessage("Ağırlık 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(1000).WithMessage("Ağırlık 1000 kg'ı aşamaz.");
    }
}
