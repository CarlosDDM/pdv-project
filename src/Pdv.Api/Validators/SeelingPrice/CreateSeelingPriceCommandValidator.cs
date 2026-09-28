using FluentValidation;
using Pdv.Application.Commands.CreateSeelingPrice;

namespace Pdv.Api.Validators.SeelingPrice;

public sealed class CreateSeelingPriceCommandValidator : AbstractValidator<CreateSeelingPriceCommand>
{
    public CreateSeelingPriceCommandValidator()
    {
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("O preço deve ser maior que zero.")
            .PrecisionScale(10, 2, ignoreTrailingZeros: true)
            .WithMessage("O preço deve ter no máximo 2 casas decimais.");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("O Id do produto é obrigatório e não pode ser vazio.");
    }
}
