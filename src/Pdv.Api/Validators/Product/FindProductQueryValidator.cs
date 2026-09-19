using FluentValidation;
using Pdv.Application.Queries.FindProduct;

namespace Pdv.Api.Validators.Product;

public class FindProductQueryValidator : AbstractValidator<FindProductQuery>
{
    public FindProductQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O Id do produto é obrigatório.")
            .NotEqual(Guid.Empty).WithMessage("O Id do produto não pode ser um Guid zerado.");
    }
}
