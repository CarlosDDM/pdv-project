using FluentValidation;
using Pdv.Application.Commands.DeleteProduct;

namespace Pdv.Api.Validators.Product;

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O Id do produto é obrigatório.")
            .GreaterThan(0).WithMessage("O Id do produto não pode ser zerado ou negativo.");
    }
}
