using FluentValidation;
using Pdv.Application.Commands.CreateProduct;

namespace Pdv.Api.Validators.Product;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome do produto é obrigatório.")
            .MaximumLength(100).WithMessage("O nome do produto não pode exceder 100 caracteres.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Tipo de produto inválido.");

        RuleFor(x => x.Brand)
            .MaximumLength(50).WithMessage("A marca não pode exceder 50 caracteres.")
            .When(x => x.Brand is not null);

        RuleFor(x => x.Barcode)
            .Length(8, 20).WithMessage("O código de barras deve ter entre 8 e 20 caracteres.")
            .Matches(@"^[0-9]+\z").WithMessage("O código de barras deve conter apenas números.")
            .When(x => x.Barcode is not null);
    }
}