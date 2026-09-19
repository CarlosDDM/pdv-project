using FluentValidation;
using Pdv.Application.Commands.DeleteProduct;

namespace Pdv.Api.Validators.Product;

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.id)
            .NotEmpty().WithMessage("O Id do produto é obrigatório.")
            .NotEqual(Guid.Empty).WithMessage("O Id do produto não pode ser um Guid zerado.");
    }
}
